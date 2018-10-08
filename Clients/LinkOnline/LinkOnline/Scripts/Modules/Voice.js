var synth;
function InitVoice() {
    synth = window.speechSynthesis;

    var inputForm = document.querySelector('form');
    var voiceSelect = document.querySelector('select');


    var voices = [];

    function populateVoiceList() {
        return;
        voices = synth.getVoices();

        for(i = 0; i < voices.length ; i++) {
            var option = document.createElement('option');
            option.textContent = voices[i].name + ' (' + voices[i].lang + ')';
    
            if(voices[i].default) {
                option.textContent += ' -- DEFAULT';
            }

            option.setAttribute('data-lang', voices[i].lang);
            option.setAttribute('data-name', voices[i].name);
            voiceSelect.appendChild(option);
        }
    }

    populateVoiceList();
    if (speechSynthesis.onvoiceschanged !== undefined) {
        speechSynthesis.onvoiceschanged = populateVoiceList;
    }
}

function IntroduceYourself(event) {
    var pitch = 1;
    var rate = .8;
    var inputTxt = "Hello, I am LiNK. How can I help you?";
    //event.preventDefault();
    voices = synth.getVoices();

    var utterThis = new SpeechSynthesisUtterance(inputTxt);
    var selectedOption = "Google US English";
    for (i = 0; i < voices.length ; i++) {
        if (voices[i].name === selectedOption) {
            utterThis.voice = voices[i];
        }
    }
    utterThis.pitch = pitch;
    utterThis.rate = rate;
    synth.speak(utterThis);

    utterThis.onpause = function (event) {
        var char = event.utterance.text.charAt(event.charIndex);
        console.log('Speech paused at character ' + event.charIndex + ' of "' +
        event.utterance.text + '", which is "' + char + '".');
    }
}
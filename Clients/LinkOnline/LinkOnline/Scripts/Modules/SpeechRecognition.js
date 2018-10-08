var speechVariables;

var recognition;
loadFunctions.push(InitSpeechRecognition);
function InitSpeechRecognition() {
    document.title = "LiNK";
    InitVoice();
    if (recognition == undefined) {
        recognition = new webkitSpeechRecognition();
        recognition.interimResults = false;
        recognition.continuous = false;
        recognition.lang = "en-GB";
        recognition.start();

        recognition.onend = function () {
            window.setTimeout(function () {
                recognition.start();
            }, 100);
        }
    }
    //recognition.grammar = "commands.grxml";
    //recognition.continuous = true;

    recognition.onaudiostart = function () {
    };

    recognition.onerror = function (event) {
    };

    recognition.onresult = function (event) {
        var result = event.results[event.results.length - 1][0].transcript.toLowerCase().trim();
        if (result == "link" || result == "ling" || result == "blink" || result == "thank" || result == "think" || result == "pink") {
            window.setTimeout(SpeechRecognition, 100);
        }
        else if (result == "hello") {
            ShowMessage(LoadLanguageText("LiNKVoice_Hello"), "LiNKVoice Color1I");
            IntroduceYourself();
            window.setTimeout(SpeechRecognition, 100);
        }
        else {
            //ShowMessage(event.results[event.results.length - 1][0].transcript, "Warning");
            InitSpeechRecognition();
        }
    }
}

function SpeechRecognition() {

    _AjaxRequest("/Handlers/LinkBiExternal.ashx", "GetVariables", "", function (response) {
        speechVariables = JSON.parse(response);
    });

    var loading = document.createElement("div");
    loading.id = "speechLoading";
    loading.className = "SpeechLoading";
    loading.innerHTML = "<div class=\"ball\"></div><div class=\"ball1\"></div>";
    document.title = "LiNK - Listening...";

    document.body.appendChild(loading);

    //var recognition = new webkitSpeechRecognition();
    //recognition.lang = "en-GB";
    //recognition.grammar = "commands.grxml";
    //recognition.continuous = true;

    recognition.onaudiostart = function () {
    };

    recognition.onerror = function (event) {
    };

    recognition.onresult = function (event) {
        ParseSpeech(event.results[event.results.length - 1][0].transcript);
    }
}

function ParseSpeech(result) {

    result = result.toLowerCase().trim();
    //ShowMessage(result, "Warning");
    document.getElementById("speechLoading").className = "SpeechLoading SpeechLoadingProcessing";

    var path;
    // FOR TESTING ONLY:
    //result = "run age of respondent on gender and campaign module";
    //result = "run age on gender";

    speechRequests = 0;

    if (result == "hide empty") {

        AjaxRequest("UpdateSetting", "Name=HideEmptyRowsAndColumns&Value=True&ClearData=True", function (response) {
            SpeechUpdateView();
        });
        return;
    }
    if (result == "show empty" || result == "so empty") {

        AjaxRequest("UpdateSetting", "Name=HideEmptyRowsAndColumns&Value=False&ClearData=True", function (response) {
            SpeechUpdateView();
        });
        return;
    }

    if (result == "switch table") {
        AjaxRequest("SwitchTable", "", function (response) {
            SpeechUpdateView();
        });
        return;
    }

    if (result == "clear" || result == "new") {
        AjaxRequest("ClearTabDefinition", "", function (response) {
            SpeechUpdateView();
        });
        return;
    }
    if (result == "new tab") {
        CreateNewReportTab();
        return;
    }
    if (result == "delete tab") {
        var idReport = $(".ReportTab_Active")[0].getAttribute("oncontextmenu").replace("ShowReportTabContextMenu(this, '", "").replace("');return false;", "");
        AjaxRequest("DeleteReportTab", "FileName=" + idReport, function (response) {
            ShowLoading(document.body);
            window.location = window.location;
        });
        return;
    }
    if (result == "hello") {
        ShowMessage(LoadLanguageText("LiNKVoice_Hello"), "LiNKVoice Color1I");
        var loading = document.getElementById("speechLoading");
        loading.parentNode.removeChild(loading);
        document.title = "LiNK";
        window.setTimeout(InitSpeechRecognition, 100);
        return;
    }

    if (true ) {//result.toLowerCase().substring(0, 3) == "run") {
        //result = result.substring(4, result.length);

        //var variables = result.split(" on ");
        var variables = result.split(" by ");
        /*if (variables.length == 1)
            variables = result.split(" by ");*/

        for (var i = 0; i < variables.length; i++) {
            var nestedVariables = variables[i].split(' and ');

            if (nestedVariables.length > 1) {
                variables[i] = nestedVariables[0];
                nestedVariables = nestedVariables.slice(1, nestedVariables.length);
            }
            else {
                nestedVariables = [];
            }

            switch (i) {
                case 0:
                    path = "Report/Variables[@Position=\"Left\"]";
                    break;
                case 1:
                    path = "Report/Variables[@Position=\"Top\"]";
                    break;
            }

            var idVariable = SpeechGetVariable(variables[i]);

            if (idVariable == undefined) {
                ShowMessage("variable '" + variables[i] + "' was not found.", "Error");
                continue;
            }

            SpeechSelectVariable(path, idVariable);

            var idVariableNested;
            for (var n = 0; n < nestedVariables.length; n++) {
                idVariableNested = SpeechGetVariable(nestedVariables[n]);

                if (idVariableNested == undefined) {
                    ShowMessage("variable '" + nestedVariables[n] + "' was not found", "Error");
                    continue;
                }

                SpeechSelectVariable(
                    path + "/Variable[@Id=\"" + idVariable + "\"]",
                    idVariableNested
                );
            }
        }
    }
    
    AjaxRequest("UpdateSetting", "Name=AutoUpdate&Value=True&ClearData=False", function (response) {
        SpeechUpdateView();
    });
}

function SpeechUpdateView() {
    if (speechRequests != 0) {
        window.setTimeout(SpeechUpdateView, 500);
        return;
    }

    //window.location = window.location;
    PopulateCrosstable();
    var loading = document.getElementById("speechLoading");
    loading.parentNode.removeChild(loading);
    document.title = "LiNK";
    InitSpeechRecognition();
}

function SpeechGetVariable(variable) {
    var idVariable = undefined;

    for (var j = 0; j < speechVariables.length; j++) {
        if (speechVariables[j].Label.toLowerCase() != variable.toLowerCase())
            continue;

        idVariable = speechVariables[j].Id;
    }


    if (idVariable == undefined) {
        var highestMatch = 0;
        var highestMatchId = undefined;

        var perc;
        for (var j = 0; j < speechVariables.length; j++) {
            perc = CompareText(speechVariables[j].Label.toLowerCase(), variable.toLowerCase());

            if (perc > highestMatch) {
                highestMatch = perc;
                highestMatchId = speechVariables[j].Id;
            }
        }

        if (highestMatch >= 50)
            idVariable = highestMatchId;
    }

    return idVariable;
}
var speechRequests = 0;

function SpeechSelectVariable(path, idVariable, idSelected) {
    var parameters = "Path=" + path +
                "&IdVariable=" + idVariable + "&IsTaxonomy=" + true;

    /*if (idSelected != undefined)
        parameters += "&IdSelected=" + idSelected;*/

    speechRequests++;
    _AjaxRequest("/Handlers/GlobalHandler.ashx", "SelectVariable", parameters, function (response) {
        speechRequests--;
    });
}

function CompareText(text1, text2) {
    var result = 0;

    var words = text1.split(' ');

    for (var i = 0; i < words.length; i++) {
        try{
            if (text2.search(words[i]) != -1)
                result++;
        }
        catch(e){}
    }

    words = text2.split(' ');

    for (var i = 0; i < words.length; i++) {
        if (text1.search(words[i]) != -1)
            result++;
    }

    return result * 100 / (text1.split(' ').length + words.length);
}
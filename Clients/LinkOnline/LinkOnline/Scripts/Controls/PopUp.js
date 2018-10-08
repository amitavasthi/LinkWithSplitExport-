function SessionExpireAlert(timeout) {
    return;
    var seconds = timeout / 1000;
    var expireTime = timeout - parseInt(10 * 60 * 1000);
    document.getElementsByName("seconds").innerHTML = toHHMMSS(seconds);

    setInterval(function () {
        seconds--;
        document.getElementById("seconds").innerHTML = toHHMMSS(seconds);

    }, 1000);
    setTimeout(function () {
        //Show Popup before x seconds of timeout.
        $find("mpeTimeout").show();
    }, expireTime);//parseInt(timeout - 600 * 1000)
    setTimeout(function () {
        window.location = '/Pages/Login.aspx';
    }, timeout);
};
function ResetSession() {
    //Redirect to refresh Session.
    window.location = window.location.href;
}
function toHHMMSS(time) {
    var sec_num = parseInt(time, 10); // don't forget the second parm    
    //var hours = Math.floor(sec_num / 3600);
    var hours = Math.floor(time / 3600);
    //var minutes = Math.floor((sec_num - (hours * 3600)) / 60
    var minutes = Math.floor((time - (Math.floor(time / 3600))) / 60);
    // var seconds = sec_num - (hours * 3600) - (minutes * 60);
    var seconds = time - (Math.floor(time / 3600)) - (Math.floor((time - (Math.floor(time / 3600))) / 60) * 60);

    if (hours < 10) {
        hours = "0" + hours;
    }
    if (minutes < 10) {
        minutes = "0" + minutes;
    }
    if (seconds < 10) {
        seconds = "0" + seconds;
    }
    //var time = hours + ':' + minutes + ':' + seconds;
    var time = minutes + ':' + seconds;
    return time;
}
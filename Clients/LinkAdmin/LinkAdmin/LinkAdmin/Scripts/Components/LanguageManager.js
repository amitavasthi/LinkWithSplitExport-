var LanguageManager = new Object();

LanguageManager.GetLabel = function (key) {
    return Request(
        "/Handlers/LanguageManager.ashx",
        "GetLabel",
        "Key=" + key
    );
}
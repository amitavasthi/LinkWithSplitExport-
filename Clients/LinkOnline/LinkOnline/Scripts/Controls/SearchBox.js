
function InitSearchBox(idSearchBox, idElementContainer) {
    try {
        var searchBox = document.getElementById(idSearchBox);

        searchBox.onkeyup = function () {
            window.setTimeout(function () { Search(searchBox); }, 50);
        }

        searchBox["SearchItems"] = new Array();

        var elements = document.getElementById(idElementContainer).childNodes;

        // Preload the search elements for faster use.
        for (var i = 0; i < elements.length; i++) {
            var searchItem = new Object();
            searchItem["Element"] = elements.item(i);
            searchItem["SearchText"] = elements.item(i).textContent;

            searchBox["SearchItems"].push(searchItem);
        }
    }
    catch (e) {
    }
}

function Search(searchBox) {
    var searchParts = new Array();

    var searchString = searchBox.value;
    var startIndex = 0;

    // Sepperate the search string by every single word.
    while (startIndex != -1) {
        startIndex = searchString.search(' ');

        if (startIndex > 0) {
            searchParts.push(searchString.substr(0, startIndex).toLowerCase());

            searchString = searchString.substr(startIndex + 1, searchString.length - startIndex);
        }
        else {
            searchParts.push(searchString.toLowerCase());

            break;
        }
    }

    for (var i = 0; i < searchBox["SearchItems"].length; i++) {
        var visible = true;
        var searchItem = searchBox["SearchItems"][i];

        // Search for every single word in the search string.
        for (var a = 0; a < searchParts.length; a++) {
            if (searchItem["SearchText"].toLowerCase().search(searchParts[a]) == -1) {
                visible = false;

                break;
            }
        }

        // When all words matched -> make element visible. Else -> hide
        if (visible) {
            searchItem["Element"].style.display = "block";
        }
        else {
            searchItem["Element"].style.display = "none";
        }
    }
}
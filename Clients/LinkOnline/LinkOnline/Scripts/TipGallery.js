function TipGalleryNext(id, interval, items, index) {
    var tipGallery = document.getElementById(id);

    if (tipGallery == undefined)
        return;

    var lbl = tipGallery.getElementsByTagName("span").item(0);

    index++;

    if (items.length == index)
        index = 0;

    DecreaseOpacity(lbl, function () {
        lbl.innerHTML = LoadLanguageText(items[index]);

        IncreaseOpacity(lbl, 50, 1.0);

        window.setTimeout(function () {
            TipGalleryNext(id, interval, items, index)
        }, interval);
    });
}
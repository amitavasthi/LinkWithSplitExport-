function TreeViewShowChildNodes(sender, id, idTreeView, path) {
    document.getElementById(id).style.display = '';
    
    sender.src = "/Images/Icons/TreeViewNodeCollapse.png";
    sender.setAttribute("onclick", "TreeViewHideChildNodes(this, '" + id + "', '" + idTreeView + "', '" + path + "')");

    ShowLoading(document.body);

    _AjaxRequest("/Handlers/WebUtilitiesHandler.ashx", "TreeViewExpandNode", "IdTreeView=" + idTreeView + "&Path=" + path, function (response) {
        var control = document.getElementById(id);

        var container = control.parentNode;
        container.removeChild(control);
            
        control = document.createElement("div");
        control.innerHTML = response;

        if (control.firstElementChild != undefined)
            control = control.firstElementChild;
        else
            control = control.firstChild;

        container.appendChild(control);

        HideLoading();
    });
}

function TreeViewHideChildNodes(sender, id, idTreeView, path) {
    document.getElementById(id).style.display = 'none';

    sender.src = "/Images/Icons/TreeViewNodeExpand.png";
    sender.setAttribute("onclick", "TreeViewShowChildNodes(this, '" + id + "', '" + idTreeView + "', '" + path + "')");


    _AjaxRequest("/Handlers/WebUtilitiesHandler.ashx", "TreeViewCollapseNode", "IdTreeView=" + idTreeView + "&Path=" + path, function (response) {
    });
}
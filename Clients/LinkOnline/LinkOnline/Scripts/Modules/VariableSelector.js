function InitVariableSelector(idControl, settings) {

    var control = document.getElementById(idControl);

    var scoreTriggerId = "";

    if (control.id.search("cphContent_") != -1) {
        scoreTriggerId = control.id.replace("cphContent_", "cphContent_imgVariableDropDown");
    }
    else {
        scoreTriggerId = "imgVariableDropDown" + control.id;
    }

    control.ScoreTrigger = document.getElementById(scoreTriggerId);
    control.ScoreTrigger.Control = control;

    control.Settings = settings;

    control.ShowScores = function (control, goneFullScreen, smoothEffect, onFinish) {
        if (control == undefined) control = this.Control;

        if (smoothEffect == undefined) smoothEffect = control.Settings.SmoothEffect;

        if (control.Settings.FullScreen && goneFullScreen != true) {
            var background = document.createElement("div");
            background.id = "FullScreenVariableSelectorControlContainerBackground";
            background.className = "FullScreenVariableSelectorControlContainerBackground";

            background.style.width = "0px";
            background.style.height = "0px";

            document.body.appendChild(background);

            IncreaseWidth(background, window.innerWidth);
            IncreaseHeight(background, window.innerHeight, undefined, true);

            window.setTimeout(function () {

                var container = document.createElement("div");
                container.id = "FullScreenVariableSelectorControlContainer";
                container.className = "FullScreenVariableSelectorControlContainer";

                var duplicate = control.cloneNode(true);
                duplicate.id = "_" + duplicate.id;

                duplicate.Settings = control.Settings;
                duplicate.ShowScores = control.ShowScores;
                duplicate.HideScores = control.HideScores;

                var duplicateSender = GetChildByAttribute(duplicate, "id", control.ScoreTrigger.id, true);
                duplicateSender.id = "_" + duplicateSender.id;

                duplicate.ScoreTrigger = duplicateSender;

                var duplicateLabel = GetChildByAttribute(duplicate, "class", "VariableSelectorVariableLabel", true).parentNode.parentNode.parentNode;
                duplicateLabel.id = "_" + duplicateLabel.id;

                container.innerHTML = "<table style=\"height:100%;width:100%;\" cellspacing=\"0\" cellpadding=\"0\"><tr><td></td></tr></table>";

                container.getElementsByTagName("td").item(0).appendChild(duplicate);

                document.body.appendChild(container);

                container.style.width = (window.innerWidth - 200) + "px";
                //container.style.height = (window.innerHeight - 200) + "px";

                duplicate.ShowScores(
                    duplicate,
                    true
                );
            }, 500);

            return;
        }

        if (editVariableLabelEnabled == true)
            return;

        _AjaxRequest("/Handlers/VariableSelector.ashx", "GetScores", "IdLanguage=" + control.Settings.IdLanguage + "&Source=" + control.Settings.Source + "&XPath=" + control.Settings.Path, function (response, control) {
            if (editVariableLabelEnabled == true)
                return;

            control.ScoreTrigger.onclick = function () {
                control.HideScores();
            };

            var container = document.getElementById("LinkBiScores" + control.id);

            if (container != undefined) {
                container.parentNode.removeChild(container);
            }

            container = document.createElement("div");
            container.id = "LinkBiScores" + control.id;
            container.className = "LinkBiScores BorderColor1";
            container.style.visibility = "hidden";
            container.style.background = "transparent";

            var result = JSON.parse(response);

            //var scoreGroups = new Array();
            scoreGroups[control.id] = new Array();

            // Run through all scores of the variable.
            for (var i = 0; i < result.Scores.length; i++) {
                var scoreControl = RenderScore(control, control.Settings.Source, control.Settings.Path, result.Scores[i], false, control.Settings.Editable);

                container.appendChild(scoreControl);

                if (result.Scores[i].Type == "ScoreGroup") {
                    scoreGroups[control.id].push(result.Scores[i]);
                }
            }

            control.appendChild(container);

            // Run through all scores of the variable.
            for (var i = 0; i < scoreGroups[control.id].length; i++) {
                RenderScoreGroupColors(scoreGroups[control.id][i]);
            }

            if (smoothEffect) {
                var height = container.offsetHeight - 4;

                if (scrollScores) {
                    if ((height + container.offsetTop) > (window.innerHeight - 100)) {
                        height = (window.innerHeight - 160) - GetOffsetTop(container);

                        applyMaxHeight = true;

                        container.style.overflowY = "auto";
                    }
                }

                container.style.height = "0px";
                container.style.visibility = "";
                container.style.background = "";

                if (scrollScores) {
                    var maxHeight = (window.innerHeight - 160) - GetOffsetTop(container);
                    container.style.maxHeight = maxHeight + "px";
                    container.style.overflowY = "auto";
                }

                IncreaseHeight(container, height, function () {
                    //container.style.minHeight = container.style.height;
                    container.style.height = "";

                    control.ScoreTrigger.src = "/Images/Icons/VariableSelector/Up.png";
                });
            }
            else {
                container.style.visibility = "";

                var height = container.offsetHeight;

                if (scrollScores) {

                    //container.style.minHeight = height + "px";
                    container.style.height = "";
                    container.style.overflowY = "auto";

                    var maxHeight = (window.innerHeight - 160) - GetOffsetTop(container);
                    container.style.maxHeight = maxHeight + "px";

                    control.ScoreTrigger.src = "/Images/Icons/VariableSelector/Up.png";
                }
            }

            if (onFinish != undefined)
                onFinish();
        }, control);
    };

    control.HideScores = function () {
        var sender = this;

        var container = document.getElementById("LinkBiScores_" + control.id);

        if (container == undefined)
            return;

        container.style.height = container.style.minHeight;
        container.style.minHeight = "";

        DecreaseHeight(container, 0, function () {

            if (container == undefined || container.parentNode == undefined)
                return;

            container.parentNode.removeChild(container);

            sender.src = "/Images/Icons/VariableSelector/Down.png";

            var fullScreenContainer = document.getElementById("FullScreenVariableSelectorControlContainer");
            var fullScreenBackground = document.getElementById("FullScreenVariableSelectorControlContainerBackground");

            if (fullScreenContainer != undefined)
                fullScreenContainer.parentNode.removeChild(fullScreenContainer);

            if (fullScreenBackground == undefined)
                return;

            DecreaseHeight(fullScreenBackground, 0, function () {
                fullScreenBackground.parentNode.removeChild(fullScreenBackground);

                try {
                    SearchVariables(undefined, _idLanguage, _enableDataCheck);
                }
                catch (e) { }
            }, true);

            DecreaseWidth(fullScreenBackground, 0, false);
        });
    }

    control.ScoreTrigger.onclick = function () {
        control.ShowScores(this.Control);
    };
}

var scrollScores = false;
var scoreGroups = new Object();
var _idLanguage;
var _enableCombine;
var _fullScreen;
var _editable;
var _onCrosstable;


function RenderScore(sender, source, xPath, score, isScoreGroup, editable) {
    if (score.Type == "ScoreGroup") {
        var container = document.createElement("table");
        var tr = document.createElement("tr");
        var tdSwiper = document.createElement("td");
        var tdScoreGroup = document.createElement("td");

        container["Type"] = "LinkBiScoreGroup";
        container["Identity"] = score.Id;
        container["Order"] = score.Order;
        container["Source"] = score.Source;
        container["Path"] = score.Path;
        container["Sender"] = sender;

        container.className = "LinkBiScore";
        container.id = "LinkBiScoreGroup" + score.Id;
        container.style.width = "100%";
        container.cellPadding = 0;
        container.cellSpacing = 0;
        container.style.background = "#" + score.Color;

        tdSwiper.className = "BackgroundColor6";
        tdSwiper.style.width = "60px";
        tdSwiper.style.minHeight = "60px";
        tdSwiper.innerHTML = "<img id=\"LinkBiScoreSwiper" + score.Id + "\" src=\"/Images/Icons/Swiper.png\" class=\"LinkBiScoreOptionSwiper\" style=\"\" />";

        tdScoreGroup.style.width = "100%";

        var control = document.createElement("div");
        control.className = "LinkBiScoreGroup";
        control["Type"] = "LinkBiScoreGroup";
        control["Identity"] = score.Id;
        control["Order"] = score.Order;
        control["Source"] = score.Source;
        control["Path"] = score.Path;
        control["Sender"] = sender;

        //control.setAttribute("oncontextmenu", "DeleteScoreGroup(this, '" + source + "', '" + xPath + "', '"+ score.Source +"', '" + score.Path + "');return false;")
        control.setAttribute("oncontextmenu", "ShowScoreMenu(this, '" + score["Source"] + "', '" + score["Path"] + "');return false;");

        var lbl = document.createElement("div");
        //lbl.className = "LinkBiScoreGroupName";
        //lbl.type = "text";
        //lbl.value = score.Name;
        lbl.style.cursor = "pointer";

        var html = "";

        html += "<table style=\"width:100%;height:60px;\" cellpadding=\"0\" cellspacing=\"0\"><tr>";

        if (sender.Settings.EnableCombine)
            html += "<td class=\"LinkBiScoreDragHandle\" id=\"LinkBiScoreDragHandle" + score.Id + "\"></td>";

        //html += "<td class=\"BackgroundColor6\" style=\"width:60px;\"><img style=\"height:60px;\" src=\"/Images/Icons/VariableSelector/ScoreGroup.png\" /></td>";

        html += "<td style=\"width:100%;\"><input id=\"txtScoreGroupName" + score.Id + "\" type=\"text\" class=\"LinkBiScoreGroupName\" value=\"" + score.Name + "\" /></td>";

        html += "</tr></table>";

        lbl.innerHTML = html;


        //lbl.setAttribute("onchange", "UpdateScoreGroupName('" + score["Source"] + "', '" + score["Path"] + "', this.value);")


        control.appendChild(lbl);

        window.setTimeout(function () {
            SetAttribute("LinkBiScoreDragHandle" + score.Id, "onmousedown", "DragScore(this.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode, '" + source + "', '" + xPath + "');");
            SetAttribute("txtScoreGroupName" + score.Id, "onchange", "UpdateScoreGroupName(this, '" + score["Source"] + "', '" + score["Path"] + "', this.value);");
            SetAttribute("txtScoreGroupName" + score.Id, "onkeydown", "if(event.keyCode != 13) return; UpdateScoreGroupName(this, '" + score["Source"] + "', '" + score["Path"] + "', this.value); this.blur(); return false;");
            SetAttribute("LinkBiScoreSwiper" + score.Id, "onclick", "ShowScoreOptions(this, '" + score["Source"] + "', '" + score["Path"] + "', " + editable + ");");

            document.getElementById("LinkBiScoreSwiper" + score.Id).Sender = sender;
        }, 10);

        tdScoreGroup.appendChild(control);

        tr.appendChild(tdSwiper);
        tr.appendChild(tdScoreGroup);

        container.appendChild(tr);

        return container;
    }
    else {
        var control = document.createElement("div");

        if (!isScoreGroup)
            control.id = "LinkBiScore" + score.Id;
        control.className = "LinkBiScore WhiteBackground Color1";
        control["Identity"] = score.Id;
        control["Order"] = score.Order;
        control["Type"] = "LinkBiScore";
        control["Source"] = score.Source;
        control["Path"] = score.Path;
        control["Sender"] = sender;
        control["Enabled"] = score.Enabled;

        control.setAttribute("oncontextmenu", "ShowScoreMenu(this, '" + score["Source"] + "', '" + score["Path"] + "');return false;");

        if (score.Enabled == false)
            control.className = "LinkBiScore Color1 LightGrayBackground";

        control["IsScoreGroup"] = isScoreGroup;

        var html = "";

        html += "<table style=\"width:100%;\" cellpadding=\"0\" cellspacing=\"0\"><tbody><tr>";

        if (!isScoreGroup)
            html += "<td class=\"BackgroundColor6\" style=\"width:60px;\"><img id=\"LinkBiScoreSwiper" + score.Id + "\" src=\"/Images/Icons/Swiper.png\" class=\"LinkBiScoreOptionSwiper\" style=\"\" /></td>";

        html += "<td style=\"padding:2px;\"><table id=\"ScoreGroupColors" + score.Id + "\" cellspacing=\"0\" cellpadding=\"0\" style=\"height:30px;width:4px;\"><tbody></tbody></table></td>";

        if (sender.Settings.EnableCombine)
            html += "<td class=\"LinkBiScoreDragHandle\" id=\"LinkBiScoreDragHandle" + score.Id + "\"></td>";

        if (!isScoreGroup)
            html += "<td id=\"LinkBiScoreLabel" + score.Id + "\" class=\"\" style=\"padding:5px;width:100%;\">";
        else
            html += "<td style=\"padding:10px;width:100%;white-space:nowrap;\" class=\"\">";

        if (!isScoreGroup)
            html += "<div id=\"txtScoreLabel" + score.Id + "\" class=\"Color1\">" + score.Label + "</div>";
        else
            html += "<span>" + score.Label + "</span>";

        //if (editable)
        //html += "<input type=\"text\" id=\"txtScoreLabel"+ score.Id +"\" class=\"Color1\" value=\"" + score.Label + "\" />";
        //else
        //html += "<span>" + score.Label + "</span>"

        html += "</td>";

        html += "</tr></tbody></table>";

        control.innerHTML = html;


        if (!isScoreGroup) {
            window.setTimeout(function () {
                if (window.location.href.indexOf("Exports.aspx") == -1 && window.location.href.indexOf("TaxonomyManager.aspx") == -1) {
                    SetAttribute("LinkBiScoreDragHandle" + score.Id, "onmousedown", "DragScore(this.parentNode.parentNode.parentNode.parentNode ,'" + source + "', '" + xPath + "');");
                    SetAttribute("LinkBiScoreSwiper" + score.Id, "onclick", "ShowScoreOptions(this, '" + score["Source"] + "', '" + score["Path"] + "', " + editable + ");");
                    SetAttribute("txtScoreLabel" + score.Id, "onblur", "UpdateScoreLabel(this.parentNode, '" + score["Source"] + "', '" + score["Path"] + "', '" + score.Id + "', this.innerHTML); this.removeAttribute('contenteditable');");
                    SetAttribute("txtScoreLabel" + score.Id, "onclick", "this.setAttribute('contenteditable', 'true');this.focus(this.innerHTML.length - 1);");
                }
                SetAttribute("txtScoreLabel" + score.Id, "onfocus", "SetCursorToEnd(this);document.body.removeAttribute('onselectstart');document.body.removeAttribute('unselectable');");
                SetAttribute("txtScoreLabel" + score.Id, "onkeydown", "if(event.keyCode != 13) return; this.blur(); return false;");

                document.getElementById("LinkBiScoreSwiper" + score.Id).Sender = sender;
            }, 100);
        }
        else {
            //control.setAttribute("oncontextmenu", "DeleteScore(this, \"" + xPath + "\",\"" + score.Path + "\"); return false;")
            control.oncontextmenu = function () {
                DeleteScore(this, score["Source"], xPath, score["Path"]);
                return false;
            };
        }

        return control;
    }
}

function SetCursorToEnd(div) {
    window.setTimeout(function () {
        var sel, range;
        if (window.getSelection && document.createRange) {
            range = document.createRange();
            range.selectNodeContents(div);
            //range.collapse(true);
            sel = window.getSelection();
            sel.removeAllRanges();
            sel.addRange(range);
        } else if (document.body.createTextRange) {
            range = document.body.createTextRange();
            range.moveToElementText(div);
            range.collapse(true);
            range.select();
        }
    }, 1);
}

function RenderScoreGroupColors(score) {
    if (score.Type != "ScoreGroup")
        return;

    for (var i = 0; i < score.Scores.length; i++) {
        var control = document.getElementById("LinkBiScore" + score.Scores[i].Id);

        if (control == undefined)
            continue;

        var colorControl = document.getElementById("ScoreGroupColors" + score.Scores[i].Id);

        var color = document.getElementById("scoreGroup" + score.Scores[i].Id + "_GroupColor" + score.Id);

        if (color != undefined) {
            color.style.background = "#" + score.Color;
        }
        else {
            colorControl.getElementsByTagName("tbody").item(0).innerHTML += "<tr><td id=\"scoreGroup" + score.Scores[i].Id + "_GroupColor" + score.Id + "\" style=\"background:#" + score.Color + ";width:100%;\"></td></tr>";
        }
    }
}

var equationScoreSource;
var equationScorePath;
function ShowScoreMenu(sender, source, path) {
    if (_enableCombine == false)
        return;

    _AjaxRequest("/Handlers/VariableSelector.ashx", "GetScore", "Source=" + source + "&Path=" + path, function (response) {
        var idSender = sender["Sender"].id;

        var score = JSON.parse(response);

        var menu = InitMenu("menuScore" + score.Id);

        if (sender.Type == "LinkBiScoreGroup") {
            if (sender.Sender.Settings.ScoreOptions.Equation) {
                var lnkEquation = document.createElement("div");

                lnkEquation.ImageUrl = "/Images/Icons/Menu/Equation.png";
                lnkEquation.innerHTML = LoadLanguageText("DefineEquation");

                lnkEquation.MenuItemClick = "EditEquation('" + score.Source + "', '" + score.Path + "');";

                menu.Items.push(lnkEquation);
            }

            var lnkDelete = document.createElement("div");

            lnkDelete.ImageUrl = "/Images/Icons/Menu/Delete.png";
            lnkDelete.innerHTML = LoadLanguageText("Delete");

            lnkDelete.MenuItemClick = "DeleteScoreGroup(" +
                "document.getElementById('" + idSender + "'), " +
                "'" + score.Source + "', " +
                "'" + score.Path + "'," +
                "'" + sender["Source"] + "', " +
                "'" + sender["Path"] + "'" +
            ");";

            menu.Items.push(lnkDelete);
        }
        else {
            if (window.location.href.indexOf("Exports.aspx") == -1 && window.location.href.indexOf("TaxonomyManager.aspx") == -1) {
                if (sender.Sender.Settings.EnableCombine) {
                    var lnkNewGroup = document.createElement("div");

                    lnkNewGroup.ImageUrl = "/Images/Icons/Menu/CreateNewGroup.png";
                    lnkNewGroup.innerHTML = LoadLanguageText("CreateNewScoreGroupMenuItem");

                    lnkNewGroup.MenuItemClick = "Combine('" +
                        idSender + "', '" +
                        sender["Source"] + "', '" +
                        sender["Path"] + "', '" +
                        "" + "', '" +
                        "" + "');this.parentNode.onmouseout();";

                    menu.Items.push(lnkNewGroup);

                    var lnkInsertMean = document.createElement("div");

                    //lnkInsertMean.ImageUrl = "/Images/Icons/Menu/InsertMean.png";
                    lnkInsertMean.innerHTML = LoadLanguageText("InsertMeanScore");

                    lnkInsertMean.MenuItemClick = "InsertMeanScore('" +
                        idSender + "', '" +
                        sender["Source"] + "', '" +
                        sender["Path"] + "', '" +
                        "" + "', '" +
                        "" + "');this.parentNode.onmouseout();";

                    menu.Items.push(lnkInsertMean);

                    var lnkInsertStandardDeviation = document.createElement("div");

                    //lnkInsertStandardDeviation.ImageUrl = "/Images/Icons/Menu/InsertStandardDeviation.png";
                    lnkInsertStandardDeviation.innerHTML = LoadLanguageText("InsertStandardDeviationScore");

                    lnkInsertStandardDeviation.MenuItemClick = "InsertStandardDeviation('" +
                        idSender + "', '" +
                        sender["Source"] + "', '" +
                        sender["Path"] + "', '" +
                        "" + "', '" +
                        "" + "');this.parentNode.onmouseout();";

                    menu.Items.push(lnkInsertStandardDeviation);

                    var lnkInsertStandardError = document.createElement("div");

                    //lnkInsertStandardError.ImageUrl = "/Images/Icons/Menu/InsertStandardError.png";
                    lnkInsertStandardError.innerHTML = LoadLanguageText("InsertStandardError");

                    lnkInsertStandardError.MenuItemClick = "InsertStandardError('" +
                        idSender + "', '" +
                        sender["Source"] + "', '" +
                        sender["Path"] + "', '" +
                        "" + "', '" +
                        "" + "');this.parentNode.onmouseout();";

                    menu.Items.push(lnkInsertStandardError);

                    var lnkInsertSampleVariance = document.createElement("div");

                    //lnkInsertSampleVariance.ImageUrl = "/Images/Icons/Menu/InsertSampleVariance.png";
                    lnkInsertSampleVariance.innerHTML = LoadLanguageText("InsertSampleVariance");

                    lnkInsertSampleVariance.MenuItemClick = "InsertSampleVariance('" +
                        idSender + "', '" +
                        sender["Source"] + "', '" +
                        sender["Path"] + "', '" +
                        "" + "', '" +
                        "" + "');this.parentNode.onmouseout();";

                    //menu.Items.push(lnkInsertSampleVariance);

                    for (var i = 0; i < scoreGroups[idSender].length; i++) {
                        var isPartOfGroup = false;

                        var groupItemId = "";
                        var groupItemSource = "";
                        var groupItemPath = "";

                        for (var a = 0; a < scoreGroups[idSender][i].Scores.length; a++) {
                            if (scoreGroups[idSender][i].Scores[a].Id == score.Id) {
                                isPartOfGroup = true;

                                groupItemId = scoreGroups[idSender][i].Scores[a].Id;
                                groupItemSource = scoreGroups[idSender][i].Scores[a].Source;
                                groupItemPath = scoreGroups[idSender][i].Scores[a].Path;

                                break;
                            }
                        }

                        if (isPartOfGroup) {
                            var lnkCombine = document.createElement("div");

                            lnkCombine.ImageUrl = "/Images/Icons/Menu/RemoveFromGroup.png";
                            lnkCombine.innerHTML = LoadLanguageText("RemoveFromGroupMenuItem").replace('{0}', scoreGroups[idSender][i].Name);

                            lnkCombine.MenuItemClick = "RemoveScoreFromGroup('" +
                                idSender + "', '" +
                                groupItemId + "', '" +
                                scoreGroups[idSender][i].Id + "', '" +
                                scoreGroups[idSender][i]["Source"] + "', '" +
                                scoreGroups[idSender][i]["Path"] + "', '" +
                                groupItemSource + "', '" +
                                groupItemPath + "');this.parentNode.onmouseout();";

                            menu.Items.push(lnkCombine);
                        }
                        else {
                            var lnkCombine = document.createElement("div");

                            lnkCombine.ImageUrl = "/Images/Icons/Menu/CombineWithGroup.png";
                            lnkCombine.innerHTML = LoadLanguageText("CombineGroupMenuItem").replace('{0}', scoreGroups[idSender][i].Name);
                            if (scoreGroups[idSender][i].Name != "Mean") {
                                if (scoreGroups[idSender][i].Name != "Standard deviation") {
                                    if (scoreGroups[idSender][i].Name != "Standard error") {
                                        lnkCombine.MenuItemClick = "Combine('" +
                                            idSender + "', '" +
                                            scoreGroups[idSender][i]["Source"] + "', '" +
                                            scoreGroups[idSender][i]["Path"] + "', '" +
                                            sender["Source"] + "', '" +
                                            sender["Path"] + "');this.parentNode.onmouseout();";

                                        menu.Items.push(lnkCombine);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        menu.Render();
    });
}

function Combine(idSender, source, path, source2, path2) {
    _AjaxRequest("/Handlers/VariableSelector.ashx", "CombineScales", "Source=" + source + "&Path=" + path + "&Source2=" + source2 + "&Path2=" + path2 + "&Name=" + "", function (response) {
        var score = JSON.parse(response);

        var isExisting = false;

        for (var i = 0; i < scoreGroups[idSender].length; i++) {
            if (scoreGroups[idSender][i].Id == score.Id) {
                scoreGroups[idSender][i] = score;
                isExisting = true;
                break;
            }
        }

        if (!isExisting) {
            var sender = document.getElementById(idSender);

            sender.ShowScores(sender, true, false);
            /*ShowScores(sender, false, _idLanguage, sender["Source"], sender["Path"], false, undefined, _editable, _onCrosstable, _enableCombine);*/
        }

        RenderScoreGroupColors(score);
    });
}

function InsertMeanScore(idSender, source, path) {
    _AjaxRequest("/Handlers/VariableSelector.ashx", "InsertMeanScore", "Source=" + source + "&Path=" + path, function (response) {
        var sender = document.getElementById(idSender);

        sender.ShowScores(sender, true, false);
    });
}

function InsertStandardDeviation(idSender, source, path) {
    _AjaxRequest("/Handlers/VariableSelector.ashx", "InsertStandardDeviation", "Source=" + source + "&Path=" + path, function (response) {
        var sender = document.getElementById(idSender);

        sender.ShowScores(sender, true, false);
    });
}

function InsertStandardError(idSender, source, path) {
    _AjaxRequest("/Handlers/VariableSelector.ashx", "InsertStandardError", "Source=" + source + "&Path=" + path, function (response) {
        var sender = document.getElementById(idSender);

        sender.ShowScores(sender, true, false);
    });
}

function InsertSampleVariance(idSender, source, path) {
    _AjaxRequest("/Handlers/VariableSelector.ashx", "InsertSampleVariance", "Source=" + source + "&Path=" + path, function (response) {
        var sender = document.getElementById(idSender);

        sender.ShowScores(sender, true, false);
    });
}

function RemoveScoreFromGroup(idSender, idScore, idScoreGroup, groupSource, groupPath, source, path) {
    _AjaxRequest("/Handlers/VariableSelector.ashx", "RemoveScoreFromGroup", "GroupSource=" + groupSource + "&GroupPath=" + groupPath + "&Source=" + source + "&Path=" + path, function (response) {
        //document.getElementById('ScoreGroupColors' + idScore).innerHTML = '<tbody></tbody>';

        var tableCell = document.getElementById("scoreGroup" + idScore + "_GroupColor" + idScoreGroup);

        if (tableCell != undefined) {
            tableCell.parentNode.parentNode.removeChild(tableCell.parentNode);
        }

        _AjaxRequest("/Handlers/VariableSelector.ashx", "GetScore", "Source=" + groupSource + "&Path=" + groupPath, function (response) {
            var score = JSON.parse(response);

            for (var i = 0; i < scoreGroups[idSender].length; i++) {
                if (scoreGroups[idSender][i].Id == score.Id) {
                    scoreGroups[idSender][i] = score;
                    break;
                }
            }

            //RenderScoreGroupColors(score);
        });
    });
}


function ShowVariableOptions(sender, source, path, editable) {
    if (window.location.href.indexOf("Exports.aspx") == -1 && window.location.href.indexOf("TaxonomyManager.aspx") == -1) {
        _AjaxRequest("/Handlers/VariableSelector.ashx", "GetScores", "Source=" + source + "&XPath=" + path, function (response) {
            var result = JSON.parse(response);            
            // Run through all scores of the variable.
            for (var i = 0; i < result.Scores.length; i++) {
                var score = result.Scores[i];

                var swiper = document.getElementById("LinkBiScoreSwiper" + score.Id);

                if (swiper == undefined)
                    continue;

                _ShowScoreOptions(swiper, score, editable);
            }

            sender.setAttribute("onclick", "HideVariableOptions(this, '" + source + "', '" + path + "', " + editable + ");");
        });
    }
}

function HideVariableOptions(sender, source, path, editable) {
    if (window.location.href.indexOf("Exports.aspx") == -1 && window.location.href.indexOf("TaxonomyManager.aspx") == -1) {
        _AjaxRequest("/Handlers/VariableSelector.ashx", "GetScores", "Source=" + source + "&XPath=" + path, function (response) {
            var result = JSON.parse(response);

            // Run through all scores of the variable.
            for (var i = 0; i < result.Scores.length; i++) {
                var score = result.Scores[i];

                var swiper = document.getElementById("LinkBiScoreSwiper" + score.Id);

                if (swiper == undefined)
                    continue;

                HideScoreOptions(swiper, score.Id, score.Source, score.Path);
            }

            sender.setAttribute("onclick", "ShowVariableOptions(this, '" + source + "', '" + path + "', " + editable + ");");
        });
    }
}

function ShowScoreOptions(sender, source, path, editable) {
    if (window.location.href.indexOf("Exports.aspx") == -1 && window.location.href.indexOf("TaxonomyManager.aspx") == -1) {
        _AjaxRequest("/Handlers/VariableSelector.ashx", "GetScore", "Source=" + source + "&Path=" + path, function (response) {
            var score = JSON.parse(response);

            _ShowScoreOptions(sender, score, editable);
        });
    }
}

function _ShowScoreOptions(sender, score, editable) {    
    if (document.getElementById("LinkBiScoreOptions" + score.Id) != undefined)
        return;

    var options = document.createElement("div");
    options.className = "ScoreOptions DarkGrayBackground";
    options.id = "LinkBiScoreOptions" + score.Id;

    var html = "";

    html += "<table cellspacing=\"0\" cellpadding=\"0\"><tr>";

    if (sender.Sender.Settings.ScoreOptions.Name)
        html += "<td>" + LoadLanguageText("Name") + "</td><td><input id=\"txtScoreName" + score.Id + "\" type=\"text\" style=\"width:200px;\" value=\"" + score.Name + "\" /></td>";

    if (sender.Sender.Settings.ScoreOptions.Factor)
        html += "<td>" + LoadLanguageText("Factor") + "</td><td><input id=\"txtScoreFactor" + score.Id + "\" type=\"number\"  step=\"any\" maxlength=\"2\" onkeypress=\"return isNumberWithDot(event)\"  onkeyup=\"if(!validnum(this)) this.value='';\" style=\"width:90px;\" value=\"" + score.Value + "\" /></td>";

    html += "</tr></table>";

    options.innerHTML = html;

    options.style.visibility = "hidden";

    sender.parentNode.appendChild(options);

    var width = options.offsetWidth;

    options.style.width = "0px";
    options.style.height = sender.parentNode.offsetHeight + "px";
    options.style.visibility = "";
    options.style.marginLeft = sender.offsetWidth + "px";

    var buttonContainer = document.createElement("td");
    var buttons = document.createElement("div");
    buttons.className = "ScoreOptions DarkGrayBackground";
    buttons.id = "LinkBiScoreButtons" + score.Id;

    var html = "";

    html += "<table cellspacing=\"0\" cellpadding=\"0\"><tr>";

    if (sender.Sender.Settings.ScoreOptions.Delete)
        html += "<td>" + LoadLanguageText("Delete") + "</td>";

    if (sender.Sender.Settings.ScoreOptions.Hide) {
        html += "<td style=\"min-width:60px;cursor:pointer\" id=\"btnScoreEnabled" + score.Id + "\">";

        if (score.Enabled)
            html += LoadLanguageText("Hide");
        else
            html += LoadLanguageText("Show");

        html += "</td>";
    }

    html += "</tr></table>";

    buttons.innerHTML = html;

    buttons.style.visibility = "hidden";

    buttonContainer.appendChild(buttons);
    sender.parentNode.parentNode.appendChild(buttonContainer);

    var width2 = buttons.offsetWidth;

    buttons.style.width = "0px";
    buttons.style.height = sender.parentNode.offsetHeight + "px";
    buttons.style.visibility = "";

    IncreaseWidth(options, width + 100);
    IncreaseWidth(buttons, width2, false);

    sender.setAttribute("onclick", "HideScoreOptions(this, '" + score.Id + "', '" + score["Source"] + "', '" + score["Path"] + "', " + editable + ");");

    SetAttribute("txtScoreName" + score.Id, "onchange", "SetScoreName('" + score["Source"] + "','" + score["Path"] + "',this.value);");
    SetAttribute("txtScoreFactor" + score.Id, "onchange", "SetScoreFactor('" + score["Source"] + "','" + score["Path"] + "',this.value);");

    if (score.Enabled == false) {
        SetAttribute("btnScoreEnabled" + score.Id, "onclick", "ShowScore(this, '" + score["Source"] + "','" + score["Path"] + "', '" + score.Id + "');");
    }
    else {
        SetAttribute("btnScoreEnabled" + score.Id, "onclick", "HideScore(this, '" + score["Source"] + "','" + score["Path"] + "', '" + score.Id + "');");
    }
}

function SetAttribute(idElement, name, value) {
    var element = document.getElementById(idElement);

    if (element == undefined)
        return;

    element.setAttribute(name, value);
}

function HideScoreOptions(sender, idScore, source, path, editable) {
    
    var options = document.getElementById("LinkBiScoreOptions" + idScore);
    var buttons = document.getElementById("LinkBiScoreButtons" + idScore);

    if (options == undefined)
        return;

    DecreaseWidth(options, 0, false, function () {
        if (options != undefined && options.parentNode != undefined)
            options.parentNode.removeChild(options);

        sender.setAttribute("onclick", "ShowScoreOptions(this, '" + source + "', '" + path + "', " + editable + ");");
    });

    DecreaseWidth(buttons, 0, true, function () {
        if (buttons != undefined && buttons.parentNode != undefined)
            buttons.parentNode.parentNode.removeChild(buttons.parentNode);
    });
}


function DeleteScoreGroup(sender, sourceScoreGroup, xPathScoreGroup, source, xPath) {
    _AjaxRequest("/Handlers/VariableSelector.ashx", "DeleteScoreGroup", "Source=" + sourceScoreGroup + "&XPath=" + xPathScoreGroup, function (response) {
        sender.ShowScores(sender, true, false);
    });
}

function DeleteScore(sender, source, xPath, xPathScore) {
    sender.onclick = function () {
        _AjaxRequest("/Handlers/VariableSelector.ashx", "DeleteScore", "Source=" + source + "&XPath=" + xPathScore, function (response) {
            sender["Sender"].ShowScores(sender["Sender"], true, false, function (result) {
                var scores = document.getElementById("LinkBiScoreGroupScores" + sender.IdScoreGroup);
                scores.style.height = "";
                scores.style.height = scores.offsetHeight + "px";
            });
        });
    }

    sender.style.background = "url('/Images/Icons/Cloud/Delete.png') 50% 50% no-repeat #FF0000";
    sender.style.color = "transparent";

    sender.setAttribute("onmouseout", "this.onclick = undefined;this.style.background='';this.style.color = '';")
}


function UpdateScoreGroupName(sender, source, xPath, value) {
    _AjaxRequest("/Handlers/VariableSelector.ashx", "UpdateScoreGroupName", "IdLanguage=" + sender.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode.Sender.Settings.IdLanguage + "&Source=" + source + "&Path=" + xPath + "&Value=" + encodeURIComponent(value), function (response) {
        //TurnGreen(sender.parentNode, [156, 84, 184]);

        var idSender = sender.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode["Sender"].id;

        for (var i = 0; i < scoreGroups[idSender].length; i++) {
            if (scoreGroups[idSender][i].Path == xPath) {
                scoreGroups[idSender][i].Name = value;
                break;
            }
        }
    });
}

function UpdateScoreLabel(sender, source, xPath, idScore, value) {
    _AjaxRequest("/Handlers/VariableSelector.ashx", "UpdateScoreLabel", "IdLanguage=" + sender.parentNode.parentNode.parentNode.parentNode.Sender.Settings.IdLanguage + "&Source=" + source + "&XPath=" + xPath + "&IdScore=" + idScore + "&Value=" + encodeURIComponent(value), function (response) {
        TurnGreen(sender, [255, 255, 255]);
    });
}

function SetScoreFactor(source, path, value) {
    _AjaxRequest("/Handlers/VariableSelector.ashx", "SetScoreFactor", "Source=" + source + "&Path=" + path + "&Value=" + value, function (response) {
    });
}

function SetScoreName(source, path, value) {
    _AjaxRequest("/Handlers/VariableSelector.ashx", "SetScoreName", "IdLanguage=" + _idLanguage + "&Source=" + source + "&Path=" + path + "&Value=" + encodeURIComponent(value), function (response) {
    });
}


function DragScore(sender, source, xPath) {
    //if (event.button != 0 || event.target.parentNode.parentNode.parentNode.parentNode != sender)
    //    return;

    sender.style.position = "absolute";
    sender.style.opacity = "0.8";
    sender.style.width = (sender.parentNode.offsetWidth - 20) + "px";
    sender.style.marginLeft = "-" + (document.getElementById("Content").scrollLeft) + "px";

    sender.className = "LinkBiScore WhiteColor GrayBackground";

    window.setTimeout(function () {
        mouseMoveFunctions.Add("DragScore", function () {
            //sender.style.top = (tempY - (sender.offsetHeight / 2) - sender.parentNode.offsetTop + sender.parentNode.scrollTop) + "px";
            sender.style.top = (tempY - (sender.offsetHeight / 2)) + "px";

            sender.className = "LinkBiScore WhiteColor GrayBackground";

            for (var i = 0; i < sender.parentNode.childNodes.length; i++) {
                var item = sender.parentNode.childNodes.item(i);

                /*if (item.className != (item["Type"] + " BackgroundColor1") && item.className != (item["Type"] + " BackgroundColor2"))
                    continue;*/
                //var itemOffsetTop = item.offsetTop + item.parentNode.offsetTop;

                if (item["Identity"] == sender["Identity"])
                    continue;

                //var itemOffsetTop = item.offsetTop + GetOffsetTop(item.parentNode);
                //var itemOffsetTop = item.offsetTop + item.parentNode.offsetTop + item.parentNode.parentNode.offsetTop + item.parentNode.parentNode.parentNode.offsetTop;
                var itemOffsetTop = GetOffsetTop(item) - GetParentScrollTop(item);

                if ((itemOffsetTop + ((item.offsetHeight / 2))) >= sender.offsetTop) {
                    if ((itemOffsetTop + ((item.offsetHeight / 2) - 10)) >= sender.offsetTop) {
                        if ((itemOffsetTop + ((item.offsetHeight / 2))) <= (sender.offsetTop + sender.offsetHeight)) {
                            if ((itemOffsetTop + ((item.offsetHeight / 2) + 10)) <= (sender.offsetTop + sender.offsetHeight)) {
                                item.className = item["Type"] + " WhiteColor GreenBackground";

                                item.style.borderTop = "";
                                item.style.borderBottom = "";
                                continue;
                            }
                            else {
                                item.style.borderTop = "3px dotted #000000";
                                item.style.borderBottom = "";

                                if (item["Type"] == "LinkBiScoreGroup")
                                    item.className = "LinkBiScore";
                                else
                                    item.className = item["Type"] + " Color1 WhiteBackground";

                                sender.className = "LinkBiScore WhiteColor GreenBackground";

                                continue;
                            }
                        }
                    }
                    else {
                        item.style.borderTop = "";
                        item.style.borderBottom = "3px dotted #000000";

                        if (item["Type"] == "LinkBiScoreGroup")
                            item.className = "LinkBiScore";
                        else
                            item.className = item["Type"] + " Color1 WhiteBackground";

                        sender.className = "LinkBiScore WhiteColor GreenBackground";

                        continue;
                    }
                }

                item.style.borderBottom = "";
                item.style.borderTop = "";

                if (item["Type"] == "LinkBiScoreGroup")
                    item.className = "LinkBiScore";
                else
                    item.className = item["Type"] + " Color1 WhiteBackground";
            }
        });

        sender.onmouseup = function () {
            mouseMoveFunctions.Delete("DragScore");

            var order;

            for (var i = 0; i < sender.parentNode.childNodes.length; i++) {
                var item = sender.parentNode.childNodes.item(i);

                /*if (item.className != item["Type"] + " BackgroundColor1" && item.className != item["Type"] + " GrayBackground")
                    continue;*/

                if (item["Identity"] == sender["Identity"])
                    continue;

                var itemOffsetTop = GetOffsetTop(item) - GetParentScrollTop(item);

                if ((itemOffsetTop + ((item.offsetHeight / 2))) >= sender.offsetTop) {
                    if ((itemOffsetTop + ((item.offsetHeight / 2) - 10)) >= sender.offsetTop) {
                        if ((itemOffsetTop + ((item.offsetHeight / 2))) <= (sender.offsetTop + sender.offsetHeight)) {
                            if ((itemOffsetTop + ((item.offsetHeight / 2) + 10)) <= (sender.offsetTop + sender.offsetHeight)) {
                                // Combine item.
                                order = item["Order"];

                                /*HideScore(undefined, sender["Source"], sender["Path"], sender["Identity"]);
                                HideScore(undefined, item["Source"], item["Path"], item["Identity"]);*/

                                var scrollTop = sender.parentNode.scrollTop;

                                _AjaxRequest("/Handlers/VariableSelector.ashx", "CombineScales", "Source=" + item["Source"] + "&Path=" + item["Path"] + "&Source2=" + sender["Source"] + "&Path2=" + sender["Path"] + "&Name=" + "", function (response) {
                                    var result = JSON.parse(response);

                                    _AjaxRequest("/Handlers/VariableSelector.ashx", "ReorderScale", "Source=" + result["Source"] + "&XPath=" + result["Path"] + "&Order=" + order, function (response) {
                                        sender["Sender"].ShowScores(sender["Sender"], true, false, function () {
                                            return;

                                            var scores = document.getElementById("LinkBiScoreGroupScores" + result["Id"]);
                                            scores.style.height = "";
                                            scores.style.height = scores.offsetHeight + "px";

                                            var control = document.getElementById("LinkBiScoreGroup" + result["Id"]);
                                            control.parentNode.scrollTop = scrollTop;

                                            control.className = "LinkBiScoreGroup BackgroundColor9";
                                            TurnGreen(control, [156, 84, 184]);
                                        });
                                    });
                                });
                            }
                            else {
                                sender.style.marginLeft = "";
                                sender.style.position = "";
                                sender.style.opacity = "";
                                sender.style.width = "";

                                sender.parentNode.insertBefore(sender, item);

                                var scrollTop = sender.parentNode.scrollTop;
                                // Reorder before item.
                                _AjaxRequest("/Handlers/VariableSelector.ashx", "ReorderScale", "Source=" + sender["Source"] + "&XPath=" + sender["Path"] + "&IdCategory=" + sender["Identity"] + "&Order=" + item["Order"], function (response) {
                                    var idSender = sender.id;

                                    sender["Sender"].ShowScores(sender["Sender"], true, false, function () {
                                        return;

                                        var sender = document.getElementById(idSender);
                                        sender.parentNode.scrollTop = scrollTop;

                                        sender.style.backgroundColor = "rgb(97, 207, 113)";

                                        window.setTimeout(function () {
                                            ConvertColor([255, 255, 255], sender, "Increase", function () {
                                                var background = "WhiteBackground";

                                                if (item["Enabled"] == false)
                                                    background = "LightGrayBackground";

                                                sender.className = item["Type"] + " Color1 " + background;
                                                sender.style.backgroundColor = "";
                                            })
                                        }, 1000);
                                    });
                                });
                            }
                        }
                    }
                    else {
                        sender.style.marginLeft = "";
                        sender.style.position = "";
                        sender.style.opacity = "";
                        sender.style.width = "";

                        if (sender.parentNode.childNodes.length > (i + 1)) {
                            sender.parentNode.insertBefore(sender, sender.parentNode.childNodes.item(i + 1));
                        }
                        else {
                            sender.parentNode.appendChild(sender);
                        }

                        var scrollTop = sender.parentNode.scrollTop;

                        // Reorder after item.
                        _AjaxRequest("/Handlers/VariableSelector.ashx", "ReorderScale", "Source=" + sender["Source"] + "&XPath=" + sender["Path"] + "&IdCategory=" + sender["Identity"] + "&Order=" + (parseInt(item["Order"]) + 1), function (response) {
                            var idSender = sender.id;

                            sender["Sender"].ShowScores(sender["Sender"], true, false);
                            /*ShowScores(
                                sender["Sender"],
                                false,
                                _idLanguage,
                                source,
                                xPath,
                                false,
                                function () {
                                    
                                },
                                _editable,
                                _onCrosstable,
                                _enableCombine
                            );*/
                        });
                    }
                }
            }

            sender.style.marginLeft = "";
            sender.style.position = "";
            sender.style.opacity = "";
            sender.style.width = "";

            for (var i = 0; i < sender.parentNode.childNodes.length; i++) {
                var item = sender.parentNode.childNodes.item(i);

                var background = "WhiteBackground";

                if (item["Enabled"] == false)
                    background = "LightGrayBackground";

                item.style.borderTop = "";
                item.style.borderBottom = "";
                item.className = item["Type"] + " Color1 " + background;
            }
        }
    }, 100);
}


function HideScore(sender, source, xPath, idScore) {
    _AjaxRequest("/Handlers/VariableSelector.ashx", "HideScore", "Source=" + source + "&XPath=" + xPath, function (response) {
        if (sender == undefined)
            return;

        var container = document.getElementById("LinkBiScore" + idScore);

        if (container != undefined)
            container.className = "LinkBiScore Color1 LightGrayBackground";

        //sender.src = "/Images/Icons/Show.png";
        sender.innerHTML = LoadLanguageText("Show");

        sender.setAttribute("onclick", "ShowScore(this, '" + source + "', '" + xPath + "', '" + idScore + "');");
    });
}

function ShowScore(sender, source, xPath, idScore) {
    _AjaxRequest("/Handlers/VariableSelector.ashx", "ShowScore", "Source=" + source + "&XPath=" + xPath, function (response) {
        if (sender == undefined)
            return;

        var container = document.getElementById("LinkBiScore" + idScore);

        if (container != undefined)
            container.className = "LinkBiScore WhiteBackground Color1";

        //sender.src = "/Images/Icons/Hide.png";
        sender.innerHTML = LoadLanguageText("Hide");

        sender.setAttribute("onclick", "HideScore(this, '" + source + "', '" + xPath + "', '" + idScore + "');");
    });
}


var editVariableLabelEnabled = false;
function EditVariableLabel(sender, source, path) {
    editVariableLabelEnabled = true;
    var labelContainer = GetChildByAttribute(sender, "class", "VariableSelectorVariableLabel", true);

    var label = labelContainer.innerHTML;
    var width = labelContainer.offsetWidth;

    //labelContainer.innerHTML = "<input type=\"text\" value=\"" + label + "\" onkeydown=\"alert(event.keyCode);if(event.keyCode != 13) return; UpdateVariableLabel(this, '" + source + "', '" + path + "');return false;\" />";
    labelContainer.innerHTML = "";

    var txtLabel = document.createElement("input");
    txtLabel.type = "text";
    txtLabel.value = label;
    txtLabel.style.width = width + "px";

    txtLabel.setAttribute("onblur", "UpdateVariableLabel(this, '" + source + "', '" + path + "');");
    txtLabel.setAttribute("onkeydown", "if(event.keyCode != 13) return; UpdateVariableLabel(this, '" + source + "', '" + path + "');return false;");

    labelContainer.appendChild(txtLabel);

    txtLabel.focus();

    sender.ondblclick = undefined;
}

function UpdateVariableLabel(sender, source, path) {
    _AjaxRequest("/Handlers/VariableSelector.ashx", "UpdateScoreLabel", "Source=" + source + "&XPath=" + path + "&Value=" + encodeURIComponent(sender.value), function (response) {
        editVariableLabelEnabled = false;

        TurnGreen(sender.parentNode.parentNode.parentNode.parentNode.parentNode, [108, 174, 224]);

        sender.parentNode.innerHTML = sender.value;

        sender.setAttribute("ondblclick", "EditVariableLabel(this, '" + source + "', '" + path + "');");
    });
}

//function ShowVariableMenu(sender, source, path, editable, type, variableName) {
function ShowVariableMenu(sender, type, variableName, settings) {

    var menu = InitMenu("menuVariable" + sender.id);


    if (window.location.href.indexOf("Exports.aspx") == -1 && window.location.href.indexOf("TaxonomyManager.aspx") == -1) {
        if (settings.EnableRename) {
            var lnkRename = document.createElement("div");

            lnkRename.ImageUrl = "/Images/Icons/Menu/Rename.png";
            lnkRename.innerHTML = LoadLanguageText("Rename");
            lnkRename.MenuItemClick = "EditVariableLabel(document.getElementById('" + sender.id + "'), '" + settings.Source + "', '" + settings.Path + "');";

            menu.Items.push(lnkRename);
        }
        if (settings.EnableDelete) {
            var lnkDelete = document.createElement("div");

            lnkDelete.ImageUrl = "/Images/Icons/Menu/Delete.png";
            lnkDelete.innerHTML = LoadLanguageText("Delete");
            lnkDelete.MenuItemClick = "DeleteVariable(document.getElementById('" + sender.id + "'), '" + settings.Source + "', '" + settings.Path + "');";
            if (window.location.href.indexOf("LinkBi.aspx") > -1)
                menu.Items.push(lnkDelete);
        }
    }


    if (type == "2" && settings.EnableCategorize == true) {
        var lnkCategorize = document.createElement("div");

        lnkCategorize.ImageUrl = "/Images/Icons/VariableSelector/Categorize.png";
        lnkCategorize.innerHTML = LoadLanguageText("Categorize");
        lnkCategorize.MenuItemClick = "CreateCategorizeTextVariableAssignment(document.getElementById('" + sender.id + "'), '" + settings.Source + "', '" + settings.Path + "', '" + variableName + "');";

        menu.Items.push(lnkCategorize);
    }

    menu.Render();
}

function DeleteVariable(sender, source, path) {
    sender.parentNode.onclick = undefined;
    _AjaxRequest("/Handlers/VariableSelector.ashx", "DeleteVariable", "Source=" + source + "&Path=" + path, function (response) {
        window.location = window.location;
    });
}

function CreateCategorizeTextVariableAssignment(idSender, source, path, variableName) {
    _AjaxRequest(window.location, "CreateCategorizeTextVariableAssignment", path, function (response) {
        //window.open(response, "_blank");

        document.getElementById("lnkCategorizeTextVariableDownloadAssignment").setAttribute("href", response);
        document.getElementById("txtCategorizeTextVariableStep3VariableName").value = variableName + "_Coded";

        var btn = document.getElementById("btnCategorizeTextVariableStep3FileUpload");
        btn.value = LoadLanguageText("Upload");
        btn.setAttribute("onclick", "CategorizeTextVariable('" + path + "');");

        InitDragBox("boxCategorizeTextVariableControl");
    });
}

function CategorizeTextVariable(path) {
    var _file = document.getElementById('fuCategorizeTextVariableStep3File');

    var variableName = document.getElementById("txtCategorizeTextVariableStep3VariableName").value;

    if (_file.files.length === 0) {
        return;
    }

    var data = new FormData();
    data.append('SelectedFile', _file.files[0]);

    var request = new XMLHttpRequest();
    request.onreadystatechange = function () {
        if (request.readyState == 4) {
            try {
                //window.location = window.location;
                alert("finished");
            } catch (e) {
                var resp = {
                    status: 'error',
                    data: 'Unknown error occurred: [' + request.responseText + ']'
                };
            }
            console.log(resp.status + ': ' + resp.data);
        }
    };

    request.upload.addEventListener('progress', function (e) {
        var progress = Math.ceil((e.loaded * 100) / e.total);
        document.getElementById("LoadingText").innerHTML = progress + '%';
    }, false);

    ShowLoading(document.getElementById("boxCategorizeTextVariableControl"));

    request.open("POST", "Variables.aspx?Method=UploadTextVariableAssignmentFile&VariableName=" + variableName + "&" + path);
    request.send(data);
}


function TurnGreen(element, original) {
    element.style.backgroundColor = "rgb(97, 207, 113)";

    window.setTimeout(function () {
        ConvertColor(original, element, "Increase", function () {
            element.style.backgroundColor = "";
        });
    }, 1000);
}
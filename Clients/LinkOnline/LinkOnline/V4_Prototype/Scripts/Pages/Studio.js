﻿Pages.Studio = {
    Load: function () {
        Menu.ClearSubMenu();
        var items = [];

        for (var i = 0; i < 5; i++) {
            items.push({
                Label: "tab " + (i + 1),
                Click: "Pages.Studio.ShowStudioTab()"
            });
        }

        Menu.ShowSubMenu(items);

        document.getElementById("pnlMain").style.backgroundColor = "";
    },
    ShowStudioTab: function () {
        document.getElementById("pnlMain").style.backgroundColor = "#FFFFFF";
        Menu.HideSubMenu();
        //" + (document.content.offsetWidth - 80) + "px
        //var source = "C:/Projects/Blueocean/Link/LinkManager/Clients/LinkOnline/LinkOnline/Fileadmin/ReportDefinitions/iheartmedia/7c384b95-8d92-4147-bd3c-4949bcceb4c7/ef6770e2-244f-4041-a253-971c40271bc7.xml";
        var source = "D:/Applications/Link/PilotH/Fileadmin/ReportDefinitions/pilot12/7c384b95-8d92-4147-bd3c-4949bcceb4c7/74f143ed-35a1-4574-ac4c-fbef9d2f0eec.xml";
        document.content.innerHTML = "<div class=\"ControlBar\" style=\"background-color:" + document.getElementById("pnlMenu").style.backgroundColor +
            ";\">" +
            "<img src=\"Images/Icons/Link.png\" style=\"float:right;height:40px;\" />" +
            "<img src=\"Images/Icons/New.png\" style=\"height:40px;\" />" +
            "<img src=\"Images/Icons/Save.png\" style=\"height:40px;\" />" +
            "</div><div style=\"margin:40px;\">" +
            "<iframe style=\"width:200px;height:" + (200) + "px;float:left;\" frameborder=\"0\" src=\"Fileadmin/Test/Filter/index.html?" + source + "\"></iframe>" +
            "<iframe style=\"width:450px;margin-left:100px;height:" + (100) + "px;\" frameborder=\"0\" src=\"Fileadmin/Test/Filter2/index.html?" + source + "\"></iframe>" +
            "<iframe style=\"width:50%;float:right;height:" + (650) + "px;\" frameborder=\"0\" src=\"Fileadmin/Test/Sunburst/Image.png\"></iframe>" +
            "<iframe style=\"width:50%;height:" + (200) + "px;\" frameborder=\"0\" src=\"Fileadmin/Test/Table2/index.html\"></iframe>" +
            "<iframe style=\"width:50%;height:" + (200) + "px;\" frameborder=\"0\" src=\"Fileadmin/Test/Table/index.html\"></iframe>" +
            "<iframe style=\"width:100%;height:" + (200) + "px;\" frameborder=\"0\" src=\"Fileadmin/Test/AreaChart/3883195.html\"></iframe>" +
            "</div>";

        Pages.Studio.Test();
    },
    Test: function () {
        return;
        var box = Box.Create("test");

        //box.innerHTML = "<table class=\"TableSettings\" style=\"margin:40px;\" cellpadding=\"0\" cellspacing=\"0\">            <tbody><tr style=\"font-size:16pt;font-weight:normal\">                <td colspan=\"2\">                    <span id=\"cphContent_lblSettingsGeneral\">general</span>                </td>                <td colspan=\"2\">                    <span id=\"cphContent_lblSettingsSignificanceDifference\">significance difference</span>                </td>            </tr>            <tr>                <td>                    <span id=\"cphContent_lblLeftPanelSettingsDecimalPlaces\">decimal&nbsp;places</span>                </td>                <td class=\"BorderColor1\" style=\"border-right-width:1px;border-right-style:solid\">                    <input name=\"ctl00$cphContent$txtLeftPanelSettingsDecimalPlaces\" type=\"number\" value=\"2\" maxlength=\"1\" id=\"cphContent_txtLeftPanelSettingsDecimalPlaces\" class=\"decimal\" onclick=\"this.onkeyup();\" onkeypress=\"if(event.keyCode == 13) {event.preventDefault();document.getElementById('cphContent_').click();return false;}\" onkeyup=\"UpdateSetting('DecimalPlaces',parseInt(this.value) < 0 ? '0': this.value == '' ? '0' : (parseInt(this.value) > 15 ? '15' : this.value));if(!validnum(this)) this.value='';\">                </td>                <td>                    <span id=\"cphContent_lblLeftPanelSettingsSigDiffEffectiveBase\">use&nbsp;effective&nbsp;base</span>                </td>                <td>                    <img class=\"Customcheckbox\" src=\"/Images/Icons/Boxes/checkbox/Inactive.png\" style=\"cursor: pointer;\"><input id=\"cphContent_chkLeftPanelSettingsSigDiffEffectiveBase\" type=\"checkbox\" name=\"ctl00$cphContent$chkLeftPanelSettingsSigDiffEffectiveBase\" onclick=\"UpdateSetting('SigDiffEffectiveBase', this.checked);\" style=\"display: none;\">                </td>            </tr>            <tr>                <td>                    <span id=\"cphContent_lblLeftPanelSettingsDisplayUnweightedBase\">display&nbsp;unweighted&nbsp;base</span>                </td>                <td class=\"BorderColor1\" style=\"border-right-width:1px;border-right-style:solid\">                    <img class=\"Customcheckbox\" src=\"/Images/Icons/Boxes/checkbox/Active.png\" style=\"cursor: pointer;\"><input id=\"cphContent_chkLeftPanelSettingsDisplayUnweightedBase\" type=\"checkbox\" name=\"ctl00$cphContent$chkLeftPanelSettingsDisplayUnweightedBase\" checked=\"checked\" onclick=\"UpdateSetting('DisplayUnweightedBase', this.checked);\" style=\"display: none;\">                </td>                <td>                    <span id=\"cphContent_lblLeftPanelSettingsSignificanceTest\">significance&nbsp;test</span>                </td>                <td>                    <img class=\"Customcheckbox\" src=\"/Images/Icons/Boxes/checkbox/Inactive.png\" style=\"cursor: pointer;\"><input id=\"cphContent_chkLeftPanelSettingsSignificanceTest\" type=\"checkbox\" name=\"ctl00$cphContent$chkLeftPanelSettingsSignificanceTest\" onclick=\"UpdateSetting('SignificanceTest', this.checked);\" style=\"display: none;\">                </td>            </tr>            <tr>                <td>                    <span id=\"cphContent_lblLeftPanelSettingsDataCheckEnabled\">mark&nbsp;empty&nbsp;variables</span>                </td>                <td class=\"BorderColor1\" style=\"border-right-width:1px;border-right-style:solid\">                    <img class=\"Customcheckbox\" src=\"/Images/Icons/Boxes/checkbox/Active.png\" style=\"cursor: pointer;\"><input id=\"cphContent_chkLeftPanelSettingsDataCheckEnabled\" type=\"checkbox\" name=\"ctl00$cphContent$chkLeftPanelSettingsDataCheckEnabled\" checked=\"checked\" onclick=\"UpdateSetting('DataCheckEnabled', this.checked);\" style=\"display: none;\">                </td>                <td>                    <span id=\"cphContent_lblLeftPanelSettingsSignificanceTestLevel\">significance&nbsp;test&nbsp;level</span>                </td>                <td>                    <select name=\"ctl00$cphContent$ddlLeftPanelSettingsSignificanceTestLevel\" id=\"cphContent_ddlLeftPanelSettingsSignificanceTestLevel\" onchange=\"UpdateSetting('SignificanceTestLevel', this.value, true);\">	<option value=\"95\">95%</option>	<option selected=\"selected\" value=\"90\">90%</option></select>                </td>            </tr>            <tr>                <td>                    <span id=\"cphContent_lblLeftPanelSettingsScrollLabels\">scroll&nbsp;headline&nbsp;labels</span>                </td>                <td class=\"BorderColor1\" style=\"border-right-width:1px;border-right-style:solid\">                    <img class=\"Customcheckbox\" src=\"/Images/Icons/Boxes/checkbox/Active.png\" style=\"cursor: pointer;\"><input id=\"cphContent_chkLeftPanelSettingsScrollLabels\" type=\"checkbox\" name=\"ctl00$cphContent$chkLeftPanelSettingsScrollLabels\" checked=\"checked\" onclick=\"UpdateSetting('ScrollLabels', this.checked);\" style=\"display: none;\">                </td>                <td colspan=\"2\" style=\"font-size:16pt;font-weight:normal\">                    <span id=\"cphContent_lblSettingsRenderer\">table</span>                </td>            </tr>            <tr>                <td>                    <span id=\"cphContent_lblLeftPanelSettingsHideEmptyRowsAndColumns\">hide&nbsp;empty&nbsp;rows&nbsp;&amp;&nbsp;columns</span>                </td>                <td class=\"BorderColor1\" style=\"border-right-width:1px;border-right-style:solid\">                    <img class=\"Customcheckbox\" src=\"/Images/Icons/Boxes/checkbox/Active.png\" style=\"cursor: pointer;\"><input id=\"cphContent_chkLeftPanelSettingsHideEmptyRowsAndColumns\" type=\"checkbox\" name=\"ctl00$cphContent$chkLeftPanelSettingsHideEmptyRowsAndColumns\" checked=\"checked\" onclick=\"UpdateSetting('HideEmptyRowsAndColumns', this.checked, true);\" style=\"display: none;\">                </td>                <td>                    <span id=\"cphContent_lblLeftPanelSettingsDisplay\">show</span>                </td>                <td class=\"BorderColor1\" style=\"\">                    <select name=\"ctl00$cphContent$ddlLeftPanelSettingsDisplay\" id=\"cphContent_ddlLeftPanelSettingsDisplay\" maxlength=\"1\" onchange=\"if (this.value == '0') { UpdateSetting('ShowValues', 'True'); UpdateSetting('ShowPercentage', 'True'); } else if (this.value == '1') { UpdateSetting('ShowValues', 'True'); UpdateSetting('ShowPercentage', 'False'); } else if (this.value == '2') { UpdateSetting('ShowValues', 'False'); UpdateSetting('ShowPercentage', 'True'); }\">	<option selected=\"selected\" value=\"0\">absolutes&nbsp;&amp;&nbsp;percentages</option>	<option value=\"1\">absolutes&nbsp;only</option>	<option value=\"2\">percentages&nbsp;only</option></select>                </td>            </tr>            <tr>                <td>                    <span id=\"cphContent_lblLeftPanelSettingsRankLeft\">rank categories</span>                </td>                <td class=\"BorderColor1\" style=\"border-right-width:1px;border-right-style:solid\">                    <img class=\"Customcheckbox\" src=\"/Images/Icons/Boxes/checkbox/Inactive.png\" style=\"cursor: pointer;\"><input id=\"cphContent_chkLeftPanelSettingsRankLeft\" type=\"checkbox\" name=\"ctl00$cphContent$chkLeftPanelSettingsRankLeft\" onclick=\"UpdateSetting('RankLeft', this.checked, true);\" style=\"display: none;\">                </td>                <td>                    <span id=\"cphContent_lblLeftPanelSettingsMinWidth\">cell&nbsp;width</span>                </td>                <td class=\"BorderColor1\" style=\"\">                    <input name=\"ctl00$cphContent$txtLeftPanelSettingsMinWidth\" type=\"number\" value=\"60\" maxlength=\"3\" id=\"cphContent_txtLeftPanelSettingsMinWidth\" onkeypress=\"if(event.keyCode == 13) {event.preventDefault();document.getElementById('cphContent_').click();return false;}\" onkeyup=\"UpdateSetting('MinWidth', isNaN(parseInt(this.value)) == true ? '0' : parseInt(this.value));if(!validnum(this)) this.value='';\">                </td>            </tr>            <tr>                <td>                    <span id=\"cphContent_lblLeftPanelSettingsMetadataLanguage\">metadata language</span>                </td>                <td class=\"BorderColor1\" style=\"border-right-width:1px;border-right-style:solid\">                    <select name=\"ctl00$cphContent$ddlLeftPanelSettingsMetadataLanguage\" id=\"cphContent_ddlLeftPanelSettingsMetadataLanguage\" onchange=\"UpdateSetting('IdLanguage', this.value, true);\">	<option selected=\"selected\" value=\"2057\">English (United Kingdom)</option></select>                </td>                <td>                    <span id=\"cphContent_lblLeftPanelSettingsMinHeight\">cell&nbsp;height</span>                </td>                <td class=\"BorderColor1\" style=\"\">                    <input name=\"ctl00$cphContent$txtLeftPanelSettingsMinHeight\" type=\"number\" value=\"45\" maxlength=\"3\" id=\"cphContent_txtLeftPanelSettingsMinHeight\" onkeypress=\"if(event.keyCode == 13) {event.preventDefault();document.getElementById('cphContent_').click();return false;}\" onkeyup=\"UpdateSetting('MinHeight', isNaN(parseInt(this.value)) == true ? '0' : parseInt(this.value));if(!validnum(this)) this.value='';\">                </td>            </tr>            <!--<tr>                <td>                    <span id=\"cphContent_lblLeftPanelSettingsRankTop\">SettingsRankTop</span>                </td>                <td class=\"BorderColor1\" style=\"border-right-width:1px;border-right-style:solid\">                    <input id=\"cphContent_chkLeftPanelSettingsRankTop\" type=\"checkbox\" name=\"ctl00$cphContent$chkLeftPanelSettingsRankTop\" onclick=\"UpdateSetting(&#39;RankTop&#39;, this.checked, true);\" />                </td>            </tr>-->        </tbody></table>";
        box.innerHTML = "<table cellpadding=\"0\" cellspacing=\"0\"><tr valign=\"top\"><td style=\"width:50px;background-color:" + document.getElementById("pnlMenu").style.backgroundColor +
            ";\"><div class=\"MenuItem MenuItem_Active\"><img src=\"Images/Icons/DataSource_Database.png\" /></div>" +
            "<div class=\"MenuItem\"><img src=\"Images/Icons/DataSource_File.png\" /></div>" +
            "<div class=\"MenuItem\"><img src=\"Images/Icons/DataSource_Web.png\" /></div></td><td style=\"background-color:#555555;width:200px;height:400px;\"><div class=\"Test\">Root</div><div class=\"Test\" style=\"margin-left:20px;\">ad test</div><div class=\"Test\" style=\"margin-left:20px;\">developer</div><div class=\"Test\" style=\"margin-left:20px;\">corporate</div></td>" +
            "<td style=\"padding:10px;width:600px;height:400px;\"></td></tr></table>";

        box.Position();
    }
};
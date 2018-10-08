<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LoggedUsers.aspx.cs" Inherits="LinkOnline.Pages.Logs.LoggedUsers" %>

<%@ Register TagPrefix="wu" Assembly="WebUtilities" Namespace="WebUtilities.Controls" %>
<asp:content id="Content1" contentplaceholderid="cphHead" runat="server">
     <meta name="viewport" content="width=device-width, initial-scale=1">
    <link rel="stylesheet" href="/Stylesheets/BootStrap/bootstrap.min.css">
    <script src="/Scripts/Bootstrap/bootstrap.min.js"></script>
    <wu:ScriptReference runat="server" Source="/Scripts/Main.js" />
    <wu:StylesheetReference Source="/Stylesheets/OverView.css" runat="server" />
    <link href="/Stylesheets/Main.css" rel="stylesheet" />
  
     <style>
         .styleclass {
             white-space: pre-wrap;
         }

         #cphContent_tblContent {
             clear: both;
         }

         .rowclass {
             margin-top: 10px;
             margin-bottom: 10px;
         }

         body {
             font-family: SegoeUILight;
         }
     </style> 
    <script>
        document.addEventListener('DOMContentLoaded', function (event) {
            var targets = document.getElementsByClassName('GridOverflow')[0];
            targets.style.maxHeight = "590px";
        });


        function fnExcelReport() {
            var tab_text = "<table border='2px'><tr>";
            var textRange; var j = 0;
            var tab = document.getElementsByClassName('GridHeadlineItemOverflow'); // id of table
            var columnCount = tab.length;
            for (j = 0 ; j < tab.length ; j++) {
                tab_text = tab_text + "<th  bgcolor='#87AFC6'>" + tab[j].innerHTML + "</th>";
                //tab_text=tab_text+"</tr>";
            }
            tab_text += "</tr>";

            tab = document.getElementsByClassName('GridRowItemOverflow'); // id of table

            for (j = 0 ; j < tab.length ; j++) {
                //tab_text = tab_text + tab.rows[j].innerHTML + "</tr>";
                if (( (j+1) %columnCount) == 0)
                    tab_text += "<td>" + tab[j].innerHTML + "</td></tr>";
                else
                    tab_text = tab_text + "<td>" + tab[j].innerHTML + "</td>";

            }


            tab_text = tab_text + "</table>";
            tab_text = tab_text.replace(/<A[^>]*>|<\/A>/g, "");//remove if u want links in your table
            tab_text = tab_text.replace(/<img[^>]*>/gi, ""); // remove if u want images in your table
            tab_text = tab_text.replace(/<input[^>]*>|<\/input>/gi, ""); // reomves input params

            var ua = window.navigator.userAgent;
            var msie = ua.indexOf("MSIE ");

            if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./))      // If Internet Explorer
            {
                txtArea1.document.open("txt/html", "replace");
                txtArea1.document.write(tab_text);
                txtArea1.document.close();
                txtArea1.focus();
                sa = txtArea1.document.execCommand("SaveAs", true, "Usage.xls");
            }
            else                 //other browser not tested on IE 11
                sa = window.open('data:application/vnd.ms-excel,' + encodeURIComponent(tab_text));

            return (sa);
        }
              </script>
  <%--  <wu:StylesheetReference runat="server" Source="../../Stylesheets/Pages/Login.css" />
     <wu:StylesheetReference runat="server" Source="../../Stylesheets/UserRole.css" />  --%> 
    </asp:content>
<asp:content id="ContentTitle" contentplaceholderid="cphTitle" runat="server">
    <h1 class="Color1"><wu:Label ID="lblPageTitle" runat="server" Name="UserLogs"></wu:Label></h1>
</asp:content>
<asp:content id="Content2" contentplaceholderid="cphContent" runat="server">
      <div class="container-fluid">  
          <div class="row" style="height:100%;">   
              <div class="col-xs-12">
                  <table style="margin-top:10px;">
           <%--      <% DatabaseCore.Items.User user = this.Core.Users.GetSingle(this.IdUser.Value);
                     if (user.HasPermission(111) || user.HasPermission(111))
                     { %>  
                <tr class="rowclass">
                    <td>
                        <div class="BackButton" onclick="window.location='UsersHome.aspx'"></div>
                    </td>
                    <td>&nbsp;
                    </td>
                </tr>
                <%  }
                    else
                    { %>
                <tr class="rowclass">
                    <td>
                        <div class="BackButton" onclick="window.location='../Overview.aspx'"></div>
                    </td>
                    <td>&nbsp;
                    </td>
                </tr>
                <%
                    }
                %>--%>
            </table>
                    <div style="margin-left:370px;">
                        <table style="width:98%;margin:auto;">
          <tr>
          
              <td style="float:right;">
                  <wu:Button ID="btnExport" runat="server"  CssClass="Button" Name="export" OnClientClick="fnExcelReport();"/>
                  <%--<button id="btnExport" onclick="fnExcelReport();"> EXPORT </button>--%>
              </td>
          </tr>
                            </table>  
                        </div>
         <div style="margin:auto;height:auto;">
             <asp:Panel id="pnlUserManagement" runat="server"></asp:Panel>
             <iframe id="txtArea1" style="display:none"></iframe>
        </div>
                  </div></div></div>
</asp:content>

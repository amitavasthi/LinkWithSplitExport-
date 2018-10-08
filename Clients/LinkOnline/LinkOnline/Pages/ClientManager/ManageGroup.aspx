<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="ManageGroup.aspx.cs" Inherits="LinkOnline.Pages.ClientManager.ManageGroup" %>

<asp:content id="Content1" contentplaceholderid="cphHead" runat="server">
     <meta name="viewport" content="width=device-width, initial-scale=1">
    <link rel="stylesheet" href="/Stylesheets/BootStrap/bootstrap.min.css">
    <script src="/Scripts/Bootstrap/bootstrap.min.js"></script>
    <wu:ScriptReference runat="server" Source="/Scripts/Main.js" />
    <wu:StylesheetReference runat="server" Source="/Stylesheets/UserRole.css" />
    <wu:StylesheetReference Source="/Stylesheets/OverView.css" runat="server" />
    <link href="/Stylesheets/Main.css" rel="stylesheet" />
</asp:content>
<asp:content id="ContentTitle" contentplaceholderid="cphTitle" runat="server">
    <h1 class="Color1"><wu:Label ID="Label1" runat="server" Name="ManageGroups"></wu:Label></h1>
</asp:content>

<asp:content id="Content2" contentplaceholderid="cphContent" runat="server">
    <script type="text/javascript">
        function func(roleId) {
            var hdnfldVariable = document.getElementById('hdnRoleId');
            hdnfldVariable.value = roleId;
            PageMethods.RemoveRole(roleId, SuccessFunction, FailedFunction);
        }
        // set the returnValueControl value 
        function SuccessFunction(returnValueControl) {
            var url = returnValueControl.trim();
            window.location = url;
        }
        // alert message on some failure
        function FailedFunction(returnedMessage, returnValueControl) {
            alert(returnedMessage.get_message());
        }

        function modify(roleId) {
            window.location = "AllocateRolePermission.aspx?mrp=" + roleId;
            return false;
        }

        //function saveall() {
        //    alert("hi");
        //    PageMethods.SaveOnMouseOut(SuccessFunction, FailedFunction);
        //}
    </script>
     <style>
         body {
             font-family: SegoeUILight;
         }

         .rowclass {
             margin-top: 20px;
             margin-bottom: 10px;
         }

         .cphContent_pnlModuleDetails {
             clear: both;
             margin-top: 10px;
         }

         .ddl {
             border: 2px solid #6CAEE0;
             border-radius: 5px;
             padding: 5px;
             /*-webkit-appearance: none;*/
             background-position: 88px;
             background-repeat: no-repeat;
             text-indent: 0.01px; /*In Firefox*/
             text-overflow: ''; /*In Firefox*/
             vertical-align: middle;
             font-weight: bold;
             margin-top: -5px;
         }

         .ddl-select select {
             background: transparent;
             width: 180px;
             padding: 5px;
             font-size: 16px;
             font-weight: bold;
             line-height: 1;
             border: 0;
             border-radius: 0;
             height: 30px;
             -webkit-appearance: none;
             color: #6CAEE0;
             margin-top: -5px;
         }
     </style>
   <div class="container-fluid">  
          <div class="row">   
              <div class="col-xs-12">
                <table style="margin-top:10px;position:absolute;">
                    <tr class="rowclass">
                       <td>
                          <wu:LinkButton ID="backButton" runat="server" CssClass="BackButton" OnClick="backButton_Click"></wu:LinkButton>
                        </td>
                        <td>&nbsp;</td>
                    </tr>
                </table> 
                  <table style="margin-left:auto;margin-right:auto;margin-top:50px;" id="tblUserGroup">
                    <tr class="rowclass">
                        <td style="float:left;">  
                                <wu:Label ID="lblRole" runat="server" Name="select user group" Font-Bold="true"></wu:Label>
                        </td>
                            <td style="float:left;">
                            &nbsp;<wu:DropdownList runat="server" ID="ddlRole"  CssClass="ddl Color1" AutoPostBack="true" OnSelectedIndexChanged="ddlRole_SelectedIndexChanged">
                            </wu:DropdownList>&nbsp;
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlRole" ErrorMessage="user group is required." ForeColor ="Red" InitialValue="select"> *</asp:RequiredFieldValidator> 
                        </td> 
                    </tr>
                  </table>
                  <table id="tblOuter" runat="server" style="margin-top:20px;margin-left:auto;margin-right:auto;margin-bottom:20px;">                
                      <tr class="rowclass">
                      <td colspan="2">
                          <asp:Panel ID="pnlModuleDetails" runat="server" Visible="True"></asp:Panel>      
                      </td> 
                      <td style="vertical-align:middle;"   >
                         <wu:ConfirmBox ID="cbRemove" runat="server"></wu:ConfirmBox>
                      </td>                   
                  </tr>
                      <tr class="rowclass">
                          <td colspan="2" align="center">
                              <wu:Button ID="btnSave" CssClass="smallButton" runat="server" Name="save" OnClick="btnModify_Click" />
                                 <wu:Button ID="btnRemove" CssClass="smallButton" runat="server" Name="remove"  OnClick="btnDelete_Click"/>
                          </td>
                      </tr>
                  </table>
                    <%--<div class="Color1" style="height: 20px;">
                        <wu:ConfirmBox ID="cbRemove" runat="server"></wu:ConfirmBox>
                    </div>
                    <div style="margin-top: 0px; margin-left: auto;margin-right:auto; margin-bottom:20px;">
                               
                    </div> --%>        
            </div>       
         </div>       
   </div>
</asp:content>

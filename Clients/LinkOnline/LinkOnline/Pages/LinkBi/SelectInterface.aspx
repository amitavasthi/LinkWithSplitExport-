<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="SelectInterface.aspx.cs" Inherits="LinkOnline.Pages.LinkBi.SelectInterface" %>

<asp:content id="Content1" contentplaceholderid="cphHead" runat="server">
    <style type="text/css">
        .Interface {
            cursor: pointer;
        }

        .SelectInterfaceMessage {
            margin: 5%;
            font-size: 25pt;
        }

        img {
        }
    </style>
</asp:content>
<asp:content id="Content3" contentplaceholderid="cphTitle" runat="server">
    <h1 class="Color1"><wu:Label ID="lblPageTitle" runat="server" Name="LinkBiTitle"></wu:Label>
        <span class="Beta">
    <wu:Label ID="lblBeta" runat="server" Name="Beta"></wu:Label>
    </span>
    </h1>
</asp:content>
<asp:content id="Content2" contentplaceholderid="cphContent" runat="server">
    <div class="SelectInterfaceMessage Color1">
        <table>
            <tr>
                <td>
                    <div class="BackButton" onclick="window.location='LinkBi.aspx'"></div>
                </td>
                <td>
                    <wu:Label ID="lblSelectInterfaceMessage" runat="server" Name="SelectInterfaceMessage"></wu:Label>
                </td>
            </tr>
        </table>
    </div>
    <table style="margin:auto;margin-top:0px;text-align:center;font-size:30pt;" class="Color1">
        <tr>
            <td>
                <asp:ImageButton style="width:250px" ID="btnInterfacePowerBi" runat="server" OnClick="btnInterfacePowerBi_Click" ImageUrl="/Images/LinkBi/Interfaces/Excel.png"></asp:ImageButton>
            </td>
            <td>
                <asp:ImageButton style="width:250px" ID="btnInterfaceXML" runat="server" OnClick="btnInterfaceXML_Click" ImageUrl="/Images/LinkBi/Interfaces/Xml.png"></asp:ImageButton>
            </td>
            <td style="display:none">
                <asp:ImageButton style="width:250px" ID="btnInterfaceTempTableTest" runat="server" OnClick="btnInterfaceTempTableTest_Click" ImageUrl="/Images/LinkBi/Interfaces/Xml.png"></asp:ImageButton>
            </td>
            <td>
                <asp:ImageButton style="width:250px" ID="btnInterfaceCSV" runat="server" OnClick="btnInterfaceCSV_Click" ImageUrl="/Images/LinkBi/Interfaces/csv.png"></asp:ImageButton>
            </td>
            <td style="display:none">
                <asp:ImageButton style="width:250px" ID="btnInterfaceCustomCharts" runat="server" OnClick="btnInterfaceCustomCharts_Click" ImageUrl="/Images/LinkBi/Interfaces/csv.png"></asp:ImageButton>
            </td>
        </tr>
        <tr>
            <td>
                excel
            </td>
            <td>
                xml
            </td>
            <td style="display:none">
                temp table test
            </td>
            <td>
                csv
            </td>
            <td style="display:none">
                custom charts
            </td>
        </tr>
        <tr>
            <td colspan="5" style="font-size:12pt;padding-top:20px;">
                <div style="margin:auto;display:inline-block;"><table><tr><td><wu:Checkbox ID="chkMailMe" runat="server" type="checkbox" /></td><td><wu:Label ID="lblMailResult" runat="server" Name="MailLinkBiResult"></wu:Label></td></tr></table></div>
            </td>
        </tr>
    </table>
        
</asp:content>

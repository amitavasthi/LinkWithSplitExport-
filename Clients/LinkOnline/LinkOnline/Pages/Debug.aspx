<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="Debug.aspx.cs" Inherits="LinkOnline.Pages.Debug" %>

<asp:content id="Content1" contentplaceholderid="cphHead" runat="server">
    <style type="text/css">
        .ActiveSessions td {
            border: 1px solid #444444;
            padding: 5px;
        }

        .HardwareSensorsHeadline {
            background:#444444;
            color:#FFFFFF;
            font-weight:bold;
        }

        .HardwareSensors td {
            border:1px solid #cccccc;
            padding:5px;
        }
    </style>
</asp:content>
<asp:content id="Content3" contentplaceholderid="cphTitle" runat="server">
    <h1 style="color:#FF0000">
        Debug
         <span class="Beta">
            <wu:Label ID="lblBeta" runat="server" Name="Beta"></wu:Label>
        </span>
    </h1>
</asp:content>
<asp:content id="Content2" contentplaceholderid="cphContent" runat="server">
    <div style="margin:1em">
        <asp:Button ID="btnClearCaseDataCache" runat="server" Text="clear case data cache" OnClick="btnClearCaseDataCache_Click" />
        <asp:Button ID="btnClearVariableCache" runat="server" Text="clear variable cache" OnClick="btnClearVariableCache_Click" />
        <br /><br />
        <h1 class="Color1">Server</h1>
        <table>
            <tr>
                <td>
                    <b>Description</b>
                </td>
                <td>
                    <asp:Label ID="lblServer" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <b>Instance</b>
                </td>
                <td>
                    <asp:Label ID="lblInstance" runat="server"></asp:Label>
                </td>
            </tr>
        </table>

        <h1 class="Color1">Version</h1>
        <table>
            <tr>
                <td>
                    <b>Link online</b>
                </td>
                <td>
                    <asp:Label ID="lblVersion" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <b>Data importer</b>
                </td>
                <td>
                    <asp:Label ID="lblVersionDataImporter" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <b>API</b>
                </td>
                <td>
                    <asp:Label ID="lblVersionAPI" runat="server"></asp:Label>
                </td>
            </tr>
        </table>
        <br /><br />
        <h1 class="Color1">Data aggregation times</h1>
        <asp:Panel ID="pnlDataAggregationTimes" runat="server"></asp:Panel>
        <br /><br />

        <h1 class="Color1">Active sessions</h1>
        <asp:Panel ID="pnlActiveSessions" runat="server" CssClass="ActiveSessions"></asp:Panel>
        <br /><br />

        <h1 class="Color1">Case data cache</h1>
        <asp:Panel ID="pnlCaseDataCache" runat="server" CssClass="ActiveSessions"></asp:Panel>
        <br /><br />

        <h1 class="Color1">Hardware sensors</h1>
        <asp:Panel ID="pnlHardwareSensors" runat="server" CssClass="HardwareSensors"></asp:Panel>
        <br /><br />
        <h1 class="Color1">Benchmark test</h1>
        <asp:Panel ID="pnlBenchmarkTestStartNew" runat="server">
            No benchmark tests running.
            <br />
            <table>
                <tr>
                    <td>
                        Count
                    </td>
                    <td>
                        <asp:TextBox ID="txtBenchmarkTestCount" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        Steps
                    </td>
                    <td>
                        <asp:TextBox ID="txtBenchmarkTestSteps" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="right">
                        <asp:Button ID="btnBenchmarkTestStart" runat="server" Text="Start" OnClick="btnBenchmarkTestStart_Click" />
                    </td>
                </tr>
            </table>
            
        </asp:Panel>
        <asp:Panel ID="pnlBenchmarkTestRunning" runat="server" Visible="false">
            Benchmark test currently running. Refresh page to update status.
            <script type="text/javascript">
                window.setTimeout(function () {
                    window.location = window.location;
                }, 5000);
            </script>
        </asp:Panel>
        <asp:Panel ID="pnlBenchmarkTestResult" runat="server" Visible="false">

        </asp:Panel>

        <wu:Button ID="btnCreateResponseIndexes" runat="server" Name="CreateResponseIndexes" OnClick="btnCreateResponseIndexes_Click" />
        <br />
        <wu:Button ID="btnRebuildResponseIndexes" runat="server" Name="RebuildResponseIndexes" OnClick="btnRebuildResponseIndexes_Click" />
        <br />
        <br />
        <wu:Button ID="btnDuplicateResponses" runat="server" Name="DuplicateResponses" OnClick="btnDuplicateResponses_Click" />
        <br />
        <br />
        <input type="button" value="generate responses" onclick="document.getElementById('cphContent_pnlGenerateResponses').style.display = '';" />
        <asp:Panel ID="pnlGenerateResponses" runat="server" style="display:none">
            <table>
                <tr>
                    <td>
                        total # of respondents
                        <br />
                        <span style="font-size:9pt;color:#cccccc;font-style:italic">
                            currently <asp:Label ID="lblGenerateResponsesRespondentCount" runat="server"></asp:Label>
                        </span>
                    </td>
                    <td>
                        <wu:TextBox ID="txtGenerateResponsesCount" runat="server"></wu:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="right">
                        <wu:Button ID="btnGenerateResponsesConfirm" runat="server" Name="Confirm" OnClick="btnGenerateResponsesConfirm_Click" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </div>
</asp:content>

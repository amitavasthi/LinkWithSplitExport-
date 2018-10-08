<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Console.ascx.cs" Inherits="LinkAdmin.Pages.Console" %>
<style type="text/css">
    .Console {
        background:#000000;
        color:#FFFFFF;
    }

    .LogEntry {
        padding:2px;
    }
    
    .LogType_Error {
        color:#FF0000;
    }
    .LogType_Success {
        color:#00FF00;
    }
    .LogType_Information {
        color:#ffd800;
    }


    .ConsoleTab {
        cursor:pointer;
        border:1px solid #444444;
        background:#444444;
        color:#FFFFFF;
        display:inline-block;
        padding:5px 10px 5px 10px;
    }
    .ConsoleTab_Active {
        border:1px solid #444444;
        background:#FFFFFF;
        color:#444444;
    }
</style>
<div class="Tabs">
<asp:Panel ID="pnlServers" runat="server"></asp:Panel>
</div>
<div class="Console">
    <asp:Panel ID="pnlConsole" runat="server"></asp:Panel>
</div>
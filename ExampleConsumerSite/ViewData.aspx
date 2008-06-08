<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewData.aspx.cs" Inherits="ExampleConsumerSite.ViewData" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <h1>OAuth Consumer Site - View Data</h1>
    <form id="form1" runat="server">
    <asp:Panel runat="server" ID="ResultsPanel">
        <p>
            Contacts for user:
            <asp:Label runat="server" ID="UserName"></asp:Label></p>
        <div>
            <asp:BulletedList ID="ContactsList" runat="server">
            </asp:BulletedList>
        </div>
    </asp:Panel>
    <asp:Label ID="ErrorInfo" runat="server"></asp:Label>
    </form>
</body>
</html>

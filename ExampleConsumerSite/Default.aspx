<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ExampleConsumerSite._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
      <a href="http://localhost:17990/Data.aspx">Click</a> to view contacts without OAuth<br />
      <asp:linkbutton id="oauthRequest" runat="server" onclick="oauthRequest_Click">Click</asp:linkbutton> to view contacts with OAuth
    </div>
    </form>
</body>
</html>

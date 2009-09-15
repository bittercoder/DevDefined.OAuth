<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserAuthorize.aspx.cs"
    Inherits="ExampleProviderSite.UserAuthorize" MasterPageFile="~/Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <h2>Access Request</h2>
    <br />
    <p>
        The consumer <strong>
            <%=ConsumerKey%></strong> is requesting access to your Account for the services(s)
        listed below. This Provider Site is not affiliated with <strong>
            <%=ConsumerKey%></strong>, and we recommend that you grant access only if you
        trust the site.
    </p>
    <p>
        If you grant access, you can revoke access at any time under 'My Account'.
        <%=ConsumerKey%>
        will not have access to your password or any other personal information from your
        Account. Access will only be granted for a set ammount of time (1 minute).</p>
    <ul>
        <li><strong>contacts</strong> The contacts service. </li>
    </ul>
    <div>
        <p>
            A 3rd party appliication
            <asp:Label runat="server" ID="consumerKey" />
            wants to access your data, do you approve?</p>
        <asp:Button runat="server" ID="Approve" Text="Grant Access" OnClick="Approve_Click" />
        <asp:Button runat="server" ID="Deny" Text="Deny Access" OnClick="Deny_Click" />
    </div>
</asp:Content>
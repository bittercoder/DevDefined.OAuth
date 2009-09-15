<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ExampleProviderSite.Login"
    MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <p>
        For this example site there are two example users, each has a different set of data,
        to be able to view the data you must login and grant access.</p>
    <table id="passwordList">
        <tr>
            <th>
                Login
            </th>
            <th>
                Password
            </th>
        </tr>
        <tr>
            <td>
                john
            </td>
            <td>
                password
            </td>
        </tr>
        <tr>
            <td>
                jane
            </td>
            <td>
                password
            </td>
        </tr>
    </table>
    <br />
    <h2>Login</h2>
    <table id="login">
        <tr>
            <td align="right">
                <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName">User Name:</asp:Label>
            </td>
            <td>
                <asp:TextBox ID="UserName" runat="server" CssClass="loginTextbox"></asp:TextBox>
                <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName"
                    ErrorMessage="User Name is required." ToolTip="User Name is required." ValidationGroup="loginForm">*</asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td align="right">
                <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password">Password:</asp:Label>
            </td>
            <td>
                <asp:TextBox ID="Password" runat="server" TextMode="Password" CssClass="loginTextbox"></asp:TextBox>
                <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password"
                    ErrorMessage="Password is required." ToolTip="Password is required." ValidationGroup="loginForm">*</asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td colspan="2" id="LoginRememberMe">
                <asp:CheckBox ID="RememberMe" runat="server" Text="Remember me next time." />
            </td>
        </tr>
        <tr>
            <td align="center" colspan="2" style="color: Red;">
                <asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal>
            </td>
        </tr>
        <tr>
            <td align="right" colspan="2" ID="LoginButtons">
                <asp:Button ID="LoginButton" runat="server" CommandName="Login" OnClick="LoginButton_Click"
                    Text="Log In" ValidationGroup="loginForm" />
            </td>
        </tr>
    </table>    
</asp:Content>

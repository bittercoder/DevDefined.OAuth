<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ExampleConsumerSite._Default" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">            
    <p>Welcome to the <a href="http://code.google.com/p/devdefined-tools/wiki/OAuth">DevDefined.OAuth</a> Consumer Example site.</p>
    <p>This example demonstrates the <a href="http://oauth.net">OAuth</a> flow of a simple application that implements <a href="http://oauth.net/core/1.0a">OAuth 1.0a</a> – this allows an application (in this case the ExampleConsumerSite project) to make a request to access the Contacts (address book) of a user on the ExampleProviderSite.</p>    
    <p>This will redirect the user to the provider site, where they must log in, and then grant access for the consumer to access the contact information.  If access is granted, the provider site will redirect back to the consumer, at which point the consumer can then access the protected data (Contacts) and display them on screen.</p>
    <p>This process ensures that the users login and password are <strong>never exposed</strong> to the consumer site, and at any point in the future the provider (user) can revoke access to the consumer site.</p>
    <p>
    <a href="ViewData.aspx">Click here</a> to view contacts without OAuth (demonstrates <a href="http://wiki.oauth.net/ProblemReporting">OAuth problem report extension support</a>)<br />
    <asp:LinkButton ID="oauthRequest" runat="server" OnClick="oauthRequest_Click">Click here</asp:LinkButton>
    to view contacts with OAuth (will start the authentication flow)
    </p>
</asp:Content>
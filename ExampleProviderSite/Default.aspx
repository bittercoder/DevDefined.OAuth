<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ExampleProviderSite._Default" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <p>Welcome to the <a href="http://code.google.com/p/devdefined-tools/wiki/OAuth">DevDefined.OAuth</a> Provider Example site.</p>
    <p>This example demonstrates the <a href="http://oauth.net">OAuth</a> flow of a simple application that implements <a href="http://oauth.net/core/1.0a">OAuth 1.0a</a> – this allows an application (in this case the ExampleConsumerSite project) to make a request to access the Contacts (address book) of a user on the ExampleProviderSite.</p>    
    <p>This site provides a contact service which can be accessed by consumers, to access it a consumer must be delegated the authority to do so by a user on this website.</p>    
</asp:Content>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AccessDenied.aspx.cs" Inherits="ExampleConsumerSite.AccessDenied" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">    
    <p>There was problem exchanging the request token for an access token!</p>
    <p>If the user just denied the request for access then this is not unusual, and is the expected way for a consumer to discover it was denied access i.e. the provider calls back to the client, and then lets the consumer make a request to the provider, which will then fail, notifying the consumer of why, if the site supports OAuth Problem Reporting</p>
    <h2>Problem Report Details</h2>
    <p>What follows are the details of the problem report that was returned from the provider.</p>
    <asp:Label ID="problemReport" runat="server" />
</asp:Content>
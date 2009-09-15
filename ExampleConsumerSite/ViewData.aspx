<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewData.aspx.cs" Inherits="ExampleConsumerSite.ViewData" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">    
    <p>
    Below are the results from an access to the contacts API exposed by the provider. If it's successfull then a list of names and email 
    addresses should be displayed.  If it fails then the reason for the failure will be displayed.  <b>Note:</b> The acces tokens only have a limited lifetime (1 minute)
    so if you wait a minute, then refresh, you should see that access is then denied.
    </p>
    <asp:Panel runat="server" ID="ResultsPanel">        
            <asp:Xml runat="server" ID="xmlFeed" TransformSource="Contacts.xslt"></asp:Xml>
    </asp:Panel>
    <asp:Label ID="ErrorInfo" runat="server"></asp:Label>    
</asp:Content>

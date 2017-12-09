<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ImageBlock.ascx.cs" Inherits="RockWeb.Plugins.Avalanche.ImageBlock" %>

<asp:UpdatePanel runat="server">
    <ContentTemplate>
        <Rock:ImageUploader runat="server" ID="img" OnImageUploaded="img_ImageUploaded" OnImageRemoved="img_ImageRemoved" />
    </ContentTemplate>
</asp:UpdatePanel>

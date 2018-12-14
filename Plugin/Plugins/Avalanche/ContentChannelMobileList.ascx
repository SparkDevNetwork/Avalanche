<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ContentChannelMobileList.ascx.cs" Inherits="RockWeb.Plugins.Avalanche.ContentChannelMobileList" %>
<script type="text/javascript">
    function clearDialog() {
        $('#rock-config-cancel-trigger').click();
    }
</script>

<asp:UpdatePanel ID="upnlContent" runat="server" UpdateMode="Conditional">
    <ContentTemplate>

        <%-- Edit Panel --%>
        <asp:Panel ID="pnlEditModal" runat="server" Visible="false">
            <Rock:ModalDialog ID="mdEdit" runat="server" OnSaveClick="lbSave_Click" Title="Channel Configuration" OnCancelScript="clearDialog();">
                <Content>

                    <asp:UpdatePanel ID="upnlEdit" runat="server">
                        <ContentTemplate>

                            <Rock:NotificationBox ID="nbError" runat="server" Heading="Error" Title="Query Error!" NotificationBoxType="Danger" Visible="false" />

                            <div class="row">
                                <div class="col-md-5">
                                    <Rock:RockDropDownList ID="ddlChannel" runat="server" Required="true" Label="Channel"
                                        DataTextField="Name" DataValueField="Guid" AutoPostBack="true" OnSelectedIndexChanged="ddlChannel_SelectedIndexChanged"
                                        Help="The channel to display items from." />
                                </div>
                                <div class="col-md-7">
                                    <Rock:RockCheckBoxList ID="cblStatus" runat="server" Label="Status" RepeatDirection="Horizontal"
                                        Help="Include items with the following status." />
                                </div>
                            </div>


                            <div class="row">
                                <div class="col-md-6">
                                    <Rock:NumberBox ID="nbCount" runat="server" CssClass="input-width-sm" Label="Items Per Page"
                                        Help="The maximum number of items to display per page (0 means unlimited)." />
                                    <Rock:NumberBox ID="nbItemCacheDuration" runat="server" CssClass="input-width-sm" Label="Item Cache Duration"
                                        Help="Number of seconds to cache the content items returned by the selected filter (use '0' for no caching)." />
                                    <Rock:NumberBox ID="nbOutputCacheDuration" runat="server" CssClass="input-width-sm" Label="Output Cache Duration"
                                        Help="Number of seconds to cache the resolved output. Only cache the output if you are not personalizing the output based on current user, current page, or any other merge field value. (use '0' for no caching)." />
                                </div>
                                <div class="col-md-6">
                                    <Rock:PagePicker ID="ppDetailPage" runat="server" Label="Detail Page" />
                                </div>
                            </div>

                            <div class="form-group">
                                <label class="control-label">
                                    Filter
                                </label>
                                <asp:HiddenField ID="hfDataFilterId" runat="server" />
                                <asp:PlaceHolder ID="phFilters" runat="server"></asp:PlaceHolder>
                            </div>
                            <div class="row">
                                <div class="col-md-6">
                                    <Rock:RockTextBox ID="tbTitleLava" runat="server" Label="Title Lava" Help="Lava to display the details of each \{\{Item\}\}"
                                        Required="false" />
                                    <Rock:RockTextBox ID="tbImageLava" runat="server" Label="Image Lava" Help="Lava to display the details of each \{\{Item\}\}"
                                        Required="false" />
                                    <Rock:RockTextBox ID="tbIconLava" runat="server" Label="Icon Lava" Help="Lava to display the details of each \{\{Item\}\}"
                                        Required="false" />
                                    <Rock:RockTextBox ID="tbSubtitleLava" runat="server" Label="Subtitle Lava" 
                                        Help="Lava to display the details of each \{\{Item\}\}" Required="false" />
                                    <Rock:RockTextBox ID="tbOrder" runat="server" Label="Order Lava" 
                                        Help="Lava to help order the items in the list \{\{Item\}\}" Required="false" />
                                </div>
                                <div class="col-md-6">
                                    <Rock:KeyValueList ID="kvlOrder" runat="server" Label="Order Items By" KeyPrompt="Field" ValuePrompt="Direction"
                                        Help="The field value and direction that items should be ordered by." />
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </Content>
            </Rock:ModalDialog>

        </asp:Panel>
        <div class="avalanche-header">
            Mobile Content Channel
        </div>
    </ContentTemplate>
</asp:UpdatePanel>

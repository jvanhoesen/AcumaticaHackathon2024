<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="True" ValidateRequest="False" CodeFile="HK200000.aspx.cs" Inherits="Page_HK200000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="Hackathon2024.HKLocationMaint" PrimaryView="Locations"/>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phL" runat="Server">
	<px:PXGrid ID="grid" runat="server" Height="400px" Width="100%" Style="z-index: 100" AllowPaging="True" AllowSearch="True" AdjustPageSize="Auto" DataSourceID="ds" SkinID="Primary" SyncPosition="True">
		<Levels>
			<px:PXGridLevel DataMember="Locations">
				<RowTemplate>
					<px:PXTextEdit ID="edZIP" runat="server" DataField="ZIP"/>
					<px:PXNumberEdit ID="edLatitude" runat="server" DataField="Latitude" />
					<px:PXNumberEdit ID="edLongitude" runat="server" DataField="Longitude"  />
				</RowTemplate>
				<Columns>
					<px:PXGridColumn DataField="ZIP" />
					<px:PXGridColumn DataField="Latitude" />
					<px:PXGridColumn DataField="Longitude" />
				</Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="200"/>
		<Mode AllowUpload="True" />
	</px:PXGrid>
</asp:Content>
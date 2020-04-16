<%@ Register TagPrefix="AM" TagName="Nav" Src="../UserControls/faculty.ascx" %>
<%@ Register TagPrefix="AM" TagName="GoBack" Src="../UserControls/goBack.ascx" %>
<%@ Page language="c#" Codebehind="Results.cs" AutoEventWireup="false" Inherits="Microsoft.VisualStudio.Academic.AssignmentManager.Faculty.Results" %>
<HTML>
	<HEAD>
		<title>
			<%Response.Write(Title);%>
		</title>
		<META http-equiv="Content-Type" content="text/html; charset=windows-1252">
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
	</HEAD>
	<body>
		<!-- nav user control here -->
		<AM:NAV id="Nav1" name="Nav1" runat="server"></AM:NAV>
		<div id="mainBody" class="mainBody">
			<form method="post" runat="server">
				<div class="itemStyle" id="tabledialog">
					<table style="LEFT: 50px; POSITION: relative; TOP: 10px" cellSpacing="0" cellPadding="0"
						width="623" border="0">
						<tr class="tableNoBorderItem">
							<td width="216">
								<AM:GoBack id="GoBack1" name="GoBack1" runat="server"></AM:GoBack>
							</td>
						</tr>
						<tr>
							<td class="infoText" width="216">
								<%Response.Write(Total_RecordsToBe_TextString);%>
								&nbsp;
							</td>
							<td>
								<asp:Label class="infoText" id="lblTotalRecords" runat="server"></asp:Label>
							</td>
						</tr>
						<tr>
							<td class="infoText" width="216">
								<%Response.Write(Total_RecordsImported_TextString);%>
								&nbsp;
							</td>
							<td>
								<asp:Label class="infoText" id="lblNumberSuccessful" runat="server"></asp:Label>
							</td>
						</tr>
						<tr>
							<td class="infoText" width="216">
								<%Response.Write(Total_RecordsFailedImported_TextString);%>
							</td>
							<td>
								<asp:Label class="infoText" id="lblNumberFailed" runat="server"></asp:Label>
							</td>
						</tr>
						<tr>
							<td colspan="2">
								<asp:Label class="infoText" id="lblUserInfo" runat="server"></asp:Label>
							</td>
						</tr>
						<tr>
							<td width="216">
								&nbsp;
							</td>
							<td>
							</td>
						</tr>
						<tr class="tableNoBorderItem">
							<td width="216">
								<asp:Button id="btnCancel" runat="server" class="webButton"></asp:Button>
							</td>
							<td>
								<asp:Button id="btnSave" runat="server" class="webButton"></asp:Button>
							</td>
						</tr>
					</table>
			</form>
		</div>
		</DIV>
	</body>
</HTML>

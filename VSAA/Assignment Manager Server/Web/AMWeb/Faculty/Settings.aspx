<%@ Register TagPrefix="AM" TagName="GoBack" Src="../UserControls/goBack.ascx" %>
<%@ Register TagPrefix="AM" TagName="Nav" Src="../UserControls/faculty.ascx" %>
<%@ Page language="c#" Codebehind="Settings.cs" AutoEventWireup="false" Inherits="Microsoft.VisualStudio.Academic.AssignmentManager.Faculty.Settings" %>
<!DOCTYPE HTML public="-//W3C//DTD HTML 4.0 Transitional//EN">
<HTML>
	<HEAD>
		<TITLE>
			<%Response.Write(Title);%>
		</TITLE>
	</HEAD>
	<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
	<meta content="C#" name="CODE_LANGUAGE">
	<script language="JavaScript" src="../common/functions.js"></script>
	<script language="javascript">
		function window.onload(){
		try {
		g_Attrib = window.external.ActiveWindow.ContextAttributes.Add("keyword","vs.am.serveradmin.settings",3); // Type '3' is an F1 keyword
		} 
		catch (helpFailed) {}
		}
	
		function window.onunload(){
		  try {
      g_Attrib.Remove();
    } catch (helpFailed) {
    }
		}
	</script> <!-- nav user control here --><AM:NAV id="Nav1" runat="server"></AM:NAV>
	<form id="Form1" method="post" encType="multipart/form-data" runat="server">
		<div class="mainBody" id="mainBody">
			<div class="itemStyle" id="tabledialog">
				<table style="LEFT: 50px; POSITION: relative; TOP: 10px" cellSpacing="0" cellPadding="0"
					width="100%" border="0">
					<tr vAlign="top">
						<td>
							<!-- general div start -->
							<table id="tblGeneralSetting" cellSpacing="0" cellPadding="2" width="100%" border="0" runat="server">
								<tr>
									<td width="20">
									</td>
									<td width="250">
										<AM:GOBACK id="GoBack1" runat="server" name="GoBack1"></AM:GOBACK>
									</td>
								</tr>
								<TR>
									<TD width="20"></TD>
									<TD width="250">
										<asp:Label id="lblAMTitle" CssClass="infoTextTitle" runat="server" Width="312px">lblAMTitle</asp:Label></TD>
								</TR>
								<tr>
									<td width="20">
									</td>
									<td width="250">
										<asp:label class="infoText" id="lblAutoGradeOnOff" runat="Server">lblAutoGradeOnOff</asp:label>
									</td>
								</tr>
								<tr>
									<td width="20">
									</td>
									<td>
										<asp:radiobutton class="infoText" id="rbtnAutoGradeOn" runat="Server" Width="10" groupname="rbtnAutoGradeGroup"
											text="rbtnAutoGradeOn"></asp:radiobutton>
									</td>
								</tr>
								<tr>
									<td width="20">
									</td>
									<td>
										<asp:radiobutton class="infoText" id="rbtnAutoGradeOff" runat="Server" Width="10" groupname="rbtnAutoGradeGroup"
											text="rbtnAutoGradeOff"></asp:radiobutton>
									</td>
								</tr>
								<tr>
									<td width="20">
									</td>
									<td width="250">
										<asp:label class="infoText" id="lblUsingSMTP" runat="server"></asp:label>
									</td>
								</tr>
								<tr>
									<td width="20">
									</td>
									<td>
										<asp:radiobutton class="infoText" id="rbtnSMTPOn" runat="Server" Width="10" groupname="rbtnSMTPGroup"
											Enabled="False"></asp:radiobutton>
									</td>
								</tr>
								<tr>
									<td width="20">
									</td>
									<td>
										<asp:radiobutton class="infoText" id="rbtnSMTPOff" runat="Server" Width="10" groupname="rbtnSMTPGroup"
											Enabled="False"></asp:radiobutton>
									</td>
								</tr>
								<tr>
									<td width="20">
									</td>
									<td width="250">
										<asp:label class="infoText" id="lblUsingSSL" runat="server"></asp:label>
									</td>
								</tr>
								<tr>
									<td width="20">
									</td>
									<td>
										<asp:radiobutton class="infoText" id="rbtnSSLOn" runat="Server" Width="10" groupname="rbtnSSLGroup"
											Enabled="False"></asp:radiobutton>
									</td>
								</tr>
								<tr>
									<td width="20">
									</td>
									<td>
										<asp:radiobutton class="infoText" id="rbtnSSLOff" runat="Server" Width="10" groupname="rbtnSSLGroup"
											Enabled="False"></asp:radiobutton>
									</td>
								</tr>
								<tr>
									<td width="20">
									</td>
									<td width="100%">
										<asp:label class="infoText" runat="server" id="lblProcessingTime"></asp:label>
									</td>
								</tr>
								<tr>
									<td width="20">
									</td>
									<td>
										<asp:textbox runat="server" class="infoTextHeaderStyle" style="CURSOR:text" id="txtProcessingTime"></asp:textbox>
									</td>
								</tr>
								<tr>
									<td width="20">
									</td>
									<td width="100%">
										<asp:label class="infoText" id="lblProjectSize" runat="server"></asp:label>
										&nbsp;
									</td>
								</tr>
								<tr>
									<td width="20">
									<td>
										<asp:textbox class="infoTextHeaderStyle" id="txtProjectSize" style="CURSOR:text" runat="server"></asp:textbox>
									</td>
								</tr>
								<tr>
									<td>
										&nbsp;
									</td>
								</tr>
								<tr>
									<td width="20">
									</td>
									<td align="left">
										<asp:button class="webButton" id="btnUpdate" runat="server" Text="btnUpdate" NAME="btnUpdate"></asp:button>
										&nbsp;
										<asp:button class="webButton" id="btnCancel" runat="server" Text="btnCancel" NAME="btnCancel"></asp:button>
									</td>
								</tr>
							</table>
							<!--general div end -->
							<!--end of general-->
							<table id="tblBottomButtons" cellSpacing="0" cellPadding="2" width="100%" border="0" runat="server">
								<tr>
									<td width="20">
										&nbsp;
									</td>
								</tr>
								<tr>
									<td width="20">
									</td>
									<td>
									</td>
									<td>
										&nbsp;
									</td>
								</tr>
								<!--end of Settings table here -->
							</table>
						</td>
					</tr>
				</table>
	</form>
	</DIV></DIV>
</HTML>

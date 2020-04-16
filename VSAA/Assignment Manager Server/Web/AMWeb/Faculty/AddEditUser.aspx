<%@ Register TagPrefix="AM" TagName="GoBack" Src="../UserControls/goBack.ascx" %>
<%@ Register TagPrefix="AM" TagName="Nav" Src="../UserControls/faculty.ascx" %>
<%@ Page language="c#" Codebehind="AddEditUser.cs" AutoEventWireup="false" Inherits="Microsoft.VisualStudio.Academic.AssignmentManager.Faculty.AddEditUser" %>
<!DOCTYPE HTML public="-//W3C//DTD HTML 4.0 Transitional//EN">
<HTML>
	<HEAD>
		<TITLE>
			<%Response.Write(Title);%>
		</TITLE>
	</HEAD>
	<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
	<meta name="CODE_LANGUAGE" Content="C#">
	<script language="JavaScript" src="../common/functions.js"></script>
	<script language="javascript">
		function window.onload(){
		try {
		g_Attrib = window.external.ActiveWindow.ContextAttributes.Add("keyword","vs.am.coursemanagement.courseusers",3); // Type '3' is an F1 keyword
		} 
		catch (helpFailed) {}
		}
	
		function window.onunload(){
		  try {
      g_Attrib.Remove();
    } catch (helpFailed) {
    }
		}
	</script> <!-- nav user control here -->
	<AM:Nav id="Nav1" runat="server" name="Nav1"></AM:Nav>
	<div id="mainBody" class="mainBody">
		<form method="post" runat="server" id="frm">
			<!-- custom code here -->
			<table style="LEFT: 50px; WIDTH: 100%; POSITION: relative; TOP: 10px" cellSpacing="0" cellPadding="0"
				border="0">
				<tr>
					<td width="7">
					</td>
					<td><asp:label id="lblUserDetails" runat="server" class="infoTextTitle"></asp:label>
						<AM:GoBack id="GoBack1" name="GoBack1" runat="server"></AM:GoBack>
					</td>
				</tr>
				<tr>
					<td width="7" rowSpan="1">
					</td>
					<td>
						<P>
							<asp:label class="infoText" id="lblDescription" runat="Server"></asp:label>
							<br>
							<asp:label class="infoText" id="lblPasswordText" runat="Server"></asp:label>
							<br>
							<asp:label class="infoText" id="lblFindInstructions" runat="Server" NAME="lblFindInstructions"></asp:label></P>
					</td>
				</tr>
				<TR>
					<TD width="7"></TD>
					<TD></TD>
				</TR>
				<TR>
					<TD width="7"></TD>
					<TD><IMG id="Img4" src="../images/Required.gif" runat="server">
						<asp:label class="infoText" id="lblRequired" runat="server"></asp:label></TD>
				</TR>
				<tr>
					<td width="7">
					</td>
					<td>
						<asp:label class="infoText" id="lblEmailAddress" runat="Server">
							lblEMailAddress</asp:label>
					</td>
				</tr>
				<tr>
					<TD align="left" width="7"><IMG id="Img1" src="../images/Required.gif" runat="server">&nbsp;</IMG></TD>
					<td>
						<asp:textbox class="infoTextHeaderStyle" id="txtEMailAddress" style="CURSOR: text" runat="Server"
							Width="580px">txtEMailAddress</asp:textbox>
					</td>
				</tr>
				<TR>
					<TD width="7"></TD>
					<TD>
						<asp:label class="infoText" id="lblUniversityIdentifier" runat="server">
							lblUniversityIdentifier</asp:label></TD>
				</TR>
				<TR>
					<TD align="left" width="7"><IMG id="Img2" src="../images/Required.gif" runat="server">&nbsp;</IMG></TD>
					<TD>
						<asp:textbox class="infoTextHeaderStyle" id="txtUniversityIdentifier" style="CURSOR: text" runat="server"
							Width="580px">
							txtUniversityIdentifier</asp:textbox></TD>
				</TR>
				<TR>
					<TD width="7"></TD>
					<TD>
						<asp:label class="infoText" id="lblUserName" runat="server">
							lblUserName</asp:label></TD>
				</TR>
				<TR>
					<TD align="left" width="7"><IMG id="Img3" src="../images/Required.gif" runat="server">&nbsp;</TD>
					<TD></IMG>
						<asp:textbox class="infoTextHeaderStyle" id="txtUserName" style="CURSOR: text" runat="server"
							Width="580px">
							txtUserName</asp:textbox></TD>
				</TR>
				<TR>
					<TD width="7"></TD>
				</TR>
				<TR>
					<TD width="7"></TD>
					<TD align="right">
						<asp:Button class="webButton" id="btnFind" name="btnFind" runat="server" width="70"></asp:Button></TD>
				</TR>
				<TR>
					<TD width="7"></TD>
					<TD>
						<asp:label class="infoText" id="lblLastName" runat="Server" colspan="2">
							lblLastName</asp:label></TD>
				</TR>
				<TR>
					<TD width="7"></TD>
					<TD>
						<asp:textbox class="infoTextHeaderStyle" id="txtLastName" style="CURSOR: text" runat="server"
							Width="580px">
							txtLastName</asp:textbox></TD>
				</TR>
				<TR>
					<TD width="7"></TD>
					<TD>
						<asp:label class="infoText" id="lblFirstName" runat="Server" colspan="2">
							lblFirstName</asp:label></TD>
				</TR>
				<TR>
					<TD width="7" height="24"></TD>
					<TD height="24">
						<asp:textbox class="infoTextHeaderStyle" id="txtFirstName" style="CURSOR: text" runat="server"
							Width="580px">
							txtFirstName</asp:textbox></TD>
				</TR>
				<TR>
					<TD width="7"></TD>
					<TD>
						<asp:label class="infoText" id="lblMiddleName" runat="Server" colspan="2">
							lblMiddleName</asp:label></TD>
				</TR>
				<TR>
					<TD width="7"></TD>
					<TD>
						<asp:textbox class="infoTextHeaderStyle" id="txtMiddleName" style="CURSOR: text" runat="server"
							Width="580px">
							txtMiddleName</asp:textbox></TD>
				</TR>
				<TR>
					<TD width="7"></TD>
					<TD vAlign="baseline" noWrap>
						<asp:label class="infoText" id="lblUserRoles" runat="server" colspan="2">lblUserRoles</asp:label></TD>
				</TR>
				<TR>
					<TD width="7"></TD>
					<TD vAlign="baseline" noWrap>
						<asp:RadioButtonList id="UserRolesList" runat="server" Width="112px" CssClass="infotext"></asp:RadioButtonList></TD>
				</TR>
				<TR>
					<TD width="7"></TD>
					<TD class="infoText" vAlign="baseline" noWrap>
						<P>
							<asp:HyperLink class="itemHotStyle" id="hlChangePassword" runat="server">
							hlChangePassword</asp:HyperLink>
						</P>
					</TD>
				</TR>
				<TR>
					<TD width="7"></TD>
					<TD align="right">
						<INPUT id="btnDelete" type="button" class="webbutton" value="btnDelete" name="btnDelete" visible="false" runat="server" onclick="showDeleteUserDialog()">
						<asp:Button class="webbutton" id="btnUpdate" runat="server" NAME="btnUpdate" width="70"></asp:Button>
						<asp:Button class="webbutton" id="btnCancel" runat="Server" NAME="btnCancel" width="70"></asp:Button></TD>
				</TR>
				<TR>
					<td width="7">
					</td>
					<TD>&nbsp; <INPUT class="invisible" id="txtAction" name="txtAction" runat="server">
					</TD>
				</TR>
			</table>
		</form>
	</div>
</HTML>

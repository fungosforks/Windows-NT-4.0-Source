<%@ Register TagPrefix="AM" TagName="GoBack" Src="../UserControls/goBack.ascx" %>
<%@ Register TagPrefix="AM" TagName="Nav" Src="../UserControls/student.ascx" %>
<%@ Page language="c#" Codebehind="ChangePassword.aspx.cs" AutoEventWireup="false" Inherits="Microsoft.VisualStudio.Academic.AssignmentManager.Student.ChangePassword" %>

<HTML>
  <HEAD><TITLE> <%Response.Write(Title);%></TITLE>
</HEAD>
	<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
	<meta content="C#" name="CODE_LANGUAGE"><!DOCTYPE HTML public="-//W3C//DTD HTML 4.0 Transitional//EN">
	<script language="JavaScript" src="../common/functions.js"></script><script language="javascript">		
		function window.onLoad()
		{
		//set the help context
		try {
		g_Attrib = window.external.ActiveWindow.ContextAttributes.Add("keyword","vs.am.serveradmin.myaccount.changepassword",3); // Type '3' is an F1 keyword
		} 
		catch (helpFailed) {}
		}
		
		function window.onUnload()
		{
		try{
			g_Attrib.Remove();		
		}
			catch(helpFailed){}
		}
		</script>
	<AM:NAV id="Nav1" name="Nav1" runat="server">
	</AM:NAV>
	<form id="Form1" method="post" runat="server">
		<div class="mainBody" id="mainBody">
			<div class="infoText" id="changepwddialog">
				<table style="LEFT: 50px; POSITION: relative" cellSpacing="0" cellPadding="0" width="80%" border="0">
					<tr>
						<td valign=bottom>
							<span class="infoTextTitle">
<%Response.Write(PageTitle);%>
							</span>
							<AM:GOBACK id="GoBack1" name="GoBack1" runat="server">
							</AM:GOBACK>
						</td>
						<td>
							&nbsp;
						</td>
					</tr>					
					<tr vAlign="top" align="left">
						<td>
							<asp:label id="lblUser" runat="server" CssClass="infoText">
							</asp:label>
							&nbsp;<asp:label id="lblUserName" CssClass="infoTextTitle" Runat="server"></asp:label>
						</td>
						<td>
							&nbsp;
						</td>
					</tr>
					<tr vAlign="top">
						<td>
							<table>
								<tr>
									<td class="infoTextDisabled">
										<table>
											<tr>
												<td width=7>&nbsp;</td>
												<td vAlign="top" align="left">
													<asp:label id="lblCurrentPwd" runat="server" CssClass="infoText">
													</asp:label>
												</td>
											</tr>
											<tr vAlign="top">
											<td width=7><img src="../images/Required.gif" runat="server" ID="Img1">
						</img></td>
                <TD>
<asp:textbox id=txtCurrentPwd style="CURSOR: text" runat="server" CssClass="infoTextHeaderStyle" NAME="txtCurrentPwd" maxlength="50" textmode="Password" Width="300px">
													</asp:textbox></TD>
											</tr>
              <TR vAlign=top>
                <TD width=7>&nbsp;</TD>
                <TD vAlign=center align=left>
<asp:label id=lblNewPwd runat="server" CssClass="infoText">
													</asp:label></TD>
                <TD>&nbsp; </TD></TR>
              <TR vAlign=top>
                <TD width=7><IMG id=Img4 src="../images/Required.gif" 
                  runat="server"> </IMG></TD>
                <TD>
<asp:textbox id=txtNewPwd style="CURSOR: text" runat="server" CssClass="infoTextHeaderStyle" NAME="txtNewPwd" maxlength="50" textmode="Password" Width="300px">
													</asp:textbox></TD></TR>
              <TR vAlign=top>
                <TD width=7>&nbsp;</TD>
                <TD vAlign=center align=left>
<asp:label id=lblConfirmPwd runat="server" CssClass="infoText">
													</asp:label></TD>
                <TD>&nbsp; </TD></TR>
              <TR vAlign=top>
                <TD width=7><IMG id=Img2 src="../images/Required.gif" 
                  runat="server"> </IMG></TD>
                <TD>
<asp:textbox id=txtConfirmPwd style="CURSOR: text" runat="server" CssClass="infoTextHeaderStyle" NAME="txtConfirmPwd" maxlength="50" textmode="Password" Width="300px">
													</asp:textbox></TD></TR>
										</table>
									</td>
								</tr>
							</table>
						</td>
					</tr>
  <TR>
    <TD>&nbsp; </TD>
    <TD>&nbsp; </TD></TR>
  <TR style="LEFT: 175px; POSITION: relative" vAlign=top>
    <TD>
<asp:button id=btnSave runat="server" CssClass="webButton">
							</asp:button>
<asp:button id=btnCancel runat="server" CssClass="webButton">
							</asp:button></TD>
    <TD>&nbsp; </TD></TR>
    <TR>
          <TD><IMG id=Img3 src="../images/Required.gif" runat="server"></IMG> 
<asp:label class=infoText id=lblRequired runat="server"></asp:label></TD></TR>
				</table>
			</div>
		</div>
	</form>
</HTML>

<%@ Page language="c#" Codebehind="ChangePassword.aspx.cs" AutoEventWireup="false" Inherits="Microsoft.VisualStudio.Academic.AssignmentManager.Faculty.ChangePassword" %>
<%@ Register TagPrefix="AM" TagName="Nav" Src="../UserControls/faculty.ascx" %>
<%@ Register TagPrefix="AM" TagName="GoBack" Src="../UserControls/goBack.ascx" %>

<HTML>
  <HEAD>
		<TITLE> <%Response.Write(Title);%>
		</TITLE><script language="JavaScript" src="../scripts/functions.js"></script><script language="javascript">		
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
		</script><!-- nav user control here -->
		
		<AM:Nav id="Nav1" runat="server" name="Nav1">
		</AM:Nav>
		<form method="post" runat="server" ID="Form1">
  </HEAD>
	<BODY>
		<div id="mainBody" class="mainBody">
			<table style="LEFT: 50px; POSITION: relative; TOP: 10px" cellSpacing="0" cellPadding="0" width="80%" border="0">
				<tr>
					<td class="infoTextTitle">
<%Response.Write(ChangePassword_AMTitle);%>
					</td>
					<td>
						<AM:GoBack id="GoBack1" name="GoBack1" runat="server">
						</AM:GoBack>
					</td>
				</tr>
				<tr align="left" valign="top">
					<td>
						<asp:Label id="lblUser" CssClass="infoText" runat="server"></asp:Label>
						&nbsp;
						<asp:Label ID="lblUserName" CssClass="infoText" Runat="server" NAME="lblUserName"></asp:Label>
					</td>
				</tr>
				<tr valign="top">
					<td>
						<table>
							<tr>
								<td class="infoTextDisabled">
									<table>
										<tr>
											<td width=7>&nbsp;</td>
											<td align="left" valign="center">
												<asp:Label id="lblNewPwd" CssClass="infoText" runat="server"></asp:Label>
											</td>
										</tr>
										<tr valign="top">
											<td width=7><img src="../images/Required.gif" runat="server" ID="Img4">
						</img></td>
											<td>
								<asp:TextBox id=txtNewPwd style="CURSOR: text" runat="server" CssClass="infoTextHeaderStyle" Width="300" textmode="Password" maxlength="50"></asp:TextBox>
											</td>
										</tr>
              <TR vAlign=top>
              <td width=7>&nbsp;</td>
                <TD vAlign=center align=left>
<asp:Label id=lblConfirmPwd runat="server" CssClass="infoText"></asp:Label></TD></TR>
              <TR vAlign=top>
               <td width=7><img src="../images/Required.gif" runat="server" ID="Img1">
						</img></td></td>
                <TD>
                
<asp:TextBox id=txtConfirmPwd style="CURSOR: text" runat="server" CssClass="infoTextHeaderStyle" Width="300" textmode="Password" maxlength="50"></asp:TextBox></TD></TR>
									</table>
								</td>
							</tr>
        <TR vAlign=top align=right>
          <TD>
<asp:Button id=btnSave runat="server" CssClass="webButton" width="70"></asp:Button>
<asp:Button id=btnCancel runat="server" CssClass="webButton" width="70"></asp:Button></TD></TR>
        <TR>
          <TD><IMG id=Img3 src="../images/Required.gif" runat="server"></IMG>
<asp:label class=infoText id=lblRequired runat="server"></asp:label> 
          </TD></TR>
						</table></FORM>
      <DIV></DIV></td>
    <DIV></DIV></tr>
		</div>
	</BODY>
</HTML>

<%@ Page language="c#" Codebehind="Logoff.aspx.cs" AutoEventWireup="false" Inherits="Microsoft.VisualStudio.Academic.AssignmentManager.Logoff" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<TITLE>
			<%Response.Write(Title);%>
		</TITLE>
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<LINK REL="StyleSheet" HREF="scripts/assnMan.css" type="text/css">
	</HEAD>
	<body topmargin="0" leftmargin="0" class="mainBody">
		<table id="navTable" name="navTable" width="100%" height="100%" border="0" cellspacing="0" cellpadding="0">
			<tr align="left" valign="top">
				<td width="145" height="77" align="left" valign="top">
					<img src="images/topLeft.jpg" width="145" height="77" alt="" border="0">
				</td>
				<td width="270" height="53" align="left" valign="top" style="Z-INDEX:-1"><span class="title"><span class="titleCap">A</span><span class="titleBody">ssignment</span> <span class="titleCap">M</span><span class="titleBody">anager</span></span>
					<img src="images/topCenter.jpg" width="270" height="53" alt="" border="0">
				</td>
			</tr>
			<tr align="left" valign="top" style="POSITION:relative;TOP:-3px">
				<td width="145" bgcolor="#333366" align="left" valign="top">
					<table width="145" cellspacing="0" cellpadding="5" border="0" id="tabMenuTable">
					</table>
				</td>
				<td width="*" align="left" valign="top">
					<asp:Label ID="lblTitle" Runat="server" class="courseName"></asp:Label>
					<br>
					&nbsp;&nbsp;
					<br>
					<div id="copyText" class="infoText" runat="server">
						<table width="100%" cellspacing="0" cellpadding="0" border="0">
							<tr>
								<td valign="top" nowrap>
									<form action="" id="loginform" method="post" runat="server">
										<p>
											<table cellspacing="5" cellpadding="5" width="399" border="0" height="68">
												<tr>
													<td style="WIDTH: 119px">
														<asp:Label class="infoText" id="lblLogoffMsg" runat="server" NAME="lblUserName" Width="377px">
																lblLogoffMsg
															</asp:Label>
													</td>
												</tr>
												<tr>
													<div class="infoText">
													</div>
													<td style="WIDTH: 119px">
														<asp:HyperLink class="itemHotStyle" Font-Size="0.7em" ID="HyperLink1" runat="server" Width="411px">HyperLink</asp:HyperLink>
													</td>
													<DIV></DIV>
												</tr>
											</table>
										</p>
									</form>
								</td>
							</tr>
						</table>
					</div>
				</td>
			</tr>
		</table>
	</body>
</HTML>

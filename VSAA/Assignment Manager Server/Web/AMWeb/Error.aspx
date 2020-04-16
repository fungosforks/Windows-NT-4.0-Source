<%@ Page language="c#" Codebehind="Error.aspx.cs" AutoEventWireup="false" Inherits="Microsoft.VisualStudio.Academic.AssignmentManager.Error" %>

<HTML>
  <HEAD>
		<TITLE> <%Response.Write(Title);%>
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
				<td width="270" height="53" align="left" valign="top">
					<img src="images/topCenter.jpg" width="270" height="53" alt="" border="0">
				</td>
			</tr>
			<tr align="left" valign="top" style="POSITION:relative;TOP:-3px">
				<td width="145" bgcolor="#333366" align="left" valign="top">
					<table width="145" cellspacing="0" cellpadding="5" border="0" id="tabMenuTable">
					</table>
				</td>
				<td width="*" align="left" valign="top">
					<asp:Label ID=lblTitle Runat=server class="courseName">
					</asp:Label>
					<br>
					<asp:Label ID=lblSubTitle Runat=server class="infoText" NAME="lblSubTitle">
					</asp:Label>
					<br>
					<asp:Label ID=lblFeedback Runat=server class="errorText" NAME="lblFeedback">
					</asp:Label>
					<div id="copyText" class="infoText" runat="server">
						<table width="100%" cellspacing="0" cellpadding="0" border="0">
							<tr>
								<td valign=top nowrap>
									<form action="" id="loginform" method=post runat=server>
										<!-- Error dialog -->
										<div id=errordialog class=itemStyle style="LEFT:50px; POSITION:relative; TOP:5px">
											<p>
												<table cellspacing=5 cellpadding=5 width=500 border=0>
													<tr>
														<td style="WIDTH: 100%">
															<asp:Label class="infoText" id=lblErrorEncountered runat="server" NAME="lblErrorEncountered">
																lblErrorEncountered
															</asp:Label>
														</td>
													</tr>
													<tr>
														<td style="WIDTH: 100%">
															<asp:Label class="errorText" id=lblErrorDetail runat="server" NAME="lblErrorDetail">
																lblErrorDetail
															</asp:Label>
														</td>
													</tr>
													<tr>
														<td style="WIDTH: 100%">
															<asp:HyperLink class="itemHotStyle" id="hlCoreTools" runat="server" NAME="hlCoreTools"> hlCoreTools </asp:HyperLink>
														</td>
													</tr>
												</table>
											</p>
										</div>
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

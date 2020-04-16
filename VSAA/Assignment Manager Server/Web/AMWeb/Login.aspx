<%@ Page language="c#" Codebehind="Login.cs" AutoEventWireup="false" Inherits="Microsoft.VisualStudio.Academic.AssignmentManager.Login" %>
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
				<td width="270" height="53" align="left" valign="top" style="z-index:-1;"><span class="title"><span class="titleCap">A</span><span class="titleBody">ssignment
						</span><span class="titleCap">M</span><span class="titleBody">anager</span></span>
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
					<asp:Label ID="lblSubTitle" Runat="server" class="infoText" NAME="lblSubTitle"></asp:Label>
					<br>
					<asp:Label ID="lblFeedback" Runat="server" class="errorText" NAME="lblFeedback" style="LEFT:20px;POSITION:relative"></asp:Label>
					<div id="copyText" class="infoText" runat="server">
						<table width="100%" cellspacing="0" cellpadding="0" border="0">
							<tr>
								<td valign="top" nowrap>
									<form action="" id="loginform" method="post" runat="server">
										<!-- Login dialog -->
										<div id="logindialog" class="itemStyle" style="LEFT:50px; POSITION:relative; TOP:10px">
											<p>
												<table width="300" border="0">
													<tr>
														<asp:Label class="infoText" id="lblUserName" runat="server" NAME="lblUserName">
															lblUserName
														</asp:Label>
													</tr>
													<tr>
														<asp:TextBox class="infoTextHeaderStyle" style="CURSOR:text" id="txtUserName" runat="server" maxlength="50" tabindex="1" NAME="txtUserName"></asp:TextBox>
													</tr>
													<tr>
														<asp:Label class="infoText" id="lblPassword" runat="server" NAME="lblPassword">
															lblPassword
														</asp:Label>
													</tr>
													<tr>
														<asp:TextBox class="infoTextHeaderStyle" style="CURSOR:text" id="txtPassword" runat="server" maxlength="50" textmode="Password" tabindex="2" NAME="txtPassword"></asp:TextBox>
													</tr>
										      <tr>
														<asp:Label visible="false" class="infoText" id="lblNewPwd" runat="server" NAME="lblNewPwd">
															lblNewPwd
														</asp:Label>
													</tr>
													<tr>
														<asp:TextBox visible="false" class="infoTextHeaderStyle" style="CURSOR:text" id="txtNewPwd" runat="server" maxlength="50" textmode="Password" tabindex="2" NAME="txtPassword"></asp:TextBox>
													</tr>
										      <tr>
														<asp:Label visible="false" class="infoText" id="lblConfirmPwd" runat="server" NAME="lblConfirmPwd">
															lblConfirmPwd
														</asp:Label>
													</tr>
													<tr>
														<asp:TextBox visible="false" class="infoTextHeaderStyle" style="CURSOR:text" id="txtConfirmPwd" runat="server" maxlength="50" textmode="Password" tabindex="2" NAME="txtPassword"></asp:TextBox>
													</tr>
													<tr "WIDTH: 119px">
														<td>
															<p>
															</p>
															<p>
																<asp:Button class="webButton" id="btnLogin" runat="server" Text="btnLogin" tabindex="3"></asp:Button>
															</p>
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

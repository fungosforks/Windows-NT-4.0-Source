<%@ Control Language="c#" AutoEventWireup="false" Codebehind="faculty.ascx.cs" Inherits="Microsoft.VisualStudio.Academic.AssignmentManager.UserControls.faculty" %>
<HTML>
	<HEAD>
		<script language="javascript">
var relativeURL= "<%Response.Write(relativeURL);%>";

// client-side function which resubmits the page; server-side Page_Load function checks the actionLogoff input tag
function Logoff_Click(){
	frmLogoff.actionLogoff.value = "logoff";
	frmLogoff.submit();	
}

		</script>
		<LINK REL="StyleSheet" HREF="<%Response.Write(relativeURL);%>scripts/assnMan.css" type="text/css">
			<script language="JavaScript" src="<%Response.Write(relativeURL);%>scripts/assnMan.js"></script>
			<script language="JavaScript" src="<%Response.Write(relativeURL);%>scripts/popUp.js"></script>
	</HEAD>
	<body leftmargin="0" topmargin="0" onload='<%
	if(SideTabId == Microsoft.VisualStudio.Academic.AssignmentManager.Common.constants.SIDE_NAV_COURSE_MANAGEMENT && TopTabId == Microsoft.VisualStudio.Academic.AssignmentManager.Common.constants.TOP_NAV_COURSE_INFO) {
		Response.Write("initStartState(leftTab0, CM0, courseManTable);");
	}
	if(SideTabId == Microsoft.VisualStudio.Academic.AssignmentManager.Common.constants.SIDE_NAV_COURSE_MANAGEMENT && TopTabId == Microsoft.VisualStudio.Academic.AssignmentManager.Common.constants.TOP_NAV_COURSE_ASSIGNMENTS) {
		Response.Write("initStartState(leftTab0, CM1, courseManTable);");
	}
	if(SideTabId == Microsoft.VisualStudio.Academic.AssignmentManager.Common.constants.SIDE_NAV_COURSE_MANAGEMENT && TopTabId == Microsoft.VisualStudio.Academic.AssignmentManager.Common.constants.TOP_NAV_COURSE_USERS) {
		Response.Write("initStartState(leftTab0, CM2, courseManTable);");
	}
	if(SideTabId == Microsoft.VisualStudio.Academic.AssignmentManager.Common.constants.SIDE_NAV_SERVER_ADMIN && TopTabId == Microsoft.VisualStudio.Academic.AssignmentManager.Common.constants.TOP_NAV_SERVER_SETTINGS) {
		Response.Write("initStartState(leftTab1, SA0, serverAdminTable);");
	}
	if(SideTabId == Microsoft.VisualStudio.Academic.AssignmentManager.Common.constants.SIDE_NAV_SERVER_ADMIN && TopTabId == Microsoft.VisualStudio.Academic.AssignmentManager.Common.constants.TOP_NAV_SERVER_MYACCOUNT) {
		Response.Write("initStartState(leftTab1, SA3, serverAdminTable);");
	}
%>'>
		<table id="navTable" name="navTable" width="100%" height="100%" border="0" cellspacing="0" cellpadding="0">
			<tr align="left" valign="top">
				<td rowspan="2" width="145" height="77" align="left" valign="top" nowrap>
					<img src="<%Response.Write(relativeURL);%>images/topLeft.jpg" width="145" height="77" alt="" border="0"></img>
				</td>
				<td width="270" height="53" align="left" valign="top" nowrap>
					<img src="<%Response.Write(relativeURL);%>images/topCenter.jpg" width="270" height="53" alt="" border="0" style="z-index:-1;"></img><span class="title"><span class="titleCap"><%Response.Write(UserControl_Faculty_CapA);%></span><span class="titleBody"><%Response.Write(UserControl_Faculty_Assignment);%>
						</span><span class="titleCap"><%Response.Write(UserControl_Faculty_CapM);%></span><span class="titleBody"><%Response.Write(UserControl_Faculty_Manager);%></span></span>
				</td>
				<td height="53" align="right" valign="bottom" nowrap>
					<form id="frmLogoff" action="" method="post">
						<div id="hlLogoff" class="bannerLinks" runat="server" onmouseOver="mStartLinkOver(this)" onmouseOut="mStartLinkOff(this)" onClick="javascript:Logoff_Click();" onkeypress="if(event.keyCode==13)this.click();" tabIndex="0">
							<input name="actionLogoff" type="hidden" value="none" ID="actionLogoff">
							<%Response.Write(UserControl_Faculty_LogOff);%>
						</div>
						<div class="bannerLinks" onClick="" onmouseOver="mStartLinkOver(this)" onmouseOut="mStartLinkOff(this)">
							<a id="hlBackToStartPage" href="<%Response.Write(StartPageLocation);%>">
								<%Response.Write(UserControl_Faculty_BackToStartPage);%>
							</a>
						</div>
					</form>
				</td>
			</tr>
			<tr align="left" valign="top">
				<td width="270" height="24" align="left" valign="top">
					&nbsp;
				</td>
				<td height="24" align="left" valign="top">
					&nbsp;
				</td>
			</tr>
			<tr align="left" valign="top" style="position:relative;top:-3;">
				<td width="145" bgcolor="#333366" align="left" valign="top">
					<table width="145" cellspacing="0" cellpadding="5" border="0" id="tabMenuTable">
						<tr>
							<td class="inActiveTab" valign="top" id="leftTab0" onmouseOver="tabMOver(0)" onMouseOut="tabMOff(0)" onkeypress="if(event.keyCode==13)this.click();" onClick='<%
							if(SideTabId != Microsoft.VisualStudio.Academic.AssignmentManager.Common.constants.SIDE_NAV_COURSE_MANAGEMENT){
								Response.Write("window.navigate(\u0022" + rootURL + "Faculty/AddEditCourse.aspx?CourseID=" + Request.QueryString.Get("CourseID") + "\u0022)");
							}
						%>'>
								<a tabIndex="1" style="cursor:hand;">
									<%
								Response.Write(UserControl_Faculty_CourseManagement);
								%>
								</a>
							</td>
						</tr>
						<tr>
							<td class="inActiveTab" valign="top" id="leftTab1" onmouseOver="tabMOver(1)" onMouseOut="tabMOff(1)" onkeypress="if(event.keyCode==13)this.click();" onClick='<%
							if(SideTabId != Microsoft.VisualStudio.Academic.AssignmentManager.Common.constants.SIDE_NAV_SERVER_ADMIN){
								Response.Write("window.navigate(\u0022" + rootURL + "Faculty/Settings.aspx?CourseID=" + Request.QueryString.Get("CourseID") + "\u0022)");
							}
						%>'>
								<a tabIndex="2" style="cursor:hand;">
									<%
								Response.Write(UserControl_Faculty_Server_Administration);
								%>
								</a>
							</td>
						</tr>
					</table>
				</td>
				<td width="*" align="left" valign="top" rowspan="2">
					&nbsp;&nbsp;
					<asp:Label ID="lblTitle" Runat="server" class="courseName"></asp:Label>
					<br>
					<asp:Label ID="lblSubTitle" Runat="server" class="infoText" style="position:relative;left:20px;"></asp:Label>
					<br>
					<asp:Label ID="Feedback" Runat="server" class="errorText" style="position:relative;left:20px;"></asp:Label>
					<div id="copyText" class="infoText">
					</div>
					<table id="courseManTable" class="innerTabBar" cellpadding="0" cellspacing="0">
						<tr id="courseTabsRow">
							<td class="inActiveInnTabA" id="CM0a" style="WIDTH:7px">
								<img id="CMtabCorner0" src="<%Response.Write(relativeURL);%>images/activeTab_Corner.gif" width="7" height="26" align="absbottom" alt="" border="0" style="Z-INDEX:9; LEFT:0px; POSITION:relative; TOP:0px"></img>
							</td>
							<td class="inActiveInnTab" id="CM0" nowrap align="middle" valign="center" onMouseOver="overInnerTabBar(this, CMtabCorner0)" onmouseOut="outInnerTabBar(this, CMtabCorner0)" onkeypress="if(event.keyCode==13)this.click();" onClick='<%
							if(TopTabId != Microsoft.VisualStudio.Academic.AssignmentManager.Common.constants.TOP_NAV_COURSE_INFO){
								Response.Write("window.navigate(\u0022" + rootURL + "Faculty/AddEditCourse.aspx?CourseID=" + Request.QueryString.Get("CourseID") + "\u0022)");
							}else{
								Response.Write("clickCMinnerTab(this)");
							}
						%>'>
								<a tabIndex="4" style="cursor:hand;">
									<%
								Response.Write(UserControl_Faculty_CourseInfo);
								%>
								</a>
							</td>
							<td class="inActiveInnTabA">
							</td>
							<td class="inActiveInnTabA" id="CM1a" style="WIDTH:7px">
								<img id="CMtabCorner1" src="<%Response.Write(relativeURL);%>images/inActiveTab_Corner.gif" width="7" height="26" align="absbottom" alt="" border="0" style="Z-INDEX:9; LEFT:0px; POSITION:relative; TOP:0px"></img>
							</td>
							<td class="inActiveInnTab" id="CM1" nowrap align="middle" valign="center" onMouseOver="overInnerTabBar(this, CMtabCorner1)" onmouseOut="outInnerTabBar(this, CMtabCorner1)" onkeypress="if(event.keyCode==13)this.click();" onClick='<%
							if(TopTabId != Microsoft.VisualStudio.Academic.AssignmentManager.Common.constants.TOP_NAV_COURSE_ASSIGNMENTS){
								Response.Write("window.navigate(\u0022" + rootURL + "Faculty/Assignments.aspx?CourseID=" + Request.QueryString.Get("CourseID") + "\u0022)");
							}else{
								Response.Write("clickCMinnerTab(this)");
							}
						%>'>
								<a tabIndex="5" style="cursor:hand;">
									<%
								Response.Write(UserControl_Faculty_CourseAssignments);
								%>
								</a>
							</td>
							<td class="inActiveInnTabA">
							</td>
							<td class="inActiveInnTabA" id="CM2a" style="WIDTH:7px">
								<img id="CMtabCorner2" src="<%Response.Write(relativeURL);%>images/inActiveTab_Corner.gif" width="7" height="26" align="absbottom" alt="" border="0" style="Z-INDEX:9; LEFT:0px; POSITION:relative; TOP:0px"></img>
							</td>
							<td class="inActiveInnTab" id="CM2" nowrap align="middle" valign="center" onMouseOver="overInnerTabBar(this, CMtabCorner2)" onmouseOut="outInnerTabBar(this, CMtabCorner2)" onkeypress="if(event.keyCode==13)this.click();" onClick='<%
							if(TopTabId != Microsoft.VisualStudio.Academic.AssignmentManager.Common.constants.TOP_NAV_COURSE_USERS){
								Response.Write("window.navigate(\u0022" + rootURL + "Faculty/Users.aspx?CourseID=" + Request.QueryString.Get("CourseID") + "\u0022)");
							}else{
								Response.Write("clickCMinnerTab(this,0)");
							}
						%>'>
								<a tabIndex="6" style="cursor:hand;">
									<%
								Response.Write(UserControl_Faculty_CourseUsers);
								%>
								</a>
							</td>
							<td class="inActiveInnTabA" style="WIDTH:250px">
								&nbsp;&nbsp;&nbsp;&nbsp;
							</td>
						</tr>
						<tr>
							<td colspan="9" align="left" valign="top" style="BORDER-RIGHT: #ffcc99 1px solid; PADDING-RIGHT: 15px; PADDING-LEFT: 15px; PADDING-BOTTOM: 15px; BORDER-LEFT: #ffcc99 1px solid; PADDING-TOP: 15px; BORDER-BOTTOM: #ffcc99 1px solid">
								<div id="courseInfo" class="courseDiv">
									<!--Course: Information-->
									&nbsp;
								</div>
								<div id="courseAssignments" class="courseDiv">
									<!--Course: Assignment Information-->
									&nbsp;
								</div>
								<div id="courseUsers" class="courseDiv">
									<!--Course: Users Information-->
									&nbsp;
								</div>
							</td>
						</tr>
					</table>
					<table id="serverAdminTable" class="innerTabBar" cellpadding="0" cellspacing="0">
						<tr id="serversTabsRow">
							<td class="activeInnTabA" id="SA0a" border="0" style="WIDTH:7px">
								<img id="SAtabCorner0" src="<%Response.Write(relativeURL);%>images/activeTab_Corner.gif" width="7" height="26" align="absbottom" alt="" border="0" style="Z-INDEX:9; LEFT:0px; POSITION:relative; TOP:0px"></img>
							</td>
							<td class="activeInnTab" id="SA0" nowrap border="0" align="middle" valign="center" onMouseOver="overInnerTabBar(this, SAtabCorner0)" onmouseOut="outInnerTabBar(this, SAtabCorner0)" onkeypress="if(event.keyCode==13)this.click();" onClick='<%
							if(TopTabId != Microsoft.VisualStudio.Academic.AssignmentManager.Common.constants.TOP_NAV_SERVER_SETTINGS){
								Response.Write("window.navigate(\u0022" + rootURL + "Faculty/Settings.aspx?CourseID=" + Request.QueryString.Get("CourseID") + "\u0022)");
							}else{
								Response.Write("clickSAinnerTab(this)");
							}
						%>'>
								<a tabIndex="7" style="cursor:hand;">
									<%
								Response.Write(UserControl_Faculty_Settings);
								%>
								</a>
							</td>
							<td class="inActiveInnTabA">
							</td>
							<td class="inActiveInnTabA" id="SA3a" border="0" style="WIDTH:7px">
								<img id="SAtabCorner3" src="<%Response.Write(relativeURL);%>images/inActiveTab_Corner.gif" width="7" height="26" align="absbottom" alt="" border="0" style="Z-INDEX:9; LEFT:0px; POSITION:relative; TOP:0px"></img>
							</td>
							<td class="inActiveInnTab" id="SA3" nowrap align="middle" border="0" valign="center" onMouseOver="overInnerTabBar(this, SAtabCorner3)" onmouseOut="outInnerTabBar(this, SAtabCorner3)" onkeypress="if(event.keyCode==13)this.click();" onClick='<%
							if(TopTabId != Microsoft.VisualStudio.Academic.AssignmentManager.Common.constants.TOP_NAV_SERVER_MYACCOUNT){
								Response.Write("window.navigate(\u0022" + rootURL + "Faculty/AddEditUser.aspx?CourseID=" + Request.QueryString.Get("CourseID") + "&UserID=-1\u0022)");
							}else{
								Response.Write("clickSAinnerTab(this)");
							}
						%>'>
								<a tabIndex="9" style="cursor:hand;">
									<%
								Response.Write(UserControl_Faculty_MyAccount);
								%>
								</a>
							</td>
							<td class="inActiveInnTabA" nowrap border="0" style="WIDTH:200px">
								&nbsp;&nbsp;&nbsp;&nbsp;
							</td>
						</tr>
						<tr>
							<td colspan="9" align="left" valign="top" style="BORDER-RIGHT: #ffcc99 1px solid; PADDING-RIGHT: 15px; PADDING-LEFT: 15px; PADDING-BOTTOM: 15px; BORDER-LEFT: #ffcc99 1px solid; PADDING-TOP: 15px; BORDER-BOTTOM: #ffcc99 1px solid">
								<div id="serSettings" class="courseDiv">
									<!--Server: Settings-->
									&nbsp;
								</div>
								<div id="serSecurity" class="courseDiv">
									<!--Server: Security-->
									&nbsp;
								</div>
								<div id="serMyAccount" class="courseDiv">
									<!--Server: My Account-->
									&nbsp;
								</div>
							</td>
						</tr>
					</table>
				</td>
			</tr>
		</table>

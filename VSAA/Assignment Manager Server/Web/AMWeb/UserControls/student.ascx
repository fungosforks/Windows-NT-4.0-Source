<%@ Control Language="c#" AutoEventWireup="false" Codebehind="student.ascx.cs" Inherits="Microsoft.VisualStudio.Academic.AssignmentManager.UserControls.student" %>
<script language="javascript">
var relativeURL= "<%Response.Write(relativeURL);%>";

// client-side function which resubmits the page; server-side Page_Load function checks the actionLogoff input tag
function Logoff_Click(){
	frmLogoff.actionLogoff.value = "logoff";
	frmLogoff.submit();	
}
		
</script>
<LINK REL="StyleSheet" HREF="<%Response.Write(relativeURL);%>scripts/assnMan.css" type="text/css">
	<script language="JavaScript" src="<%Response.Write(relativeURL);%>scripts/assnManStudent.js"></script>
	<script language="JavaScript" src="<%Response.Write(relativeURL);%>scripts/popUp.js"></script>
	<body leftmargin="0" topmargin="0" onload='<%
	//Student Course - CourseInfo Tab
	if(SideTabId == Microsoft.VisualStudio.Academic.AssignmentManager.Common.constants.SIDE_NAV_STUDENT_COURSE && TopTabId == Microsoft.VisualStudio.Academic.AssignmentManager.Common.constants.TOP_NAV_STUDENT_COURSE_INFO) {
		Response.Write("initStartState(leftTab0, CM0, courseManTable);");
	}
	//Student Course - CourseAssignments Tab
	if(SideTabId == Microsoft.VisualStudio.Academic.AssignmentManager.Common.constants.SIDE_NAV_STUDENT_COURSE && TopTabId == Microsoft.VisualStudio.Academic.AssignmentManager.Common.constants.TOP_NAV_STUDENT_COURSE_ASSIGNMENTS) {
		Response.Write("initStartState(leftTab0, CM1, courseManTable);");
	}
	//Student Change Password - Change Password Tab
	if(SideTabId == Microsoft.VisualStudio.Academic.AssignmentManager.Common.constants.SIDE_NAV_STUDENT_CHANGE_PASSWORD && TopTabId == Microsoft.VisualStudio.Academic.AssignmentManager.Common.constants.TOP_NAV_STUDENT_CHANGE_PASSWORD) {
		Response.Write("initStartState(leftTab1, PASS0, passwordTable);");
	}
%>'>
		<table id="navTable" name="navTable" width="100%" height="100%" border="0" cellspacing="0" cellpadding="0">
			<tr align="left" valign="top">
				<td rowspan="2" width="145" height="77" align="left" valign="top" nowrap>
					<img src="<%Response.Write(relativeURL);%>images/topLeft.jpg" width="145" height="77" alt="" border="0"></img>
				</td>
				<td width="270" height="53" align="left" valign="top" nowrap>
					<img src="<%Response.Write(relativeURL);%>images/topCenter.jpg" width="270" height="53" alt="" border="0" style="z-index:-1;"></img><span class="title"><span class="titleCap"><%Response.Write(UserControl_Student_CapA);%></span><span class="titleBody"><%Response.Write(UserControl_Student_Assignment);%></span> <span class="titleCap"><%Response.Write(UserControl_Student_CapM);%></span><span class="titleBody"><%Response.Write(UserControl_Student_Manager);%></span></span>
				</td>
				<td height="53" align="left" style="LEFT:750px;POSITION:absolute" valign="bottom" nowrap>
					<form id="frmLogoff" action="" method="post">
						<div class="bannerLinks" runat="server" onmouseOver="mStartLinkOver(this)" onmouseOut="mStartLinkOff(this)" onClick="javascript:Logoff_Click();" ID="Div1" onkeypress="if(event.keyCode==13)this.click();" tabIndex="0">
							<input name="actionLogoff" type="hidden" value="none" ID="actionLogoff">
							<%Response.Write(UserControl_Student_LogOff);%>
						</div>
						<div class="bannerLinks" onClick="" onmouseOver="mStartLinkOver(this)" onmouseOut="mStartLinkOff(this)">
							<a href="<%Response.Write(StartPageLocation);%>">
								<%Response.Write(UserControl_Student_BackToStartPage);%>
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
			<tr align="left" valign="top" style="POSITION:relative;TOP:-3;">
				<td width="145" bgcolor="#333366" align="left" valign="top">
					<table width="145" cellspacing="0" cellpadding="5" border="0" id="tabMenuTable">
						<tr>
							<td class="inActiveTab" valign="top" id="leftTab0" onmouseOver="tabMOver(0)" onMouseOut="tabMOff(0)" onkeypress="if(event.keyCode==13)this.click();" onClick='<%
							if(SideTabId != Microsoft.VisualStudio.Academic.AssignmentManager.Common.constants.SIDE_NAV_STUDENT_COURSE){
								Response.Write("window.navigate(\u0022" + rootURL + "Student/CourseInfo.aspx?CourseID=" + Request.QueryString.Get("CourseID") + "\u0022)");
							}else{
								Response.Write("setLeftMenu(this,0)");
							}
						%>'>
								<a tabIndex="1" style="cursor:hand;">
									<%Response.Write(UserControl_Student_Course);%>
								</a>
							</td>
							
						</tr>
						<tr>
							<td class="inActiveTab" valign="top" id="leftTab1" onmouseOver="tabMOver(1)" onMouseOut="tabMOff(1)" onkeypress="if(event.keyCode==13)this.click();" onClick='<%
							if(SideTabId != Microsoft.VisualStudio.Academic.AssignmentManager.Common.constants.SIDE_NAV_STUDENT_CHANGE_PASSWORD){
								Response.Write("window.navigate(\u0022" + rootURL + "Student/ChangePassword.aspx?CourseID=" + Request.QueryString.Get("CourseID") + "\u0022)");
							}else{
								Response.Write("setLeftMenu(this,0)");
							}
						%>'>
								<a tabIndex="3" style="cursor:hand;">
									<%Response.Write(UserControl_Student_Password);%>
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
					<div id="copyText" class="infoText" runat="server">
					</div>
					<table id="courseManTable" class="innerTabBar" cellpadding="0" cellspacing="0">
						<tr id="courseTabsRow">
							<td class="inActiveInnTabA" id="CM0a" style="width:7;">
								<img id="CMtabCorner0" src="<%Response.Write(relativeURL);%>images/activeTab_Corner.gif" width="7" height="29" align="absbottom" alt="" border="0" style="Z-INDEX:9; LEFT:0px; POSITION:relative; TOP:0px">
							</td>
							<td class="inActiveInnTab" id="CM0" nowrap align="center" valign="middle" onMouseOver="overInnerTabBar(this, CMtabCorner0)" onmouseOut="outInnerTabBar(this, CMtabCorner0)" onkeypress="if(event.keyCode==13)this.click();" onClick='<%
							if(TopTabId != Microsoft.VisualStudio.Academic.AssignmentManager.Common.constants.TOP_NAV_STUDENT_COURSE_INFO){
								Response.Write("window.navigate(\u0022" + rootURL + "Student/CourseInfo.aspx?CourseID=" + Request.QueryString.Get("CourseID") + "\u0022)");
							}else{
								Response.Write("clickCMinnerTab(this,0)");
							}
						%>'><a tabIndex="4" style="cursor:hand;">
									<%Response.Write(UserControl_Student_CourseInfo);%>
								</a>
							</td>
							<td class="inActiveInnTabA">
							</td>
							<td class="inActiveInnTabA" id="CM1a" style="WIDTH:7px">
								<img id="CMtabCorner1" src="<%Response.Write(relativeURL);%>images/inActiveTab_Corner.gif" width="7" height="29" align="absbottom" alt="" border="0" style="Z-INDEX:9; LEFT:0px; POSITION:relative; TOP:0px"></img>
							</td>
							<td class="inActiveInnTab" id="CM1" nowrap align="center" valign="middle" onMouseOver="overInnerTabBar(this, CMtabCorner1)" onmouseOut="outInnerTabBar(this, CMtabCorner1)" onkeypress="if(event.keyCode==13)this.click();" onClick='<%
							if(TopTabId != Microsoft.VisualStudio.Academic.AssignmentManager.Common.constants.TOP_NAV_STUDENT_COURSE_ASSIGNMENTS){
								Response.Write("window.navigate(\u0022" + rootURL + "Student/Assignments.aspx?CourseID=" + Request.QueryString.Get("CourseID") + "\u0022)");
							}else{
								Response.Write("clickCMinnerTab(this,0)");
							}
						%>'><a tabIndex="5" style="cursor:hand;">
									<%Response.Write(UserControl_Student_CourseAssignments);%>
								</a>
							</td>
							<td class="inActiveInnTabA" nowrap width="300">
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
							</td>
						</tr>
					</table>
					<table id="passwordTable" class="innerTabBar" cellpadding="0" cellspacing="0">
						<tr id="passwordTabsRow">
							<td class="activeInnTabA" id="PASS0a" style="width:7;">
								<img id="PASStabCorner0" src="<%Response.Write(relativeURL);%>images/activeTab_Corner.gif" width="7" height="29" align="absbottom" alt="" border="0" style="position:relative; left:0; top:0; z-index:9;">
							</td>
							<td class="activeInnTab" id="PASS0" nowrap align="center" valign="middle" onMouseOver="overInnerTabBar(this, PASStabCorner0)" onmouseOut="outInnerTabBar(this, PASStabCorner0)" onClick='<%
							if(TopTabId != Microsoft.VisualStudio.Academic.AssignmentManager.Common.constants.TOP_NAV_STUDENT_CHANGE_PASSWORD){
								Response.Write("window.navigate(\u0022" + rootURL + "Student/ChangePassword.aspx?CourseID=" + Request.QueryString.Get("CourseID") + "&UserID=" + userId + "\u0022)");
							}else{
								Response.Write("clickPASSinnerTab(this,0)");
							}
						%>'><a tabIndex="7" style="cursor:hand;">
									<%Response.Write(UserControl_Student_Password2);%>
								</a>
							</td>
							<td class="inActiveInnTabA" nowrap width="485">
								&nbsp;&nbsp;&nbsp;&nbsp;
							</td>
						</tr>
						<tr>
							<td colspan="3" align="left" valign="top" style="BORDER-RIGHT: #ffcc99 1px solid; PADDING-RIGHT: 15px; PADDING-LEFT: 15px; PADDING-BOTTOM: 15px; BORDER-LEFT: #ffcc99 1px solid; PADDING-TOP: 15px; BORDER-BOTTOM: #ffcc99 1px solid">
								<div id="passChange" class="courseDiv">
									&nbsp;
								</div>
							</td>
						</tr>
					</table>
				</td>
			</tr>
		</table>

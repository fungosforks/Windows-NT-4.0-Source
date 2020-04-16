<%@ Register TagPrefix="AM" TagName="GoBack" Src="../UserControls/goBack.ascx" %>
<%@ Page language="c#" Codebehind="Assignments.aspx.cs" AutoEventWireup="false" Inherits="Microsoft.VisualStudio.Academic.AssignmentManager.Student.Assignments" %>
<%@ Register TagPrefix="AM" TagName="Nav" Src="../UserControls/student.ascx" %>
<HTML>
	<HEAD>
		<title>
<%Response.Write(Title);%>
		</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<!DOCTYPE HTML public="-//W3C//DTD HTML 4.0 Transitional//EN">
		<script language="JavaScript" src="../common/functions.js"></script><script language="javascript">
		function window.onload(){
		try {
		g_Attrib = window.external.ActiveWindow.ContextAttributes.Add("vs.am.coursemanagement.courseassignments","vs.vshomepage.get.mycourses.workwithcourse",3); // Type '3' is an F1 keyword
		} 
		catch (helpFailed) {}
		}
	
		function window.onunload(){
		  try {
      g_Attrib.Remove();
    } catch (helpFailed) {
    }
		}
	</script><!-- nav user control here -->
		
		<AM:Nav id="Nav1" runat="server" name="Nav1">
		</AM:Nav>
		<form method="post" runat="server">
	</HEAD>
	<BODY>
		<div id="mainBody" class="mainBody">
			<table width="75%" height="80%" cellspacing="0" cellpadding="0" border="0" style="LEFT:150px; POSITION:relative; TOP:10px">
				<tr>
					<td align="right">
						<AM:GoBack id="GoBack1" name="GoBack1" runat="server">
						</AM:GoBack>
					</td>
				</tr>
			</table>
			<table width="100%" height="80%" cellspacing="0" cellpadding="0" border="0" style="LEFT:50px; POSITION:relative; TOP:10px">
				<tr>
					<td>
						<div id="feedbacklabel" class="infoText">
							<asp:Label id="lblCourseName" runat="server" NAME="lblCourseName"></asp:Label>
						</div>
						<p>
							<asp:DataList id="dlAssignments" runat="server" class="datalistStyle" NAME="dlAssignments" OnItemDataBound="dlAssignments_ItemDataBound">
								<AlternatingItemStyle cssclass="rowStyleEven" />
								<HeaderStyle cssclass="infoTextTableHeaderStyle" />
								<ItemStyle cssclass="rowStyleOdd" />
								<HeaderTemplate>
									<table width="100%" class="tableStyle" border="0" cellspacing="0" cellpadding="0">
										<tr class="infoTextTableHeaderStyle">
											<td align="left" WIDTH="120px">
<%Response.Write(assignments);%>
											</td>
											<td align="left" WIDTH="79px">
<%Response.Write(dueDate);%>
											</td>
											<td align="center" width="63px">
<%Response.Write(getStarter);%>
											</td>
											<td align="center" width="63px">
<%Response.Write(submitProject);%>
											</td>
											<td align="center" WIDTH="75px">
<%Response.Write(dateSubmitted);%>
											</td>
											<td align="center" width="63px">
<%Response.Write(autoCompile);%>
											</td>
											<td align="center" width="63px">
<%Response.Write(autoGrade);%>
											</td>
											<td align="left" width="63px">
<%Response.Write(grade);%>
											</td>
										</tr>
									</table>
								</HeaderTemplate>
								<ItemTemplate>
									<table width="100%" id="tblMessages1" border="0" cellspacing="0" cellpadding="0">
										<tr align="center" class="infoText">
											<td width="120px" align="left">
												<a id="dlAssignmentName" class="itemHotStyle" href='AssignmentGrade.aspx?AssignmentID=<%# DataBinder.Eval(Container.DataItem, "AssignmentID")%>&CourseID=<%Response.Write(courseId);%>&Exp=1'>
<%# DataBinder.Eval(Container.DataItem, "ShortName")%>
												</a>
											</td>
											<td id="dlAssignmentsDueDate" width="79px" align="left">
<%# DataBinder.Eval(Container.DataItem, "DueDate", "{0:g}")%>
											</td>
											<td width="63px" align="center">
												<!--
												<a href='../Student/UploadDownload.aspx?Action=downloadstarter&AssignmentID=<%# DataBinder.Eval(Container.DataItem, "AssignmentID")%>&CourseID=<%Response.Write(courseId);%>'>
												</a>
											-->
												
												<asp:HyperLink ID="hlStarterAvailable" NavigateUrl="UploadDownload.aspx" ImageUrl="../images/downloadProject.gif" Visible="False" Runat="server" />
												<asp:Image ID="imgStarterNotAvailable" ImageUrl="../images/0.gif" Visible="False" Runat="server" />
											</td>
											<td width="63px" align="center">
												<a id="dlAssignmentsUploadSubmission" href='../Student/UploadDownload.aspx?Action=uploadsubmission&CourseID=<%Response.Write(courseId.ToString());%>&AssignmentID=<%# DataBinder.Eval(Container.DataItem, "AssignmentID")%>'><img src="../images/upload.gif" border="0"> </a>
											</td>
											<td id="dlAssignmentsSubmitDate" width="75px" align="left">
<%# DataBinder.Eval(Container.DataItem, "LastSubmitDate", "{0:g}")%>
												&nbsp;
											</td>
											<td width="63px" align="center">
												<img id="dlAssignmentsAutoBuildStatus" src='../images/<%#DataBinder.Eval(Container.DataItem, "AutoCompileStatus")%>.gif' border="0"></img>
											</td>
											<td width="63px" align="center">
												<img id="dlAssignmentsAutoCheckStatus" src='../images/<%#DataBinder.Eval(Container.DataItem, "AutoGradeStatus")%>.gif' border="0"></img>
											</td>
											<td width="63px" align="left">
												<a id="dlAssignmentsGrade" class="itemHotStyle" href='AssignmentGrade.aspx?AssignmentID=<%# DataBinder.Eval(Container.DataItem, "AssignmentID")%>&CourseID=<%Response.Write(courseId);%>'>
<%#DataBinder.Eval(Container.DataItem, "OverallGrade")%>
												</a>
											</td>
										</tr>
									</table>
								</ItemTemplate>
							</asp:DataList>
						</p>
						</FORM>
					</td>
				</tr>
			</table>
			<table style="LEFT:50px; POSITION:relative; TOP:10px" border="0" class="infoText">
				<tr>
					<td valign="bottom" align="left">
						<img src="../images/2.gif" border="0">
					</td>
					<td valign="top">
						<asp:label id="lblCompleted" border="0" runat="server"></asp:label>
					</td>
				</tr>
				<tr>
					<td align="left" valign="bottom">
						<img src="../images/1.gif" border="0">
					</td>
					<td valign="top">
						<asp:label id="lblPending" border="0" runat="server"></asp:label>
					</td>
				</tr>
				<tr>
					<td align="left" valign="bottom">
						<img src="../images/3.gif" border="0">
					</td>
					<td valign="top">
						<asp:label id="lblFailed" runat="server" border="0" NAME="lblFailed"></asp:label>
					</td>
				</tr>
			</table>
		</div>
	</BODY>
</HTML>

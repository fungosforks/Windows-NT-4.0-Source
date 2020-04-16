<%@ Page language="c#" Codebehind="Assignments.cs" AutoEventWireup="false" Inherits="Microsoft.VisualStudio.Academic.AssignmentManager.Faculty.Assignments" %>
<%@ Register TagPrefix="AM" TagName="Nav" Src="../UserControls/faculty.ascx" %>
<%@ Register TagPrefix="AM" TagName="GoBack" Src="../UserControls/goBack.ascx" %>
<HTML>
	<HEAD>
		<TITLE>
			<%Response.Write(Title);%>
		</TITLE>
	</HEAD>
	<meta name="vs_showGrid" content="True">
	<script language="JavaScript" src="../common/functions.js"></script>
	<script language="javascript">
		function window.onload(){
		try {
		g_Attrib = window.external.ActiveWindow.ContextAttributes.Add("keyword","vs.am.coursemanagement.courseassignments",3); // Type '3' is an F1 keyword
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
		<!-- custom code here -->
		<table width="100%" height="80%" cellspacing="0" cellpadding="0" border="0" style="LEFT:50px; POSITION:relative; TOP:10px">
			<tr>
				<td align="left">
					<AM:GoBack id="GoBack1" name="GoBack1" runat="server"></AM:GoBack>
				</td>
			</tr>
			<tr>
				<td valign="top" nowrap>
					<div id="feedbacklabel" class="head1">
						<asp:Label id="lblCourseName" runat="server"></asp:Label>
					</div>
					<p>
						<asp:DataList id="dlAssignments" runat="server" class="datalistStyle">
							<AlternatingItemStyle cssclass="rowStyleEven" />
							<HeaderStyle cssclass="infoTextTableHeaderStyle" />
							<ItemStyle cssclass="rowStyleOdd" />
							<HeaderTemplate>
								<table width="100%" class="tableStyle" border="0" cellspacing="0" cellpadding="0">
									<tr class="infoTextTableHeaderStyle">
										<td nowrap width="150">
											<%Response.Write(Assignments_Text_String);%>
										</td>
										<td nowrap width="200">
											<%Response.Write(DueDate_Text_String);%>
										</td>
										<td nowrap width="150" align="center">
											<%Response.Write(NumberSubmitting_Text_String);%>
										</td>
										<td nowrap width="125" align="center">
											<%Response.Write(UploadStarter_Text_String);%>
										</td>
									</tr>
								</table>
							</HeaderTemplate>
							<ItemTemplate>
								<table width="100%" class="infoText" border="0" cellspacing="0" cellpadding="0">
									<tr>
										<td nowrap width="150">
											<a id="dlAssignmentsName" class="itemHotStyle" href='AddEditAssignment.aspx?AssignmentID=<%#DataBinder.Eval(Container.DataItem, "AssignmentID")%>&CourseID=<%Response.Write(Request.QueryString["CourseID"].ToString()); %>'>
												<%#DataBinder.Eval(Container.DataItem, "ShortName")%>
											</a>
										</td>
										<td id="dlAssignmentsDueDate" nowrap width="200">
											<%#DataBinder.Eval(Container.DataItem, "DueDate", "{0:G}")%>
										</td>
										<td nowrap width="150" align="center">
											<a id="dlAssignmentsNumberSubmitted" class="itemHotStyle" href='Submissions.aspx?AssignmentID=<%#DataBinder.Eval(Container.DataItem, "AssignmentID")%>&CourseID=<%Response.Write(Request.QueryString["CourseID"].ToString()); %>'>
												<%#DataBinder.Eval(Container.DataItem, "NumberSubmitted")%>
												/
												<%#DataBinder.Eval(Container.DataItem, "TotalNumber")%>
											</a>
										</td>
										<td nowrap width="125" align="center">
											<a id="dlAssignmentsUploadStarter" href='UploadDownload.aspx?Action=UploadStarter&AssignmentID=<%#DataBinder.Eval(Container.DataItem, "AssignmentID")%>&CourseID=<%Response.Write(Request.QueryString["CourseID"].ToString()); %>'>
												<img src="../images/upload.gif" border="0"> </a>
										</td>
									</tr>
								</table>
							</ItemTemplate>
							<FooterTemplate></FooterTemplate>
						</asp:DataList>
					</p>
					<div class="infoText">
						<asp:HyperLink class="itemHotStyle" id="hlAddAssignment" runat="server" NavigateUrl="AddEditAssignment.aspx"
							NAME="hlAddAssignment"></asp:HyperLink><BR>
						&nbsp;
					</div>
				</td>
			</tr>
		</table>
	</div>
</HTML>

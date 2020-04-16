<%@ Register TagPrefix="AM" TagName="Nav" Src="../UserControls/faculty.ascx" %>
<%@ Register TagPrefix="AM" TagName="GoBack" Src="../UserControls/goBack.ascx" %>
<%@ Page language="c#" Codebehind="Submissions.cs" AutoEventWireup="false" Inherits="Microsoft.VisualStudio.Academic.AssignmentManager.Faculty.Submissions" %>
<html>
	<head>
		<title>
			<%Response.Write(Title);%>
		</title>
		<script language="JavaScript" src="../scripts/functions.js"></script>
	</head>
	<AM:Nav id="Nav1" runat="server" name="Nav1"></AM:Nav>
	<div id="mainBody" class="mainBody">
		<!-- custom code here -->
		<form id="Form1" encType="multipart/form-data" runat="server">
			<table style="LEFT:50px; POSITION:relative; TOP:10px">
				<tr>
					<td>
						<asp:label id="lblSubmissions" runat="server" class="infoTextPageTitle"></asp:label>
						<AM:GoBack id="GoBack1" name="GoBack1" runat="server"></AM:GoBack>
					</td>
					<td>
					</td>
				</tr>
				<tr>
					<td valign="top" nowrap>
						<!-- action buttons -->
						<p>
							<asp:DataList id="dlUserAssignments" runat="server">
								<HeaderTemplate>
									<table cellspacing="0" cellpadding="3" class="tableStyle" border="0">
										<tr class="columnHead">
											<td nowrap align="middle">
												&nbsp;
											</td>
											<td nowrap align="middle">
												<% Response.Write(Student_Text_String);%>
											</td>
											<td nowrap align="middle">
												<% Response.Write(Date_Last_Submitted_Text_String);%>
											</td>
											<td nowrap align="middle">
												<% Response.Write(Auto_Compile_Text_String);%>
											</td>
											<td nowrap align="middle">
												<% Response.Write(Auto_Grade_Text_String);%>
											</td>
											<td nowrap align="middle">
												<% Response.Write(Grade_Text_String);%>
											</td>
											<td nowrap align="middle">
												<% Response.Write(Get_Submission_Text_String);%>
											</td>
										</tr>
								</HeaderTemplate>
								<ItemTemplate>
									<tr>
										<td nowrap align="center">
											<asp:CheckBox id="chkItemRerun" runat="server"></asp:CheckBox>
											<asp:TextBox id=txtUserAssignmentID runat="server" class="invisible" Text='<%#DataBinder.Eval(Container.DataItem, "UserAssignmentID")%>'>
											</asp:TextBox>
										</td>
										<td nowrap align="center">
											<a id="dlUserAssignmentsUser" class="itemHotStyle" href='AddEditUser.aspx?UserID=<%#DataBinder.Eval(Container.DataItem, "UserID")%>&<%Response.Write(Request.QueryString.ToString());%>'>
												<%#DataBinder.Eval(Container.DataItem, "LastName")%>
												,&nbsp;&nbsp;<%#DataBinder.Eval(Container.DataItem, "FirstName")%></a>
										</td>
										<td id="dlUserAssignmentsDateSubmitted" nowrap align="center">
											<%#DataBinder.Eval(Container.DataItem, "LastSubmitDate", "{0:g}")%>
										</td>
										<td id="dlUserAssignmentsAutoBuild" nowrap align="center">
											<img src='../images/<%#DataBinder.Eval(Container.DataItem, "AutoCompileStatus")%>.gif' border="0">
										</td>
										<td id="dlUserAssignmentsAutoCheck" nowrap align="center">
											<img src='../images/<%#DataBinder.Eval(Container.DataItem, "AutoGradeStatus")%>.gif' border="0">
										</td>
										<td nowrap align="center">
											<a id="dlUserAssignmentsGrade" class="itemHotStyle" href='GradeSubmission.aspx?UserID=<%#DataBinder.Eval(Container.DataItem, "UserID")%>&<%Response.Write(Request.QueryString.ToString());%>'>
												<%#DataBinder.Eval(Container.DataItem, "OverallGrade")%>
											</a>
										</td>
										<td nowrap align="center">
											<a id="dlUserAssignmentsDownloadSubmission" href='UploadDownload.aspx?CourseID=<%=courseId%>&StudentID=<%#DataBinder.Eval(Container.DataItem, "UserID")%>&AssignmentID=<%Response.Write(assignmentId.ToString());%>&Action=downloadsubmission'>
												<img src="../images/downloadProject.gif" border="0"> </a>
										</td>
									</tr>
								</ItemTemplate>
								<FooterTemplate>
			</table>
			</FooterTemplate> </asp:DataList> </P> </TR>
			<tr>
				<td align="right">
					<asp:Button id="btnAutoCompile" text="btnAutoCompile" runat="server" class="webButton" width="150"></asp:Button>
					<asp:Button id="btnAutoGrade" text="btnAutoGrade" runat="server" class="webButton" width="150"></asp:Button>
				</td>
			</tr>
			</TABLE>
		</form>
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
</html>

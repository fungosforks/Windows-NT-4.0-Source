<%@ Register TagPrefix="AM" TagName="GoBack" Src="../UserControls/goBack.ascx" %>
<%@ Register TagPrefix="AM" TagName="Nav" Src="../UserControls/faculty.ascx" %>
<%@ Page language="c#" Codebehind="AddEditAssignment.cs" AutoEventWireup="false" Inherits="Microsoft.VisualStudio.Academic.AssignmentManager.Faculty.AddEditAssignment" %>
<HTML>
	<HEAD>
		<TITLE>
			<%Response.Write(Title);%>
		</TITLE>
	</HEAD>
	<script language="JavaScript" src="../common/functions.js"></script>
	<script language="javascript">
		//********************************************// //*** Global Variables ***// //********************************************// //****Global Error strings<%
	Response.Write("var err_No_Action = '" + UploadDownload_err_No_Action + "';\r\n");
	Response.Write("var err_No_Files_Downloaded = '" + UploadDownload_err_No_Files_Downloaded + "';\r\n");
	Response.Write("var err_No_Files_Uploaded = '" + UploadDownload_err_No_Files_Uploaded + "';\r\n");
	Response.Write("var err_Server_Location_Not_Found = '" + UploadDownload_err_Server_Location_Not_Found + "';\r\n");
	Response.Write("var err_Local_Download_Location_Not_Exist = '" + UploadDownload_err_Local_Download_Location_Not_Exist + "';\r\n");
	Response.Write("var err_XMLFileList_Load_Failed = '" + UploadDownload_err_XMLFileList_Load_Failed + "';\r\n");
	Response.Write("var err_File_Does_Not_Exist = '" + UploadDownload_err_File_Does_Not_Exist + "';\r\n");
	Response.Write("var err_Attempt_To_Copy = '" + UploadDownload_err_Attempt_To_Copy + "';\r\n");
	Response.Write("var err_Attempt_To_Copy_Failed = '" + UploadDownload_err_Attempt_To_Copy_Failed + "';\r\n");
	Response.Write("var err_Upload_Location_Not_Available = '" + UploadDownload_err_Upload_Location_Not_Available + "';\r\n");
	Response.Write("var err_Download_Location_Not_Available = '" + UploadDownload_err_Download_Location_Not_Available + "';\r\n");
	Response.Write("var prob_Opening_Web_Project = '" + UploadDownload_prob_Opening_Web_Project + "';\r\n");
	Response.Write("var err_Select_Proj_To_Upload = '" + UploadDownload_err_Select_Proj_To_Upload + "';\r\n");
	Response.Write("var err_Terminate_And_Delete = '" + UploadDownload_err_Terminate_And_Delete + "';\r\n");
	Response.Write("var err_The_Copy_of = '" + UploadDownload_err_The_Copy_of + "';\r\n");
	Response.Write("var err_Failed = '" + UploadDownload_err_Failed + "';");
	Response.Write("var err_Exceeded_Max_Size = '" + UploadDownload_err_Exceeded_Max_Size + "';\r\n");
	Response.Write("var err_MB_No_More_Files_Uploaded = '" + UploadDownload_err_MB_No_More_Files_Uploaded + "';\r\n");
	Response.Write("var err_Select_Location = '" + UploadDownload_err_Select_Location + "';\r\n");
	Response.Write("var err_Opening_Project = '" + UploadDownload_err_Opening_Project + "';\r\n");
	Response.Write("var err_Upload_Failed = '" + UploadDownload_err_Upload_Failed + "';\r\n");
	Response.Write("var err_code_hiding_failed = '" + UploadDownload_err_code_hiding_failed + "';\r\n");
	Response.Write("var err_Addin_Not_Loaded = '" + UploadDownload_err_Addin_Not_Loaded + "';\r\n");
	Response.Write("var err_ProjType_Not_Supported = '" + UploadDownload_err_ProjType_Not_Supported + "';\r\n");
	Response.Write("var msg_File_Copy_Complete = '" + UploadDownload_msg_File_Copy_Complete + "';\r\n");
	Response.Write("var msg_Files_Were_Uploaded = '" + UploadDownload_msg_Files_Were_Uploaded + "';\r\n");
	Response.Write("var dir_Code_Stipping_TempDir = '" + UploadDownload_dir_Code_Stipping_TempDir + "';\r\n");
	Response.Write("var btn_Add_Text = '" + UploadDownload_btn_Add_Text + "';\r\n");
	Response.Write("var btn_UploadProject_Text = '" + UploadDownload_btn_UploadProject_Text + "';\r\n");
	Response.Write("var UploadDownload_RemoveCode_Text = '" + UploadDownload_RemoveCode_Text + "';\r\n");
	Response.Write("var UploadDownload_SelectProject = '" + UploadDownload_SelectProject + "';\r\n");
	Response.Write("var AddEditAssignment_WriteOverStarterProject = '" + AddEditAssignment_WriteOverStarterProject + "';\r\n");
	Response.Write("var UploadDownload_StatusBarUploadingText = '" + UploadDownload_StatusBarUploadingText + "';\r\n");
	Response.Write("var UploadDownload_CloseCurrentSol = '" + UploadDownload_CloseCurrentSol + "';\r\n");
	Response.Write("var UploadDownload_PromptToSave = '" + UploadDownload_PromptToSave + "';\r\n");
	Response.Write("var UploadDownload_DownloadPrompt = '" + UploadDownload_DownloadPrompt + "';\r\n");
	Response.Write("var UploadDownload_NoSolutionOrProject = '" + UploadDownload_NoSolutionOrProject + "';\r\n");
	Response.Write("var UploadDownload_DownloadRedirectUrl = '" + UploadDownload_DownloadRedirectUrl + "';\r\n");
	Response.Write("var UploadDownload_AlreadyStarterUploaded = '" + UploadDownload_AlreadyStarterUploaded + "';\r\n");

		%>
		
		function popFlag(){
			if(frm.txtCalendarFlag.value == ""){
				frm.txtCalendarFlag.value = "1";
			}
		}
	</script>
	<script language="JavaScript" src="../scripts/functions.js"></script>
	<script language="JavaScript" src="../scripts/Upload.js"></script>
	<script language="JavaScript" src="../scripts/Miscellaneous.js"></script>
	<script language="JavaScript" src="../scripts/popUp.js"></script>
	<script language="javascript">
		function window.onload(){
		try {
		g_Attrib = window.external.ActiveWindow.ContextAttributes.Add("keyword","vs.am.coursemanagement.courseassignments.addnew",3); // Type '3' is an F1 keyword
		} 
		catch (helpFailed) {}
		}
		
		function window.onunload(){
		  try {
      g_Attrib.Remove();
    } catch (helpFailed) {
    }
		}
	</script> <!--initialize div tags --><AM:NAV id="Nav1" runat="server" name="Nav1"></AM:NAV>
	<form id="frm" method="post" encType="multipart/form-data" runat="server">
		<div class="mainBody" id="mainBody">
			<div class="itemStyle" id="tabledialog">
				<table style="LEFT: 50px; POSITION: relative; TOP: 10px" height="80%" cellSpacing="0" cellPadding="0"
					width="80%" border="0">
					<tr>
						<td><asp:label class="infoTextTitle" id="lblAssignment" runat="Server" NAME="lblAssignment"></asp:label><AM:GOBACK id="GoBack1" runat="server" name="GoBack1"></AM:GOBACK></td>
					</tr>
					<tr vAlign="top">
						<td>
							<!--start of general-->
							<table id="tblGeneralAssignmentHeader" cellSpacing="0" cellPadding="2" width="100%" border="0"
								runat="server">
								<tr class="infoTextHeaderStyle">
									<td width="20">&nbsp;
									</td>
								</tr>
								<TR>
									<TD width="20" height="23"></TD>
									<TD height="23"><IMG id="Img3" src="../images/Required.gif" runat="server">
										<asp:label class="infoText" id="lblRequired" runat="server" Width="208px"></asp:label></TD>
								</TR>
								<tr>
									<td width="20">&nbsp;
									</td>
									<td><asp:label class="infoText" id="lblGeneralHeader" runat="Server" colspan="2">
											lblGeneralHeader
										</asp:label></td>
								</tr>
							</table>
							<!-- general div start -->
							<div id="divGeneralAssignment">
								<table id="tblGeneralAssignment" cellSpacing="0" cellPadding="2" width="100%" border="0"
									runat="server">
									<tr>
										<td width="20"></td>
										<td><asp:label class="infoText" id="lblGeneralAssignmentName" runat="Server">
												lblGeneralAssignmentName
											</asp:label></td>
									</tr>
									<tr>
										<td align="right" width="20"><IMG id="Img2" src="../images/Required.gif" runat="server"></IMG>
										</td>
										<TD><asp:textbox class="infoTextHeaderStyle" id="txtGeneralAssignmentName" style="CURSOR: text" runat="Server"
												width="100%">
												txtGeneralAssignmentName</asp:textbox></TD>
									</tr>
									<TR>
										<TD width="20"></TD>
										<TD><asp:label class="infoText" id="lblGeneralAssignmentDescription" runat="server" NAME="lblGeneralAssignmentDescription">
												lblGeneralAssignmentDescription
											</asp:label></TD>
									</TR>
									<TR>
										<TD width="20"></TD>
										<TD><asp:textbox class="infoTextHeaderStyle" id="txtGeneralAssignmentDescription" style="CURSOR: text"
												runat="server" NAME="txtGeneralAssignmentDescription" width="100%" textmode="MultiLine" Height="75px">
												txtGeneralAssignmentDescription</asp:textbox></TD>
									</TR>
									<TR>
										<TD width="20"></TD>
										<TD><asp:label class="infoText" id="lblGeneralAssignmentHomePageURL" runat="server" NAME="lblGeneralAssignmentHomePageURL">
												lblGeneralAssignmentHomePageURL
											</asp:label></TD>
									</TR>
									<TR>
										<TD width="20"></TD>
										<TD><asp:textbox class="infoTextHeaderStyle" id="txtGeneralAssignmentHomePageURL" style="CURSOR: text"
												runat="server" NAME="txtGeneralAssignmentHomePageURL" width="100%">
												txtGeneralAssignmentHomePageURL</asp:textbox></TD>
									</TR>
									<TR>
										<TD width="20">&nbsp;</TD>
										<TD></TD>
									</TR>
									<TR class="columnHead">
										<TD width="20"></TD>
										<TD></TD>
									</TR>
									<TR>
										<TD width="20"></TD>
										<TD>
											<P><asp:label class="infoText" id="lblGeneralAssignmentDueDate" runat="server" NAME="lblGeneralAssignmentDueDate">
												lblGeneralAssignmentDueDate
											</asp:label>&nbsp;&nbsp;&nbsp;
												<asp:textbox id="txtDueDate" name="txtDueDate" runat="server" Width="200px" Enabled="False" Visible="False" ReadOnly="True"></asp:textbox></P>
										</TD>
									</TR>
									<TR>
										<TD align="right" width="20"><IMG id="Img4" src="../images/Required.gif" runat="server"></IMG>
										</TD>
										<TD vAlign="middle">
											<asp:Calendar id="calDueDate" runat="server" Width="200px" Height="180px" BackColor="White" BorderColor="#635DAD"
												ForeColor="Black" Font-Names="Verdana" DayNameFormat="FirstLetter" Font-Size="8pt" CellPadding="4">
												<SelectorStyle BackColor="#CCCCCC"></SelectorStyle>
												<NextPrevStyle ForeColor="White" VerticalAlign="Bottom" BackColor="#635DAD"></NextPrevStyle>
												<DayHeaderStyle Font-Size="7pt" Font-Bold="True" ForeColor="White" BackColor="#635DAD"></DayHeaderStyle>
												<SelectedDayStyle Font-Bold="True" ForeColor="White" BackColor="#635DAD"></SelectedDayStyle>
												<TitleStyle Font-Bold="True" ForeColor="White" BorderColor="Transparent" BackColor="#635DAD"></TitleStyle>
												<WeekendDayStyle BackColor="#FFFFCC"></WeekendDayStyle>
												<OtherMonthDayStyle ForeColor="Gray"></OtherMonthDayStyle>
											</asp:Calendar><asp:label class="infoText" id="lblGeneralAssignmentDueTime" runat="server" NAME="lblGeneralAssignmentDueTime">
												lblGeneralAssignmentDueTime
											</asp:label><BR>
											<SELECT class="infoTextHeaderStyle" id="cboTime" name="cboTime" runat="server" width="100%">
											</SELECT></TD>
									</TR>
									<TR>
										<TD width="20"></TD>
										<TD><asp:checkbox class="infoText" id="chkGeneralAssignmentMultiSubmit" runat="server" NAME="chkGeneralAssignmentMultiSubmit"
												Text="chkGeneralAssignmentMultiSubmit"></asp:checkbox></TD>
									</TR>
									<TR>
										<TD width="20"></TD>
										<TD></TD>
									</TR>
									<TR>
										<TD width="20"></TD>
										<TD><asp:label class="infoText" id="lblGeneralAssignmentStarterProject" runat="server">
												lblGeneralAssignmentStarterProject
											</asp:label></TD>
									</TR>
									<TR>
										<TD width="20"></TD>
										<TD><SELECT class="infoTextHeaderStyle" id="cboGeneralAssignmentStarterProject" name="cboGeneralAssignmentStarterProject"
												width="100%"></SELECT>
											<asp:textbox class="invisible" id="txtStarterProject" runat="server" name="txtStarterProject"></asp:textbox></TD>
									</TR>
									<TR>
										<TD width="20"></TD>
										<TD><INPUT class="infoText" id="chkRemoveStudentCode" type="checkbox" CHECKED name="chkRemoveStudentCode">
											<SPAN class="infoText">
												<%Response.Write(UploadDownload_RemoveCode_Text);%>
											</SPAN></TD>
									</TR>
								</table>
							</div> <!--general div end --> <!--end of general--> <!--start auto-compile-->
							<TABLE id="tblAutoCompileHeader" cellSpacing="0" cellPadding="2" width="100%" border="0"
								runat="server">
								<TR class="infoTextHeaderStyle">
									<TD width="20">&nbsp;
									</TD>
								</TR>
								<TR class="columnHead">
									<TD width="20">&nbsp;
									</TD>
									<TD><asp:label class="infoText" id="lblAutoCompileHeader" runat="Server" colspan="2">
											lblAutoCompileHeader
										</asp:label></TD>
									<TD><asp:checkbox class="infoText" id="chkAutoCompileAssignmentAutompileOn" runat="server" NAME="chkAutoCompileAssignmentAutompileOn"
											Text="chkAutoCompileAssignmentAutompileOn"></asp:checkbox></TD>
								</TR>
							</TABLE>
							<DIV id="divAutoCompile">
								<TABLE id="tblAutoCompile" cellSpacing="0" cellPadding="2" width="100%" border="0" runat="server">
									<TR>
										<TD width="20"></TD>
										<TD></TD>
									</TR>
									<TR>
										<TD width="20"></TD>
										<TD></TD>
									</TR>
									<TR>
										<TD width="20"></TD>
										<TD><asp:label class="infoText" id="lblAutoCompileAssignmentCompileType" runat="server" NAME="lblAutoCompileAssignmentCompileType">
												lblAutoCompileAssignmentCompileType
											</asp:label></TD>
									</TR>
									<TR>
										<TD width="20"></TD>
										<TD><asp:dropdownlist class="infoTextHeaderStyle" id="cboAutoCompileAssignmentCompileType" runat="server"
												NAME="cboAutoCompileAssignmentCompileType" text="cboAutoCompileAssignmentCompileType"></asp:dropdownlist></TD>
									</TR>
								</TABLE>
							</DIV> <!--end of auto-compile--> <!--start auto-grade-->
							<TABLE id="tblAutoGradeHeader" cellSpacing="0" cellPadding="2" width="100%" border="0"
								runat="server">
								<TR>
									<TD width="20">&nbsp;
									</TD>
								</TR>
								<TR class="columnHead">
									<TD width="20">&nbsp;
									</TD>
									<TD><asp:label class="infoText" id="lblAutoGradeHeader" runat="Server" colspan="2">
											lblAutoGradeHeader
										</asp:label></TD>
									<TD><asp:checkbox class="infoText" id="chkAutoGradeAssignmentAutoGradeOn" runat="server" NAME="chkAutoGradeAssignmentAutoGradeOn"
											Text="chkAutoGradeAssignmentAutoGradeOn"></asp:checkbox></TD>
								</TR>
							</TABLE>
							<DIV id="divAutoGrade">
								<TABLE id="tblAutoGrade" cellSpacing="0" cellPadding="2" width="100%" border="0" runat="server">
									<TR>
										<TD width="20"></TD>
										<TD></TD>
									</TR>
									<TR>
										<TD width="20"></TD>
										<TD></TD>
									</TR>
									<TR>
										<TD width="20"></TD>
										<TD><asp:label class="infoText" id="lblCommandLineArgs" runat="server">
												lblCommandLineArgs
											</asp:label></TD>
									</TR>
									<TR>
										<TD width="20"></TD>
										<TD><asp:textbox class="infoTextHeaderStyle" id="txtCommandLineArgs" style="CURSOR: text" runat="server"
												NAME="txtCommandLineArgs" width="100%"></asp:textbox></TD>
									</TR>
									<TR>
										<TD width="20"></TD>
										<TD><asp:label class="infoText" id="lblAutoGradeAssignmentInputFile" runat="server" NAME="lblAutoGradeAssignmentInputFile">
												lblAutoGradeAssignmentInputFile
											</asp:label></TD>
									</TR>
									<TR>
										<TD width="20"></TD>
										<TD><INPUT class="infoTextHeaderStyle" id="inAutoGradeAssignmentInputFile" style="WIDTH: 100%"
												type="file" name="inAutoGradeAssignmentInputFile" runat="server">
										</TD>
									</TR>
									<TR>
										<TD width="20"></TD>
										<TD><asp:label class="infoText" id="lblAutoGradeAssignmentOutputFile" runat="server">
												lblAutoGradeAssignmentOutputFile
											</asp:label></TD>
									</TR>
									<TR>
										<TD width="20"></TD>
										<TD><INPUT class="infoTextHeaderStyle" id="inAutoGradeAssignmentOutputFile" style="WIDTH: 100%"
												type="file" name="inAutoGradeAssignmentOutputFile" runat="server">
										</TD>
									</TR>
									<TR>
										<TD width="20"></TD>
										<TD><asp:label class="infoText" id="lblAutoGradeAssignmentGradeType" runat="server" NAME="lblAutoGradeAssignmentGradeType">
												lblAutoGradeAssignmentGradeType
											</asp:label></TD>
									</TR>
									<TR>
										<TD width="20"></TD>
										<TD><asp:dropdownlist class="infoTextHeaderStyle" id="cboAutoGradeAssignmentGradeType" runat="server"
												NAME="cboAutoGradeAssignmentGradeType" text="cboAutoGradeAssignmentGradeType"></asp:dropdownlist></TD>
									</TR>
								</TABLE>
							</DIV> <!--end of auto-grade--> <!--start notifications-->
							<TABLE id="tblNotificationHeader" cellSpacing="0" cellPadding="2" width="100%" border="0"
								runat="server">
								<TR>
									<TD width="20">&nbsp;
									</TD>
								</TR>
								<TR class="columnHead">
									<TD width="20">&nbsp;
									</TD>
									<TD width="25%"><asp:label class="infoText" id="lblNotificationHeader" runat="Server" NAME="lblNotificationHeader"
											colspan="2"></asp:label></TD>
									<TD></TD>
								</TR>
							</TABLE>
							<DIV id="divNotification">
								<TABLE id="tblNotification" cellSpacing="0" cellPadding="2" width="100%" border="0" runat="server">
									<TR class="infoText">
										<TD width="20"></TD>
										<TD></TD>
										<TD></TD>
									</TR>
									<TR>
										<TD width="20"></TD>
										<TD vAlign="middle" width="20"><asp:checkbox class="infoText" id="chkSendReminder" runat="Server" name="chkSendReminder"></asp:checkbox></TD>
										<TD vAlign="middle"><asp:label class="infoText" id="lblReminderNotice" runat="Server" NAME="lblReminderNotice"></asp:label></TD>
									</TR>
									<TR>
										<TD width="20"></TD>
										<TD width="20">&nbsp;
										</TD>
										<TD vAlign="middle"><asp:textbox id="txtReminderDays" style="CURSOR: text" runat="server" NAME="txtReminderDays"
												width="25px"></asp:textbox>&nbsp;
											<asp:label class="infoText" id="lblReminderDay" runat="server"></asp:label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
										</TD>
										<TD><asp:comparevalidator class="errorText" id="validateReminderDays" runat="server" display="Dynamic" operator="DataTypeCheck"
												type="Integer" controltovalidate="txtReminderDays"></asp:comparevalidator></TD>
									</TR>
									<TR>
										<TD width="20"></TD>
										<TD vAlign="middle" width="20"><asp:checkbox class="infoText" id="chkPastDueNotice" runat="Server" name="chkPastDueNotice"></asp:checkbox></TD>
										<TD vAlign="middle"><asp:label class="infoText" id="lblPastDueNotice" runat="server"></asp:label></TD>
									</TR>
									<TR>
										<TD width="20"></TD>
										<TD width="20">&nbsp;
										</TD>
										<TD vAlign="middle"><asp:textbox id="txtPastDueDays" style="CURSOR: text" runat="server" width="25px"></asp:textbox>&nbsp;
											<asp:label class="infoText" id="lblPastDueDays" runat="server"></asp:label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
										</TD>
										<TD><asp:comparevalidator class="errorText" id="validPastDueDays" runat="server" display="Dynamic" operator="DataTypeCheck"
												type="Integer" controltovalidate="txtPastDueDays"></asp:comparevalidator></TD>
									</TR>
									<TR>
										<TD width="20"></TD>
										<TD vAlign="middle" width="20"><asp:checkbox class="infoText" id="chkUpdatedProjectNotice" runat="Server" name="chkUpdatedProjectNotice"></asp:checkbox></TD>
										<TD vAlign="middle"><asp:label class="infoText" id="lblUpdatedProject" runat="server" NAME="lblUpdatedProject"></asp:label></TD>
									</TR>
									<TR>
										<TD width="20"></TD>
										<TD width="20">&nbsp;
										</TD>
										<TD vAlign="middle">&nbsp;&nbsp;&nbsp;
										</TD>
									</TR>
								</TABLE>
							</DIV> <!--end of Notifications-->
							<TABLE id="tblBottomButtons" cellSpacing="0" cellPadding="2" width="100%" border="0" runat="server">
								<TR>
									<TD width="20">&nbsp;
									</TD>
									<TD></TD>
								</TR>
								<TR>
									<TD width="20"></TD>
									<TD align="right">
										<P>
											<INPUT id="btnDelete" visible="false" type="button" class="webbutton" value="Delete" name="btnDelete"
												runat="server" onclick="showDeleteAssignmentDialog()"></INPUT>&nbsp; <INPUT class="webButton" id="btnUpdate2" onclick="javascript: txtAction.value='submit'; return UploadProject(cboGeneralAssignmentStarterProject, false, true);"
												type="submit" value="Submit Query" name="btnUpdate2" runat="server" Text="btnUpdate2">
											</INPUT>&nbsp; <INPUT class=webButton id=btnCancel2 onclick="window.navigate('<%Response.Write(cancelUrl);%>');" type=button value="<%Response.Write(AddEditAssignment_btnCancel2);%>" name="<%Response.Write(AddEditAssignment_btnCancel2);%>"></INPUT>
											<br>
										</P>
										</INPUT></TD>
								</TR>
								<TR>
									<TD>&nbsp;
									</TD>
								</TR> <!--end of assignments table here --></TABLE>
						</td>
					</tr>
				</table>
			</div>
			<INPUT class="invisible" id="txtNewGUID" name="txtNewGUID" runat="server"> <INPUT class="invisible" id="txtUploadLocation" name="txtUploadLocation" runat="server">
			<INPUT class="invisible" id="txtFilesUploadedXML" name="txtFilesUploadedXML" runat="server">
			<INPUT class="invisible" id="txtSolutionName" name="txtSolutionName" runat="server">
			<INPUT class="invisible" id="txtMaxUploadSize" name="txtMaxUploadSize" runat="server">
			<INPUT class="invisible" id="txtDirSize" name="txtDirSize" runat="server"> <INPUT class="invisible" id="txtExistingStarterProject" name="txtExistingStarterProject"
				runat="server"> <INPUT class="invisible" id="txtAction" name="txtAction" runat="server">
	</form>
	</DIV>
	<SCRIPT language="javascript">
		InitializeAssignmentPage();		
	</SCRIPT>
</HTML>

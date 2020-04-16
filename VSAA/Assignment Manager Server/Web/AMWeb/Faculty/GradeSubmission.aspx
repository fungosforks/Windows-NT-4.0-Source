<%@ Page language="c#" Codebehind="GradeSubmission.cs" AutoEventWireup="false" Inherits="Microsoft.VisualStudio.Academic.AssignmentManager.Faculty.GradeSubmission" EnableSessionState="False" %>
<%@ Register TagPrefix="AM" TagName="Nav" Src="../UserControls/faculty.ascx" %>
<%@ Register TagPrefix="AM" TagName="GoBack" Src="../UserControls/goBack.ascx" %>
<HTML>
	<HEAD>
		<TITLE>
			<%=Title%>
		</TITLE>
		<script language="JavaScript" src="../common/functions.js"></script>
		<script language="javascript">
			
		</script>
		<AM:NAV id="Nav1" name="Nav1" runat="server">
		</AM:NAV>
		<!-- custom code here -->
		<form id="formGrade" name="formGrade" method="post" runat="server">
		<!-- user table / div start -->
	</HEAD>
	<BODY>
		<div class="mainBody" id="mainBody">
			<table style="LEFT: 50px; POSITION: relative; TOP: 20px" cellSpacing="0" cellPadding="0" width="100%" border="0">
				<tr>
					<td width="50%" nowrap>
						<asp:label class="infoText" id="lblStudentLabel" runat="server">
							lblStudentLabel
						</asp:label>
						&nbsp;
						<asp:label class="infoTextTitle" id="lblStudent" runat="server">
							lblStudent
						</asp:label>
					</td>
					<td width="50%" align="right">
						<AM:GoBack id="GoBack1" name="GoBack1" runat="server">
						</AM:GoBack>
					</td>
				</tr>
				<tr>
					<td>
						&nbsp;
					</td>
				</tr>
			</table>
			<!-- user table /div end -->
			<!-- grade assignment start -->
			<div class="itemStyle" id="divGradeInfo">
				<table style="LEFT: 50px; POSITION: relative; TOP: 20px" height="100%" cellSpacing="0" cellPadding="0" width="100%" border="0">
					<TBODY>
						<tr>
							<td width="30%">
								<asp:label class="infoText" Text="lblAssignmentLabel" id="lblAssignmentLabel" runat="server">
									lblAssignmentLabel
								</asp:label>
							</td>
						</tr>
						<tr>
							<td class="infoTextDisabled">
								<asp:label Text="lblAssignmentLabel" id="lblAssignment" runat="server">
									lblAssignment
								</asp:label>
							</td>
						</tr>
						<tr>
							<td>
								&nbsp;
							</td>
						</tr>
						<tr>
							<td>
								<asp:label class="infoText" id="lblDateSubmittedLabel" runat="server">
									lblDateSubmittedLabel
								</asp:label>
							</td>
						</tr>
						<tr>
							<td nowrap class="infoTextDisabled">
								<asp:label id="lblDateSubmitted" runat="server">
									lblDateSubmitted
								</asp:label>
							</td>
						</tr>
						<tr>
							<td>
								&nbsp;
							</td>
						</tr>
						<tr>
							<td>
								<asp:label class="infoText" id="lblGradeLabel" runat="server">
									lblGradeLabel
								</asp:label>
							</td>
						</tr>
						<tr>
							<td>
								<asp:TextBox class="infoTextHeaderStyle" style="CURSOR:text" Width="70" id="txtGrade" runat="server"></asp:TextBox>
							</td>
						</tr>
						<tr>
							<td>
								&nbsp;
							</td>
						</tr>
						<tr>
							<td vAlign="top">
								<asp:label class="infoText" id="lblCommentsLabel" runat="server">
									lblCommentsLabel
								</asp:label>
							</td>
						</tr>
						<tr>
							<td>
								<asp:TextBox CssClass="infoTextHeaderStyle" id="txtComments" runat="server" TextMode="MultiLine" Width="600" style="CURSOR:text" Height="75" NAME="txtComments" EnableViewState="False"></asp:TextBox>
							</td>
						</tr>
						<tr>
							<td>
								&nbsp;
							</td>
						</tr>
						<tr>
							<td align="right">
								<asp:Button id="btnAutoCompile" text="btnAutoCompile" runat="server" class="webButton" width="150"></asp:Button>
								<asp:Button id="btnAutoGrade" text="btnAutoGrade" runat="server" class="webButton" width="150"></asp:Button>
								<asp:Button id="btnSaveGrade" CssClass="webButton" runat="server" Text="Save Grade"></asp:Button>
								<asp:Button id="btnCancel" CssClass="webButton" runat="server" Text="Cancel"></asp:Button>
							</td>
						</tr>
						<tr>
							<td>
								&nbsp;
							</td>
						</tr>
						<tr>
							<td class="infoTextTableHeaderStyle" valign="center" height="25" colspan="2">
								&nbsp;
								<asp:label CssClass="infoText" id="lblCompileDetailsLabel" runat="server">
									lblCompileDetailsLabel
								</asp:label>
							</td>
						</tr>
						<tr>
							<td>
								<div id="divCompileResults" name="divCompileResults" runat="server">
									<table width="80%" cellspacing="0" cellpadding="0" border="0" style="LEFT:50px; POSITION:relative; TOP:10px">
										<tr>
											<td valign="top">
												<asp:label class="infoText" id="lblCompileDateLabel" runat="server">
													lblCompileDateLabel
												</asp:label>
											</td>
										</tr>
										<tr>
											<td>
												<asp:label class="infoTextDisabled" Width="550" id="lblCompileDate" runat="server">
													lblCompileDate
												</asp:label>
											</td>
										</tr>
										<tr>
											<td vAlign="top">
												<asp:label class="infoText" id="lblCompileResultLabel" runat="server">
													lblCompileResultLabel
												</asp:label>
											</td>
										</tr>
										<tr>
											<td>
												<asp:label class="infoTextDisabled" id="lblCompileResult" Width="550" runat="server">
													lblCompileResult
												</asp:label>
											</td>
										</tr>
										<tr>
											<td vAlign="top">
												<asp:label class="infoText" id="lblCompileDetailLabel" runat="server">
													lblCompileDetailLabel
												</asp:label>
											</td>
										</tr>
										<tr>
											<td>
												<asp:textbox class="infoTextDisabled" id="txtCompileDetails" Width="550" runat="server" TextMode="MultiLine"></asp:textbox>
											</td>
										</tr>
									</table>
								</div>
							</td>
						</tr>
						<tr>
							<td>
								&nbsp;
							</td>
						</tr>
						<tr>
							<td class="infoTextTableHeaderStyle" valign="center" height="25" colspan="2">
								&nbsp;
								<asp:label class="infoText" id="lblAutoGradeDetailsLabel" runat="server">
									lblAutoGradeDetailsLabel
								</asp:label>
							</td>
						</tr>
						<tr>
							<td>
								<div id="divAutoGradeResults" name="divAutoGradeResults" runat="server">
									<table width="400" cellspacing="0" cellpadding="0" border="0" style="LEFT:50px; POSITION:relative; TOP:10px">
										<tr>
											<td valign="top">
												<asp:label class="infoText" id="lblAutoGradeDateLabel" runat="server">
													lblAutoGradeDateLabel
												</asp:label>
											</td>
										</tr>
										<tr>
											<td>
												<asp:textbox class="infoTextDisabled" id="txtAutoGradeDate" Width="550" runat="server"></asp:textbox>
											</td>
										</tr>
										<tr>
											<td vAlign="top">
												<asp:label class="infoText" id="lblAutoGradeResultLabel" runat="server">
													lblAutoGradeResultLabel
												</asp:label>
											</td>
										</tr>
										<tr>
											<td>
												<asp:textbox class="infoTextDisabled" id="txtAutoGradeResult" Width="550" runat="server"></asp:textbox>
											</td>
										</tr>
										<tr>
											<td vAlign="top">
												<asp:label class="infoText" id="lblAutoGradeDetailLabel" runat="server">
													lblAutoGradeDetailLabel
												</asp:label>
											</td>
										<tr>
											<td>
												<asp:textbox class="infoTextDisabled" id="txtAutoGradeDetails" Width="550" runat="server" TextMode="MultiLine"></asp:textbox>
											</td>
										</tr>
									</table>
								</div>
							</td>
						</tr>
					</TBODY>
				</table>
			</div>
			</FORM>
		</div>
	</BODY>
</HTML>

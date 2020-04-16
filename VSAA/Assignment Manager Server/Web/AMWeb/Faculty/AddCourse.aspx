<%@ Page language="c#" Codebehind="AddCourse.cs" AutoEventWireup="false" Inherits="Microsoft.VisualStudio.Academic.AssignmentManager.Faculty.AddCourse" %>
<%@ Register TagPrefix="AM" TagName="Nav" Src="../UserControls/faculty.ascx" %>
<HTML>
	<HEAD>
		<TITLE>
			<%Response.Write(Title);%>
		</TITLE>
		<AM:NAV id="Nav1" name="Nav1" runat="server"></AM:NAV>
	</HEAD>
	<BODY>
		<div class="mainBody" id="mainBody">
			<form action="addcourse.aspx" method="post" runat="server">
				<table style="LEFT: 50px; POSITION: relative; TOP: 10px" cellSpacing="0" cellPadding="0" width="623" border="0">
					<TBODY>
						<tr>
							<td>&nbsp;
							</td>
						</tr>
						<tr>
							<td vAlign="top">
								<DIV class="infoText"><asp:label id="lblPrompt" runat="server" visible="false"></asp:label><br>
									<asp:hyperlink class="itemHotStyle" id="hlWorkWithCourse" runat="server" visible="false"></asp:hyperlink></DIV>
								<DIV class="infoText">&nbsp;</DIV>
								<DIV class="infoText"><asp:label id="lblInvalidLink" runat="server">lblInvalidLink</asp:label><br>
									<asp:hyperlink class="itemHotStyle" id="hlCopyCourse" runat="server" visible="false"></asp:hyperlink><input class="invisible" id="txtCourseOfferingID" name="txtCourseOfferingID" runat="server">
									<input class="invisible" id="txtCourseID" name="txtCourseID" runat="server"> <input class="invisible" id="txtCourseGUID" name="txtCourseGUID" runat="server">
								</DIV>
							</td>
						</tr>
					</TBODY></table>
			</form>
		</div>
	</BODY>
</HTML>

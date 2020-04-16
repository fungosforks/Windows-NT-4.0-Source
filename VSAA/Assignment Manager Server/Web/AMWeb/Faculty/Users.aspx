<%@ Page language="c#" Codebehind="Users.cs" AutoEventWireup="false" Inherits="Microsoft.VisualStudio.Academic.AssignmentManager.Faculty.Users" %>
<%@ Register TagPrefix="AM" TagName="Nav" Src="../UserControls/faculty.ascx" %>
<%@ Register TagPrefix="AM" TagName="GoBack" Src="../UserControls/goBack.ascx" %>
<html>
	<head>
		<title>
<%Response.Write(Title);%>
		</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<!DOCTYPE HTML public="-//W3C//DTD HTML 4.0 Transitional//EN">
		<script language="JavaScript" src="../scripts/functions.js"></script>
	</head>
	<script language="javascript">
		function window.onload(){
		try {
		g_Attrib = window.external.ActiveWindow.ContextAttributes.Add("keyword","vs.am.coursemanagement.courseusers",3); // Type '3' is an F1 keyword
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
	<div id="mainBody" class="mainBody">
		<form method="post" runat="server">
			<!-- custom code here -->
			
			<table width="80%" height="80%" cellspacing="0" cellpadding="0" border="0" style="LEFT:50px; POSITION:relative; TOP:10px">
				<TBODY>
					<tr>
						<td align="right">
							<AM:GoBack id="GoBack1" name="GoBack1" runat="server">
							</AM:GoBack>
						</td>
					</tr>
					<tr>
						<td>
							<asp:DataList id="dlUsers" runat="server" class="datalistStyle">
								<AlternatingItemStyle cssclass="rowStyleEven" />
								<HeaderStyle cssclass="infoTextTableHeaderStyle" />
								<ItemStyle cssclass="rowStyleOdd" />
								<HeaderTemplate>
									<table class="tableStyle" border="0" cellspacing="0" cellpadding="0">
										<tr class="infoTextTableHeaderStyle">
											<td nowrap width="150">
<%=Users_Text_String_Name%>
											</td>
											<td nowrap width="150">
<%=Users_Text_String_Email%>
											</td>
											<td nowrap width="150">
<%=Users_Text_String_UniversityID%>
											</td>
											<td nowrap width="150">
<%=Users_Text_String_UserName%>
											</td>
											<td nowrap width="150">
<%=Users_Text_String_LastUpdated%>
											</td>
										</tr>
									</table>
								</HeaderTemplate>
								<ItemTemplate>
									<table class="infoText" border="0" cellspacing="0" cellpadding="0">
										<tr>
											<td nowrap width="150" align="left">
												<a id="dlUsersName" class="itemHotStyle" href='AddEditUser.aspx?CourseID=<%Response.Write(Request.QueryString.Get("CourseID"));%>&UserID=<%#DataBinder.Eval(Container.DataItem, "UserID")%>'>
<%#DataBinder.Eval(Container.DataItem, "LastName")%>
													,&nbsp;<%#DataBinder.Eval(Container.DataItem, "FirstName")%>&nbsp;<%#DataBinder.Eval(Container.DataItem, "MiddleName")%></a>
											</td>
											<td nowrap width="150" align="left">
												<a id="dlUsersEmail" class="itemHotStyle" href='MailTo:<%#DataBinder.Eval(Container.DataItem, "Email")%>'>
<%#DataBinder.Eval(Container.DataItem, "Email")%>
												</a>
											</td>
											<td id="dlUsersUnivID" nowrap width="150" align="left">
<%#DataBinder.Eval(Container.DataItem, "UniversityIdentifier")%>
											</td>
											<td id="dlUsersUserName" nowrap width="150" align="left">
<%#DataBinder.Eval(Container.DataItem, "UserName")%>
											</td>
											<td id="dlUsersLastUpdatedName" nowrap width="150" align="left">
<%#DataBinder.Eval(Container.DataItem, "LastUpdatedDate", "{0:G}")%>
											</td>
										</tr>
									</table>
								</ItemTemplate>
								<FooterTemplate>
								</FooterTemplate>
							</asp:DataList>
							</P>
							<div class="infoText">
								<asp:HyperLink class="itemHotStyle" id="hlAddUser" runat="server" NAME="hlAddUser">
								</asp:HyperLink>
								<br />
								<asp:HyperLink class="itemHotStyle" id="hlImportUsers" runat="server" NAME="hlImportUsers">
								</asp:HyperLink>
							</div>
						</td>
					</tr>
				</TBODY>
			</table>
		</form>
	</div>
	</body>
</html>

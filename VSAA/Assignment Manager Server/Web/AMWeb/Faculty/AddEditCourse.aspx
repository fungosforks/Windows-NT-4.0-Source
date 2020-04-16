<%@ Register TagPrefix="AM" TagName="Nav" Src="../UserControls/faculty.ascx" %>
<%@ Page language="c#" Codebehind="AddEditCourse.cs" AutoEventWireup="false" Inherits="Microsoft.VisualStudio.Academic.AssignmentManager.Faculty.AddEditCourse" %>
<%@ Register TagPrefix="AM" TagName="GoBack" Src="../UserControls/goBack.ascx" %>
<!DOCTYPE HTML public="-//W3C//DTD HTML 4.0 Transitional//EN">
<HTML>
	<HEAD>
		<TITLE>
			<%Response.Write(Title);%>
		</TITLE>
	</HEAD>
	<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
	<meta name="CODE_LANGUAGE" Content="C#">
	<script language="JavaScript" src="../common/functions.js"></script>
	<script language="JavaScript" src="../scripts/popUp.js"></script>
	<script language="javascript">
				function showResourcesDiv(obj, div){
				if(obj.src.substring(obj.src.lastIndexOf("/")+1) == "pls.gif"){
					//show the div
					obj.src = obj.src.substring(0,obj.src.lastIndexOf("/")+1) + "mns.gif";
					div.className = "";
					var additionalOffset = 0;
					if(div.offsetHeight < 75) {additionalOffset = 75 - div.offsetHeight};

					courseManTable.style.height = mainBody.offsetHeight + div.offsetHeight + additionalOffset;
				}else{
					//close the div and replac mns with pls
					obj.src = obj.src.substring(0,obj.src.lastIndexOf("/")+1) + "pls.gif";
					div.className = "invisible";
					courseManTable.style.height = mainBody.offsetHeight - div.offsetHeight + 60;
				}
				
			}
	</script>
	<script language="javascript">
		function window.onload(){
		try {
		g_Attrib = window.external.ActiveWindow.ContextAttributes.Add("keyword","vs.am.coursemanagement.courseinfo",3); // Type '3' is an F1 keyword
		} 
		catch (helpFailed) {}
		}
	
		function window.onunload(){
		  try {
      g_Attrib.Remove();
    } catch (helpFailed) {
    }
		}
	</script>
	<AM:Nav id="Nav1" runat="server" name="Nav1"></AM:Nav>
	<form method="post" runat="server" id="frm" name="frm">
		<div id="mainBody" class="mainBody">
			<table style="LEFT: 50px; POSITION: relative; TOP: 10px" cellspacing="0" cellpadding="0"
				width="623" border="0">
				<tr valign="top">
					<td width="625">
						<!-- general div start -->
						<div id="divGeneralAssignment">
							<table id="tblGeneralAssignment" cellspacing="0" cellpadding="2" width="100%" border="0"
								runat="server">
								<tr>
									<td width="7">
									</td>
									<td width="453">
										<AM:GoBack id="GoBack1" name="GoBack1" runat="server"></AM:GoBack>
									</td>
									<td width="306">
									</td>
								</tr>
								<tr>
									<td width="7">
									</td>
									<td width="453">
										<asp:label class="infoText" id="lblShortName" runat="server"></asp:label>
									</td>
									<td width="306">
									</td>
								</tr>
								<tr>
									<td align="right" width="7">
										<img src="../images/Required.gif" runat="server" ID="Img3"></img>
									</td>
									<td width="453">
										<asp:textbox class="infoTextHeaderStyle" id="txtShortNameValue" style="CURSOR: text" runat="server"
											NAME="txtShortNameValue" Width="362" maxlength="100"></asp:textbox>
									</td>
									<TD width="306">
									</TD>
								</tr>
								<TR>
									<TD width="7">
										&nbsp;
									</TD>
									<TD width="453">
										<asp:Label class="infoText" id="lblHomePageURL" runat="server"></asp:Label>
									</TD>
									<TD width="306">
									</TD>
								</TR>
								<TR>
									<TD width="7">
									</TD>
									<TD width="453">
										<asp:textbox class="infoTextHeaderStyle" id="txtHomePageURL" style="CURSOR: text" runat="server"
											Width="362" maxlength="500"></asp:textbox>
									</TD>
									<TD width="306">
									</TD>
								</TR>
								<TR>
									<TD width="7">
										&nbsp;
									</TD>
									<TD width="453">
									</TD>
									<TD width="306">
									</TD>
								</TR>
								<TR>
									<TD width="7">
										&nbsp;
									</TD>
									<TD>
										<asp:Label class="infoText" id="lblStudentURL" runat="server" NAME="lblStudentURL"></asp:Label>
									</TD>
								</TR>
								<TR>
									<TD width="7">
										&nbsp;
									</TD>
									<TD>
										<asp:textbox class="infoTextHeaderStyle" id="txtStudentURL" runat="server" NAME="txtStudentURL"
											Width="362" maxlength="500" readonly="true" enabled="false"></asp:textbox>
									</TD>
								</TR>
								<TR>
									<TD width="7">
									</TD>
									<TD vAlign="top" width="453">
										<asp:label class="infoText" id="lblDescription" runat="server"></asp:label>
									</TD>
									<TD width="306">
									</TD>
								</TR>
								<TR>
									<TD width="7">
									</TD>
									<TD colSpan="2" width="453">
										<asp:textbox class="infoTextHeaderStyle" id="txtDescription" style="CURSOR: text" runat="server"
											Width="363" maxlength="4000" TextMode="MultiLine" Rows="4"></asp:textbox>
									</TD>
									<TD width="306">
									</TD>
								</TR>
							</table>
							<TABLE id="Table1" cellSpacing="0" cellPadding="2" width="60%" border="0" runat="server">
								<TBODY>
									<TR>
										<TD class="infoText" width="7">
										</TD>
										<TD vAlign="middle">
											<TABLE class="infoTextDisabled" id="Table2" cellSpacing="0" cellPadding="2" width="100%"
												runat="server">
												<TBODY>
													<TR>
														<TD>
															<DIV>
																<IMG id="resourcesGif" onclick="showResourcesDiv(this, divCourseResources);" src="../images/pls.gif"
																	border="0" style="CURSOR:hand;POSITION:relative;TOP:3px" onkeypress="if(event.keyCode==13)this.click();"
																	tabIndex="0">
																<SPAN>
																	<asp:label id="lblCourseResources" text="Course Resources" Runat="server"></asp:label>
																</SPAN><SPAN onclick="showResourceDialog()">
																	<asp:label class="itemHotStyle" id="lblAddResource" Runat="server"></asp:label>
																</SPAN>
															</DIV>
															<asp:TextBox class="invisible" id="txtResourceName" name="txtResourceName" runat="server"></asp:TextBox>
															<asp:TextBox class="invisible" id="txtResourceValue" name="txtResourceValue" runat="server"></asp:TextBox>
															<DIV class="invisible" id="divCourseResources" name="divCourseResources" runat="server">
																<asp:DataList id="dlCourseResources" runat="server" NAME="dlCourseResources">
																	<HeaderTemplate>
																		<table cellspacing="1" cellpadding="3" width="100%" border="0">
																	</HeaderTemplate>
																	<ItemTemplate>
																		<tr class="infoText">
																			<td width="20">
																				&nbsp;
																			</td>
																			<td nowrap>
																				<a ID="dlResourcesName" class="itemHotStyle" href='<%#DataBinder.Eval(Container.DataItem, "ResourceValue")%>'>
																					<%#DataBinder.Eval(Container.DataItem, "Name")%>
																				</a>
																			</td>
																			<td nowrap align="left">
																				<img ID="dlResourcesDelete" src="../images/delete_ICON.gif" style="CURSOR:hand" name="btnRemove" OnClick='txtCourseResourceID.value = <%#DataBinder.Eval(Container.DataItem, "CourseResourceID")%>;showDeleteResourceDialog()'></img>
																			</td>
																		</tr>
																	</ItemTemplate>
																	<FooterTemplate>
											</TABLE>
											</FOOTERTEMPLATE> </asp:DataList> <INPUT id="txtCourseResourceID" type="hidden" name="txtCourseResourceID" runat="server">
											<asp:TextBox id="txtDelete" NAME="txtDelete" Runat="server" CssClass="invisible"></asp:TextBox>
											<asp:TextBox id="txtAdd" NAME="txtAdd" Runat="server" CssClass="invisible"></asp:TextBox>
						</div>
					</td>
				</tr>
				</TD></TR>
			</table>
			</TD></TR></TBODY></TABLE>
		</div>
		</TD></TR></TBODY></TABLE>
		<P>
		</P>
		<TABLE width="428">
			<TR>
				<TD align="right">
					<asp:Button class="webButton" id="btnUpdate" runat="server" NAME="btnUpdate" text="Update"></asp:Button>
				</TD>
			</TR>
			<TR>
				<TD width="220" align="right">
					<IMG id="Img2" src="../images/Required.gif" runat="server"></IMG>
					<asp:label class="infoText" id="lblRequired" runat="server"></asp:label>
				</TD>
			</TR>
		</TABLE>
		</DIV>
		<DIV>
		</DIV>
	</form>
	</TABLE>
</HTML>

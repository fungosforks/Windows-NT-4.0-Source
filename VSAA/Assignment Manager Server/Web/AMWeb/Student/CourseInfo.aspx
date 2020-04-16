<%@ Register TagPrefix="AM" TagName="Nav" Src="../UserControls/student.ascx" %>
<%@ Page language="c#" Codebehind="CourseInfo.aspx.cs" AutoEventWireup="false" Inherits="Microsoft.VisualStudio.Academic.AssignmentManager.Student.CourseInfo" %>
<%@ Register TagPrefix="AM" TagName="GoBack" Src="../UserControls/goBack.ascx" %>
<!DOCTYPE HTML public="-//W3C//DTD HTML 4.0 Transitional//EN">
<HTML>
	<HEAD>
		<TITLE>
			<%Response.Write(Title);%>
		</TITLE>
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
					courseManTable.style.height = mainBody.offsetHeight + 100 - div.offsetHeight;
				}				
			}
		</script>
		<AM:Nav id="Nav1" runat="server" name="Nav1">
		</AM:Nav>
		<form method="post" runat="server" id="frm" name="frm">
	</HEAD>
	<BODY>
		<div id="mainBody" class="mainBody">
			<table style="LEFT: 50px; POSITION: relative; TOP: 10px" cellspacing="0" cellpadding="0" width="80%" border="0">
				<tr>
					<td style="WIDTH: 453px">
						<AM:GoBack id="GoBack1" name="GoBack1" runat="server">
						</AM:GoBack>
					</td>
				</tr>
				<tr>
					<td>
						<asp:label class="infoText" id="lblShortName" runat="server"></asp:label>
					</td>
				</tr>
				<tr>
					<td class="infoTextDisabled">
						<asp:Label id="lblShortNameValue" runat="server" NAME="lblShortNameValue"></asp:Label>
					</td>
				</tr>
				<tr>
					<td>
						<asp:Label id="lblHomePageURL" class="infoText" runat="server"></asp:Label>
					</td>
				</tr>
				<tr>
					<td class="infoTextDisabled">
						<asp:HyperLink ID="linkHomePageURLText" Runat="server" class="itemHotStyle"></asp:HyperLink>
					</td>
				</tr>
				<tr>
					<td valign="top">
						<asp:label class="infoText" id="lblDescription" runat="server"></asp:label>
					</td>
				</tr>
				<tr>
					<td>
						<asp:TextBox id="txtDescriptionText" width="100%" ReadOnly="true" Class="infoTextDisabled" Height="75" runat="server" TextMode="MultiLine"></asp:TextBox>
					</td>
				</tr>
				<tr>
					<td>
						&nbsp;
					</td>
				</tr>
			</table>
			<table cellspacing="0" cellpadding="0" width="100%" border="0">
				<TBODY>
					<tr>
						<td valign="center">
							<table style="LEFT: 50px; POSITION: relative; TOP: 10px" cellspacing="0" class="infoTextDisabled" width="80%" runat="server" ID="Table2">
								<TBODY>
									<tr>
										<td>
											<div>
												<img src="../images/pls.gif" id="resourcesGif" onclick="showResourcesDiv(this, divCourseResources);" border="0" style="CURSOR:hand;POSITION:relative;TOP:3px" onkeypress="if(event.keyCode==13)this.click();" tabIndex="0">
												<span>
													<asp:label id="lblCourseResources" Runat="server" text="Course Resources">
														Course Resources
													</asp:label>
												</span><span></span>
											</div>
											<div id="divCourseResources" name="divCourseResources" class="invisible" runat="server">
												<asp:DataList id="dlCourseResources" runat="server" NAME="dlCourseResources">
													<HeaderTemplate>
														<table cellspacing="1" cellpadding="3" width="100%" border="0">
													</HeaderTemplate>
													<ItemTemplate>
														<TBODY>
															<tr class="infoText">
																<td width="20">
																	&nbsp;
																</td>
																<td nowrap>
																	<a id="dlCourseResourcesName" class="itemHotStyle" href='<%#DataBinder.Eval(Container.DataItem, "ResourceValue")%>'>
																		<%#DataBinder.Eval(Container.DataItem, "Name")%>
																	</a>
																</td>
																<td nowrap align="left">
																</td>
															</tr>
													</ItemTemplate>
													<FooterTemplate>
								</TBODY>
							</table>
			</footertemplate> </asp:DataList>
		</div>
		</TD></TR>
		<DIV>
		</DIV>
		</TD></TR></TBODY></TABLE></TD></TR></TBODY></TABLE></FORM></TBODY></DIV>
	</BODY>
</HTML>

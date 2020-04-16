<%@ Page language="c#" Codebehind="AssignmentGrade.aspx.cs" AutoEventWireup="false" Inherits="Microsoft.VisualStudio.Academic.AssignmentManager.Student.AssignmentGrade" %>
<%@ Register TagPrefix="AM" TagName="Nav" Src="../UserControls/student.ascx" %>
<%@ Register TagPrefix="AM" TagName="GoBack" Src="../UserControls/goBack.ascx" %>
<HTML>
	<HEAD>
		<TITLE>
			<%Response.Write(Title);%>
		</TITLE>
		<script language="JavaScript" src="../common/functions.js"></script>
		<script language="javascript">
			function showCompileDiv(obj, div){
				if(obj.src.substring(obj.src.lastIndexOf("/")+1) == "pls.gif"){
					//show the div
					obj.src = obj.src.substring(0,obj.src.lastIndexOf("/")+1) + "mns.gif";
					div.className = "";
					courseManTable.style.height = mainBody.offsetHeight + div.offsetHeight;
				}else{
					//close the div and replac mns with pls
					obj.src = obj.src.substring(0,obj.src.lastIndexOf("/")+1) + "pls.gif";
					div.className = "invisible";
					courseManTable.style.height = mainBody.offsetHeight + 100 - div.offsetHeight;
				}
				
			}
			function showAutoGradeDiv(obj, div){
				if(obj.src.substring(obj.src.lastIndexOf("/")+1) == "pls.gif"){
					//show the div
					obj.src = obj.src.substring(0,obj.src.lastIndexOf("/")+1) + "mns.gif";
					div.className = "";
					courseManTable.style.height = mainBody.offsetHeight + div.offsetHeight;
				}else{
					//close the div and replac mns with pls
					obj.src = obj.src.substring(0,obj.src.lastIndexOf("/")+1) + "pls.gif";
					div.className = "invisible";
					courseManTable.style.height = mainBody.offsetHeight + 100 - div.offsetHeight;
				}
				
			}
			function showDescription(obj,txt){
				if(obj.src.substring(obj.src.lastIndexOf("/")+1) == "pls.gif"){
					//show the div
					obj.src = obj.src.substring(0,obj.src.lastIndexOf("/")+1) + "mns.gif";
					txt.className = "infoTextDisabled";
					courseManTable.style.height = mainBody.offsetHeight + 75;
				}else{
					//close the div and replac mns with pls
					obj.src = obj.src.substring(0,obj.src.lastIndexOf("/")+1) + "pls.gif";
					txt.className = "invisible";
					courseManTable.style.height = mainBody.offsetHeight + 100;
				}
			}
		</script>
	</HEAD>
	<body>
		<AM:Nav id="Nav1" runat="server" name="Nav1">
		</AM:Nav>
		<div id="mainBody" class="mainBody">
			<!-- custom code here -->
			<form runat="server">
				<table width="100%" cellSpacing="0" cellPadding="0" border="0" style="LEFT:50px; POSITION:relative; TOP:20px">
					<tr>
						<td>
							<asp:label class="infoTextTitle" id="lblAssignmentLabel" runat="server"></asp:label>
							&nbsp;&nbsp;
							<asp:label class="infoText" id="lblAssignment" runat="server"></asp:label>
						</td>
						<td align="right">
							<AM:GoBack id="GoBack1" name="GoBack1" runat="server">
							</AM:GoBack>
						</td>
					</tr>
					<TR>
						<TD>&nbsp;</TD>
					</TR>
					<TR>
						<TD>
							<asp:Label class="infoText" id="lblDueDate" runat="server"></asp:Label>
							<asp:Label class="infoText" id="lblDueDateValue" runat="server"></asp:Label>
						</TD>
					</TR>
					<TR>
						<TD>
							<asp:Label class="infoText" id="lblAssignmentWebPage" runat="server"></asp:Label>
							<asp:HyperLink class="infoText" style="cursor:hand;text-decoration: underline;color:#666699;" id="hlAssignmentWebPage" runat="server"></asp:HyperLink>
						</TD>
					</TR>
					<TR>
						<TD><IMG id=Img1 style="position:relative;top:3px;cursor:hand;"  onkeypress="if(event.keyCode==13)this.click();" tabIndex=0 onclick="showDescription(this, txtDescription);" 
       <%if(Request.QueryString.Get("Exp") == "1"){Response.Write("src='../images/mns.gif'");}else{Response.Write("src='../images/pls.gif'");}%> src="../images/mns.gif" border=0>
							<asp:Label class="infoText" id="lblDescription" runat="server"></asp:Label>
						</TD>
					</TR>
					<TR>
						<TD>
							<asp:TextBox id="txtDescription" runat="server" Width="100%" Height="75" TextMode="MultiLine" Enabled="true" ReadOnly="True"></asp:TextBox>
						</TD>
					</TR>
					<TR>
						<TD></TD>
					</TR>
					<tr>
						<td>&nbsp;</td>
					</tr>
					<tr>
						<td>
							<asp:label Width="600" class="infoText" id="lblDateSubmittedLabel" runat="server"></asp:label>
						</td>
					</tr>
					<tr>
						<td nowrap>
							<asp:label Width="100%" class="infoTextDisabled" id="lblDateSubmitted" runat="server"></asp:label>
						</td>
					</tr>
					<tr>
						<td>
							<asp:label Width="100%" class="infoText" id="lblGradeLabel" runat="server"></asp:label>
						</td>
					</tr>
					<tr>
						<td>
							<asp:label Width="100%" class="infoTextDisabled" id="lblGrade" runat="server"></asp:label>
						</td>
					</tr>
					<tr>
						<td vAlign="top">
							<asp:label Width="100%" class="infoText" id="lblCommentsLabel" runat="server"></asp:label>
						</td>
					</tr>
					<tr>
						<td>
							<asp:TextBox CssClass="infoTextDisabled" id="lblComments" Runat="server" Enabled="True" ReadOnly="true" TextMode="MultiLine" Width="100%" Height="75"></asp:TextBox>
						</td>
					</tr>
					<tr>
						<td>
							&nbsp;
						</td>
					</tr>
					<tr class="infoText">
						<td class="infoTextTableHeaderStyle" valign="center" height="25" colspan="2">
							<img src="../images/pls.gif" id="resourcesGif" onclick="showCompileDiv(this, divCompileResults);" border="0" style="position:relative;top:3px;cursor:hand;" onkeypress="if(event.keyCode==13)this.click();" tabIndex="0">
							&nbsp;
							<asp:label id="lblCompileDetailsLabel" runat="server"></asp:label>
						</td>
					</tr>
					<tr>
						<td>
							<div id="divCompileResults" name="divCompileResults" class="invisible" runat="server">
								<table width="100%" cellspacing="0" cellpadding="0" border="0" style="LEFT:50px; POSITION:relative; TOP:10px">
									<tr>
										<td valign="top" colspan="2">
											<asp:label class="infoText" id="lblCompileDateLabel" runat="server"></asp:label>
										</td>
									</tr>
									<tr>
										<td nowrap align="left" colspan="2">
											<asp:label class="infoTextDisabled" id="lblCompileDate" runat="server"></asp:label>
										</td>
									</tr>
									<tr>
										<td vAlign="top" colspan="2">
											<asp:label class="infoText" id="lblCompileResultLabel" runat="server"></asp:label>
										</td>
									</tr>
									<tr>
										<td colspan="2">
											<asp:Label class="infoTextDisabled" id="lblCompileResult" runat="server"></asp:Label>
										</td>
									</tr>
									<tr>
										<td vAlign="top" colspan="2">
											<asp:label class="infoText" id="lblCompileDetailLabel" runat="server"></asp:label>
										</td>
									</tr>
									<tr>
										<td colspan="2">
											<asp:TextBox Height="90px" Width="100%" class="infoTextDisabled" id="lblCompileDetails" Enabled="True" ReadOnly="True" TextMode="MultiLine" runat="server"></asp:TextBox>
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
					<tr class="infoText">
						<td vAlign="center" height="25" class="infoTextTableHeaderStyle" colspan="2">
							<img src="../images/pls.gif" id="AutoGradeGif" onclick="showAutoGradeDiv(this, divAutoGradeResults);" border="0" style="position:relative;top:3px;cursor:hand;" onkeypress="if(event.keyCode==13)this.click();" tabIndex="0">
							&nbsp;
							<asp:label id="lblAutoGradeDetailsLabel" runat="server"></asp:label>
						</td>
					</tr>
					<tr>
						<td>
							<div id="divAutoGradeResults" name="divAutoGradeResults" class="invisible" runat="server">
								<table width="100%" cellspacing="0" cellpadding="0" border="0" style="LEFT:50px; POSITION:relative; TOP:10px">
									<tr>
										<td valign="top">
											<asp:label class="infoText" id="lblAutoGradeDateLabel" runat="server"></asp:label>
										</td>
									</tr>
									<tr>
										<td nowrap>
											<asp:label class="infoTextDisabled" id="lblAutoGradeDate" runat="server"></asp:label>
										</td>
									</tr>
									<tr>
										<td vAlign="top">
											<asp:label class="infoText" id="lblAutoGradeResultLabel" runat="server"></asp:label>
										</td>
									</tr>
									<tr>
										<td>
											<asp:label class="infoTextDisabled" id="lblAutoGradeResult" runat="server"></asp:label>
										</td>
									</tr>
									<tr>
										<td vAlign="top">
											<asp:label class="infoText" id="lblAutoGradeDetailLabel" runat="server"></asp:label>
										</td>
									</tr>
									<tr>
										<td>
											<asp:TextBox Height="90px" Width="100%" class="infoTextDisabled" id="lblAutoGradeDetails" runat="server" Enabled="True" ReadOnly="True" TextMode="MultiLine"></asp:TextBox>
										</td>
									</tr>
								</table>
							</div>
						</td>
					</tr>
				</table>
			</form>
		</div>
	</body>
</HTML>

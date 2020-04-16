<%@ Register TagPrefix="AM" TagName="Nav" Src="../UserControls/faculty.ascx" %>
<%@ Register TagPrefix="AM" TagName="GoBack" Src="../UserControls/goBack.ascx" %>
<%@ Page language="c#" Codebehind="ImportFormPreview.aspx.cs" AutoEventWireup="false" Inherits="Microsoft.VisualStudio.Academic.AssignmentManager.Faculty.ImportFormPreview" %>
<HTML>
	<HEAD>
		<TITLE>
			<% Response.Write(Title);%>
		</TITLE>
	</HEAD>
	<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
	<meta content="C#" name="CODE_LANGUAGE">
	<script language="javascript">

function hideDisplayTable() {
	if (document.Form1.ImageButton1.alt == "expand")
	{ 
		document.Form1.ImageButton1.src = "images/mns.gif";
		document.Form1.all["tblFixedWidthEntry"].style.display = "";
		document.Form1.ImageButton1.alt = "contract";
	}
	else
	{
		document.Form1.ImageButton1.src = "images/pls.gif";
		document.Form1.all["tblFixedWidthEntry"].style.display = "none";
		document.Form1.ImageButton1.alt = "expand";
	}
}
	</script> <!-- nav user control here -->
	<AM:Nav id="Nav1" runat="server" name="Nav1"></AM:Nav>
	<div id="mainBody" class="mainBody">
		<form id="Form1" encType="multipart/form-data" runat="server">
			<div class="itemStyle" id="tabledialog">
				<table style="LEFT: 50px; POSITION: relative; TOP: 10px" cellSpacing="0" cellPadding="0"
					width="623" border="0">
					<tr>
						<td colspan="4">
							<asp:Label id="lblTitle" name="lblTitle" class="infoTextTitle" runat="server"></asp:Label>
							<AM:GoBack id="GoBack1" name="GoBack1" runat="server"></AM:GoBack>
						</td>
					</tr>
					<tr>
						<td colspan="4">
							<asp:Label id="lblDescription" name="lblDescription" class="infoText" runat="server" width="475px"></asp:Label>
						</td>
					</tr>
					<tr>
						<td colspan="4">
							&nbsp;
						</td>
					</tr>
					<tr>
						<td width="7"></td>
						<td width="230">
							<asp:label id="lblLastName" runat="server" class="infoText" name="lblLastName"></asp:label>
						</td>
						<td width="7"></td>
						<td width="230">
							<asp:label id="lblFirstName" runat="server" class="infoText" name="lblFirstName"></asp:label>
						</td>
					</tr>
					<tr>
						<td width="7">
							<img src="../images/Required.gif" id="Img1">&nbsp;</img>
						</td>
						<td width="230">
							<asp:DropDownList class="infoTextHeaderStyle" id="cboLastName" runat="server" name="cboLastName"></asp:DropDownList>
						</td>
						<td width="7">
							<img src="../images/Required.gif" id="Img1">&nbsp;</img>
						</td>
						<td width="230">
							<asp:DropDownList id="cboFirstName" runat="server" class="infoTextHeaderStyle" name="cboFirstName"></asp:DropDownList>
						</td>
					</tr>
					<tr>
						<td width="7"></td>
						<td width="230">
							<asp:label id="lblMiddleName" runat="server" class="infoText" name="lblMiddleName"></asp:label>
						</td>
						<td width="7"></td>
						<td width="230">
							<asp:label id="lblEmailAddress" runat="server" class="infoText" name="lblEmailAddress"></asp:label>
						</td>
					</tr>
					<tr>
						<td width="7"></td>
						<td width="230">
							<asp:DropDownList id="cboMiddleName" runat="server" class="infoTextHeaderStyle" name="cboMiddleName"></asp:DropDownList>
						</td>
						<td align="left" width="7">
							<img src="../images/Required.gif" id="Img1">&nbsp;</img>
						</td>
						<td width="230">
							<asp:DropDownList id="cboEmailAddress" runat="server" class="infoTextHeaderStyle" name="cboEmailAddress"></asp:DropDownList>
						</td>
					</tr>
					<tr>
						<td width="7"></td>
						<td width="230">
							<asp:label id="lblUniversityID" runat="server" class="infoText" name="lblUniversityID"></asp:label>
						</td>
						<td width="7"></td>
						<td width="230">
							<asp:label id="lblUserName" runat="server" class="infoText" name="lblUserName"></asp:label>
						</td>
					</tr>
					<tr>
						<td align="left" width="7">
							<img src="../images/Required.gif" id="Img1">&nbsp;</img>
						</td>
						<td width="230">
							<asp:DropDownList id="cboUniversityID" runat="server" class="infoTextHeaderStyle" name="cboUniversityID"></asp:DropDownList>
						</td>
						<td align="left" width="7">
							<img src="../images/Required.gif" id="Img1">&nbsp;</img>
						</td>
						<td width="230">
							<asp:DropDownList id="cboUserName" runat="server" class="infoTextHeaderStyle" name="cboUserName"></asp:DropDownList>
						</td>
					</tr>
					<tr>
						<td colspan="4">
							&nbsp;
						</td>
					</tr>
					<tr>
						<td colspan="4">
							<asp:Button id="btnImportRecords" runat="server" class="webButton" width="70px"></asp:Button>
							<asp:Button id="btnCancel" runat="server" class="webButton" NAME="btnCancel" width="70px"></asp:Button>
						</td>
					</tr>
					<tr>
						<td colspan="4">
							<img src="../images/Required.gif" id="Img4"></img>
							<span id="lblRequired" class="infoText">
								- Required field indicator</span>
						</td>
					</tr>
				</table>
			</div>
			<asp:TextBox id="txtImportedFileLocation" runat="server" class="infoTextHeaderStyle" style="DISPLAY:none; VISIBILITY:hidden"
				name="txtImportedFileLocation" width="475px"></asp:TextBox>
		</form>
	</div>
</HTML>

<%@ Register TagPrefix="AM" TagName="GoBack" Src="../UserControls/goBack.ascx" %>
<%@ Register TagPrefix="AM" TagName="Nav" Src="../UserControls/faculty.ascx" %>
<%@ Page language="c#" Codebehind="ImportForm.cs" AutoEventWireup="false" Inherits="Microsoft.VisualStudio.Academic.AssignmentManager.Faculty.ImportForm" %>

<HTML>
  <HEAD>
		<TITLE> <% Response.Write(Title);%>
		</TITLE>
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
  </HEAD>
	<body>
	<AM:Nav id="Nav1" runat="server" name="Nav1">
	</AM:Nav>
	<form id="Form1" encType="multipart/form-data" runat="server" method="post">
		<div id="mainBody" class="mainBody">
			<div class="itemStyle" id="tabledialog">
				<table style="LEFT: 50px; POSITION: relative; TOP: 10px" cellSpacing="0" cellPadding="0" width="100%" border="0">
					<tr>
						<td align="right" colspan="2">
							<AM:GoBack id="GoBack1" name="GoBack1" runat="server">
							</AM:GoBack>
						</td>
					</tr>
					<tr>
						<td width="7"></td>
						<td colspan="2">
							<asp:label class="infoText" id="lblSelectFile" runat="server">
							</asp:label>
						</td>
					</tr>
					<tr>
						<td align="left" width="7">
							<img src="../images/Required.gif" id="Img1" >&nbsp;</img>
						</td>
						<td colspan="2">
							<input id="txtUploadFile" type="file" style="WIDTH:90%" runat="server" class="infoTextHeaderStyle">
						</td>
					</tr>
					<tr>
						<td width="7"></td>
						<td colspan="2">
							<asp:label class="infoText" id="lblTypeOfFile" runat="server">
							</asp:label>
						</td>
					</tr>
					<tr>
						<td width="7"></td>
						<td colspan="2">
							<asp:label class="infoText" id="lblDelimitedCharacter" runat="server">
							</asp:label>
						</td>
					</tr>
					<tr>
						<td align="left" width="7">
							<img src="../images/Required.gif" id="Img1" >&nbsp;</img>
						</td>
						<td width="150">
							<asp:DropDownList id="cboDelimitingCharacter" name="cboDelimitingCharacter" runat="server" width="70">
							</asp:DropDownList>
						</td>
						<td>
							<asp:Button id="btnPreview" runat="server" class="webButton" name="btnPreview" style="LEFT:100px;POSITION:relative" Width="70">
							</asp:Button>
							<asp:Button id="btnCancel" runat="server" class="webButton" NAME="btnCancel" style="LEFT:100px;POSITION:relative" Width="70" CommandName="Cancel">
							</asp:Button>
						</td>
					</tr>
					<tr>
						<td colspan="3">
							<span id="lblRequired" class="infoText">&nbsp;</span>
						</td>
					</tr>
					<tr>
						<td colspan="3">
							<img src="../images/Required.gif" id="Img4" ></img> <span id="lblRequired" class="infoText">
								- Required field indicator</span>
						</td>
					</tr>
				</table>
			</div>
	</form></DIV> </body>
</HTML>

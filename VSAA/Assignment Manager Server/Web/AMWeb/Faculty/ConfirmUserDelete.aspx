<%@ Page language="c#" Codebehind="ConfirmUserDelete.aspx.cs" AutoEventWireup="false" Inherits="Microsoft.VisualStudio.Academic.AssignmentManager.Faculty.ConfirmUserDelete" %>
<%@ Register TagPrefix="AM" TagName="Nav" Src="../UserControls/faculty.ascx" %>
<HTML>
	<HEAD>
		<title>
			<%Response.Write(Title);%>
		</title>
		<LINK REL="StyleSheet" HREF="../scripts/assnMan.css" type="text/css">
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form method="post" runat="server" ID="Form1">
			<table align=center>
				<br>
				<br>
				<tr>
					<td align=center>
						<asp:Label id=lblConfirmDelete CssClass="infoText" runat="server">
						</asp:Label>
					</td>
				</tr>
			</table>
			<br>
			<table align=center>
				<tr>
					<td align=center>
						<button onclick="OnOk()" class="webButton" id="OkButton" style="position:relative; top:0;left:-8; width: 5em;">
							<%Response.Write(Yes);%>
						</button>
						<button onClick="OnCancel()" class="webButton" id="CancelButton" style="position:relative; top:0;left:0; width: 5em;">
							<%Response.Write(No);%>
						</button>
					</td>
				</tr>
			</table>
		</form>
		<script language="JavaScript">
		
	// This items passed to this dialog are either:
	// - NOTHING (just adding)
	// - Two-element Array (edit)
	//  - Old description
	//  - Old link
	

	function OnOk() {
		
		   window.returnValue = new Array("1");			  
		  window.close();
	}	 
	
	function OnCancel()
	{
		 window.returnValue = new Array("0");			  
		  window.close();
	}
	
	// Whenever the user hits enter, we want to trigger the OK button on the form;
	// when they hit esc, we want to trigger the cancel button.
	function HandleKeyDown() {
	  var i = window.event.keyCode;

	  switch (i) {
	  case 13: // ENTER
		OkButton.click();
		break;
	  case 27: // ESC
		CancelButton.click();
		break;
	  default:
		break;
	  }
	}
		</script> </form>
	</body>
</HTML>

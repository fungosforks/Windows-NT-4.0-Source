<%@ Page language="c#" Codebehind="AddResource.aspx.cs" AutoEventWireup="false" Inherits="Microsoft.VisualStudio.Academic.AssignmentManager.Faculty.AddResource" %>
<HTML>
	<HEAD>
		<title>
			<%Response.Write(AddResource_Title);%>
		</title>
		<LINK REL="StyleSheet" HREF="../scripts/assnMan.css" type="text/css">
	</HEAD>
	<body leftmargin="0" topmargin="0" ms_positioning="GridLayout">
		<TABLE height="199" cellSpacing="0" cellPadding="0" width="360" border="0" ms_2d_layout="TRUE">
			<TR vAlign="top">
				<TD width="24" height="16">
					<script language="JavaScript">
	var _AddResourceErrorNotEnoughFields_Text = "<%Response.Write(AddResource_NotEnoughFields);%>";

	function window.onload(){
		try {
		g_Attrib = window.external.ActiveWindow.ContextAttributes.Add("keyword","vs.AMAddResources",3); // Type '3' is an F1 keyword
		} 
		catch (helpFailed) {}
		}
	
		function window.onunload(){
		  try {
      g_Attrib.Remove();
    } catch (helpFailed) {
    }
		}


	function OnOk() {
	
	  if ((resourceName.value == "") || (resourceLink.value == "")) {
		window.returnValue = null;
		alert(_AddResourceErrorNotEnoughFields_Text);
		return;
	  }
	  
	  var link = "";
		link = Select1.value;
		link = (link + resourceLink.value);
		window.returnValue = new Array(resourceName.value, link, "1");
		window.close();
	}

	function OnCancel() {
	  window.returnValue = null;
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
					</script>
				</TD>
				<TD width="32"></TD>
				<TD width="136"></TD>
				<TD width="96"></TD>
				<TD width="72"></TD>
			</TR>
			<TR vAlign="top">
				<TD height="24"></TD>
				<TD colSpan="2">
					<asp:Label id="lblAddResource" runat="server" class="infoText" Width="144px" Height="16px">lblAddResource</asp:Label></TD>
				<TD colSpan="2"></TD>
			</TR>
			<TR vAlign="top">
				<TD height="16"></TD>
				<TD colSpan="4">
					<asp:Label id="lblResourceName" runat="server" class="infoText" Width="320px" Height="8px">lblResourceName</asp:Label></TD>
			</TR>
			<TR vAlign="top">
				<TD height="16"></TD>
				<TD colSpan="4">
					<asp:Label id="lblNameExample" runat="server" class="infoText" Width="328px" Height="16px">lblNameExample</asp:Label></TD>
			</TR>
			<TR vAlign="top">
				<TD height="40"></TD>
				<TD colSpan="4"><INPUT id="resourceName" onkeydown="HandleKeyDown()" type="text" maxLength="50" size="50"
						name="resourceName"></TD>
			</TR>
			<TR vAlign="top">
				<TD height="16"></TD>
				<TD colSpan="3">
					<asp:Label id="lblResourceLink" runat="server" class="infoText" Width="176px" Height="8px">lblResourceLink</asp:Label></TD>
				<TD></TD>
			</TR>
			<TR vAlign="top">
				<TD height="16"></TD>
				<TD colSpan="4">
					<asp:Label id="lblLinkExample" runat="server" class="infoText" Width="328px" Height="8px">lblLinkExample</asp:Label></TD>
			</TR>
			<TR vAlign="top">
				<TD height="32"></TD>
				<TD><SELECT id="Select1" onkeydown="HandleKeyDown()" name="Select1" runat="server"></SELECT></TD>
				<TD colSpan="3"><INPUT id="resourceLink" onkeydown="HandleKeyDown()" type="text" maxLength="500" size="38"
						name="resourceLink"></TD>
			</TR>
			<TR vAlign="top">
				<TD colSpan="3" height="23"></TD>
				<TD>
					<button onclick="OnOk()" class="webButton" id="OkButton" type="button" style="LEFT:0px; WIDTH:5em; POSITION:relative; TOP:0px">
						<%Response.Write(AddResource_OK);%>
					</button></TD>
				<TD>
					<button onClick="OnCancel()" class="webButton" id="CancelButton" type="button" style="LEFT:0px; WIDTH:5em; POSITION:relative; TOP:0px">
						<%Response.Write(AddResource_Cancel);%>
					</button></TD>

			</TR>
		</TABLE>
	</body>
</HTML>

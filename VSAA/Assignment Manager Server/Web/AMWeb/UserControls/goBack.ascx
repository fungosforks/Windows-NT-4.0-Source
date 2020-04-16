<%@ Control Language="c#" AutoEventWireup="false" Codebehind="goBack.ascx.cs" Inherits="Microsoft.VisualStudio.Academic.AssignmentManager.UserControls.goBack" %>
<div nowrap class="goBack" style="POSITION:relative;top:<%Response.Write(GoBack_top);%>;left:<%Response.Write(GoBack_left);%>;">
	<a id="hlHelp" class="itemHotStyle" target="_blank" href='<%Response.Write(GoBack_HelpUrl);%>'>
		<%Response.Write(GoBack_Help);%>
	</a>&nbsp;&nbsp;&nbsp;<% if(GoBackIncludeBack == true){ %>
	<span class="itemHotStyle">
		<a href="<%Response.Write(GoBack_BackURL);%>"><img border="0" src="../images/goBack.gif">
			<%Response.Write(GoBack_Back);%>
		</a>
	</span>
	<%}%>
</div>

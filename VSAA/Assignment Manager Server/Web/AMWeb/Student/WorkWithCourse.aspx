<%@ Register TagPrefix="AM" TagName="Nav" Src="../UserControls/student.ascx" %>
<%@ Page language="c#" Codebehind="WorkWithCourse.aspx.cs" AutoEventWireup="false" Inherits="Microsoft.VisualStudio.Academic.AssignmentManager.Student.WorkWithCourse" %>

<HTML>
	<HEAD>
		<title>
			<%Response.Write(Title);%>
		</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<LINK REL="StyleSheet" HREF="../common/styles.css" type="text/css">
		<!DOCTYPE HTML public="-//W3C//DTD HTML 4.0 Transitional//EN">
		<AM:Nav id="Nav1" runat="server" name="Nav1">
		</AM:Nav>
	</HEAD>
	<BODY>
		<div id="mainBody" class="mainBody">
		</div>
	</BODY>
</HTML>

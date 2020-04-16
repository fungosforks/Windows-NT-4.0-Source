// Handles the display of a simple DHTML dialog capable of displaying
// a custom title, field name, and old value for the field to edit. The
// field to edit in this one is a textarea, providing multi-line support.
function showResourceDialog() {
	var rtnArray;
	rtnArray = showModalDialog("AddResource.aspx", new Array(),
                         "dialogWidth:355px; dialogHeight:250px; center:yes;status:no");
	if(rtnArray != null) {
		frm.txtResourceName.value = rtnArray[0];
		frm.txtResourceValue.value = rtnArray[1];
		frm.txtAdd.value = rtnArray[2];
		frm.submit();
	}
}

function showDeleteResourceDialog() {
	var rtnArray;
	rtnArray = showModalDialog("ConfirmResourceDelete.aspx", new Array(), "dialogWidth:355px; dialogHeight:240px; center:yes;status:no");
	
	if(rtnArray !=null)
	{
		frm.txtDelete.value = rtnArray[0];
		frm.submit();
	}
}

function showDeleteAssignmentDialog()
{
	var rtnArray;
	rtnArray = showModalDialog("ConfirmAssignmentDelete.aspx", new Array(), "dialogWidth:355px; dialogHeight:240px; center:yes;status:no");
	
	if(rtnArray !=null && rtnArray[0] == "1")
	{
		frm.txtAction.value = "DeleteAssignment";
		frm.submit();
	}
}

function showDeleteUserDialog()
{
	var rtnArray;
	rtnArray = showModalDialog("ConfirmUserDelete.aspx", new Array(), "dialogWidth:355px; dialogHeight:240px; center:yes;status:no");
	
	if(rtnArray !=null && rtnArray[0] == "1")
	{
		frm.txtAction.value = "DeleteUser";
		frm.submit();
	}
}
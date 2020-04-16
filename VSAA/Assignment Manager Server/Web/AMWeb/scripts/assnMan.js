var activeTab;
var currentTab = "CM0";
var curCMTab = 0;
var curSATab = 1;
var curMSGTab =2;

//init Function
function initStartState(leftMenu, innerTab, mainTable)	{	
	activeTab = "";
	setLeftMenu(leftMenu, innerTab);
	mainTable.style.height = mainBody.offsetHeight + 100;
	if(mainBody.offsetWidth > 625){
		mainTable.style.width = mainBody.offsetWidth + 20;  //add 20 to account for left displacement
	}else{
		mainTable.style.width = 625;
	}
	switch (leftMenu.id)	{
		case "leftTab0":
			courseTabsRow.style.height = "25px";
			break;
		case "leftTab1":
			serversTabsRow.style.height = "25px";
			break;
	}
}


//left menu functions
function tabMOver(whichCell)	{
	if (tabMenuTable.cells[whichCell].className != "activeTab")	{
		tabMenuTable.cells[whichCell].style.backgroundColor = "#ff9900";
	}
}
function tabMOff(whichCell)	{
	if (tabMenuTable.cells[whichCell].className != "activeTab")	{
		tabMenuTable.cells[whichCell].style.backgroundColor = "";
	}
}

function setLeftMenu(leftMenu, innerTab)	
{
	var xID = leftMenu.id;
	var xClass = leftMenu.className;
	
	if (xID !=activeTab)	{
		activeTab = xID;
		// set the display of all in the content area to NONE
		clearAllInnerTabs();
		//reset all items in the LEFT MENU
		clearAllLeftMenus();
		leftMenu.className = "activeTab";

		whichMenu = leftMenu.id;
		switch (whichMenu)	{
			case "leftTab0":
				setInnerCMTab(innerTab);
				break;
			case "leftTab1":
				setInnerSATab(innerTab);
				break;
		}
	}
}

function clearAllLeftMenus()
{
	leftTab0.className = "inActiveTab";
	leftTab1.className = "inActiveTab";
}

//Tab Menu Functions
function clearAllInnerTabs()	{
	//Course Management Stuff
	courseManTable.style.display = "none";
	courseInfo.style.display = "none";
	courseAssignments.style.display = "none";
	courseUsers.style.display = "none";
	
	//server admin
	serverAdminTable.style.display = "none";
	serSettings.style.display = "none";
	serSecurity.style.display = "none";
	serMyAccount.style.display = "none";
}


function overInnerTabBar(obj, imgID)	{
	if (obj.className =="inActiveInnTab")	{
		obj.style.color = "yellow";
		obj.style.backgroundColor = "#6699cc";
		imgID.src = relativeURL + "images/inActiveTab_CornerRO.gif";
	}
}

function outInnerTabBar(obj, imgID)	{
	if (obj.className =="inActiveInnTab" )	{
		obj.style.color = "";
		obj.style.backgroundColor = "";
		imgID.src = relativeURL + "images/inActiveTab_Corner.gif";
	}
}

//Course Management INNER TAB
function setInnerCMTab(innerTab)	{	
	if (innerTab == 0)	{
		resetInnerCMTabBar();		
		setActiveCMTab(curCMTab);
	} else 	{
		resetInnerCMTabBar();		
		setActiveCMTab(innerTab)
	}
} 

function resetInnerCMTabBar()	{
	CM0.className = "inActiveInnTab";
	CM1.className = "inActiveInnTab";
	CM2.className = "inActiveInnTab";
	
	CM0a.className = "inActiveInnTabA";
	CM1a.className = "inActiveInnTabA";
	CM2a.className = "inActiveInnTabA";
	
	CMtabCorner0.src = relativeURL + "images/inActiveTab_Corner.gif";
	CMtabCorner1.src = relativeURL + "images/inActiveTab_Corner.gif";
	CMtabCorner2.src = relativeURL + "images/inActiveTab_Corner.gif";
	
	eval(currentTab+".style.color = '#336699'");
	
	courseManTable.style.display = "none";
}

function setActiveCMTab(innerTab)
{
	clearAllInnerTabs();
	innerTab.className = "activeInnTab";

	switch (innerTab)	{
		case 0:
			CM0.className = "activeInnTab";
			CM0a.className = "activeInnTabA";
			CMtabCorner0.src = relativeURL + "images/activeTab_Corner.gif";
			courseManTable.style.display = "inline";
			courseInfo.style.display = "inline";
			break;
		case CM0:
			CM0.className = "activeInnTab";
			CM0a.className = "activeInnTabA";
			CMtabCorner0.src = relativeURL + "images/activeTab_Corner.gif";
			courseManTable.style.display = "inline";
			courseInfo.style.display = "inline";
			break;
		case CM1:
			CM1.className = "activeInnTab";
			CM1a.className = "activeInnTabA";
			CMtabCorner1.src = relativeURL + "images/activeTab_Corner.gif";
			courseManTable.style.display = "inline";
			courseAssignments.style.display = "inline";
			break;
		case CM2:
			CM2.className = "activeInnTab";
			CM2a.className = "activeInnTabA";
			CMtabCorner2.src = relativeURL + "images/activeTab_Corner.gif";
			courseManTable.style.display = "inline";
			courseUsers.style.display = "inline";
			break;
	}
	eval(currentTab+".style.color = '#336699'");
	activeTab = innerTab;
	curCMTab = innerTab;
}

function clickCMinnerTab(obj)	{
	var x = obj.className;
	if (x =="InnTabRO")	{
		var x = obj.id;
		resetInnerCMTabBar();
		currentTab = x;
		setActiveCMTab(obj);
	}
}


//Server Administration  INNER TAB
function setInnerSATab(innerTab)	{	
	resetInnerSATabBar();		
	setActiveSATab(innerTab)
} 

function resetInnerSATabBar()	{
	SA0.className = "inActiveInnTab";
	SA3.className = "inActiveInnTab";
	
	SA0a.className = "inActiveInnTabA";
	SA3a.className = "inActiveInnTabA";
	
	
	SAtabCorner0.src = relativeURL + "images/inActiveTab_Corner.gif";
	SAtabCorner3.src = relativeURL + "images/inActiveTab_Corner.gif";
	
	eval(currentTab+".style.color = '#336699'");
	
	serverAdminTable.style.display = "none";
}

function setActiveSATab(innerTab)
{
	clearAllInnerTabs();
	innerTab.className = "activeInnTab";
	switch (innerTab)	{
		case 0:
			SA0.className = "activeInnTab";
			SA0a.className = "activeInnTabA";
			SAtabCorner0.src = relativeURL + "images/activeTab_Corner.gif";
			serverAdminTable.style.display = "inline";
			serSettings.style.display = "inline";
			break;
		case SA0:
			SA0.className = "activeInnTab";
			SA0a.className = "activeInnTabA";
			SAtabCorner0.src = relativeURL + "images/activeTab_Corner.gif";
			serverAdminTable.style.display = "inline";
			serSettings.style.display = "inline";
			break;
		case SA3:
			SA3.className = "activeInnTab";
			SA3a.className = "activeInnTabA";
			SAtabCorner3.src = relativeURL + "images/activeTab_Corner.gif";
			serverAdminTable.style.display = "inline";
			serMyAccount.style.display = "inline";
			break;

	}
	eval(currentTab+".style.color = '#336699'");
	activeTab = innerTab;
	curSATab = innerTab;
}

function clickSAinnerTab(obj)	{
	var x = obj.className;
	if (x =="InnTabRO")	{
		var x = obj.id;
		resetInnerSATabBar();
		currentTab = x;
		setActiveSATab(obj);
	}
}

//MSG INNER TAB

function setInnerMSGTab(innerTab)	{	
	resetInnerMSGTabBar();		
	setActiveMSGTab(innerTab)
} 

function resetInnerMSGTabBar()	{
	MSG0.className = "inActiveInnTab";
	MSG1.className = "inActiveInnTab";
	
	MSG0a.className = "inActiveInnTabA";
	MSG1a.className = "inActiveInnTabA";
	
	
	MSGtabCorner0.src = relativeURL + "images/inActiveTab_Corner.gif";
	MSGtabCorner1.src = relativeURL + "images/inActiveTab_Corner.gif";
	eval(currentTab+".style.color = '#336699'");
}

function setActiveMSGTab(innerTab)
{
	clearAllInnerTabs();
	innerTab.className = "activeInnTab";
	switch (innerTab)	{
		case 0:
			MSG0.className = "activeInnTab";
			MSG0a.className = "activeInnTabA";
			MSGtabCorner0.src = relativeURL + "images/activeTab_Corner.gif";
			break;
	}
	eval(currentTab+".style.color = '#336699'");
	activeTab = innerTab;
	curMSGTab = innerTab;
}

function clickMSGinnerTab(obj)	{
	var x = obj.className;
	if (x =="InnTabRO")	{
		var x = obj.id;
		resetInnerMSGTabBar();
		currentTab = x;
		setActiveMSGTab(obj);
	}
}

////

function mStartLinkOver(obj)	{
	obj.style.color = "#FF0000";
}

function mStartLinkOff(obj)	{
	obj.style.color = "";
}

function goBack(int){
	window.history.go(int);
}

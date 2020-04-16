var activeTab;
var currentTab = "CM0";
var curCMTab = 0;
var curPASSTab = 1;

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
	courseTabsRow.style.height = "30px";
	passwordTabsRow.style.height = "30px";
	
}

//left menu functions
function tabMOver(whichCell)	{
	if (tabMenuTable.cells[whichCell].className != "activeTab")	{
		tabMenuTable.cells[whichCell].style.backgroundColor = "#ff9900";
		if(whichCell == 2){
			numMessages.style.color = "white";
		}
	}
}
function tabMOff(whichCell)	{
	if (tabMenuTable.cells[whichCell].className != "activeTab")	{
		tabMenuTable.cells[whichCell].style.backgroundColor = "";
		if(whichCell == 2){
			numMessages.style.color = "";
		}
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
				//copyText.innerHTML = courseText;
				break;
			case "leftTab1":
				setInnerPASSTab(innerTab);
				//copyText.innerHTML = msgText;
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
	
	//server admin
	passwordTable.style.display = "none";
	passChange.style.display = "none";
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
	
	CM0a.className = "inActiveInnTabA";
	CM1a.className = "inActiveInnTabA";
	
	
	CMtabCorner0.src = relativeURL + "images/inActiveTab_Corner.gif";
	CMtabCorner1.src = relativeURL + "images/inActiveTab_Corner.gif";
	
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


//Password  INNER TAB
function setInnerPASSTab(innerTab)	{	
		resetInnerPASSTabBar();		
		setActivePASSTab(innerTab);
} 

function resetInnerPASSTabBar()	{
	PASS0.className = "inActiveInnTab";
	PASS0a.className = "inActiveInnTabA";
	PASStabCorner0.src = relativeURL + "images/inActiveTab_Corner.gif";
	
	eval(currentTab+".style.color = '#336699'");
	passwordTable.style.display = "none";
}

function setActivePASSTab(innerTab)
{
	clearAllInnerTabs();
	innerTab.className = "activeInnTab";
	switch (innerTab)	{
		case 0:
			PASS0.className = "activeInnTab";
			PASS0a.className = "activeInnTabA";
			PASStabCorner0.src = relativeURL + "images/activeTab_Corner.gif";
			passwordTable.style.display = "inline";
			passChange.style.display = "inline";
			break;
		//3-27 jculhane changed from SAO to PASSO for the case because of an error w/ SAO being undefined.  Works fine now.
		case PASS0:
			PASS0.className = "activeInnTab";
			PASS0a.className = "activeInnTabA";
			PASStabCorner0.src = relativeURL + "images/activeTab_Corner.gif";
			passwordTable.style.display = "inline";
			passChange.style.display = "inline";
			break;
	}
	eval(currentTab+".style.color = '#336699'");
	activeTab = innerTab;
	curPASSTab = innerTab;
}

function clickPASSinnerTab(obj)	{
	var x = obj.className;
	if (x =="InnTabRO")	{
		var x = obj.id;
		resetInnerSATabBar();
		currentTab = x;
		setActivePASSTab(obj);
	}
}


function mStartLinkOver(obj)	{
	obj.style.color = "#FF0000";
}

function mStartLinkOff(obj)	{
	obj.style.color = "";
}

function goBack(int){
	window.history.go(int);
}

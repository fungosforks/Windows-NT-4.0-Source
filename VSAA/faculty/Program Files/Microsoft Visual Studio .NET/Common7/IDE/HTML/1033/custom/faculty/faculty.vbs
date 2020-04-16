function JScriptArrayToSafeArray(oJSArray)
	dim aOutput()
	redim aOutput(oJSArray.length - 1)
	
	for i = 0 to oJSArray.length - 1
		aOutput(i) = oJSArray.item(i)
	next
		
	JScriptArrayToSafeArray = aOutput
end function

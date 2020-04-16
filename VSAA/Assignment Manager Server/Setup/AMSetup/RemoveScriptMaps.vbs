'removes all ScriptMaps from AMUpload and AMDownload vdirs; removes applications
'Sets MimeMap settings on AMDownload to allow project types
'Configures for SSL.

'remove ScriptMaps from AMUpload
set websvc = GetObject("IIS://localhost/w3svc/1/root/AMUpload")
smaps = websvc.ScriptMaps
websvc.ScriptMaps = EmptyArray
websvc.Authanonymous = true
websvc.AuthBasic = false
websvc.AuthNTLM = false
websvc.SetInfo

'remove application for AMUpload
websvc.AppDelete

'remove ScriptMaps from AMDownload
set websvc = GetObject("IIS://localhost/w3svc/1/root/AMDownload")
smaps = websvc.ScriptMaps
websvc.ScriptMaps = EmptyArray
websvc.Authanonymous = true
websvc.AuthBasic = false
websvc.AuthNTLM = false
websvc.SetInfo

'remove application for AMDownload
websvc.AppDelete

'Set MimeMaps on AMDownload to allow project types
Dim aMimeMap, i 
Const ADS_PROPERTY_UPDATE=2 

'Get the current mappings from the MimeMap property 
aMimeMap=websvc.GetEx("MimeMap") 

dim mapexts, ext
mapexts = Array(".vbproj", ".csproj", ".vcproj", ".vjproj", ".ico" ,".rc",".cpp",".h",".resx",".ncb",".vcproj",".txt",".def",".rc2",".manifest",".bmp",".jsl",".vjsproj",".vbdproj",".vbproj",".vb",".cs",".csproj",".csdproj",".idl")

For Each ext In mapexts
    'Add a new MIME-type 
    i=UBound(aMimeMap)+1 
    Redim Preserve aMimeMap(i) 
    Set aMimeMap(i)=CreateObject("MimeMap") 
    aMimeMap(i).Extension= ext
    aMimeMap(i).MimeType="text/plain" 
    websvc.PutEx ADS_PROPERTY_UPDATE,"MimeMap",aMimeMap 
    websvc.SetInfo 
Next

'determine if SSL is being required for this install
If WScript.Arguments.Count > 1 Then
	set args = WScript.Arguments
	If args(1) = "true" Then
		Dim WebRoot
		WebRoot = "IIS://localhost/w3svc/1/root/" & args(0)
		'set the AMWeb root to require SSL
		set websvc = GetObject(WebRoot)
		websvc.AccessSSL = True
		on error resume next
		websvc.SetInfo
	End If
End If


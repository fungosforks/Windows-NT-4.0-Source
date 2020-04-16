'remove the SSL requirement from the metabase
set args = WScript.Arguments
Dim WebRoot
WebRoot = "IIS://localhost/w3svc/1/root/" & args(0)
'set the AMWeb root to require SSL
set websvc = GetObject(WebRoot)
websvc.AccessSSL = False
on error resume next
websvc.SetInfo

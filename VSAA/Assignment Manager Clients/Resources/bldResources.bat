@REM
@REM Copyright © 2000-2003 Microsoft Corporation.  All rights reserved.
@REM
@REM
@REM This source code is licensed under Microsoft Shared Source License
@REM for the Visual Studio .NET Academic Tools Source Licensing Program
@REM For a copy of the license, see http://www.msdnaa.net/assignmentmanager/sourcelicense/
@REM
rem
rem Generate resource file
rem
resgen %1\AMClientResources.txt %1\AMClientResources.resources

csc /r:System.dll /r:mscorlib.dll /t:library /res:AMClientResources.resources /out:AMClientResources.dll *.cs
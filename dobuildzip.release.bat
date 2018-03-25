<!-- : Begin batch script
@echo off
cscript //nologo "%~f0?.wsf" %cd%\Builds
PAUSE
exit /b

----- Begin wsf script --->
<job><script language="VBScript">
Function Zip(sFile,sArchiveName)
  'This script is provided under the Creative Commons license located
  'at http://creativecommons.org/licenses/by-nc/2.5/ . It may not
  'be used for commercial purposes with out the expressed written consent
  'of NateRice.com
 
  Set oFSO = WScript.CreateObject("Scripting.FileSystemObject")
  Set oShell = WScript.CreateObject("Wscript.Shell")
 
  '--------Find Working Directory--------
  aScriptFilename = Split(Wscript.ScriptFullName, "\")
  sScriptFilename = aScriptFileName(Ubound(aScriptFilename))
  sWorkingDirectory = Replace(Wscript.ScriptFullName, sScriptFilename, "")
  '--------------------------------------
 
  '-------Ensure we can find 7z.exe------
  If oFSO.FileExists(sWorkingDirectory & "\" & "7z.exe") Then
    s7zLocation = ""
  ElseIf oFSO.FileExists("C:\Program Files\7-Zip\7z.exe") Then
    s7zLocation = "C:\Program Files\7-Zip\"
  Else
    Zip = "Error: Couldn't find 7z.exe"
    Exit Function
  End If
  '--------------------------------------
 
  oShell.Run """" & s7zLocation & "7z.exe"" a -tzip -y """ & sArchiveName & """ " _
  & sFile, 0, True   
 
  If oFSO.FileExists(sArchiveName) Then
    Zip = 1
  Else
    Zip = "Error: Archive Creation Failed."
  End If
End Function
 
Function UnZip(sArchiveName,sLocation)
  'This script is provided under the Creative Commons license located
  'at http://creativecommons.org/licenses/by-nc/2.5/ . It may not
  'be used for commercial purposes with out the expressed written consent
  'of NateRice.com
 
  Set oFSO = WScript.CreateObject("Scripting.FileSystemObject")
  Set oShell = WScript.CreateObject("Wscript.Shell")
 
  '--------Find Working Directory--------
  aScriptFilename = Split(Wscript.ScriptFullName, "\")
  sScriptFilename = aScriptFileName(Ubound(aScriptFilename))
  sWorkingDirectory = Replace(Wscript.ScriptFullName, sScriptFilename, "")
  '--------------------------------------
 
  '-------Ensure we can find 7z.exe------
  If oFSO.FileExists(sWorkingDirectory & "\" & "7z.exe") Then
    s7zLocation = ""
  ElseIf oFSO.FileExists("C:\Program Files\7-Zip\7z.exe") Then
    s7zLocation = "C:\Program Files\7-Zip\"
  Else
    UnZip = "Error: Couldn't find 7z.exe"
    Exit Function
  End If
  '--------------------------------------
 
  '-Ensure we can find archive to uncompress-
  If Not oFSO.FileExists(sArchiveName) Then
    UnZip = "Error: File Not Found."
    Exit Function
  End If
  '--------------------------------------
 
  oShell.Run """" & s7zLocation & "7z.exe"" e -y -o""" & sLocation & """ """ & _
  sArchiveName & """", 0, True
  UnZip = 1
End Function

	Set args = WScript.Arguments
	Set fso = CreateObject("Scripting.FileSystemObject")
	WScript.Echo fso.GetFileVersion(args(0) & "\SpacepuppyUnityFramework\SpacepuppyUnityFramework.dll")
	Zip args(0) & "\SpacepuppyUnityFramework", args(0) & "\SpacepuppyUnityFramework." & fso.GetFileVersion(args(0) & "\SpacepuppyUnityFramework\SpacepuppyUnityFramework.dll") & ".zip"
</script></job>
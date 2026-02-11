Set oWS = WScript.CreateObject("WScript.Shell")
sLinkFile = oWS.SpecialFolders("Desktop") & "\SecureSolution2.lnk"
Set oLink = oWS.CreateShortcut(sLinkFile)

' Get the current directory
currentDir = CreateObject("Scripting.FileSystemObject").GetParentFolderName(WScript.ScriptFullName)

' Set the shortcut properties
oLink.TargetPath = "powershell.exe"
oLink.Arguments = "-ExecutionPolicy Bypass -File """ & currentDir & "\start_secure_solution.ps1"""
oLink.WorkingDirectory = currentDir
oLink.Description = "Start SecureSolution2 - File Processing System"
oLink.IconLocation = "powershell.exe,0"

' Save the shortcut
oLink.Save

WScript.Echo "Desktop shortcut created successfully!"
WScript.Echo "You can now double-click 'SecureSolution2' on your desktop to start the application."

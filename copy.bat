@echo off
set TestDir=F:\Projects\�ҵ���Ŀ\RemoteControl\Output
rmdir /s /q %TestDir%
md %TestDir%

md %TestDir%\RemoteControl.Server
xcopy /s F:\Projects\�ҵ���Ŀ\RemoteControl\RemoteControl.Server\bin\x86\Debug %TestDir%\RemoteControl.Server

md %TestDir%\RemoteControl.Client
xcopy /s F:\Projects\�ҵ���Ŀ\RemoteControl\RemoteControl.Client\bin\x86\Debug %TestDir%\RemoteControl.Client
rem %TestDir%\RemoteControl.Client\combineDlls.bat
rem copy /y %TestDir%\RemoteControl.Client\RemoteControl.Client.dat %TestDir%\RemoteControl.Server\RemoteControl.Client.dat

C:\Users\Administrator\AppData\Roaming\Microsoft\Windows\SendTo\ClearCSharpUselessFiles.exe %TestDir%
@echo off
PUSHD %~dp0
ilmerge.exe /ndebug /targetplatform:v4 /target:exe /out:RemoteControl.Client_c.exe /log RemoteControl.Client.exe RemoteControl.Audio.dll RemoteControl.Protocals.dll Newtonsoft.Json.Lite.dll 
copy /y RemoteControl.Client_c.exe RemoteControl.Client.dat
del RemoteControl.Client_c.exe
# Win32.TokenPrivileges
Adjust token privileges of a Win32 process

## Build

* Install chocolatey 
	```batch
	@"%SystemRoot%\System32\WindowsPowerShell\v1.0\powershell.exe" -NoProfile -InputFormat None -ExecutionPolicy Bypass -Command "iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))" && SET "PATH=%PATH%;%ALLUSERSPROFILE%\chocolatey\bin"
	```
* Install fake
	
	```batch
	choco install fake
	
* Upgrade fake
	
	```batch
	choco upgrade fake
	
	```
	
* Build
	
	```batch
	fake run build.fsx
	```

# Win32.TokenPrivileges
Adjust token privileges of a Win32 process.

##Example

```Csharp
using Win32.TokenPrivileges;

[...]

var currentProcess = Process.GetCurrentProcess();
Console.WriteLine("Privilege status before:");
Console.WriteLine($"SeBackupPrivilege: {PrivilegeProvider.HasPrivilege(null, currentProcess,PrivilegeName.SeBackupPrivilege)}");
Console.WriteLine($"SeRestorePrivilege: {PrivilegeProvider.HasPrivilege(null, currentProcess, PrivilegeName.SeRestorePrivilege)}"); 
using (new AdjustPrivilege(PrivilegeName.SeBackupPrivilege))
{
   using (new AdjustPrivilege(PrivilegeName.SeRestorePrivilege))
   {    
      Console.WriteLine("Privileges should now be granted.");
      Console.WriteLine($"SeBackupPrivilege: {PrivilegeProvider.HasPrivilege(null, currentProcess,PrivilegeName.SeBackupPrivilege)}");
      Console.WriteLine($"SeRestorePrivilege: {PrivilegeProvider.HasPrivilege(null, currentProcess,PrivilegeName.SeRestorePrivilege)}"); 
   }
}
Console.WriteLine("Privilege status after:");
Console.WriteLine($"SeBackupPrivilege: {PrivilegeProvider.HasPrivilege(null, currentProcess,PrivilegeName.SeBackupPrivilege)}");
Console.WriteLine($"SeRestorePrivilege: {PrivilegeProvider.HasPrivilege(null, currentProcess, PrivilegeName.SeRestorePrivilege)}"); 

[...]


```

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

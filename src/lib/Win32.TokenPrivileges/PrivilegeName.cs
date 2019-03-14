namespace Win32.TokenPrivileges
{
    public enum PrivilegeName
    {
        /// <summary>
        /// Required to assign the primary token of a process.
        /// User Right: Replace a process-level token.
        /// </summary>
        SeAssignPrimaryTokenPrivilege,

        /// <summary>
        /// Required to generate audit-log entries. Give this privilege to secure servers.
        /// User Right: Generate security audits.
        /// </summary>
        SeAuditPrivilege,

        /// <summary>
        /// Required to perform backup operations. This privilege causes the system to grant all read access control to any file, regardless of the access control list (ACL) specified for the file. Any access request other than read is still evaluated with the ACL. This privilege is required by the RegSaveKey and RegSaveKeyExfunctions. The following access rights are granted if this privilege is held:
        /// READ_CONTROL
        /// ACCESS_SYSTEM_SECURITY
        /// FILE_GENERIC_READ
        /// FILE_TRAVERSE
        /// User Right: Back up files and directories.
        /// </summary>
        SeBackupPrivilege,

        /// <summary>
        /// Required to receive notifications of changes to files or directories. This privilege also causes the system to skip all traversal access checks. It is enabled by default for all users.
        /// User Right: Bypass traverse checking.
        /// </summary>
        SeChangeNotifyPrivilege,

        /// <summary>
        /// Required to create named file mapping objects in the global namespace during Terminal Services sessions. This privilege is enabled by default for administrators, services, and the local system account.
        /// User Right: Create global objects.
        ///  Windows XP/2000:  This privilege is not supported. Note that this value is supported starting with Windows Server 2003, Windows XP with SP2, and Windows 2000 with SP4.
        /// </summary>
        SeCreateGlobalPrivilege,

        /// <summary>
        /// Required to create a paging file.
        /// User Right: Create a pagefile.
        /// </summary>
        SeCreatePagefilePrivilege,

        /// <summary>
        /// Required to create a permanent object.
        /// User Right: Create permanent shared objects.
        /// </summary>
        SeCreatePermanentPrivilege,


        /// <summary>
        /// Required to create a symbolic link.
        /// User Right: Create symbolic links.
        /// </summary>
        SeCreateSymbolicLinkPrivilege,

        /// <summary>
        /// Required to create a primary token.
        /// User Right: Create a token object.
        /// </summary>
        SeCreateTokenPrivilege,

        /// <summary>
        /// Required to debug and adjust the memory of a process owned by another account.
        /// User Right: Debug programs.
        /// </summary>
        SeDebugPrivilege,

        /// <summary>
        /// Required to mark user and computer accounts as trusted for delegation.
        /// User Right: Enable computer and user accounts to be trusted for delegation.
        /// </summary>
        SeEnableDelegationPrivilege,

        /// <summary>
        /// Required to impersonate.
        /// User Right: Impersonate a client after authentication.
        /// Windows XP/2000:  This privilege is not supported. Note that this value is supported starting with Windows Server 2003, Windows XP with SP2, and Windows 2000 with SP4.
        /// </summary>
        SeImpersonatePrivilege,

        /// <summary>
        /// Required to increase the base priority of a process.
        /// User Right: Increase scheduling priority.
        /// </summary>
        SeIncreaseBasePriorityPrivilege,

        /// <summary>
        /// Required to increase the quota assigned to a process.
        /// User Right: Adjust memory quotas for a process.
        /// </summary>
        SeIncreaseQuotaPrivilege,

        /// <summary>
        /// Required to allocate more memory for applications that run in the context of users.
        /// User Right: Increase a process working set.
        /// </summary>
        SeIncreaseWorkingSetPrivilege,

        /// <summary>
        /// Required to load or unload a device driver.
        /// User Right: Load and unload device drivers.
        /// </summary>
        SeLoadDriverPrivilege,

        /// <summary>
        /// Required to lock physical pages in memory.
        /// User Right: Lock pages in memory.
        /// </summary>
        SeLockMemoryPrivilege,

        /// <summary>
        /// Required to create a computer account.
        /// User Right: Add workstations to domain.
        /// </summary>
        SeMachineAccountPrivilege,

        /// <summary>
        /// Required to enable volume management privileges.
        /// User Right: Manage the files on a volume.
        /// </summary>
        SeManageVolumePrivilege,

        /// <summary>
        /// Required to gather profiling information for a single process.
        /// User Right: Profile single process.
        /// </summary>
        SeProfileSingleProcessPrivilege,

        /// <summary>
        /// Required to modify the mandatory integrity level of an object.
        /// User Right: Modify an object label.
        /// </summary>
        SeRelabelPrivilege,

        /// <summary>
        /// Required to shut down a system using a network request.
        /// User Right: Force shutdown from a remote system.
        /// </summary>
        SeRemoteShutdownPrivilege,

        /// <summary>
        /// Required to perform restore operations. This privilege causes the system to grant all write access control to any file, regardless of the ACL specified for the file. Any access request other than write is still evaluated with the ACL. Additionally, this privilege enables you to set any valid user or group SID as the owner of a file. This privilege is required by the RegLoadKey function. The following access rights are granted if this privilege is held:
        /// WRITE_DAC
        /// WRITE_OWNER
        /// ACCESS_SYSTEM_SECURITY
        /// FILE_GENERIC_WRITE
        /// FILE_ADD_FILE
        /// FILE_ADD_SUBDIRECTORY
        /// DELETE
        /// User Right: Restore files and directories.      
        /// </summary>
        SeRestorePrivilege,

        /// <summary>
        /// Required to perform a number of security-related functions, such as controlling and viewing audit messages. This privilege identifies its holder as a security operator.
        /// User Right: Manage auditing and security log.
        /// </summary>
        SeSecurityPrivilege,

        /// <summary>
        /// Required to shut down a local system.
        /// User Right: Shut down the system.
        /// </summary>
        SeShutdownPrivilege,

        /// <summary>
        /// Required for a domain controller to use the LDAP directory synchronization services. This privilege enables the holder to read all objects and properties in the directory, regardless of the protection on the objects and properties. By default, it is assigned to the Administrator and LocalSystem accounts on domain controllers.
        /// User Right: Synchronize directory service data.
        /// </summary>
        SeSyncAgentPrivilege,

        /// <summary>
        /// Required to modify the nonvolatile RAM of systems that use this type of memory to store configuration information.
        /// User Right: Modify firmware environment values.
        /// </summary>
        SeSystemEnvironmentPrivilege,

        /// <summary>
        /// Required to gather profiling information for the entire system.
        /// User Right: Profile system performance.
        /// </summary>
        SeSystemProfilePrivilege,

        /// <summary>
        /// Required to modify the system time.
        /// User Right: Change the system time.
        /// </summary>
        SeSystemtimePrivilege,

        /// <summary>
        /// Required to take ownership of an object without being granted discretionary access. This privilege allows the owner value to be set only to those values that the holder may legitimately assign as the owner of an object.
        /// User Right: Take ownership of files or other objects.
        /// </summary>
        SeTakeOwnershipPrivilege,

        /// <summary>
        /// This privilege identifies its holder as part of the trusted computer base. Some trusted protected subsystems are granted this privilege.
        /// User Right: Act as part of the operating system.
        /// </summary>
        SeTcbPrivilege,

        /// <summary>
        /// Required to adjust the time zone associated with the computer's internal clock.
        /// User Right: Change the time zone.
        /// </summary>
        SeTimeZonePrivilege,

        /// <summary>
        /// Required to access Credential Manager as a trusted caller.
        /// User Right: Access Credential Manager as a trusted caller.
        /// </summary>
        SeTrustedCredManAccessPrivilege,

        /// <summary>
        /// Required to undock a laptop.
        /// User Right: Remove computer from docking station.
        /// </summary>
        SeUndockPrivilege,

        /// <summary>
        /// Required to read unsolicited input from a terminal device.
        /// User Right: Not applicable.
        /// </summary>
        SeUnsolicitedInputPrivilege,

        /// <summary>
        /// Unknown privilege
        /// </summary>
        UnKnownPrivilege
    }
}
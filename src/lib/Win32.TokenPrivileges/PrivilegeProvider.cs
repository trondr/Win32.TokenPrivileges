using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Win32.TokenPrivileges
{
    public class PrivilegeProvider
    {
        /// <summary>
        /// Lookup privilege value
        /// </summary>
        /// <param name="systemName"></param>
        /// <param name="privilegeName"></param>
        /// <returns></returns>
        public static PrivilegeValue LookupPrivilegeValue(string systemName, PrivilegeName privilegeName)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT || !CheckEntryPoint("advapi32.dll", "LookupPrivilegeValueA"))
            {
                throw new PrivilegeException("Failed to lookup privilege value. LookupPrivilegeValue() is not supported.");
            }

            PrivilegeValue privilegePrivilegeValue = new PrivilegeValue();
            if (LookupPrivilegeValue(systemName, privilegeName.ToString(), ref privilegePrivilegeValue) == 0)
            {
                throw new PrivilegeException($"Failed to lookup privilege value for privilege '{privilegeName}'. Win32 error: {FormatError(Marshal.GetLastWin32Error())}");
            }
            return privilegePrivilegeValue;
        }

        /// <summary>
        /// Look up privilege name
        /// </summary>
        /// <param name="systemName"></param>
        /// <param name="privilegeValue"></param>
        /// <returns></returns>
        public static string LookupPrivilegeName(string systemName, PrivilegeValue privilegeValue)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT || !CheckEntryPoint("advapi32.dll", "LookupPrivilegeNameA"))
            {
                throw new PrivilegeException("Failed to lookup privilege name. LookupPrivilegeName() is not supported.");
            }

            var name = new StringBuilder();
            int nameLength = 0;
            var ptrLuid = IntPtr.Zero;
            try
            {
                ptrLuid = Marshal.AllocHGlobal(Marshal.SizeOf(privilegeValue));
                Marshal.StructureToPtr(privilegeValue, ptrLuid, true);
                LookupPrivilegeName(systemName, ptrLuid, null, ref nameLength); // Call once to get the name length
                if (nameLength == 0)
                {
                    throw new PrivilegeException("Failed to lookup privilege name. The specified LocallyUniquePrivilegeValue resulted in a privilege name length of size 0. LocallyUniquePrivilegeValue is most probably invalid. Win32 error: " + FormatError(Marshal.GetLastWin32Error()));
                }
                name.EnsureCapacity(nameLength + 1);
                if (!LookupPrivilegeName(null, ptrLuid, name, ref nameLength)) // call again to get the actual privilege name
                {
                    throw new PrivilegeException("Failed to lookup privilege name (last call). Win32 error: " + FormatError(Marshal.GetLastWin32Error()));
                }
                return name.ToString();
            }
            finally
            {
                Marshal.FreeHGlobal(ptrLuid);
            }
        }

        /// <summary>
        /// Look up privilege name
        /// </summary>
        /// <param name="systemName"></param>
        /// <param name="name"></param>
        /// <param name="languageId"></param>
        /// <returns></returns>
        public static string LookupPrivilegeDisplayName(string systemName, string name, out int languageId)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT || !CheckEntryPoint("advapi32.dll", "LookupPrivilegeDisplayNameA"))
            {
                throw new PrivilegeException("Failed to lookup privilege displayName. LookupPrivilegeDisplayName() is not supported.");
            }

            StringBuilder displayName = new StringBuilder();
            int displayNameLength = 0;
            languageId = 0;
            LookupPrivilegeDisplayName(systemName, name, displayName, ref displayNameLength, ref languageId); // Call once to get the display name length
            if (displayNameLength == 0)
            {
                throw new PrivilegeException(string.Format("Failed to lookup privilege display name. The specified privilege name '{0}' resulted in a privilege display name length of size 0. Privilege name '{0}' is most probably invalid. Win32 error: {1}", name, FormatError(Marshal.GetLastWin32Error())));
            }
            displayName.EnsureCapacity(displayNameLength + 1);
            if (!LookupPrivilegeDisplayName(null, name, displayName, ref displayNameLength, ref languageId)) // call again to get the actual privilege display name
            {
                throw new PrivilegeException($"Failed to lookup privilege display name for privilege name '{name}'. Win32 error: {FormatError(Marshal.GetLastWin32Error())}");
            }
            return displayName.ToString();
        }

        /// <summary>
        /// Adjust token privileges for specified process
        /// </summary>
        /// <param name="process"></param>
        /// <param name="disableAllPrivileges"></param>
        /// <param name="newState"></param>
        /// <returns>Previous state</returns>
        public static Privileges AdjustTokenPrivileges(Process process, bool disableAllPrivileges, Privileges newState)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT || !CheckEntryPoint("advapi32.dll", "AdjustTokenPrivileges"))
            {
                throw new PrivilegeException("Failed to adjust token privilege. AdjustTokenPrivileges() is not supported.");
            }
            IntPtr tokenHandle = IntPtr.Zero;
            if (OpenProcessToken(process.Handle, (int)(TokenAccessRights.TokenAdjustPrivileges | TokenAccessRights.TokenQuery), ref tokenHandle) == 0)
            {
                throw new PrivilegeException("Failed to open process token. Win32 error: " + FormatError(Marshal.GetLastWin32Error()));
            }
            Privileges previousState = new Privileges();
            int previousStateSize = 0;

            int newStateSize = sizeof(int) + (sizeof(int) * 2 + sizeof(UInt32)) * newState.PrivilegeCount;
            if (AdjustTokenPrivileges(tokenHandle, disableAllPrivileges, ref newState, newStateSize, ref previousState, ref previousStateSize) == 0)
            {
                throw new PrivilegeException("Failed to enable token privilege. Win32 error: " + FormatError(Marshal.GetLastWin32Error()));
            }
            return previousState;
        }

        /// <summary>
        /// Get privileges of the specified process
        /// </summary>
        /// <param name="systemName"></param>
        /// <param name="process"></param>
        /// <returns></returns>
        public static List<Privilege> GetPrivileges(string systemName, Process process)
        {
            List<Privilege> privilegeList = new List<Privilege>();
            //Get process token
            IntPtr tokenHandle = IntPtr.Zero;
            if (OpenProcessToken(process.Handle, (int)(TokenAccessRights.TokenAdjustPrivileges | TokenAccessRights.TokenQuery), ref tokenHandle) == 0)
            {
                throw new PrivilegeException("Failed to open process token. Win32 error: " + FormatError(Marshal.GetLastWin32Error()));
            }
            // Get length required for Privileges by specifying buffer length of 0
            GetTokenInformation(tokenHandle, TokenInformationClass.TokenPrivileges, IntPtr.Zero, 0, out var privilegesSize);
            var lastError = Marshal.GetLastWin32Error();
            if (lastError == ErrorInsufficientBuffer)
            {
                IntPtr privilegesBuffer = IntPtr.Zero;
                try
                {
                    privilegesBuffer = Marshal.AllocHGlobal((int)privilegesSize);
                    if (!GetTokenInformation(tokenHandle, TokenInformationClass.TokenPrivileges, privilegesBuffer, privilegesSize, out privilegesSize))
                    {
                        throw new PrivilegeException("Failed to get token information. Win32 error: " + FormatError(Marshal.GetLastWin32Error()));
                    }
                    Privileges privileges = (Privileges)Marshal.PtrToStructure(privilegesBuffer, typeof(Privileges));
                    if (privileges.PrivilegeCount > PrivilegeConstants.MaxPrivilegeCount)
                    {
                        throw new PrivilegeException($"Number of privileges exceeds hardcoded maximum of {PrivilegeConstants.MaxPrivilegeCount}. Max size must be set to greater or equal to {privileges.PrivilegeCount} and code be recompiled by a developer to fix this problem.");
                    }
                    for (var i = 0; i < privileges.PrivilegeCount; i++)
                    {
                        Privilege privilege = new Privilege(systemName, privileges.PrivilegeValueAndAttributesArray[i].PrivilegeValue, privileges.PrivilegeValueAndAttributesArray[i].Attributes);
                        privilegeList.Add(privilege);
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(privilegesBuffer);
                }
            }
            return privilegeList;
        }

        /// <summary>
        /// Check if specified process has a privilege
        /// </summary>
        /// <param name="systemName"></param>
        /// <param name="process"></param>
        /// <param name="privilegeName"></param>
        public static bool HasPrivilege(string systemName, Process process, PrivilegeName privilegeName)
        {
            var privileges = GetPrivileges(systemName, process);
            foreach (var privilege in privileges)
            {
                if (privilege.PrivilegeName == privilegeName)
                {
                    if ((privilege.Attributes & PrivilegeAttributes.PrivilegeEnabled) > 0)
                    {
                        return true;
                    }
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Enable privilege
        /// </summary>      
        /// <param name="process"></param>
        /// <param name="privilegeName"></param>
        /// <returns>Previous state for the privilege</returns>
        public static Privileges EnablePrivilege(Process process, PrivilegeName privilegeName)
        {
            var newState = new Privileges
            {
                PrivilegeCount = 1,
                PrivilegeValueAndAttributesArray = new PrivilegeValueAndAttributes[PrivilegeConstants.MaxPrivilegeCount]
            };
            newState.PrivilegeValueAndAttributesArray[0].Attributes = (uint)PrivilegeAttributes.PrivilegeEnabled;
            newState.PrivilegeValueAndAttributesArray[0].PrivilegeValue = LookupPrivilegeValue(null, privilegeName);
            return AdjustTokenPrivileges(process, false, newState);
        }

        /// <summary>
        /// Disable privilege
        /// </summary>
        /// <param name="process"></param>
        /// <param name="privilegeName"></param>
        /// <returns>Previous state for the privilege</returns>
        public static Privileges DisablePrivilege(Process process, PrivilegeName privilegeName)
        {
            Privileges newState = new Privileges
            {
                PrivilegeCount = 1,
                PrivilegeValueAndAttributesArray = new PrivilegeValueAndAttributes[PrivilegeConstants.MaxPrivilegeCount]
            };
            newState.PrivilegeValueAndAttributesArray[0].Attributes = (uint)PrivilegeAttributes.PrivilegeDisabled;
            newState.PrivilegeValueAndAttributesArray[0].PrivilegeValue = LookupPrivilegeValue(null, privilegeName);
            return AdjustTokenPrivileges(process, false, newState);
        }

        /// <summary>
        /// Checks whether a specified method exists on the local computer.
        /// </summary>
        /// <param name="library">The library that holds the method.</param>
        /// <param name="method">The entry point of the requested method.</param>
        /// <returns>True if the specified method is present, false otherwise.</returns>
        internal static bool CheckEntryPoint(string library, string method)
        {
            IntPtr libPtr = LoadLibrary(library);
            if (!libPtr.Equals(IntPtr.Zero))
            {
                if (!GetProcAddress(libPtr, method).Equals(IntPtr.Zero))
                {
                    FreeLibrary(libPtr);
                    return true;
                }
                FreeLibrary(libPtr);
            }
            return false;
        }

        #region Native methods
        /// <summary>
        /// The LookupPrivilegeValue function retrieves the locally unique identifier (LUID) used on a specified system to locally represent the specified privilege name.
        /// </summary>
        /// <param name="lpSystemName">Pointer to a null-terminated string specifying the name of the system on which the privilege name is looked up. If a null string is specified, the function attempts to find the privilege name on the local system.</param>
        /// <param name="lpName">Pointer to a null-terminated string that specifies the name of the privilege, as defined in the Winnt.h header file. For example, this parameter could specify the constant SE_SECURITY_NAME, or its corresponding string, "SeSecurityPrivilege".</param>
        /// <param name="lpPrivilegeValue">Pointer to a variable that receives the locally unique identifier by which the privilege is known on the system, specified by the lpSystemName parameter.</param>
        /// <returns>If the function succeeds, the return value is nonzero.<br></br><br>If the function fails, the return value is zero. To get extended error information, call Marshal.GetLastWin32Error.</br></returns>
        [DllImport("advapi32.dll", EntryPoint = "LookupPrivilegeValueA", CharSet = CharSet.Ansi)]
        private static extern int LookupPrivilegeValue(string lpSystemName, string lpName, ref PrivilegeValue lpPrivilegeValue);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool LookupPrivilegeName(string lpSystemName, IntPtr lpLuid, StringBuilder lpName, ref int cchName);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool LookupPrivilegeDisplayName(string lpSystemName, string lpName, StringBuilder lpDisplayName, ref int cchDisplayName, ref int lpLanguageId);

        /// <summary>
        /// The OpenProcessToken function opens the access token associated with a process.
        /// </summary>
        /// <param name="processHandle">Handle to the process whose access token is opened.</param>
        /// <param name="desiredAccess">Specifies an access mask that specifies the requested types of access to the access token. These requested access types are compared with the token's discretionary access-control list (DACL) to determine which accesses are granted or denied.</param>
        /// <param name="tokenHandle">Pointer to a handle identifying the newly-opened access token when the function returns.</param>
        /// <returns>If the function succeeds, the return value is nonzero.<br></br><br>If the function fails, the return value is zero. To get extended error information, call Marshal.GetLastWin32Error.</br></returns>
        [DllImport("advapi32.dll", EntryPoint = "OpenProcessToken", CharSet = CharSet.Ansi)]
        private static extern int OpenProcessToken(IntPtr processHandle, int desiredAccess, ref IntPtr tokenHandle);

        /// <summary>
        /// The GetTokenInformation function retrieves a specified type of information about an access token. The calling process must have appropriate access rights to obtain the information. For more information see http://msdn.microsoft.com/en-us/library/aa446671(v=VS.85).aspx
        /// </summary>
        /// <param name="tokenHandle"></param>
        /// <param name="tokenInformationClass"></param>
        /// <param name="tokenInformation"></param>
        /// <param name="tokenInformationLength"></param>
        /// <param name="returnLength"></param>
        /// <returns></returns>
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool GetTokenInformation(IntPtr tokenHandle, TokenInformationClass tokenInformationClass, IntPtr tokenInformation, uint tokenInformationLength, out uint returnLength);
        internal const int ErrorInsufficientBuffer = 0x7a;


        /// <summary>
        /// The AdjustTokenPrivileges function enables or disables privileges in the specified access token. Enabling or disabling privileges in an access token requires TOKEN_ADJUST_PRIVILEGES access.
        /// </summary>
        /// <param name="tokenHandle">Handle to the access token that contains the privileges to be modified. The handle must have TOKEN_ADJUST_PRIVILEGES access to the token. If the PreviousState parameter is not NULL, the handle must also have TOKEN_QUERY access.</param>
        /// <param name="disableAllPrivileges">Specifies whether the function disables all of the token's privileges. If this value is TRUE, the function disables all privileges and ignores the NewState parameter. If it is FALSE, the function modifies privileges based on the information pointed to by the NewState parameter.</param>
        /// <param name="newState">Pointer to a TOKEN_PRIVILEGES structure that specifies an array of privileges and their attributes. If the DisableAllPrivileges parameter is FALSE, AdjustTokenPrivileges enables or disables these privileges for the token. If you set the SE_PRIVILEGE_ENABLED attribute for a privilege, the function enables that privilege; otherwise, it disables the privilege. If DisableAllPrivileges is TRUE, the function ignores this parameter.</param>      
        /// <param name="bufferLength">Specifies the size, in bytes, of the buffer pointed to by the PreviousState parameter. This parameter can be zero if the PreviousState parameter is NULL.</param>
        /// <param name="previousState">Pointer to a buffer that the function fills with a TOKEN_PRIVILEGES structure that contains the previous state of any privileges that the function modifies. This parameter can be NULL.</param>      
        /// <param name="returnLength">Pointer to a variable that receives the required size, in bytes, of the buffer pointed to by the PreviousState parameter. This parameter can be NULL if PreviousState is NULL.</param>
        /// <returns>If the function succeeds, the return value is nonzero. To determine whether the function adjusted all of the specified privileges, call Marshal.GetLastWin32Error.</returns>
        [DllImport("advapi32.dll", EntryPoint = "AdjustTokenPrivileges", CharSet = CharSet.Ansi)]
        private static extern int AdjustTokenPrivileges(IntPtr tokenHandle, [MarshalAs(UnmanagedType.Bool)]bool disableAllPrivileges, ref Privileges newState, int bufferLength, ref Privileges previousState, ref int returnLength);

        /// <summary>
        /// The FormatMessage function formats a message string. The function requires a message definition as input. The message definition can come from a buffer passed into the function. It can come from a message table resource in an already-loaded module. Or the caller can ask the function to search the system's message table resource(s) for the message definition. The function finds the message definition in a message table resource based on a message identifier and a language identifier. The function copies the formatted message text to an output buffer, processing any embedded insert sequences if requested.
        /// </summary>
        /// <param name="dwFlags">Specifies aspects of the formatting process and how to interpret the lpSource parameter. The low-order
        /// byte of dwFlags specifies how the function handles line breaks in the output buffer. The low-order byte can
        /// also specify the maximum width of a formatted output line.</param>
        /// <param name="lpSource">Specifies the location of the message definition. The type of this parameter depends
        /// upon the settings in the dwFlags parameter.</param>
        /// <param name="dwMessageId">Specifies the message identifier for the requested message. This parameter is ignored
        /// if dwFlags includes FORMAT_MESSAGE_FROM_STRING.</param>
        /// <param name="dwLanguageId">Specifies the language identifier for the requested message. This parameter is ignored
        /// if dwFlags includes FORMAT_MESSAGE_FROM_STRING.</param>
        /// <param name="lpBuffer">Pointer to a buffer for the formatted (and null-terminated) message. If dwFlags includes
        /// FORMAT_MESSAGE_ALLOCATE_BUFFER, the function allocates a buffer using the LocalAlloc function, and places the
        /// pointer to the buffer at the address specified in lpBuffer.</param>
        /// <param name="nSize">If the FORMAT_MESSAGE_ALLOCATE_BUFFER flag is not set, this parameter specifies the maximum
        /// number of TCHARs that can be stored in the output buffer. If FORMAT_MESSAGE_ALLOCATE_BUFFER is set, this parameter specifies
        /// the minimum number of TCHARs to allocate for an output buffer. For ANSI text, this is the number of bytes; for Unicode
        /// text, this is the number of characters.</param>
        /// <param name="arguments">Pointer to an array of values that are used as insert values in the formatted message. A %1 in
        /// the format string indicates the first value in the Arguments array; a %2 indicates the second argument; and so on.</param>
        /// <returns>If the function succeeds, the return value is the number of TCHARs stored in the output buffer,
        /// excluding the terminating null character.<br></br><br>If the function fails, the return value is zero. To get extended
        /// error information, call Marshal.GetLastWin32Error.</br></returns>
        [DllImport("user32.dll", EntryPoint = "FormatMessageA", CharSet = CharSet.Ansi)]
        private static extern int FormatMessage(int dwFlags, IntPtr lpSource, int dwMessageId, int dwLanguageId, StringBuilder lpBuffer, int nSize, int arguments);

        /// <summary>
        /// Formats an error number into an error message.
        /// </summary>
        /// <param name="number">The error number to convert.</param>
        /// <returns>A string representation of the specified error number.</returns>
        protected static string FormatError(int number)
        {
            try
            {
                StringBuilder buffer = new StringBuilder(255);
                FormatMessage(FormatMessageFromSystem, IntPtr.Zero, number, 0, buffer, buffer.Capacity, 0);
                return buffer.ToString();
            }
            catch (Exception)
            {
                return "Unspecified error [" + number + "]";
            }

        }

        private const int FormatMessageFromSystem = 0x1000;

        /// <summary>
        /// The LoadLibrary function maps the specified executable module into the address space of the calling process.
        /// </summary>
        /// <param name="lpLibFileName">Pointer to a null-terminated string that names the executable module (either a .dll or .exe file). The name specified is the file name of the module and is not related to the name stored in the library module itself, as specified by the LIBRARY keyword in the module-definition (.def) file.</param>
        /// <returns>If the function succeeds, the return value is a handle to the module.<br></br><br>If the function fails, the return value is NULL. To get extended error information, call Marshal.GetLastWin32Error.</br></returns>
        [DllImport("kernel32.dll", EntryPoint = "LoadLibraryA", CharSet = CharSet.Ansi)]
        private static extern IntPtr LoadLibrary(string lpLibFileName);

        /// <summary>
        /// The FreeLibrary function decrements the reference count of the loaded dynamic-link library (DLL). When the reference count reaches zero, the module is unmapped from the address space of the calling process and the handle is no longer valid.
        /// </summary>
        /// <param name="hLibModule">Handle to the loaded DLL module. The LoadLibrary or GetModuleHandle function returns this handle.</param>
        /// <returns>If the function succeeds, the return value is nonzero.<br></br><br>If the function fails, the return value is zero. To get extended error information, call Marshal.GetLastWin32Error.</br></returns>
        [DllImport("kernel32.dll", EntryPoint = "FreeLibrary", CharSet = CharSet.Ansi)]
        private static extern int FreeLibrary(IntPtr hLibModule);

        /// <summary>
        /// The GetProcAddress function retrieves the address of an exported function or variable from the specified dynamic-link library (DLL).
        /// </summary>
        /// <param name="hModule">Handle to the DLL module that contains the function or variable. The LoadLibrary or GetModuleHandle function returns this handle.</param>
        /// <param name="lpProcName">Pointer to a null-terminated string containing the function or variable name, or the function's ordinal value. If this parameter is an ordinal value, it must be in the low-order word; the high-order word must be zero.</param>
        /// <returns>If the function succeeds, the return value is the address of the exported function or variable.<br></br><br>If the function fails, the return value is NULL. To get extended error information, call Marshal.GetLastWin32Error.</br></returns>
        [DllImport("kernel32.dll", EntryPoint = "GetProcAddress", CharSet = CharSet.Ansi)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);
        #endregion
    }
}
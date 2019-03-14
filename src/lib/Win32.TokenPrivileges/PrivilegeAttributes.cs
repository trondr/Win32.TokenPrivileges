using System;

namespace Win32.TokenPrivileges
{
    [Flags]
    public enum PrivilegeAttributes : uint
    {
        PrivilegeDisabled = 0x00000,
        PrivilegeEnabledByDefault = 0x00001,
        PrivilegeEnabled = 0x00002,
        PrivilegeUsedForAccess = 0x80000000
    }
}
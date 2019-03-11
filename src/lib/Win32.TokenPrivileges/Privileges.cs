using System.Runtime.InteropServices;

namespace Win32.TokenPrivileges
{
    /// <summary>
    /// The Privileges structure contains information about a set of privileges for an access token.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Privileges
    {
        /// <summary>
        /// Specifies the number of entries in the Privileges array.
        /// </summary>
        public int PrivilegeCount;
        /// <summary>
        /// Specifies an array of PrivilegeValueAndAttributes structures. Each structure contains the Luid and attributes of a privilege.
        /// </summary>      
        //TODO: Implement marshalling of variable size array. Not sure if this is even possible without some kind of dynamic compilation.
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = PrivilegeConstants.MaxPrivilegeCount)]
        public PrivilegeValueAndAttributes[] PrivilegeValueAndAttributesArray;

    }
}
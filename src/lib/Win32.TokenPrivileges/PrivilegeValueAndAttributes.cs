using System;
using System.Runtime.InteropServices;

namespace Win32.TokenPrivileges
{
    ///<summary>
    /// The PrivilegeValueAndAttributes structure represents a locally unique privilge value and its attributes.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PrivilegeValueAndAttributes
    {
        /// <summary>
        /// Specifies an locally unique privilege value.
        /// </summary>
        public PrivilegeValue PrivilegeValue;
        /// <summary>
        /// Specifies attributes of the locally unique privilege value. This value contains up to 32 one-bit flags. Its meaning is dependent on the definition and use of the locally unique privilege value.
        /// </summary>
        public UInt32 Attributes;
    }
}
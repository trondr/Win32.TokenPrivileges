using System.Runtime.InteropServices;

namespace Win32.TokenPrivileges
{
    /// <summary>
    /// An privilege value is a 64-bit value guaranteed to be unique only on the system on which it was generated and uniqueness is guaranteed only until the system is restarted.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PrivilegeValue
    {
        /// <summary>
        /// The low order part of the 64 bit value.
        /// </summary>
        public int LowPart;

        ///<summary>
        /// The high order part of the 64 bit value.
        /// </summary>
        public int HighPart;
    }
}
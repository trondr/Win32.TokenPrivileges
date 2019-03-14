using System;

namespace Win32.TokenPrivileges
{
    public class PrivilegeException : Exception
    {
        public PrivilegeException() : base() { }
        public PrivilegeException(string message) : base(message) { }
    }
}
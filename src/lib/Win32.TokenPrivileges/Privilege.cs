using System;

namespace Win32.TokenPrivileges
{
    public class Privilege
    {
        // ReSharper disable once UnusedMember.Local
        private Privilege() { }

        public Privilege(string systemName, PrivilegeValue privilegeId, uint attributes)
        {
            _systemName = systemName;
            _privilegeId = privilegeId;
            Attributes = (PrivilegeAttributes)attributes;
        }

        private readonly PrivilegeValue _privilegeId;
        private readonly string _systemName;

        /// <summary>
        /// Get privilege attributes
        /// </summary>
        public PrivilegeAttributes Attributes { get; }

        public PrivilegeName PrivilegeName
        {
            get
            {
                if (!_privilegeName.HasValue)
                {
                    Enum.TryParse(PrivilegeProvider.LookupPrivilegeName(_systemName, _privilegeId), out PrivilegeName tempPrivilegeName);
                    _privilegeName = tempPrivilegeName;
                }
                return _privilegeName.Value;
            }
        }
        private PrivilegeName? _privilegeName;

    }
}
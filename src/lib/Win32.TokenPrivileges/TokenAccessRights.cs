using System;

namespace Win32.TokenPrivileges
{
    [Flags]
    internal enum TokenAccessRights
    {
        /// <summary>
        /// Required to change the default owner, primary group, or DACL of an access token.
        /// </summary>
        TokenAdjustDefault = 0x00080,

        /// <summary>
        /// Required to adjust the attributes of the groups in an access token.
        /// </summary>
        TokenAdjustGroups = 0x00040,

        /// <summary>
        /// Required to enable or disable the privileges in an access token.
        /// </summary>
        TokenAdjustPrivileges = 0x00020,

        /// <summary>
        /// Required to adjust the session ID of an access token. The SE_TCB_NAME privilege is required.
        /// </summary>
        TokenAdjustSessionid = 0x00100,

        /// <summary>
        /// Required to attach a primary token to a process. The SE_ASSIGNPRIMARYTOKEN_NAME privilege is also required to accomplish this task.
        /// </summary>
        TokenAssignPrimary = 0x00001,

        /// <summary>
        /// Required to duplicate an access token.
        /// </summary>
        TokenDuplicate = 0x00002,

        /// <summary>
        /// Combines STANDARD_RIGHTS_EXECUTE and TOKEN_IMPERSONATE.
        /// </summary>
        TokenExecute = 0x20000,

        /// <summary>
        /// Required to attach an impersonation access token to a process.
        /// </summary>
        TokenImpersonate = 0x00004,

        /// <summary>
        /// Required to query an access token.
        /// </summary>
        TokenQuery = 0x00008,

        /// <summary>
        /// Required to query the source of an access token.
        /// </summary>
        TokenQuerySource = 0x00010,

        /// <summary>
        /// Combines STANDARD_RIGHTS_READ and TOKEN_QUERY.
        /// </summary>
        TokenRead = 0x20008,

        /// <summary>
        /// Combines STANDARD_RIGHTS_WRITE, TOKEN_ADJUST_PRIVILEGES, TOKEN_ADJUST_GROUPS, and TOKEN_ADJUST_DEFAULT.
        /// </summary>
        TokenWrite = 0x200e0,

        /// <summary>
        /// Combines all possible access rights for a token.
        /// </summary>
        TokenAllAccess = 0xf01ff

    }
}
using System;
using System.Diagnostics;

namespace Win32.TokenPrivileges
{
    /// <summary>
    /// Adjust token privileges
    /// </summary>
    public class AdjustPrivilege : IDisposable
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public AdjustPrivilege(PrivilegeName privilegeName)
        {
            _process = Process.GetCurrentProcess();
            _previousState = PrivilegeProvider.EnablePrivilege(_process, privilegeName);
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~AdjustPrivilege()
        {
            Cleanup();
        }

        #region Properties and fields
        private readonly Process _process;
        private readonly Privileges _previousState;
        //private readonly PrivilegeName _privilegeName;
        #endregion


        #region IDisposable Implementation and Cleanup
        /// <summary>
        /// Cleanup
        /// </summary>
        protected virtual void Cleanup()
        {
            //Restore previous state
            PrivilegeProvider.AdjustTokenPrivileges(_process, false, _previousState);
        }

        /// <summary>
        /// Indicates wether the object has been disposed.
        /// </summary> 
        protected bool Disposed { get; private set; } = false;

        /// <summary>
        /// Dispose object
        /// </summary>
        public void Dispose()
        {
            if (Disposed == false)
            {
                Cleanup();
                Disposed = true;
                GC.SuppressFinalize(this);
            }
        }
        #endregion
    }
}

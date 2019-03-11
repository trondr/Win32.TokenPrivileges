using System.Diagnostics;
using NUnit.Framework;

namespace Win32.TokenPrivileges.Tests
{
    [TestFixture]
    public class AdjustPrivilegeTest
    {
        [Test]
        [Category("UnitTests")]
        public void AdjustPrivilegeConstructorTest()
        {
            var process = Process.GetCurrentProcess();
            var privilegeName = PrivilegeName.SeTimeZonePrivilege;
            Assert.IsFalse(PrivilegeProvider.HasPrivilege(null, process, privilegeName));
            using (new AdjustPrivilege(PrivilegeName.SeTimeZonePrivilege))
            {
                Assert.IsTrue(PrivilegeProvider.HasPrivilege(null, process, privilegeName));
            }
            Assert.IsFalse(PrivilegeProvider.HasPrivilege(null, process, privilegeName));
        }

        [Test]
        [Category("UnitTests")]
        public void GetPrivilegesTest()
        {
            var process = Process.GetCurrentProcess();
            var privileges = PrivilegeProvider.GetPrivileges(null, process);
            Assert.IsTrue(privileges.Count > 0,"Privileges count: " + privileges.Count);
        }
    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineWorldsDeep.Core;

namespace NwdUnitTestProject
{
    [TestClass]
    public class ConfigurationTest
    {
        [TestInitialize]
        public void TestInitialize()
        {
            Configuration.TestMode = false;
        }

        [TestMethod]
        public void TestPhoneSyncSynergyArchivedFolder()
        {
            Assert.AreEqual(Configuration.PhoneSyncSynergyArchivedFolder,
                @"C:\NWD-SYNC\phone\NWD\synergy\archived");
        }

        [TestMethod]
        public void TestPhoneSyncSynergyArchivedFolderTestMode()
        {
            Configuration.TestMode = true;

            Assert.AreEqual(Configuration.PhoneSyncSynergyArchivedFolder,
                @"C:\NWD-SNDBX\NWD-SYNC\phone\NWD\synergy\archived");
        }

        [TestMethod]
        public void TestPhoneSyncSynergyFolder()
        {
            Assert.AreEqual(Configuration.PhoneSyncSynergyFolder,
                @"C:\NWD-SYNC\phone\NWD\synergy");
        }

        [TestMethod]
        public void TestPhoneSyncSynergyFolderTestMode()
        {
            Configuration.TestMode = true;

            Assert.AreEqual(Configuration.PhoneSyncSynergyFolder,
                @"C:\NWD-SNDBX\NWD-SYNC\phone\NWD\synergy");
        }

        [TestMethod]
        public void TestGetPhoneSyncSynergyFilePath()
        {
            Assert.AreEqual(Configuration.GetPhoneSyncSynergyFilePath("Test"),
                @"C:\NWD-SYNC\phone\NWD\synergy\Test.txt");
        }

        [TestMethod]
        public void TestGetPhoneSyncSynergyFilePathTestMode()
        {
            Configuration.TestMode = true;

            Assert.AreEqual(Configuration.GetPhoneSyncSynergyFilePath("Test"),
                @"C:\NWD-SNDBX\NWD-SYNC\phone\NWD\synergy\Test.txt");
        }

        [TestMethod]
        public void TestGetPhoneSyncSynergyArchiveFilePathTestMode()
        {
            Configuration.TestMode = true;

            Assert.AreEqual(Configuration.GetPhoneSyncSynergyArchiveFilePath("Test"),
                @"C:\NWD-SNDBX\NWD-SYNC\phone\NWD\synergy\archived\Test.txt");
        }


        [TestMethod]
        public void TestGetPhoneSyncSynergyArchiveFilePath()
        {
            Assert.AreEqual(Configuration.GetPhoneSyncSynergyArchiveFilePath("Test"),
                @"C:\NWD-SYNC\phone\NWD\synergy\archived\Test.txt");
        }
    }
}

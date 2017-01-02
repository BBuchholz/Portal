using NineWorldsDeep.Tagger;
using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NwdUnitTestProject
{
    /// <summary>
    /// Summary description for FileElementTest
    /// </summary>
    [TestClass]
    public class FileElementTest
    {
        List<FileElement> lst;
        FileElement fe;

        public FileElementTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestInitialize()]
        public void TestInitialize()
        {
            //fe = new FileElement() { Name = "Test.txt", Path = @"C:\NWD-SNDBX\Test.txt" };
            //lst = new List<FileElement>();
        }

        [TestMethod]
        public void TestMatrixPath()
        {
            //TEST DRIVEN DEVELOPMENT
            // MatrixPath should be an attribute of FileElement
            // which holds the NWD specific, platform independant path to file
            // eg. "NWD/config/warhouse.cfg" or "NWD-AUX/voicememos/20160106212500.wav"
            // we need to test that Path="C:\NWD\something" becomes "NWD/something"
        }

        [TestMethod]
        public void TestContains()
        {
            ////This is hackish, need to test that List.Contains will work as expected for FileElements
            //Assert.AreEqual(0, lst.Count);

            //lst.Add(fe);

            //Assert.AreEqual(1, lst.Count);

            //lst.Add(fe); //should be able to add again

            //Assert.AreEqual(2, lst.Count);

            //Assert.IsTrue(lst.Contains(fe));

            //FileElement feOther = new FileElement() { Name = "Other.txt", Path = @"C:\NWD-SNDBX\Other.txt" };

            //Assert.IsFalse(lst.Contains(feOther));
            
            //if (!lst.Contains(feOther))
            //{
            //    lst.Add(feOther);
            //}

            //Assert.AreEqual(3, lst.Count);


            //if (!lst.Contains(feOther))
            //{
            //    lst.Add(feOther);
            //}

            //Assert.AreEqual(3, lst.Count); //should still be 3
        }
    }
}

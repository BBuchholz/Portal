using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineWorldsDeep.Parser;
using System.Collections.Generic;

namespace NwdUnitTestProject
{
    [TestClass]
    public class ParserTest
    {
        private Parser p;

        private String mixedContentPre;
        private String mixedContentPost;
        private String mixedContentOutside;
        private String mixedContentInside;
        
        private String atomicContentPre;
        private String atomicContentPost;
        private String atomicContentOutside;
        private String atomicContentInside;
        private String atomicMultiple;

        private String inputBasic;
        private String inputNested;
        private String inputNestedAlphaNumeric;
    
        private String nestedKey;
        private String nestedKeyAlphaNumeric;
    
        private String nestedKeyEmptyKeyNode;
    
        private String inputMissingBraces1;
        private String inputMissingBraces2;
        private String inputMissingEquals1;
        private String inputMissingEquals2;
        private String inputSpacedEquals1;
        private String inputSpacedEquals2;
        private String inputSpacedEquals3;
    
        public ParserTest()
        {
            mixedContentPre = 
                "testing some text before aTag={something}";            
            mixedContentPost = 
                "aTag={something} with text appearing after it";
            mixedContentOutside = 
                "some text surrounding aTag={something} with " +
                "text on either side";
            mixedContentInside = 
                "aTag={with something} surrounding content " +
                "along with anotherTag={on the other side}";

            atomicContentPre = "atomic={testing some text before " +
                "aTag={something}}";
            atomicContentPost = "atomic={aTag={something} with " +
                "text appearing after it}";
            atomicContentOutside = "atomic={some text surrounding " +
                "aTag={something} with text on either side}";
            atomicContentInside = "atomic={aTag={with something} " +
                "surrounding content along with anotherTag={on " +
                "the other side}}";
            atomicMultiple = "atomic1={something} atomic2={something}";

            inputBasic = "tags={some tag, another tag}";
            inputNested = "displayName={tags={some tag, another tag}}";
            inputNestedAlphaNumeric =
                    "displayName={tags1={some tag} tags2={another tag}}";

            nestedKey = "grandparent/parent/child/grandchild";
            nestedKeyAlphaNumeric =
                    "grandparent1/parent2/child3/grandchild4";

            nestedKeyEmptyKeyNode = "grandparent//child/grandchild";

            inputMissingBraces1 = 
                "displayName={tags={some tag, another tag}";
            inputMissingBraces2 = 
                "displayName={tags=some tag, another tag}}";
            inputMissingEquals1 = 
                "displayName={tags{some tag, another tag}}";
            inputMissingEquals2 = 
                "displayName{tags={some tag, another tag}}";
            inputSpacedEquals1 = 
                "displayName ={tags={some tag, another tag}}";
            inputSpacedEquals2 = 
                "displayName= {tags={some tag, another tag}}";
            inputSpacedEquals3 = 
                "displayName = {tags={some tag, another tag}}";

        }

        [TestInitialize]
        public void TestInitialize()
        {
            p = new Parser();
        }
        
        [TestMethod]
        public void TestGetFirstKey()
        {
            Assert.IsNull(p.GetFirstKey("not a key to be found"));
            Assert.AreEqual(p.GetFirstKey(mixedContentOutside), "aTag");
            Assert.AreEqual(p.GetFirstKey(atomicContentInside), "atomic");
        }

        [TestMethod]
        public void TestTrimKeyVal()
        {
            Assert.AreEqual(p.TrimKeyVal("aTag", mixedContentOutside), 
                "some text surrounding with text on either side");
            Assert.AreEqual(p.TrimKeyVal("aTag", mixedContentInside), 
                "surrounding content along with anotherTag={on the other side}");
            Assert.AreEqual(p.TrimKeyVal("atomic", atomicContentPre), "");
        }

        [TestMethod]
        public void testValidateMatchBraces()
        {   
            //valid inputs
            Assert.IsTrue(p.validateMatchBraces(inputBasic));
            Assert.IsTrue(p.validateMatchBraces(inputNested));
            Assert.IsTrue(p.validateMatchBraces(inputNestedAlphaNumeric));

            //invalid inputs
            Assert.IsFalse(p.validateMatchBraces(inputMissingBraces1));
            Assert.IsFalse(p.validateMatchBraces(inputMissingBraces2));
        }

        /**
         * Test of validateOpenKeyFormat method, of class Parser.
         */
        [TestMethod]
        public void testValidateOpenKeyFormat()
        {   
            //valid inputs for this test
            Assert.IsTrue(p.validateOpenKeyFormat(inputBasic));
            Assert.IsTrue(p.validateOpenKeyFormat(inputNested));
            Assert.IsTrue(p.validateOpenKeyFormat(inputMissingBraces1));
            Assert.IsTrue(p.validateOpenKeyFormat(inputMissingBraces2));
            Assert.IsTrue(p.validateOpenKeyFormat(inputNestedAlphaNumeric));

            //invalid inputs for this test
            Assert.IsFalse(p.validateOpenKeyFormat(inputMissingEquals1));
            Assert.IsFalse(p.validateOpenKeyFormat(inputMissingEquals2));
            Assert.IsFalse(p.validateOpenKeyFormat(inputSpacedEquals1));
            Assert.IsFalse(p.validateOpenKeyFormat(inputSpacedEquals2));
            Assert.IsFalse(p.validateOpenKeyFormat(inputSpacedEquals3));

        }

        /**
         * Test of validate method, of class Parser.
         */
        [TestMethod]
        public void TestValidate()
        {
            //mixed content
            Assert.IsTrue(p.validate(mixedContentPre));
            Assert.IsTrue(p.validate(mixedContentPost));
            Assert.IsTrue(p.validate(mixedContentOutside));
            Assert.IsTrue(p.validate(mixedContentInside));
            
            //atomic content
            Assert.IsTrue(p.validate(atomicContentPre));
            Assert.IsTrue(p.validate(atomicContentPost));
            Assert.IsTrue(p.validate(atomicContentOutside));
            Assert.IsTrue(p.validate(atomicContentInside));
            Assert.IsTrue(p.validate(atomicMultiple));

            //valid input
            Assert.IsTrue(p.validate(inputBasic));
            Assert.IsTrue(p.validate(inputNested));
            Assert.IsTrue(p.validate(inputNestedAlphaNumeric));

            //invalid input
            Assert.IsFalse(p.validate(inputMissingBraces1));
            Assert.IsFalse(p.validate(inputMissingBraces2));
            Assert.IsFalse(p.validate(inputMissingEquals1));
            Assert.IsFalse(p.validate(inputMissingEquals2));
            Assert.IsFalse(p.validate(inputSpacedEquals1));
            Assert.IsFalse(p.validate(inputSpacedEquals2));
            Assert.IsFalse(p.validate(inputSpacedEquals3));

        }

        [TestMethod]
        public void TestIsAtomic()
        {
            Assert.IsFalse(p.IsAtomic(mixedContentPre));
            Assert.IsFalse(p.IsAtomic(mixedContentPost));
            Assert.IsFalse(p.IsAtomic(mixedContentOutside));
            Assert.IsFalse(p.IsAtomic(mixedContentInside));

            Assert.IsTrue(p.IsAtomic(atomicContentPre));
            Assert.IsTrue(p.IsAtomic(atomicContentPost));
            Assert.IsTrue(p.IsAtomic(atomicContentOutside));
            Assert.IsTrue(p.IsAtomic(atomicContentInside));
            Assert.IsTrue(p.IsAtomic(atomicMultiple));
        }

        [TestMethod]
        public void TestExtract()
        {
            String key = "tags";
            String expResult = "some tag, another tag";
            String result = p.Extract(key, inputBasic);
            Assert.AreEqual(expResult, result);

            key = "displayName";
            result = p.Extract(key, inputNested);
            expResult = "tags={some tag, another tag}";
            Assert.AreEqual(expResult, result);

            key = "displayName";
            result = p.Extract(key, inputNestedAlphaNumeric);
            expResult = "tags1={some tag} tags2={another tag}";
            Assert.AreEqual(expResult, result);

            key = "displayName/tags1";
            result = p.Extract(key, inputNestedAlphaNumeric);
            expResult = "some tag";
            Assert.AreEqual(expResult, result);

            key = "displayName/tags2";
            result = p.Extract(key, inputNestedAlphaNumeric);
            expResult = "another tag";
            Assert.AreEqual(expResult, result);


            key = "nonExistentKey";
            result = p.Extract(key, inputNestedAlphaNumeric);
            expResult = null;
            Assert.AreEqual(expResult, result);

        }

        /**
         * Test of validateNestedKey method, of class Parser.
         */
        [TestMethod]
        public void testValidateNestedKey()
        {   
            Assert.IsTrue(p.validateNestedKey(nestedKey));
            Assert.IsTrue(p.validateNestedKey(nestedKeyAlphaNumeric));
            Assert.IsFalse(p.validateNestedKey(nestedKeyEmptyKeyNode));
        }

        [TestMethod]
        public void testGetInvertedKeyStack()
        {  
            Stack<string> expResult = new Stack<string>();

            expResult.Push("grandchild");
            expResult.Push("child");
            expResult.Push("parent");
            expResult.Push("grandparent");

            Stack<string> result = p.GetInvertedKeyStack(nestedKey);

            Assert.AreEqual(expResult.Count, result.Count);          

            while(result.Count > 0)
            {
                Assert.AreEqual(expResult.Pop(), result.Pop());
            }
        }

        /**
         * Test of validateNonEmptyKeyNodes method, of class Parser.
         */
        [TestMethod]
    public void testValidateNonEmptyKeyNodes()
        {
            Assert.IsTrue(p.validateNonEmptyKeyNodes(nestedKey));
            Assert.IsTrue(p.validateNonEmptyKeyNodes(nestedKeyAlphaNumeric));
            Assert.IsFalse(p.validateNonEmptyKeyNodes(nestedKeyEmptyKeyNode));
        }

        /**
         * Test of extractOne method, of class Parser.
         */
        [TestMethod]
        public void testExtractOne()
        {
            String key = "tags";
            String expResult = "some tag, another tag";
            String result = p.extractOne(key, inputBasic);
            Assert.AreEqual(expResult, result);

            key = "displayName";
            result = p.extractOne(key, inputNested);
            expResult = "tags={some tag, another tag}";
            Assert.AreEqual(expResult, result);

            result = p.extractOne(key, inputNestedAlphaNumeric);
            expResult = "tags1={some tag} tags2={another tag}";
            Assert.AreEqual(expResult, result);
        }
    }
}

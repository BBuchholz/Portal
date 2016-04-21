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

        private string inputMultiTag = "Some text mentioning completedAt={} " +
            "more than once, like completedAt={} again, " +
            "with a timestamp appended completedAt={20151226133500}";
        private string inputMultiTagExpectedTrimLastKeyResult = 
            "Some text mentioning completedAt={} " +
            "more than once, like completedAt={} again, with a timestamp appended";
        //This is a nonsense example, but verifies that the first key is trimmed leaving the second
        private string inputMultiTagExpectedTrimKeyResult = "Some text mentioning " +
            "more than once, like completedAt={} again, with a timestamp appended completedAt={20151226133500}";

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
        public void TestTrimLastKeyVal()
        {
            Assert.AreEqual(inputMultiTagExpectedTrimLastKeyResult,
                p.TrimLastKeyVal("completedAt", inputMultiTag));
        }

        [TestMethod]
        public void TestTrimKeyValMultiTag()
        {
            Assert.AreEqual(inputMultiTagExpectedTrimKeyResult,
                p.TrimKeyVal("completedAt", inputMultiTag));
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
        public void TestValidateMatchBraces()
        {   
            //valid inputs
            Assert.IsTrue(p.ValidateMatchBraces(inputBasic));
            Assert.IsTrue(p.ValidateMatchBraces(inputNested));
            Assert.IsTrue(p.ValidateMatchBraces(inputNestedAlphaNumeric));

            //invalid inputs
            Assert.IsFalse(p.ValidateMatchBraces(inputMissingBraces1));
            Assert.IsFalse(p.ValidateMatchBraces(inputMissingBraces2));
        }

        /**
         * Test of validateOpenKeyFormat method, of class Parser.
         */
        [TestMethod]
        public void TestValidateOpenKeyFormat()
        {   
            //valid inputs for this test
            Assert.IsTrue(p.ValidateOpenKeyFormat(inputBasic));
            Assert.IsTrue(p.ValidateOpenKeyFormat(inputNested));
            Assert.IsTrue(p.ValidateOpenKeyFormat(inputMissingBraces1));
            Assert.IsTrue(p.ValidateOpenKeyFormat(inputMissingBraces2));
            Assert.IsTrue(p.ValidateOpenKeyFormat(inputNestedAlphaNumeric));

            //invalid inputs for this test
            Assert.IsFalse(p.ValidateOpenKeyFormat(inputMissingEquals1));
            Assert.IsFalse(p.ValidateOpenKeyFormat(inputMissingEquals2));
            Assert.IsFalse(p.ValidateOpenKeyFormat(inputSpacedEquals1));
            Assert.IsFalse(p.ValidateOpenKeyFormat(inputSpacedEquals2));
            Assert.IsFalse(p.ValidateOpenKeyFormat(inputSpacedEquals3));

        }

        /**
         * Test of validate method, of class Parser.
         */
        [TestMethod]
        public void TestValidate()
        {
            //multiTag
            Assert.IsTrue(p.Validate(inputMultiTag));

            //mixed content
            Assert.IsTrue(p.Validate(mixedContentPre));
            Assert.IsTrue(p.Validate(mixedContentPost));
            Assert.IsTrue(p.Validate(mixedContentOutside));
            Assert.IsTrue(p.Validate(mixedContentInside));
            
            //atomic content
            Assert.IsTrue(p.Validate(atomicContentPre));
            Assert.IsTrue(p.Validate(atomicContentPost));
            Assert.IsTrue(p.Validate(atomicContentOutside));
            Assert.IsTrue(p.Validate(atomicContentInside));
            Assert.IsTrue(p.Validate(atomicMultiple));

            //valid input
            Assert.IsTrue(p.Validate(inputBasic));
            Assert.IsTrue(p.Validate(inputNested));
            Assert.IsTrue(p.Validate(inputNestedAlphaNumeric));

            //invalid input
            Assert.IsFalse(p.Validate(inputMissingBraces1));
            Assert.IsFalse(p.Validate(inputMissingBraces2));
            Assert.IsFalse(p.Validate(inputMissingEquals1));
            Assert.IsFalse(p.Validate(inputMissingEquals2));
            Assert.IsFalse(p.Validate(inputSpacedEquals1));
            Assert.IsFalse(p.Validate(inputSpacedEquals2));
            Assert.IsFalse(p.Validate(inputSpacedEquals3));

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
            Assert.IsTrue(p.ValidateNestedKey(nestedKey));
            Assert.IsTrue(p.ValidateNestedKey(nestedKeyAlphaNumeric));
            Assert.IsFalse(p.ValidateNestedKey(nestedKeyEmptyKeyNode));
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
            Assert.IsTrue(p.ValidateNonEmptyKeyNodes(nestedKey));
            Assert.IsTrue(p.ValidateNonEmptyKeyNodes(nestedKeyAlphaNumeric));
            Assert.IsFalse(p.ValidateNonEmptyKeyNodes(nestedKeyEmptyKeyNode));
        }

        /**
         * Test of extractOne method, of class Parser.
         */
        [TestMethod]
        public void testExtractOne()
        {
            String key = "tags";
            String expResult = "some tag, another tag";
            String result = p.ExtractOne(key, inputBasic);
            Assert.AreEqual(expResult, result);

            key = "displayName";
            result = p.ExtractOne(key, inputNested);
            expResult = "tags={some tag, another tag}";
            Assert.AreEqual(expResult, result);

            result = p.ExtractOne(key, inputNestedAlphaNumeric);
            expResult = "tags1={some tag} tags2={another tag}";
            Assert.AreEqual(expResult, result);
        }
    }
}

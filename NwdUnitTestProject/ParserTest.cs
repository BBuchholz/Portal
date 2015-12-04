using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineWorldsDeep.Parser;
using System.Collections.Generic;

namespace NwdUnitTestProject
{
    [TestClass]
    public class ParserTest
    {
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

            inputBasic = "tags={some tag, another tag}";
            inputNested = "displayName={tags={some tag, another tag}}";
            inputNestedAlphaNumeric =
                    "displayName={tags1={some tag} tags2={another tag}}";

            nestedKey = "grandparent/parent/child/grandchild";
            nestedKeyAlphaNumeric =
                    "grandparent1/parent2/child3/grandchild4";

            nestedKeyEmptyKeyNode = "grandparent//child/grandchild";

            inputMissingBraces1 = "displayName={tags={some tag, another tag}";
            inputMissingBraces2 = "displayName={tags=some tag, another tag}}";
            inputMissingEquals1 = "displayName={tags{some tag, another tag}}";
            inputMissingEquals2 = "displayName{tags={some tag, another tag}}";
            inputSpacedEquals1 = "displayName ={tags={some tag, another tag}}";
            inputSpacedEquals2 = "displayName= {tags={some tag, another tag}}";
            inputSpacedEquals3 = "displayName = {tags={some tag, another tag}}";

        }
        
        [TestMethod]
        public void testValidateMatchBraces()
        {            
            Parser p = new Parser();

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
            Parser p = new Parser();

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
    public void testValidate()
        {
            Parser p = new Parser();

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
        public void testExtract()
        {
            Parser p = new Parser();

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
            Parser p = new Parser();

            Assert.IsTrue(p.validateNestedKey(nestedKey));
            Assert.IsTrue(p.validateNestedKey(nestedKeyAlphaNumeric));
            Assert.IsFalse(p.validateNestedKey(nestedKeyEmptyKeyNode));
        }

        [TestMethod]
        public void testGetInvertedKeyStack()
        {            
            Parser p = new Parser();

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
            Parser p = new Parser();

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
            Parser p = new Parser();

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

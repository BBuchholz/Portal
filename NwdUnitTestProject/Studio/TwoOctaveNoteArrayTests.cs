using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineWorldsDeep.Studio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Studio.Tests
{
    [TestClass()]
    public class TwoOctaveNoteArrayTests
    {

        [TestMethod()]
        public void EqualsTest()
        {
            TwoOctaveNoteArray notesMatchOne =
                new TwoOctaveNoteArray();
            TwoOctaveNoteArray notesMatchTwo =
                new TwoOctaveNoteArray();
            TwoOctaveNoteArray notesNonMatch =
                new TwoOctaveNoteArray();

            notesMatchOne[3] = true;
            notesMatchOne[5] = true;
            notesMatchOne[7] = true;
            
            notesMatchTwo[3] = true;
            notesMatchTwo[5] = true;
            notesMatchTwo[7] = true;

            notesNonMatch[3] = true;
            notesNonMatch[5] = true;
            notesNonMatch[7] = true;
            notesNonMatch[9] = true;

            Assert.AreEqual(notesMatchOne, notesMatchTwo);
            Assert.AreNotEqual(notesMatchOne, notesNonMatch);
            Assert.AreNotEqual(notesMatchTwo, notesNonMatch);
        }

        [TestMethod()]
        public void GetHashCodeTest()
        {
            TwoOctaveNoteArray notesMatchOne =
                new TwoOctaveNoteArray();
            TwoOctaveNoteArray notesMatchTwo =
                new TwoOctaveNoteArray();
            TwoOctaveNoteArray notesNonMatch =
                new TwoOctaveNoteArray();

            notesMatchOne[3] = true;
            notesMatchOne[5] = true;
            notesMatchOne[7] = true;

            notesMatchTwo[3] = true;
            notesMatchTwo[5] = true;
            notesMatchTwo[7] = true;

            notesNonMatch[3] = true;
            notesNonMatch[5] = true;
            notesNonMatch[7] = true;
            notesNonMatch[9] = true;

            Assert.AreEqual(notesMatchOne.GetHashCode(), 
                notesMatchTwo.GetHashCode());
            Assert.AreNotEqual(notesMatchOne.GetHashCode(), 
                notesNonMatch.GetHashCode());
            Assert.AreNotEqual(notesMatchTwo.GetHashCode(), 
                notesNonMatch.GetHashCode());
        }
    }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineWorldsDeep.Studio.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Studio.Utils.Tests
{
    [TestClass()]
    public class ChordTests
    {

        [TestMethod()]
        public void ParseToNodeTest()
        {
            // Dm/a
            TwoOctaveNoteArray notes = new TwoOctaveNoteArray();

            notes[9] = true;
            notes[14] = true;
            notes[17] = true;

            Chord dMa = Chord.ParseToNode("Dm/a").Chord;

            Assert.AreEqual<TwoOctaveNoteArray>(notes, dMa.ChordNotes);

            // Am
            notes = new TwoOctaveNoteArray();

            notes[9] = true;
            notes[12] = true;
            notes[16] = true;

            Chord aM = Chord.ParseToNode("Am").Chord;

            Assert.AreEqual<TwoOctaveNoteArray>(notes, aM.ChordNotes);

            // E/g#
            notes = new TwoOctaveNoteArray();

            notes[8] = true;
            notes[11] = true;
            notes[16] = true;

            Chord eGsharp = Chord.ParseToNode("E/g#").Chord;

            Assert.AreEqual<TwoOctaveNoteArray>(notes, eGsharp.ChordNotes);
        }

        [TestMethod()]
        public void MajorTest()
        {
            //c major
            TwoOctaveNoteArray notes = new TwoOctaveNoteArray();

            notes[0] = true;
            notes[4] = true;
            notes[7] = true;

            Chord chord = Chord.Major("C");

            Assert.AreEqual(notes, chord.ChordNotes);

            //e major
            notes = new TwoOctaveNoteArray();

            notes[4] = true;
            notes[8] = true;
            notes[11] = true;

            chord = Chord.Major("E");

            Assert.AreEqual(notes, chord.ChordNotes);
        }

        [TestMethod()]
        public void MinorTest()
        {
            //c minor
            TwoOctaveNoteArray notes = new TwoOctaveNoteArray();

            notes[0] = true;
            notes[3] = true;
            notes[7] = true;

            Chord chord = Chord.Minor("C");

            Assert.AreEqual(notes, chord.ChordNotes);

            //e minor
            notes = new TwoOctaveNoteArray();

            notes[4] = true;
            notes[7] = true;
            notes[11] = true;

            chord = Chord.Minor("E");

            Assert.AreEqual(notes, chord.ChordNotes);
        }
    }
}
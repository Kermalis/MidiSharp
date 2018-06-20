//----------------------------------------------------------------------- 
// <copyright file="EventTests.cs" company="Stephen Toub"> 
//     Copyright (c) Stephen Toub. All rights reserved. 
// </copyright> 
//----------------------------------------------------------------------- 

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MidiSharp.Events.Meta.Text;
using System;

namespace MidiSharp.Tests
{
    [TestClass]
    public sealed class EventTests
    {
        [TestMethod]
        public void TestBaseTextMetaMidiEvents()
        {
            TestBaseTextMetaMidiEvent((deltaTime, text) => new CopyrightTextMetaMidiEvent(null, deltaTime, text));
            TestBaseTextMetaMidiEvent((deltaTime, text) => new CuePointTextMetaMidiEvent(null, deltaTime, text));
            TestBaseTextMetaMidiEvent((deltaTime, text) => new DeviceNameTextMidiEvent(null, deltaTime, text));
            TestBaseTextMetaMidiEvent((deltaTime, text) => new InstrumentTextMetaMidiEvent(null, deltaTime, text));
            TestBaseTextMetaMidiEvent((deltaTime, text) => new LyricTextMetaMidiEvent(null, deltaTime, text));
            TestBaseTextMetaMidiEvent((deltaTime, text) => new MarkerTextMetaMidiEvent(null, deltaTime, text));
            TestBaseTextMetaMidiEvent((deltaTime, text) => new ProgramNameTextMetaMidiEvent(null, deltaTime, text));
            TestBaseTextMetaMidiEvent((deltaTime, text) => new SequenceTrackNameTextMetaMidiEvent(null, deltaTime, text));
            TestBaseTextMetaMidiEvent((deltaTime, text) => new TextMetaMidiEvent(null, deltaTime, text));
        }

        private void TestBaseTextMetaMidiEvent(Func<long, string, BaseTextMetaMidiEvent> factory)
        {
            Utils.AssertThrows<ArgumentOutOfRangeException>(() => factory(-1, ""));
            Utils.AssertThrows<ArgumentNullException>(() => factory(0, null));

            foreach (long deltaTime in new long[] { 0, 1, 10, 100, long.MaxValue })
            {
                foreach (string text in new string[] { "", "a", " ", "abcd", "ab\r\ncd" })
                {
                    BaseTextMetaMidiEvent ev = factory(deltaTime, text);
                    Assert.AreEqual(ev.DeltaTime, deltaTime);
                    Assert.AreEqual(ev.Text, text);

                    BaseTextMetaMidiEvent clone = (BaseTextMetaMidiEvent)ev.DeepClone();
                    Assert.AreEqual(ev.GetType(), clone.GetType());
                    Assert.AreEqual(ev.Owner, clone.Owner);
                    Assert.AreEqual(ev.DeltaTime, clone.DeltaTime);
                    Assert.AreEqual(ev.Text, clone.Text);
                    Assert.AreEqual(ev.MetaEventID, clone.MetaEventID);
                    Assert.AreEqual(ev.ToString(), clone.ToString());

                    ev.Text = "Foo";
                    Assert.AreEqual(ev.Text, "Foo");

                    ev.DeltaTime = 42;
                    Assert.AreEqual(ev.DeltaTime, 42);

                    Assert.AreNotEqual(ev.Text, clone.Text);
                    Assert.AreNotEqual(ev.DeltaTime, clone.DeltaTime);
                    Assert.AreNotEqual(ev.ToString(), clone.ToString());
                    Assert.AreEqual(ev.MetaEventID, clone.MetaEventID);
                }
            }
        }
    }
}
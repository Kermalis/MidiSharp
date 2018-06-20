//----------------------------------------------------------------------- 
// <copyright file="AftertouchNoteVoiceMidiEvent.cs" company="Stephen Toub"> 
//     Copyright (c) Stephen Toub. All rights reserved. 
// </copyright> 
//----------------------------------------------------------------------- 

using System;
using System.Globalization;
using System.IO;

namespace MidiSharp.Events.Voice.Note
{
    /// <summary>MIDI event to modify a note according to the aftertouch of a key.</summary>
    public sealed class AftertouchNoteVoiceMidiEvent : NoteVoiceMidiEvent
    {
        /// <summary>The category status byte for Aftertouch messages.</summary>
        internal const byte CategoryId = 0xA;
        /// <summary>The pressure of the note (0x0 to 0x7F).</summary>
        private byte m_pressure;

        /// <summary>Initialize the Aftertouch MIDI event message.</summary>
        /// <param name="owner">The track that owns this event.</param>
        /// <param name="deltaTime">The amount of time before this event.</param>
        /// <param name="channel">The channel (0x0 through 0xF) for this voice event.</param>
        /// <param name="note">The name of the MIDI note to modify ("C0" to "G10").</param>
        /// <param name="pressure">The velocity of the note (0x0 to 0x7F).</param>
        public AftertouchNoteVoiceMidiEvent(MidiTrack owner, long deltaTime, byte channel, string note, byte pressure) :
            this(owner, deltaTime, channel, GetNoteValue(note), pressure)
        {
        }

        /// <summary>Initialize the Aftertouch MIDI event message.</summary>
        /// <param name="owner">The track that owns this event.</param>
        /// <param name="deltaTime">The amount of time before this event.</param>
        /// <param name="percussion">The percussion instrument to modify.</param>
        /// <param name="pressure">The pressure of the note (0x0 to 0x7F).</param>
        /// <remarks>Channel 10 (internally 0x9) is assumed.</remarks>
        public AftertouchNoteVoiceMidiEvent(MidiTrack owner, long deltaTime, GeneralMidiPercussion percussion, byte pressure) :
            this(owner, deltaTime, (byte)SpecialChannel.Percussion, GetNoteValue(percussion), pressure)
        {
        }

        /// <summary>Initialize the Aftertouch MIDI event message.</summary>
        /// <param name="owner">The track that owns this event.</param>
        /// <param name="deltaTime">The amount of time before this event.</param>
        /// <param name="channel">The channel (0x0 through 0xF) for this voice event.</param>
        /// <param name="note">The MIDI note to modify (0x0 to 0x7F).</param>
        /// <param name="pressure">The pressure of the note (0x0 to 0x7F).</param>
        public AftertouchNoteVoiceMidiEvent(MidiTrack owner, long deltaTime, byte channel, byte note, byte pressure) :
            base(owner, deltaTime, CategoryId, channel, note)
        {
            Pressure = pressure;
        }

        /// <summary>Generate a string representation of the event.</summary>
        /// <returns>A string representation of the event.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}\t0x{1:X2}", base.ToString(), m_pressure); 
        }

        /// <summary>Write the event to the output stream.</summary>
        /// <param name="outputStream">The stream to which the event should be written.</param>
        public override void Write(Stream outputStream)
        {
            base.Write(outputStream);
            outputStream.WriteByte(m_pressure);
        }

        /// <summary>Gets or sets the pressure of the note (0x0 to 0x7F).</summary>
        public byte Pressure
        {
            get { return m_pressure; }
            set { Validate.SetIfInRange("Pressure", ref m_pressure, value, 0, 127); }
        }

        /// <summary>The second parameter as sent in the MIDI message.</summary>
        internal override byte Parameter2 { get { return m_pressure; } }

        /// <summary>Creates a deep copy of the MIDI event.</summary>
        /// <returns>A deep clone of the MIDI event.</returns>
        public override MidiEvent DeepClone()
        {
            return new AftertouchNoteVoiceMidiEvent(Owner, DeltaTime, Channel, Note, Pressure);
        }
    }
}

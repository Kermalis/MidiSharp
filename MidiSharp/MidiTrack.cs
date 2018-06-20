//----------------------------------------------------------------------- 
// <copyright file="MidiTrack.cs" company="Stephen Toub"> 
//     Copyright (c) Stephen Toub. All rights reserved. 
// </copyright> 
//----------------------------------------------------------------------- 

using MidiSharp.Events;
using MidiSharp.Events.Meta;
using MidiSharp.Events.Meta.Text;
using MidiSharp.Headers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace MidiSharp
{
    /// <summary>Represents a single MIDI track in a MIDI file.</summary>
    [DebuggerDisplay("Name = {TrackName}, Events = {Events.Count}")]
    public sealed class MidiTrack : IEnumerable<MidiEvent>
    {
        /// <summary>Gets the collection of MIDI events that are a part of this track.</summary>
        public readonly MidiEventCollection Events;

        /// <summary>Gets or sets whether an end of track marker is required for writing out the entire track.</summary>
        /// <remarks>
        /// MIDI files require an end of track marker at the end of every track.  
        /// Setting this to false could have negative consequences.
        /// </remarks>
        public bool RequireEndOfTrack = true;

        /// <summary>Gets the channel this track is using.</summary>
        /// <remarks>-1 indicates it does not use a channel.</remarks>
        public int Channel { get; internal set; } = -1;

        /// <summary>Gets whether an end of track event has been added.</summary>
        public bool HasEndOfTrack
        {
            get
            {
                // Determine whether the last event is an end of track event
                return Events.Count > 0 ?
                    Events[Events.Count - 1] is EndOfTrackMetaMidiEvent :
                    false;
            }
        }

        /// <summary>Gets the name of the track, based on finding the first track name event in the track, if one exists.</summary>
        internal string TrackName
        {
            get
            {
                SequenceTrackNameTextMetaMidiEvent nameEvent = null;
                foreach (MidiEvent ev in Events)
                {
                    if (ev is SequenceTrackNameTextMetaMidiEvent nev)
                        nameEvent = nev;
                }
                return nameEvent?.Text;
            }
        }

        /// <summary>Initialize the track.</summary>
        public MidiTrack()
        {
            // Create the buffer to store all event information
            Events = new MidiEventCollection();
        }

        /// <summary>Initializes the track with a copy of the data in another track.</summary>
        /// <returns>The track to copy.</returns>
        public MidiTrack(MidiTrack source) : this()
        {
            Validate.NonNull("source", source);
            RequireEndOfTrack = source.RequireEndOfTrack;
            foreach (var e in source.Events)
            {
                Events.Add(e.DeepClone());
            }
        }

        /// <summary>Gets an enumerator for the tracks in the sequence.</summary>
        /// <returns>An enumerator for the tracks in the sequence.</returns>
        IEnumerator IEnumerable.GetEnumerator() { return Events.GetEnumerator(); }

        /// <summary>Gets an enumerator for the tracks in the sequence.</summary>
        /// <returns>An enumerator for the tracks in the sequence.</returns>
        public IEnumerator<MidiEvent> GetEnumerator() { return Events.GetEnumerator(); }

        /// <summary>Write the track to the output stream.</summary>
        /// <param name="outputStream">The output stream to which the track should be written.</param>
        internal void Write(Stream outputStream)
        {
            Validate.NonNull("outputStream", outputStream);

            // Make sure we have an end of track marker if we need one
            if (!HasEndOfTrack && RequireEndOfTrack) throw new InvalidOperationException("The track cannot be written until it has an end of track marker.");

            using (MemoryStream memStream = new MemoryStream())
            {
                // Get the event data and write it out
                for (int i = 0; i < Events.Count; i++)
                {
                    Events[i].Write(memStream);
                }

                // Tack on the header and write the whole thing out to the main stream.
                MTrkChunkHeader header = new MTrkChunkHeader(memStream.ToArray());
                header.Write(outputStream);
            }
        }

        /// <summary>Writes the track to a string in human-readable form.</summary>
        /// <returns>A human-readable representation of the events in the track.</returns>
        public override string ToString()
        {
            // Create a writer, dump to it, return the string
            using (StringWriter writer = new StringWriter(CultureInfo.InvariantCulture))
            {
                ToString(writer);
                return writer.ToString();
            }
        }

        /// <summary>Dumps the MIDI track to the writer in human-readable form.</summary>
        /// <param name="writer">The writer to which the track should be written.</param>
        public void ToString(TextWriter writer)
        {
            Validate.NonNull("writer", writer);
            foreach (MidiEvent ev in Events)
            {
                writer.WriteLine(ev.ToString());
            }
        }

        /// <summary>Merges two track's events.</summary>
        /// <param name="track">The track to take events from.</param>
        public void Merge(MidiTrack track)
        {
            if (track == this) return;
            foreach (var e in track.Events)
            {
                if (e is EndOfTrackMetaMidiEvent) continue;
                for (int i = 0; i < Events.Count - 1; i++)
                {
                    if (e.AbsoluteTime >= Events[i].AbsoluteTime && e.AbsoluteTime <= Events[i + 1].AbsoluteTime)
                    {
                        e.DeltaTime = Events[i + 1].AbsoluteTime - e.AbsoluteTime;
                        Events[i + 1].DeltaTime -= e.DeltaTime;
                        e.Owner = this;
                        for (int j = i + 1; j < Events.Count - 2; j++) // Insert this at the end of the batch for this AbsoluteTime
                        {
                            if (Events[j + 1].DeltaTime != 0)
                            {
                                Events.Insert(j, e);
                                break;
                            }
                        }
                        break;
                    }
                }
            }
        }
    }
}
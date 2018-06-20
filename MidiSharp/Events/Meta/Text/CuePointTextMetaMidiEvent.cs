//----------------------------------------------------------------------- 
// <copyright file="CuePointTextMetaMidiEvent.cs" company="Stephen Toub"> 
//     Copyright (c) Stephen Toub. All rights reserved. 
// </copyright> 
//----------------------------------------------------------------------- 

namespace MidiSharp.Events.Meta.Text
{
    /// <summary>A cue point name meta event.</summary>
    public sealed class CuePointTextMetaMidiEvent : BaseTextMetaMidiEvent
    {
        /// <summary>The meta id for this event.</summary>
        internal const byte MetaId = 0x7;

        /// <summary>Initialize the cue point meta event.</summary>
        /// <param name="owner">The track that owns this event.</param>
        /// <param name="deltaTime">The amount of time before this event.</param>
        /// <param name="text">The text associated with the event.</param>
        public CuePointTextMetaMidiEvent(MidiTrack owner, long deltaTime, string text) : base(owner, deltaTime, MetaId, text) { }
    }
}

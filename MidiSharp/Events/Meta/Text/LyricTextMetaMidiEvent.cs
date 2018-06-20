//----------------------------------------------------------------------- 
// <copyright file="LyricTextMetaMidiEvent.cs" company="Stephen Toub"> 
//     Copyright (c) Stephen Toub. All rights reserved. 
// </copyright> 
//----------------------------------------------------------------------- 

namespace MidiSharp.Events.Meta.Text
{
    /// <summary>A lyric name meta event.</summary>
    public sealed class LyricTextMetaMidiEvent : BaseTextMetaMidiEvent
    {
        /// <summary>The meta id for this event.</summary>
        internal const byte MetaId = 0x5;

        /// <summary>Initialize the lyric meta event.</summary>
        /// <param name="owner">The track that owns this event.</param>
        /// <param name="deltaTime">The amount of time before this event.</param>
        /// <param name="text">The text associated with the event.</param>
        public LyricTextMetaMidiEvent(MidiTrack owner, long deltaTime, string text) : base(owner, deltaTime, MetaId, text) { }
    }
}

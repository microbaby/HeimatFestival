using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace RhythmHeavenMania
{
    [Serializable]
    public class Beatmap
    {
        public float bpm;
        public List<Entity> entities = new List<Entity>();
        public List<TempoChange> tempoChanges = new List<TempoChange>();

        [Serializable]
        public class Entity : ICloneable
        {
            public float beat;
            public int track;
            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)] public float length;
            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)] public float valA;
            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)] public int type;
            public string datamodel;
            [JsonIgnore] public Editor.Track.TimelineEventObj eventObj;

            public object Clone()
            {
                return this.MemberwiseClone();
            }
        }

        [Serializable]
        public class TempoChange : ICloneable
        {
            public float beat;
            public float length;
            public float tempo;

            public object Clone()
            {
                return this.MemberwiseClone();
            }
        }
    }
}
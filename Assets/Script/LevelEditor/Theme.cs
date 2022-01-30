using System;
using UnityEngine;

namespace RhythmHeavenMania.Editor
{
    [Serializable]
    public class Theme
    {
        public string name;
        public Properties properties;
        
        [Serializable]
        public class Properties
        {
            public string TempoLayerCol;
            public string MusicLayerCol;

            public string Layer1Col;
            public string Layer2Col;
            public string Layer3Col;
            public string Layer4Col;

            public string EventSelectedCol;
            public string EventNormalCol;

            public string BeatMarkerCol;
            public string CurrentTimeMarkerCol;

            public string BoxSelectionCol;
            public string BoxSelectionOutlineCol;
        }
    }
}
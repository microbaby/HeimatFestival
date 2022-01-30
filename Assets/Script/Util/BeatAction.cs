using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmHeavenMania.Util
{
    public class BeatAction : MonoBehaviour
    {
        private int index;
        private List<Action> actions = new List<Action>();

        public delegate void EventCallback();

        public class Action
        {
            public float beat { get; set; }
            public EventCallback function { get; set; }

            public Action(float beat, EventCallback function)
            {
                this.beat = beat;
                this.function = function;
            }
        }

        public static void New(GameObject prefab, List<Action> actions)
        {
            BeatAction beatAction = prefab.AddComponent<BeatAction>();
            beatAction.actions = actions;
        }

        private void Update()
        {
            float songPositionInBeats = Conductor.instance.songPositionInBeats;

            for (int i = 0; i < actions.Count; i++)
            {
                if (songPositionInBeats >= actions[i].beat && index == i)
                {
                    actions[i].function.Invoke();
                    index++;
                }
            }
        }
    }

}
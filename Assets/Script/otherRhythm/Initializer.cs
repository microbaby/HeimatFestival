using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmHeavenMania
{
    public class Initializer : MonoBehaviour
    {
        public TextAsset level; //json file
        public AudioClip music; 
        public bool debugUI;

        public bool playOnStart = false;
        public bool editor = false;
        public bool autoplay = false;

        private void Start()
        {
            GameObject Cameras = Instantiate(Resources.Load<GameObject>("Prefabs/Cameras")); 
            Cameras.name = "Cameras";
            GameObject MainCamera = Cameras.transform.GetChild(0).gameObject;
           // GameObject CursorCamera = Cameras.transform.GetChild(1).gameObject;

            //remove cursor
            //GameObject Cursor = Instantiate(Resources.Load<GameObject>("Prefabs/Cursor"));
            //Cursor.name = "Cursor";

            GameObject Games = new GameObject();
            Games.name = "Games";

            GameObject GameManager = new GameObject();
            GameManager.name = "GameManager";
            GameManager gameManager = GameManager.AddComponent<GameManager>();
            gameManager.playOnStart = playOnStart;
            gameManager.autoplay = autoplay;

            gameManager.txt = level;
            gameManager.GamesHolder = Games;
            //gameManager.CircleCursor = Cursor.transform.GetChild(0).GetComponent<CircleCursor>();
            gameManager.GameCamera = MainCamera.GetComponent<Camera>();
            //gameManager.CursorCam = CursorCamera.GetComponent<Camera>();

            GameObject Profiler = Instantiate(Resources.Load<GameObject>("Prefabs/GameProfiler"));
            Profiler.name = "GameProfiler";
            if (!debugUI)
            {
                Profiler.GetComponent<DebugUI>().enabled = false;
                Profiler.transform.GetChild(0).gameObject.SetActive(false);
            }

            GameObject Conductor = new GameObject();
            Conductor.name = "Conductor";
            AudioSource source = Conductor.AddComponent<AudioSource>();
            source.clip = music;
            Conductor.AddComponent<Conductor>();
            Conductor.GetComponent<Conductor>().musicSource = source;
            // Conductor.AddComponent<AudioDspTimeKeeper>();

            if (editor)
            {
                this.GetComponent<RhythmHeavenMania.Editor.Editor>().Init();
            }
            else
            {
                gameManager.Init();
            }
        }
    }
}
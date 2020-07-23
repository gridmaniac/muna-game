using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Celestial
{
    public class CelestialEditor : EditorWindow
    {
        public static EditorWindow window;
        public CelestialGenerator generator;
        public GameObject celestial;
        public CelestialSeed seed;
        public CelestialSeed lastSeed;

        [MenuItem("Window/Celestial Editor")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            window = EditorWindow.GetWindow(typeof(CelestialEditor));
            window.position = new Rect((Screen.resolutions[0].width / 2), (Screen.resolutions[0].height / 2), 400, 700);
            window.minSize = new Vector2(450, 450);
        }

        public CelestialEditor()
        {
            this.generator = new CelestialGenerator();
            this.seed = new CelestialSeed(512, 120.0f, 0.1f, 0.5f, 0.5f);
            this.lastSeed = seed;
        }

        private void OnGUI()
        {
            if (window == null)
                return;

            if (!this.seed.Equals(this.lastSeed))
            {
                if (this.celestial != null)
                {
                    this.generator.UpdateCelestial(this.celestial, this.seed);
                    this.generator.ClearCollider(this.celestial);
                }
            }
            this.lastSeed = (CelestialSeed)this.seed.Clone();

            this.seed.resolution = EditorGUILayout.IntSlider("Resolution", this.seed.resolution, 8, 16384);
            this.seed.radius = EditorGUILayout.Slider("Radius", this.seed.radius, 0.0f, 2000.0f);

            this.seed.topoFreq = EditorGUILayout.Slider("Topo Freq", this.seed.topoFreq, -0.00001f, 2.0f);
            this.seed.topoAmp = EditorGUILayout.Slider("Topo Amp", this.seed.topoAmp, -this.seed.radius, this.seed.radius);

            this.seed.details = EditorGUILayout.Slider("Details", this.seed.details, -this.seed.radius, this.seed.radius);

            if (GUILayout.Button("Generate Collider"))
            {
                this.generator.GenerateCollider(this.celestial);
            }


            EditorGUILayout.LabelField("Celestial", EditorStyles.boldLabel);
            this.celestial = (GameObject)EditorGUILayout.ObjectField(this.celestial, typeof(GameObject), true);


            if (GUILayout.Button("Generate Celestial"))
            {
                this.celestial = this.generator.GenerateCelestial(this.seed);
            }

        }
    }
}

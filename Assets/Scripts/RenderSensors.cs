using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DeathStar
{
    public class RenderSensors : MonoBehaviour
    {
        public GameObject sensor;
        public GameObject parent;
        public float scaleFactor;

        private List<Renderer> renderers;
        private Color colorOFF;

        void Start()
        {
            TextAsset coordinates = Resources.Load("sensors") as TextAsset;
            print(coordinates.text);
            SensorCoordinates sc = JsonUtility.FromJson<SensorCoordinates>(coordinates.text);
            print(sc.name);
            print(sc.x.Length);
            print(sc.y.Length);
            print(sc.z.Length);

            this.renderers = new List<Renderer>();
            this.colorOFF = sensor.GetComponent<Renderer>().sharedMaterial.color;

            for (int i = 0; i < sc.x.Length; i++)
            {
                GameObject go = (GameObject) Instantiate(this.sensor, new Vector3(sc.x[i]/ this.scaleFactor, sc.y[i]/ this.scaleFactor, sc.z[i]/ this.scaleFactor), Quaternion.identity);
                go.transform.parent = this.parent.transform;
                renderers.Add(go.GetComponent<Renderer>());
            }

            StartCoroutine(SimulateData(sc.x.Length-1));
        }

        private IEnumerator SimulateData(int length)
        {
            List<int> ids = new List<int>();
            while(true)
            {
                yield return new WaitForSeconds(Random.Range(.21f, .6f));
                ids.Clear();

                for (int i = 0; i < 200; i++)
                {
                    ids.Add(Random.Range(0, length));
                }

                StartCoroutine(Flash(ids));
            }
        }

        private IEnumerator Flash(List<int> ids)
        {
            MaterialPropertyBlock props = new MaterialPropertyBlock();
            float t = Time.time;

            for (int i = 0; i < ids.Count; i++)
            {
                props.SetColor("_Color", new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
                this.renderers[ids[i]].SetPropertyBlock(props);
            }

            yield return new WaitForSeconds(.1f);

            for (int i = 0; i < ids.Count; i++)
            {
                props.SetColor("_Color", this.colorOFF);
                this.renderers[ids[i]].SetPropertyBlock(props);
            }
        }
    }
}
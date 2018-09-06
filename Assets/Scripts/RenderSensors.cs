using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DeathStar
{
    public class RenderSensors : MonoBehaviour
    {
        public GameObject sensor;
        public GameObject parent;
        public Texture2D heatmap;
        public Text eventnum;
        public Text eventID;
        public float scaleFactor;

        private List<Renderer> renderers;
        private Color colorOFF;
        private SensorCoordinates sc;
        private Events evs;

        void Start()
        {
            TextAsset coordinates = Resources.Load("sensors") as TextAsset;
            print(coordinates.text);
            sc = JsonUtility.FromJson<SensorCoordinates>(coordinates.text);
            print(sc.name);
            print(sc.x.Length);
            print(sc.y.Length);
            print(sc.z.Length);

            TextAsset events = Resources.Load("events3") as TextAsset;
            print(events.text);
            evs = JsonUtility.FromJson<Events>(events.text);
            print(evs.events.Length);

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
            List<int> charges = new List<int>();
            for (int i = 0; i < evs.events.Length; i++)
            {
                yield return new WaitForSeconds(Random.Range(.5f, .51f));
                ids.Clear();
                charges.Clear();
                eventnum.text = evs.events[i].evento.ToString();
                eventID.text = evs.events[i].ID.ToString();

                for (int j = 0; j < evs.events[i].sensor.Length; j++)
                {
                    ids.Add(evs.events[i].sensor[j]);
                    charges.Add(evs.events[i].charge[j]);
                }

                StartCoroutine(Flash(ids, charges));
            }
        }

        private IEnumerator Flash(List<int> ids, List<int> charges)
        {
            MaterialPropertyBlock props = new MaterialPropertyBlock();
            float t = Time.time;

            for (int i = 0; i < ids.Count; i++)
            {
                //props.SetColor("_Color", new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
                props.SetColor("_Color", heatmap.GetPixel(charges[i],0));
                if(ids[i] < this.renderers.Count)
                    this.renderers[ids[i]].SetPropertyBlock(props);
            }

            yield return new WaitForSeconds(.4f);

            for (int i = 0; i < ids.Count; i++)
            {
                props.SetColor("_Color", this.colorOFF);
                if (ids[i] < this.renderers.Count)
                    this.renderers[ids[i]].SetPropertyBlock(props);
            }
        }
    }
}
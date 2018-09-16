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
        public LineRenderer lr;
        public Texture2D heatmap;
        public Text eventnum;
        public Text eventID;
        public float scaleFactor;
        public Material lineMaterial;
        private Color colorOFF;
        public Color background;
        public Color lineColor;

        private List<Renderer> renderers;
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

            TextAsset events = Resources.Load("events-bright") as TextAsset;
            print(events.text);
            evs = JsonUtility.FromJson<Events>(events.text);
            print(evs.events.Length);

            this.renderers = new List<Renderer>();
            this.colorOFF = sensor.GetComponent<Renderer>().sharedMaterial.color;
            this.background = new Color(39f / 255f, 39f / 255f, 39f / 255f, 12f/255f);

            for (int i = 0; i < sc.x.Length; i++)
            {
                GameObject go = (GameObject) Instantiate(this.sensor, new Vector3(sc.x[i]/ this.scaleFactor, sc.z[i]/ this.scaleFactor, sc.y[i]/ this.scaleFactor), Quaternion.identity);
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
                yield return new WaitForSeconds(.8f);
                ids.Clear();
                charges.Clear();
                int line_index = 0;
                eventnum.text = evs.events[i].evento.ToString();
                eventID.text = evs.events[i].ID.ToString();
                this.lr.positionCount = 1 + evs.events[i].sensor.Length*2;

                for (int j = 0; j < evs.events[i].sensor.Length; j++)
                {
                    ids.Add(evs.events[i].sensor[j]);
                    charges.Add(evs.events[i].charge[j]);
                    //print(lr.positionCount);
                    //print(1 + j * 2);
                    if (evs.events[i].charge[j] > 1000)
                    {
                        if (ids[j] < this.renderers.Count)
                        {
                            this.lr.SetPosition(line_index, this.renderers[evs.events[i].sensor[j]].gameObject.transform.position);
                            this.lr.SetPosition((line_index + 1), Vector3.zero);
                        }
                        else
                        {
                            this.lr.SetPosition((line_index), Vector3.zero);
                            this.lr.SetPosition((line_index + 1), Vector3.zero);
                        }
                        line_index += 2;
                    }
                }

                StartCoroutine(Flash(ids, charges));
            }
        }

        private IEnumerator Flash(List<int> ids, List<int> charges)
        {
            MaterialPropertyBlock props = new MaterialPropertyBlock();
            Color[] flashcolors = new Color[ids.Count];
            float t = Time.time;
            this.lineMaterial.SetColor("_TintColor", lineColor);

            for (int i = 0; i < ids.Count; i++)
            {
                //props.SetColor("_Color", new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
                props.SetColor("_Color", flashcolors[i] = heatmap.GetPixel(charges[i],0));
                //flashcolors[i] = props;
                if(ids[i] < this.renderers.Count)
                    this.renderers[ids[i]].SetPropertyBlock(props);
            }

            yield return new WaitForSeconds(.2f);
            while( Time.time < t+.5f )
            { 
                //t += Time.deltaTime;
                for (int i = 0; i < ids.Count; i++)
                {
                    if (ids[i] < this.renderers.Count)
                    {
                        props.SetColor("_Color", Color.Lerp(flashcolors[i], this.colorOFF, (Time.time-t) * 2f));
                        this.renderers[ids[i]].SetPropertyBlock(props);
                    }
                }
                this.lineMaterial.SetColor("_TintColor", Color.Lerp(lineColor, this.background, (Time.time - t) * 2f));

                yield return null;
            }

            /*yield return new WaitForSeconds(.4f);
            
            for (int i = 0; i < ids.Count; i++)
            {
                props.SetColor("_Color", this.colorOFF);
                if (ids[i] < this.renderers.Count)
                    this.renderers[ids[i]].SetPropertyBlock(props);
            }*/
        }
    }
}
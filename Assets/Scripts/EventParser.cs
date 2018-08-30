using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventParser : MonoBehaviour {

	void Start () {
        TextAsset events = Resources.Load("events") as TextAsset;
        print(events.text);
        Events sc = JsonUtility.FromJson<Events>(events.text);
        print(sc.events.Length);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

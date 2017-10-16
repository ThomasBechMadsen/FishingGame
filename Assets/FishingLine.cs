using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingLine : MonoBehaviour {

    Transform player;
    Transform pullPoint;
    LineRenderer line;

	// Use this for initialization
	void Start () {
        player = GetComponent<SpearScript>().owner;
        pullPoint = GetComponent<SpearScript>().pullPoint;
        line = GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        line.SetPosition(0, player.position);
        line.SetPosition(1, pullPoint.position);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WaterFloat : MonoBehaviour {

    Rigidbody rb;

    public List<Transform> floaters = new List<Transform>();


	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        foreach (Transform floater in floaters) {
            if (floater.position.y < 0) {
                float forceFactor = 1 - floater.position.y;
                Vector3 floaterForce = (rb.mass * -Physics.gravity * forceFactor)/floaters.Count;
                rb.AddForceAtPosition(floaterForce, floater.position, ForceMode.Force);
            }
        }
	}
}

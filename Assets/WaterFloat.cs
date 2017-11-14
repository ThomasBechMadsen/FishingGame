using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WaterFloat : MonoBehaviour {

    public GameObject waterSurface;
    public float floaterStrength;
    public List<Transform> floaters = new List<Transform>();
    Rigidbody rb;


    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        foreach (Transform floater in floaters) {
            //Raycast up to hit watersurface
            RaycastHit hit;
            Debug.DrawRay(floater.position, Vector3.up * 100, Color.green);
            if(Physics.Raycast(floater.position, Vector3.up, out hit, 100, LayerMask.GetMask("WaterSurface"))) { //Raycast does not detect backside of mesh
                float forceFactor = 1 + hit.distance;
                Vector3 floaterForce = (floaterStrength * -Physics.gravity * forceFactor)/floaters.Count;
                rb.AddForceAtPosition(floaterForce, floater.position, ForceMode.Force);
            }
        }
	}
}

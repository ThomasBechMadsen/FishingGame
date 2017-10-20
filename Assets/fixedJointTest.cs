using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fixedJointTest : MonoBehaviour {

    public GameObject a;
    FixedJoint joint;
	// Use this for initialization
	void Start () {
        joint = gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = a.GetComponent<Rigidbody>();
    }
	
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCameraTargetScript : MonoBehaviour {

    public float maxDistance;
	
	// Update is called once per frame
	void Update () {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 pos = mouseRay.origin + (mouseRay.direction * -Camera.main.transform.position.z);
        transform.position = Vector3.ClampMagnitude(pos, maxDistance);
    }
}

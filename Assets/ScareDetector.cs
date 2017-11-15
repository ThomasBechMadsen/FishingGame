using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScareDetector : MonoBehaviour {

    FishAI ai;

    void Start()
    {
        ai = transform.parent.GetComponent<FishAI>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Spear")
        {
            ai.changeState(FishAI.states.scared);
        }
    }
}

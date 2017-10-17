using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeleporter : MonoBehaviour {

    public Transform player;
    public Transform endPosition;

    void OnTriggerStay(Collider other)
    {
        if (other.transform == player && Input.GetKeyDown("w"))
        {
            player.GetComponent<PlayerController>().setControl(Control.Walking);
            player.position = endPosition.position;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearScript : MonoBehaviour {

    public float swimTime;
    public float initialForce;
    public float forwardsForce;
    public float airDrag;
    public float waterDrag;
    public float hookedDrag;
    public float airTurnForce;
    public float timeAlive;
    public bool controlable = true;
    public float damage;


    public Transform pullPoint;
    public Transform owner;
    public float pullForce;
    public float pickupDistance;

    Rigidbody rb;
    Coroutine currentState;
    
    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.right * initialForce, ForceMode.Impulse);
        currentState = StartCoroutine(swimState());
	}

    void FixedUpdate()
    {
        timeAlive += Time.fixedDeltaTime;
        if (timeAlive > swimTime)
        {
            controlable = false;
        }
    }

	IEnumerator swimState()
    {
        if (transform.position.y > 0)
        {
            rb.useGravity = true;
            rb.drag = airDrag;
            airTurn();
        }
        else
        {
            rb.useGravity = false;
            rb.drag = waterDrag;
        }

        if (controlable) {
            rb.AddForce(transform.right * forwardsForce, ForceMode.Force);
            yield return new WaitForFixedUpdate();
            currentState = StartCoroutine(swimState());
        }
        else
        {
            StopCoroutine(currentState);
            currentState = StartCoroutine(caughtState());
        }
    }

    IEnumerator caughtState()
    {
        if (Vector3.Distance(owner.position, transform.position) < pickupDistance)
        {
            Destroy(gameObject);
            owner.GetComponent<PlayerController>().SetCameraTarget(owner);
        }
        rb.AddForceAtPosition((owner.position - transform.position).normalized * pullForce, pullPoint.position, ForceMode.Acceleration);
        yield return new WaitForFixedUpdate();
        currentState = StartCoroutine(caughtState());
    }

    void airTurn()
    {
        int direction = -1;
        if(transform.eulerAngles.z > 90 && transform.eulerAngles.z < 270)
        {
            direction = 1;
        }
        rb.AddTorque(direction * transform.forward * airTurnForce, ForceMode.Force);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Fish") {
            rb.drag = hookedDrag;
            //other.transform.parent = transform;
            other.GetComponent<FishScript>().dealDamage(damage);
            StopCoroutine(currentState);
            currentState = StartCoroutine(caughtState());
            controlable = false;
            GetComponent<Collider>().enabled = false;
            //transform.GetChild(2).GetComponent<Collider>().enabled = false;
        }
    }
}

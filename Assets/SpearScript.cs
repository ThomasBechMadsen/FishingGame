using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearScript : MonoBehaviour {

    public float swimTime;
    public float initialForce;
    public float forwardsForce;
    public float airDrag;
    public float waterDrag;
    public float emptyPullDrag;
    public float airTurnForce;
    public float timeAlive;
    public bool controlable = true;
    public float damage;


    public Transform pullPoint;
    public Transform owner;
    public Transform boat;
    public float pullForce;
    public float pickupDistance;

    Rigidbody rb;
    FixedJoint joint;
    SpringJoint springJoint;
    Coroutine currentState;
    
    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.right * initialForce, ForceMode.Impulse);
        boat = owner.GetComponent<PlayerController>().boat;
        currentState = StartCoroutine(swimState());
        springJoint = GetComponent<SpringJoint>();
        springJoint.connectedBody = owner.GetComponent<Rigidbody>();

    }

    void FixedUpdate()
    {
        timeAlive += Time.fixedDeltaTime;
        if (timeAlive > swimTime)
        {
            controlable = false;
        }
    }

    void airTurn()
    {
        int direction = -1;
        if (transform.eulerAngles.z > 90 && transform.eulerAngles.z < 270)
        {
            direction = 1;
        }
        rb.AddTorque(direction * transform.forward * airTurnForce, ForceMode.Force);
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
            GetComponent<Collider>().enabled = false;
            rb.drag = emptyPullDrag;
            StopCoroutine(currentState);
            currentState = StartCoroutine(pullState());
        }
    }

    IEnumerator pullState()
    {
        if (Vector3.Distance(owner.position, transform.position) < pickupDistance)
        {
            if (GetComponent<FixedJoint>() != null) {
                Destroy(GetComponent<FixedJoint>().connectedBody.gameObject);
                boat.GetComponent<Cargo>().addCargo("Fish", GetComponent<FixedJoint>().connectedBody.GetComponent<Rigidbody>().mass);
            }
            Destroy(gameObject);
            owner.GetComponent<PlayerController>().SetCameraTarget(owner);
        }
        rb.AddForceAtPosition((owner.position - transform.position).normalized * pullForce, pullPoint.position, ForceMode.Acceleration);
        yield return new WaitForFixedUpdate();
        currentState = StartCoroutine(pullState());
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Fish") {
            StopCoroutine(currentState);
            currentState = StartCoroutine(pullState());

            //Attach spear
            joint = gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = other.GetComponent<Rigidbody>();
            springJoint.maxDistance = Vector3.Distance(springJoint.connectedBody.transform.position, transform.position);

            other.GetComponent<FishHealth>().dealDamage(damage);
            controlable = false;
            GetComponent<Collider>().enabled = false;
        }
    }
}

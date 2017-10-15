using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Control { Walking, Sailing, Fishing };

public class PlayerController : MonoBehaviour {

    Rigidbody characterRb;
    Rigidbody boatRb;
    Rigidbody spearRb;
    Control currentControl = Control.Walking;
    SpearScript ss;
    CameraScript cs;

    public float walkForce;
    public float rowForce;
    public float spearTurnForce;
    public Transform boat;
    public Transform rowPoint;
    public GameObject spear;
    public Transform steerSeat;
    public Transform fishSeat;
    public float interactionDistance;
    public GameObject mainCamera;

    // Use this for initialization
    void Start () {
        characterRb = GetComponent<Rigidbody>();
        boatRb = boat.GetComponent<Rigidbody>();
        spearRb = spear.GetComponent<Rigidbody>();
        ss = spear.GetComponent<SpearScript>();
        cs = mainCamera.GetComponent<CameraScript>();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        switch (currentControl)
        {
            case Control.Walking:
                horizontalWalk();
                SetCameraTarget(transform);
                break;
            case Control.Sailing:
                horizontalSail();
                SetCameraTarget(boat);
                break;
            case Control.Fishing:
                mainMouseButton();
                spearTurn();
                //camera target is set on throw
                break;
        }
        interact();
    }

    void horizontalWalk()
    {
        float horizontal = Input.GetAxis("Horizontal");
        characterRb.velocity = new Vector3(horizontal * walkForce, characterRb.velocity.y, 0);
    }

    void horizontalSail()
    {
        float horizontal = Input.GetAxis("Horizontal");
        boatRb.AddForceAtPosition(new Vector3(horizontal * rowForce,0,0), rowPoint.position, ForceMode.Force);
    }

    void spearTurn()
    {
        float horizontal = Input.GetAxis("Horizontal");
        if (ss.timeAlive < ss.swimTime) {
            spearRb.AddTorque(-spearRb.transform.forward * spearTurnForce * horizontal, ForceMode.Force);
        }
    }

    void interact()
    {
        if (Input.GetKeyDown("e"))
        {
            if (currentControl == Control.Walking && Vector3.Distance(transform.position, steerSeat.position) <= interactionDistance) {
                currentControl = Control.Sailing;
                transform.position = steerSeat.position;
            }
            else if (currentControl == Control.Walking && Vector3.Distance(transform.position, fishSeat.position) <= interactionDistance)
            {
                currentControl = Control.Fishing;
                transform.position = fishSeat.position;
            }
            else
            {
                currentControl = Control.Walking;
            }
        }
    }

    void mainMouseButton()
    {
        if (currentControl == Control.Fishing && Input.GetMouseButtonDown(0))
        {
            throwSpear();
        }
    }

    public void SetCameraTarget(Transform target)
    {
        cs.target = target;
    }

    void throwSpear()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 mousePosition = mouseRay.origin + (mouseRay.direction * -Camera.main.transform.position.z);
        float angle = Vector3.Angle(transform.right, new Vector3(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y, 0));//Vector3.Angle(transform.position, mousePosition);
        if (mousePosition.y < 0)
        {
            angle = -angle;
        }
        GameObject g = Instantiate(spear, fishSeat.position, Quaternion.AngleAxis(angle, Vector3.forward));
        spearRb = g.GetComponent<Rigidbody>();
        ss = g.GetComponent<SpearScript>();
        ss.owner = transform;
        SetCameraTarget(g.transform);
    }
}

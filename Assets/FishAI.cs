﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishAI : MonoBehaviour {

    public enum states { idle, dead, fleeing, scared };
    public Transform boat;

    public float idleVelocity;
    public float fleeForce;
    public float scareVelocity;
    public float destinationReachDistance;
    public float maxHeight;
    public float turnForce;
    public float fleeingTurnForce;

    [Range(0, 1)]
    public float turnChance;

    [Header("Idle")]
    public float minIdleX;
    public float maxIdleX;
    public float maxIdleY;

    [Header("Fleeing")]
    public float minFleeingX;
    public float maxFleeingX;
    public float maxFleeingYUp;
    public float maxFleeingYDown;

    [Header("Scared")]
    public float minScaredX;
    public float maxScaredX;
    public float maxScaredY;

    Coroutine currentRoutine;
    Rigidbody rb;

    public Vector3 destination = Vector3.zero;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        changeState(states.idle);
	}


    Vector3 newIdleDestination()
    {
        float x = Random.Range(minIdleX, maxIdleX);
        float y = Random.Range(-maxIdleY, maxIdleY);

        //Determine direction
        int direction = 1;
        if (transform.forward.x < 0)
        {
            direction = -1;
        }
        if (Random.value < turnChance)
        {
            direction *= -1;
        }

        //validate
        if (!validDestination(x, y))
        {
            return newIdleDestination();
        }
        return new Vector3(transform.position.x + x * direction, transform.position.y + y, 0);
    }

    Vector3 newFleeingDestination()
    {
        float x = Random.Range(minFleeingX, maxFleeingX);
        float y = Random.Range(-maxFleeingYDown, maxFleeingYUp);

        //Determine direction
        int direction = 1;
        if (boat.position.x > transform.position.x)
        {
            direction = -1;
        }

        //validate
        if (!validDestination(x, y))
        {
            return newFleeingDestination();
        }
        return new Vector3(transform.position.x + x * direction, transform.position.y + y, 0);
    }

    Vector3 newScaredDestination()
    {
        float x = Random.Range(minScaredX, maxScaredX);
        float y = Random.Range(-maxScaredY, maxScaredY);

        //Determine direction
        int direction = 1;
        if (boat.position.x > transform.position.x) //Swim away from player
        {
            direction = -1;
        }

        //validate
        if (!validDestination(x, y))
        {
            return newScaredDestination();
        }
        return new Vector3(transform.position.x + x * direction, transform.position.y + y, 0);
    }

    bool validDestination(float x, float y)
    {
        if (transform.position.y + y >= maxHeight || 
            Physics.Linecast(boat.position, new Vector3(transform.position.x + x, transform.position.y + y, 0), LayerMask.GetMask("Terrain")))
        {
            return false;
        }
        return true;
    }

    public void changeState(states newState)
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }
        switch (newState)
        {
            case states.idle:
                print("Idle");
                keepDirection();
                currentRoutine = StartCoroutine(idle());
                break;
            case states.fleeing:
                print("Fleeing");
                currentRoutine = StartCoroutine(fleeing());
                break;
            case states.dead:
                print("Dead");
                currentRoutine = StartCoroutine(dead());
                break;
            case states.scared:
                print("Scared");
                destination = newScaredDestination();
                Debug.DrawLine(transform.position, destination, Color.yellow, 60);
                keepDirection();
                currentRoutine = StartCoroutine(scare());
                break;
        }
    }

    /// <summary>
    /// Swim around peacefully.
    /// If no destination is set, generate new destination.
    /// </summary>
    /// <returns></returns>
    IEnumerator idle()
    {
        //Check if destination is reached
        if (destination == Vector3.zero || Vector3.Distance(transform.position, destination) <= destinationReachDistance)
        {
            destination = newIdleDestination();
            keepDirection();
            Debug.DrawLine(transform.position, destination, Color.white, 60);
        }

        //Turn to destination
        if (transform.TransformDirection(transform.forward).normalized.y < (destination - transform.position).normalized.y)
        {
            rb.AddTorque(-transform.right * turnForce);
        }
        else {
            rb.AddTorque(transform.right * turnForce);
        }

        if (transform.position.y < -0.5)
        {
            //Move towards destination
            rb.velocity = transform.forward * idleVelocity;
        }

        //Repeat
        yield return new WaitForFixedUpdate();
        StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(idle());
    }

    /// <summary>
    /// Sets destination far away
    /// Swim at full speed away
    /// </summary>
    /// <returns></returns>
    IEnumerator fleeing()
    {
        //Check if destination is reached
        if (destination == Vector3.zero || Vector3.Distance(transform.position, destination) <= destinationReachDistance)
        {
            destination = newFleeingDestination();
            Debug.DrawLine(transform.position, destination, Color.red, 60);
        }

        //Turn to destination
        if (transform.TransformDirection(transform.forward).normalized.y < (destination - transform.position).normalized.y)
        {
            rb.AddTorque(-transform.right * fleeingTurnForce);
        }
        else
        {
            rb.AddTorque(transform.right * fleeingTurnForce);
        }

        if (transform.position.y < -0.5) {
            //Move towards destination
            rb.AddForce(transform.forward * fleeForce);
        }

        //Repeat
        yield return new WaitForFixedUpdate();
        StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(fleeing());
    }

    /// <summary>
    /// Quickly swim a short distance in opposite direction
    /// </summary>
    /// <returns></returns>
    IEnumerator scare()
    {
        //Check if destination is reached
        if (destination == Vector3.zero || Vector3.Distance(transform.position, destination) <= destinationReachDistance)
        {
            StopCoroutine(currentRoutine);
            currentRoutine = StartCoroutine(idle());
            yield return null;
        }

        //Turn to destination
        if (transform.TransformDirection(transform.forward).normalized.y < (destination - transform.position).normalized.y)
        {
            rb.AddTorque(-transform.right * turnForce);
        }
        else
        {
            rb.AddTorque(transform.right * turnForce);
        }

        if (transform.position.y < -0.5)
        {
            //Move towards destination
            rb.velocity = transform.forward * scareVelocity;
        }

        //Repeat
        yield return new WaitForFixedUpdate();
        StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(scare());
    }

    /// <summary>
    /// Float to the surface
    /// </summary>
    /// <returns></returns>
    IEnumerator dead()
    {
        rb.useGravity = true;
        GetComponent<WaterFloat>().enabled = true;
        yield return null;
    }

    /// <summary>
    /// If fish is looking away from destination
    /// DO NOT USE WHILE FLEEING, SPEAR WILL NOT FLIP
    /// </summary>
    void keepDirection()
    {
        if (Vector3.Dot(transform.forward, (destination - transform.position).normalized) < 0)
        {
            transform.Rotate(Vector3.up, 180);
        }
    }
}

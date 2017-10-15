using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishScript : MonoBehaviour {

    public float swimSpeed;
    public float fleeForce;
    public float health;
    public float bleedDps;

    Coroutine currentRoutine;
    Rigidbody rb;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        currentRoutine = StartCoroutine(idle());
	}

    public void dealDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            FishManager.removeFish(gameObject);
            StopCoroutine(currentRoutine);
            currentRoutine = StartCoroutine(dead());
        }
        else
        {
            StartCoroutine(bleed());
            StopCoroutine(currentRoutine);
            currentRoutine = StartCoroutine(fleeing());
        }
    }

    IEnumerator idle()
    {
        //transform.Translate(Vector3.up * swimspeed * Time.deltaTime);
        rb.velocity = new Vector3(swimSpeed * transform.up.x, rb.velocity.y, 0);
        yield return new WaitForFixedUpdate();
        currentRoutine = StartCoroutine(idle());
    }

    IEnumerator dead()
    {
        print("dead");
        //Float upwards
        //Floaters from boat here?
        rb.useGravity = true;
        GetComponent<WaterFloat>().enabled = true;
        yield return null;
    }

    IEnumerator fleeing()
    {
        //flee
        print("flee");
        rb.AddForce(new Vector3(fleeForce, 0, 0), ForceMode.Impulse);
        yield return new WaitForFixedUpdate();
        currentRoutine = StartCoroutine(fleeing());
    }

    IEnumerator bleed()
    {
        yield return new WaitForSeconds(1);
        print("bleed damage");
        dealDamage(bleedDps);
    }
}

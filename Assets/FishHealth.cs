using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishHealth : MonoBehaviour {

    public int minHealth;
    public int maxHealth;
    public float health;
    public float bleedDps;

    FishAI ai;
    bool bleeding = false;

    void Start()
    {
        health = Random.Range(minHealth, maxHealth);
        ai = GetComponent<FishAI>();
    }

    public void dealDamage(float damage)
    {
        health -= damage;
        checkDeath();
        if (!bleeding) {
            bleeding = true;
            StartCoroutine(bleed());
            ai.changeState(FishAI.states.fleeing);
        }
    }

    void checkDeath()
    {
        if (health <= 0)
        {
            FishManager.removeFish(gameObject);
            ai.changeState(FishAI.states.dead);
        }
    }
    
    IEnumerator bleed()
    {
        yield return new WaitForSeconds(1);
        dealDamage(bleedDps);
        yield return bleed();
    }
}

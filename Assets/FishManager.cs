using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishManager : MonoBehaviour {

    public List<GameObject> fishTypes;
    public int fishCountTarget;
    public int startFishCount;
    public float spawnSleepTime;

    public float minDepth, maxDepth;
    public float minDistance, maxDistance;

    public Transform camera;

    static int activeFish = 0;

	// Use this for initialization
	void Start () {
        for (int i = 0; i < startFishCount; i++)
        {
            spawnFish();
        }
        StartCoroutine(spawnLoop());
	}

    void spawnFish()
    {
        float depth = Random.Range(minDepth, maxDepth);
        int side = 1;
        if(Random.value > 0.5f)
        {
            side = -1;
        }
        float distance = Random.Range(minDistance, maxDistance);
        int direction = -90;
        if (side > 0)
        {
            direction = 90;
        }

        activeFish++;
        Instantiate(fishTypes[Random.Range(0, fishTypes.Count - 1)], new Vector3(side * distance, depth, 0), Quaternion.AngleAxis(direction, Vector3.forward));
    }

    IEnumerator spawnLoop()
    {
        if (activeFish < fishCountTarget)
        {
            spawnFish();
        }
        yield return new WaitForSeconds(spawnSleepTime);
        StartCoroutine(spawnLoop());
    }

    public static void removeFish(GameObject g)
    {
        activeFish--;
    }
}

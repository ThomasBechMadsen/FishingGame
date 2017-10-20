using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishManager : MonoBehaviour {

    public List<GameObject> fishTypes;
    public int fishCountTarget;
    public int startFishCount;
    public float spawnSleepTime;

    public Transform upperLeftBoundary, bottomRightBoundary;

    public Transform camera;
    public bool ignoreCamera;
    public Transform boat;

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
        float xPos;
        float yPos;
        xPos = Random.Range(upperLeftBoundary.position.x, bottomRightBoundary.position.x);
        yPos = Random.Range(bottomRightBoundary.position.y, upperLeftBoundary.position.y);

        if (!validSpawnPoint(xPos, yPos))
        {
            //print("blocked!");
            return;
        }

        activeFish++;
        GameObject g = Instantiate(fishTypes[Random.Range(0, fishTypes.Count - 1)], new Vector3(xPos, yPos, 0), Quaternion.Euler(0, 90, 0));
        g.GetComponent<FishAI>().boat = boat;
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

    bool validSpawnPoint(float x, float y) //Move to fishAI;
    {
        //Check if fish is underground
        bool underground = Physics.Linecast(boat.position, new Vector3(x, y, 0), LayerMask.GetMask("Terrain"));
        //Check if player can see the fish when spawned
        Vector3 viewPortPosition = camera.GetComponent<Camera>().WorldToViewportPoint(new Vector3(x, y, 0));
        bool visible = true;
        if (viewPortPosition.x > 1 || viewPortPosition.x < 0 || viewPortPosition.y > 1 || viewPortPosition.y < 0)
        {
            //print("outside view x: " + viewPortPosition.x + ", y: " + viewPortPosition.y);
            visible = false;
        }
        if (ignoreCamera)
        {
            visible = false;
        }

        return !(underground || visible);
    }
}

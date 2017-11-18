using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cargo : MonoBehaviour {

    public Rigidbody rigidBody;
    [Range(0, 1)]
    public float massReduction;
    public List<FishCargo> cargo = new List<FishCargo>();
    float baseWeight;
    float cargoWeight;

    void Start()
    {
        baseWeight = rigidBody.mass;
    }

    public void addCargo(string name, float weight)
    {
        cargo.Add(new FishCargo(name, weight));
        calculateWeight();
    }

    public void removeCargo(string name)
    {
        for (int i = 0; i < cargo.Count; i++)
        {
            if (cargo[i].name == name)
            {
                cargo.RemoveAt(i);
                break;
            }
        }
        calculateWeight();
    }

    void calculateWeight()
    {
        float cargoWeight = 0;
        foreach(FishCargo entry in cargo)
        {
            cargoWeight += entry.weight;
        }
        rigidBody.mass = baseWeight + (cargoWeight * massReduction);
    }
}

public struct FishCargo
{
    public string name;
    public float weight;

    public FishCargo(string name, float weight)
    {
        this.name = name;
        this.weight = weight;
    }
}
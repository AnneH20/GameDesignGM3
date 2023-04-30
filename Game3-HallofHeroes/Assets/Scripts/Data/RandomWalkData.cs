using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RandomWalkData_", menuName = "ScriptableObjects/RandomWalkData", order = 1)]

public class RandomWalkData : ScriptableObject
{
    public int walklen = 10;
    public int iterations = 10;
    public bool useRandomSeed = true;
}

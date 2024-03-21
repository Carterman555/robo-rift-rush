using System;
using UnityEngine;

/// <summary>
/// create a float that you can randomize between two floats
/// </summary>
[Serializable]
public class RandomFloat
{
    public float AvgValue;
    public float ValueVariance;
    public float CurrentValue { get; private set; }

    public float RandomizeCurrent()
    {
        CurrentValue = AvgValue + UnityEngine.Random.Range(-ValueVariance, ValueVariance);
        return CurrentValue;
    }
}
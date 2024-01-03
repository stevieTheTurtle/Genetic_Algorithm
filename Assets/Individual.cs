using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

public abstract class Individual : MonoBehaviour
{
    public DNA genotype;
    public float Fitness { get; private set; }

    protected void Start()
    {
        Fitness = 0f;
    }

    public abstract void SetGenotype(DNA newGenotype);

    public void SetFitness(float fitness)
    {
        this.Fitness = fitness;
    }
}

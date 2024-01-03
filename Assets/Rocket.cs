using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RocketIndividual : Individual
{
    private Rigidbody rbody;
    
    private void Start()
    {
        base.Start();
        rbody = GetComponent<Rigidbody>();
    }

    public override void SetGenotype(DNA newGenotype)
    {
        genotype = newGenotype;
    }

    public void AddForce(Vector3 force)
    {
        rbody.AddForce(force, ForceMode.Force);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RocketIndividual : MonoBehaviour
{
    [SerializeField] private float mass;
    [SerializeField] private float genotypeLength;

    public DNA genotype;
    public float Fitness { get; private set; }
    private int genotypeIndex;
    
    
    [Header("DEBUG")] 
    [SerializeField] private Vector3 acceleration;
    [SerializeField] private Vector3 velocity;
    
    public void SetFitness(float fitness)
    {
        this.Fitness = fitness;
    }

    public void Reset()
    {
        Fitness = 0f;
        genotypeIndex = 0;

        this.transform.position = Vector3.zero;
        this.transform.rotation = Quaternion.identity;
        
        acceleration = Vector3.zero;
        velocity = Vector3.zero;
    }
    
    private void Start()
    {
        Reset();
        SetGenotype(new DNA(FindObjectOfType<RocketSimulationManager>().generationEvolutionFixedFrames));
    }

    private void Update()
    {
        this.transform.forward = velocity.normalized;
    }

    private void FixedUpdate()
    {
        if (genotypeIndex >= genotype.genes.Length)
        {
            throw new Exception("Genes array out of bound!");
            Debug.LogError("Genes array out of bound!");
            return;
        }
        
        Vector3 phenotypeForce = genotype.genes[genotypeIndex];
        AddForce(phenotypeForce);
        genotypeIndex++;
        
        this.velocity += acceleration * Time.fixedDeltaTime;
        this.transform.position += this.velocity * Time.fixedDeltaTime;
    }

    public void SetGenotype(DNA newGenotype)
    {
        genotype = newGenotype;
    }

    public void AddForce(Vector3 force)
    {
        acceleration += force / mass;
    }
}

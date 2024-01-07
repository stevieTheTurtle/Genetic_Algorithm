using System;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[Serializable]
public class DNA
{
    public float maxForce = 30f;
    public Vector3[] genes;

    private Random _random;

    private Vector3 GetRandomGene()
    {
        float x = (_random.NextFloat(0f,1f) - 0.5f) * 2f;
        float y = (_random.NextFloat(0f,1f) - 0.5f) * 2f;
        float z = (_random.NextFloat(0f,1f) - 0.5f) * 2f;
        Vector3 randomVector = new Vector3(x, y, z);
        randomVector = randomVector.normalized * _random.NextFloat(0f, maxForce);

        return randomVector;
    }

    private void InitRandom()
    {
        _random = new Random((uint) System.DateTime.Now.Ticks);
    }
    
    public DNA(int genotypeLength, bool toRandomize)
    {
        InitRandom();
        
        genes = new Vector3[genotypeLength];

        if (!toRandomize) return;
        for (int i = 0; i < genotypeLength; i++)
            genes[i] = GetRandomGene();
    }
    
    public DNA(Vector3[] genes)
    {
        InitRandom();
        
        this.genes = genes;
    }

    public DNA Crossover(DNA partner)
    {
        DNA child = new DNA(genes.Length, false);
        
        NativeArray<Vector3> thisGenes = new NativeArray<Vector3>(this.genes, Allocator.TempJob); 
        NativeArray<Vector3> partnerGenes = new NativeArray<Vector3>(partner.genes, Allocator.TempJob);
        NativeArray<Vector3> childGenes = new NativeArray<Vector3>(child.genes, Allocator.TempJob);
        
        var parallelJob = new CrossoverJob()
        {
            random = this._random,
            
            thisGenes = thisGenes,
            partnerGenes = partnerGenes,
            childGenes = childGenes
        };
        
        //Scheduling the parallel jobs
        JobHandle jobHandle = parallelJob.Schedule(genes.Length, 64);
        
        //Wait for jobs to complete
        jobHandle.Complete();
        
        child = new DNA(childGenes.ToArray());
        
        thisGenes.Dispose();
        partnerGenes.Dispose();
        childGenes.Dispose();
        
        return child;
    }
    
    struct CrossoverJob : IJobParallelFor
    {
        public Unity.Mathematics.Random random;
        
        [ReadOnly] public NativeArray<Vector3> thisGenes;
        [ReadOnly] public NativeArray<Vector3> partnerGenes;
        
        public NativeArray<Vector3> childGenes;
        
        public void Execute(int i)
        {
            if (random.NextFloat(0f, 1f) < 0.5f)
                childGenes[i] = thisGenes[i];
            else
                childGenes[i] = partnerGenes[i];
        }
    }

    public void Mutate(float geneMutationRate)
    {
        for (int i = 0; i < genes.Length; i++)
        {
            if (_random.NextFloat(0f, 1f) < geneMutationRate)
                genes[i] = GetRandomGene();
        }
    }
}

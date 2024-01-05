using System.Security.Cryptography;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class DNA
{
    public float maxForce = 30f;
    public Vector3[] genes;

    private Vector3 GetRandomGene()
    {
        System.Random rnd = new System.Random();
        
        float x = ((float) rnd.NextDouble() - 0.5f) * 2f;
        float y = ((float) rnd.NextDouble() - 0.5f) * 2f;
        float z = ((float) rnd.NextDouble() - 0.5f) * 2f;
        Vector3 randomVector = new Vector3(x, y, z);
        randomVector = randomVector.normalized * Random.Range(0f, maxForce);

        return randomVector;
    }
    
    public DNA(int genotypeLength)
    {
        genes = new Vector3[genotypeLength];

        for (int i = 0; i < genotypeLength; i++)
            genes[i] = GetRandomGene();
    }
    
    public DNA(Vector3[] genes)
    {
        this.genes = genes;
    }

    public DNA Crossover(DNA partner)
    {
        DNA child = new DNA(genes.Length);
        
        //CoinFlip Crossover
        for (int i = 0; i < genes.Length; i++)
        {
            NativeArray<Vector3> thisGenes = new NativeArray<Vector3>(this.genes, Allocator.TempJob); 
            NativeArray<Vector3> partnerGenes = new NativeArray<Vector3>(partner.genes, Allocator.TempJob);
            NativeArray<Vector3> childGenes = new NativeArray<Vector3>(child.genes, Allocator.TempJob);
            
            var job = new CrossoverJob()
            {
                thisGenes = thisGenes,
                partnerGenes = partnerGenes,
                childGenes = childGenes
            };
            
            //CHECK IF THIS HAS DIRECTLY TO DO WITH FOR LOOP
            JobHandle jobHandle = job.Schedule(position.Length, 64);
            
            child = new DNA(childGenes.ToArray());
            
            thisGenes.Dispose();
            partnerGenes.Dispose();
            childGenes.Dispose();
        }
        //CHECK FOR EVERY JOB TO COMPLETE!
        jobHandle.Complete();
        
        return child;
    }
    
    struct CrossoverJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<Vector3> thisGenes;
        [ReadOnly] public NativeArray<Vector3> partnerGenes;
        
        public NativeArray<Vector3> childGenes;
        
        public void Execute(int i)
        {
            if (Random.Range(0f, 1f) < 0.5f)
                childGenes = thisGenes;
            else
                childGenes = partnerGenes;
        }
    }

    public void Mutate(float geneMutationRate)
    {
        for (int i = 0; i < genes.Length; i++)
        {
            if (Random.Range(0f, 1f) < geneMutationRate)
                genes[i] = GetRandomGene();
        }
    }
}

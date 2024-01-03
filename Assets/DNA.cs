using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[System.Serializable]
public class DNA
{
    public Gene[] genes;
    
    public DNA(int genotypeLength)
    {
        genes = new Gene[genotypeLength];
        
        for (int i = 0; i < genotypeLength; i++)
        {
            genes[i] = new Gene();
        }
    }

    public DNA Crossover(DNA partner)
    {
        DNA child = new DNA(genes.Length);

        //CoinFlip Crossover
        for (int i = 0; i < genes.Length; i++)
        {
            if (Random.Range(0f, 1f) < 0.5f)
                child.genes[i] = this.genes[i];
            else
                child.genes[i] = partner.genes[i];
        }
        
        return child;
    }

    public void Mutate(float geneMutationRate)
    {
        for (int i = 0; i < genes.Length; i++)
        {
            if (Random.Range(0f, 1f) < geneMutationRate)
                genes[i] = new Gene();
        }
    }
}

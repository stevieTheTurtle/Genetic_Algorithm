using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class DNA
{
    private const int GenLen = 18;
    private float _geneMutationRate = 0.01f;
    public Gene[] Genes;
    public float Fitness { get; private set; }
    
    public DNA()
    {
        Genes = new Gene[GenLen];
        Fitness = 0f;
        
        for (int i = 0; i < Genes.Length; i++)
        {
            Genes[i] = new Gene();
        }
    }

    public DNA(string s)
    {
        if (s.Length != GenLen)
        {
            Debug.LogError("target.stringLength !=18 !");
            return;
        }

        Genes = new Gene[GenLen];
        Fitness = 0f;
        
        for (int i = 0; i < Genes.Length; i++)
        {
            Genes[i] = new Gene();
            Genes[i].Allele = s[i];
        }
    }

    public void CalculateFitness(DNA target)
    {
        int score = 0;
        for (int i = 0; i < Genes.Length; i++)
        {
            if (Genes[i].Equals(target.Genes[i])) 
                score++;
        }

        Fitness = ((float) score) / ((float) Genes.Length);
    }

    public DNA Crossover(DNA partner)
    {
        DNA child = new DNA();

        /* //Midpoint Crossover
        int midpoint = Random.Range(0, genes.Length-1);
        for (int i = 0; i < genes.Length; i++)
        {
            if (i > midpoint)
                child.genes[i] = this.genes[i];
            else
                child.genes[i] = partner.genes[i];
        } */

        //CoinFlip Crossover
        for (int i = 0; i < Genes.Length; i++)
        {
            if (Random.Range(0f, 1f) < 0.5f)
                child.Genes[i] = this.Genes[i];
            else
                child.Genes[i] = partner.Genes[i];
        }
        
        return child;
    }

    public void Mutate()
    {
        for (int i = 0; i < Genes.Length; i++)
        {
            if (Random.Range(0f, 1f) < _geneMutationRate)
                Genes[i] = new Gene();
        }
    }
}

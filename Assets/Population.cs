using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Population
{
    public int populationSize;
    public float averageFitness { get; private set; }
    public DNA bestIndividual { get; private set; }
    public int generationNumber { get; private set; }
    public List<DNA> individuals = new List<DNA>();

    public Population(int populationSize)
    {
        this.populationSize = populationSize;
        averageFitness = 0f;
        generationNumber = 0;
        bestIndividual = null;
        
        for (int i = 0; i < this.populationSize; i++)
        {
            individuals.Add(new DNA());
        }
    }

    public void Selection()
    {
        //Construct the mating pool, giving more probability to higher fitness individual to reproduce
        List<DNA> matingPool = new List<DNA>();
        
        for (int i = 0; i < populationSize; i++)
        {
            int n = (int) (individuals[i].Fitness * 100f);
            for (int j = 0; j < n; j++)
            {
                matingPool.Add(individuals[i]);
            }
        }

        if (matingPool.Count == 0)
        {
            Debug.LogError("MatingPool count is zero!");
            return;
        }
        
        //Make next generation using crossover
        List<DNA> newPopulation = new List<DNA>();

        for (int i = 0; i < populationSize; i++)
        {
            int a = 0;
            int b = 0;
            while (a == b)
            {
                a = Random.Range(0, matingPool.Count);
                b = Random.Range(0, matingPool.Count);
            }

            DNA parentA = matingPool[a];
            DNA parentB = matingPool[b];
            
            DNA child = parentA.Crossover(parentB);
            child.Mutate();
            newPopulation.Add(child);
        }
        
        individuals = newPopulation;

        generationNumber++;
    }

    public float AverageFitness()
    {
        float averageFitness = 0f;
        
        for (int i = 0; i < populationSize; i++)
        {
            averageFitness += individuals[i].Fitness;
        }
        averageFitness /= populationSize;

        this.averageFitness = averageFitness;
        
        return averageFitness;
    }

    public DNA BestIndividual()
    {
        DNA bestIndividual = individuals[0];
        
        for (int i = 1; i < populationSize; i++)
            if (bestIndividual.Fitness < individuals[i].Fitness)
                bestIndividual = individuals[i];

        return bestIndividual;
    }
    
}

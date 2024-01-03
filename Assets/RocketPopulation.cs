using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class RocketPopulation : MonoBehaviour
{
    public int populationSize;
    public float averageFitness;
    public RocketIndividual bestRocket;
    public int generationNumber;
    public List<RocketIndividual> individuals;
    
    private void Start()
    {
        populationSize = 0;
        averageFitness = 0f;
        generationNumber = 0;
        bestRocket = null;
    }

    public void Reset()
    {
        Start();
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

    public Individual BestIndividual()
    {
        Individual bestIndividual = individuals[0];
        
        for (int i = 1; i < populationSize; i++)
            if (bestIndividual.Fitness < individuals[i].Fitness)
                bestIndividual = individuals[i];

        return bestIndividual;
    }
    
    public void SpawnPopulation(GameObject individualPrefab, int populationSize)
    {
        individuals = new List<RocketIndividual>();
        this.populationSize = populationSize;
        for (int i = 0; i < this.populationSize; i++)
        {
            SpawnIndividual(individualPrefab);
        }
    }
    
    /*
    public void SpawnPopulation(GameObject individualPrefab, DNA[] populationGenotypes)
    {
        individuals = new List<RocketIndividual>();
        for (int i = 0; i < populationGenotypes.Length; i++)
        {
            SpawnIndividual(individualPrefab, populationGenotypes[i]);
        }
    }
    */

    public RocketIndividual SpawnIndividual(GameObject individualPrefab)
    {
        GameObject individualObj = Instantiate(individualPrefab, Vector3.zero, Quaternion.identity);
        individualObj.transform.parent = this.transform;
        
        RocketIndividual individual = individualObj.GetComponent<RocketIndividual>();
        individuals.Add(individual);
        
        return individual;
    }
    
    public RocketIndividual SpawnIndividual(GameObject individualPrefab, DNA genotype)
    {
        RocketIndividual individual = SpawnIndividual(individualPrefab);
        individual.genotype = genotype;

        return individual;
    }
}

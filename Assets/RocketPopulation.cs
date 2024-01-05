using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class RocketPopulation : MonoBehaviour
{
    public int populationSize;
    public float averageFitness;
    public RocketIndividual bestRocket;
    public List<RocketIndividual> individuals;
    
    private void Start()
    {
        populationSize = FindObjectOfType<RocketSimulationManager>().GetPopulationSize();
        averageFitness = 0f;
        bestRocket = null;
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

    public RocketIndividual BestIndividual()
    {
        RocketIndividual bestIndividual = individuals[0];
        
        for (int i = 1; i < populationSize; i++)
            if (bestIndividual.Fitness < individuals[i].Fitness)
                bestIndividual = individuals[i];

        return bestIndividual;
    }
    
    public void SpawnPopulationRandom(GameObject individualPrefab, int populationSize)
    {
        
        individuals = new List<RocketIndividual>();
        this.populationSize = populationSize;
        
        for (int i = 0; i < this.populationSize; i++)
        {
            SpawnIndividual(individualPrefab);
        }
        
    }

    public RocketIndividual SpawnIndividual(GameObject individualPrefab)
    {
        GameObject individualObj = Instantiate(individualPrefab, Vector3.zero, Quaternion.identity);
        individualObj.transform.parent = this.transform;
        individualObj.name = "Rocket #" +individuals.Count;
        
        RocketIndividual individual = individualObj.GetComponent<RocketIndividual>();
        individual.SetGenotype(new DNA(FindObjectOfType<RocketSimulationManager>().generationEvolutionFixedFrames, true));
        individuals.Add(individual);
        
        return individual;
    }

    public void ChangeIndividualGenotype(RocketIndividual rocketIndividual,DNA newGenotype)
    {
        rocketIndividual.SetGenotype(newGenotype);
    }
}

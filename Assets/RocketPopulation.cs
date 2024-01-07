using System;
using System.Collections.Generic;
using System.IO;
using Leguar.TotalJSON;
using UnityEngine;

[Serializable]
public struct GenerationSave
{
    [SerializeField] int simulationStepsPerGeneration;
    [SerializeField] int generation; 
    [SerializeField] List<RocketIndividual> bestIndividuals;
    
    public GenerationSave(int simStep, int gen, List<RocketIndividual> individuals)
    {
        simulationStepsPerGeneration = simStep;
        generation = gen;
        bestIndividuals = individuals;
    }
}

[Serializable]
public class RocketPopulation : MonoBehaviour
{
    public int populationSize;
    public List<RocketIndividual> individuals;
    
    private void Start()
    {
        populationSize = FindObjectOfType<RocketSimulationManager>().GetPopulationSize();
    }

    public float AverageFitness()
    {
        float averageFitness = 0f;
        
        for (int i = 0; i < populationSize; i++)
        {
            averageFitness += individuals[i].GetFitness();
        }
        averageFitness /= populationSize;
        
        return averageFitness;
    }

    public List<RocketIndividual> BestIndividuals(int n)
    {
        List<RocketIndividual> sortedIndividuals = new List<RocketIndividual>(individuals);
        sortedIndividuals.Sort((x, y) => y.GetFitness().CompareTo(x.GetFitness()));

        return sortedIndividuals.GetRange(0,n);
    }
    
    public void SpawnPopulationRandom(GameObject individualPrefab, int populationSize)
    {
        
        individuals = new List<RocketIndividual>();
        this.populationSize = populationSize;

        RocketIndividual individual;
        
        for (int i = 0; i < this.populationSize; i++)
        {
            individual = SpawnIndividual(individualPrefab);
            individual.SetId(i);
        }
        
    }

    public RocketIndividual SpawnIndividual(GameObject individualPrefab)
    {
        GameObject individualObj = Instantiate(individualPrefab, Vector3.zero, Quaternion.identity);
        individualObj.transform.parent = this.transform;
        individualObj.name = "Rocket #" +individuals.Count;
        
        RocketIndividual individual = individualObj.GetComponent<RocketIndividual>();
        individual.SetGenotype(new DNA(FindObjectOfType<RocketSimulationManager>().simulationStepsPerGeneration, true));
        individuals.Add(individual);
        
        return individual;
    }

    public void ChangeIndividualGenotype(RocketIndividual rocketIndividual,DNA newGenotype)
    {
        rocketIndividual.SetGenotype(newGenotype);
    }
    
    public void SaveGenerationToFile(int simulationSteps, int generationNumber, int nBest)
    {
        List<RocketIndividual> bestIndividuals = BestIndividuals(nBest);
        
        GenerationSave genStruct = new GenerationSave(simulationSteps, generationNumber, bestIndividuals);
        
        JSON json = JSON.Serialize(genStruct);
        string jsonString = json.CreatePrettyString();
        
        string logPath = Application.dataPath + "/SimulationLogs/"+"Generation_" + generationNumber + ".json";
        StreamWriter writer = File.CreateText(logPath);
        
        writer.Write(jsonString);
        writer.Close();
    }
}

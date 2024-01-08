using System;
using System.Collections.Generic;
using System.IO;
using Leguar.TotalJSON;
using UnityEngine;

[Serializable]
public struct GenerationSave
{
    [SerializeField] public int simulationStepsPerGeneration;
    [SerializeField] public int generation; 
    [SerializeField] public List<RocketIndividualSave> bestIndividualsSave;
    
    public GenerationSave(int simStep, int gen, List<RocketIndividualSave> individualsSave)
    {
        simulationStepsPerGeneration = simStep;
        generation = gen;
        bestIndividualsSave = individualsSave;
    }
}

[Serializable]
public class RocketPopulation : MonoBehaviour
{
    public int populationSize;
    public List<RocketIndividual> individuals;
    
    private void Start()
    {
        //populationSize = FindObjectOfType<RocketSimulationManager>().GetPopulationSize();
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
    
    public void SpawnPopulation(GameObject individualPrefab, int simulationStepsPerGeneration)
    {
        if (individuals.Count == 0)
        {
            Debug.LogError("population.individuals.Count == 0!");
            return;
        }

        this.populationSize = individuals.Count;
        
        RocketIndividual individual;
        for (int i = 0; i < this.populationSize; i++)
        {
            individual = SpawnIndividual(individualPrefab, simulationStepsPerGeneration);
        }
        
    }
    
    public void SpawnPopulationRandom(GameObject individualPrefab, int populationSize, int simulationStepsPerGeneration)
    {
        
        individuals = new List<RocketIndividual>();
        this.populationSize = populationSize;

        RocketIndividual individual;
        
        for (int i = 0; i < this.populationSize; i++)
        {
            individual = SpawnIndividual(individualPrefab, simulationStepsPerGeneration);
            individual.SetId(i);
        }
        
    }

    public RocketIndividual SpawnIndividual(GameObject individualPrefab, int simulationStepsPerGeneration)
    {
        GameObject individualObj = Instantiate(individualPrefab, Vector3.zero, Quaternion.identity);
        individualObj.transform.parent = this.transform;
        individualObj.name = "Rocket #" +individuals.Count;
        
        RocketIndividual individual = individualObj.GetComponent<RocketIndividual>();
        individual.SetGenotype(new DNA(simulationStepsPerGeneration, true));
        individuals.Add(individual);
        
        return individual;
    }

    public void ChangeIndividualGenotype(RocketIndividual rocketIndividual,DNA newGenotype)
    {
        rocketIndividual.SetGenotype(newGenotype);
    }
    
    public void SaveGenerationToFile(int simulationSteps, int generationNumber, int nBest)
    {
        List<RocketIndividualSave> individualSaves = new List<RocketIndividualSave>();
        List<RocketIndividual> bestIndividuals = BestIndividuals(nBest);

        int i = 0;
        foreach (RocketIndividual individual in bestIndividuals)
        {
            individualSaves.Add(new RocketIndividualSave(bestIndividuals[i].GetId(), bestIndividuals[i].GetGenotype()));
            i++;
        }
        
        GenerationSave genStruct = new GenerationSave(simulationSteps, generationNumber, individualSaves);
        
        JSON json = JSON.Serialize(genStruct);
        string jsonString = json.CreatePrettyString();
        
        string logPath = Application.dataPath + "/SimulationLogs/"+"Generation_" + generationNumber + ".json";
        StreamWriter writer = File.CreateText(logPath);
        
        writer.Write(jsonString);
        writer.Close();
    }
    
    public void LoadSpawnGenerationFromFile(GameObject individualPrefab, int generationNumber, out int simulationSteps)
    {
        string logPath = Application.dataPath + "/SimulationLogs/"+"Generation_" + generationNumber + ".json";

        if (!File.Exists(logPath))
        {
            Debug.LogError(logPath+" - does not exists!");
            simulationSteps = -1;
            return;
        }
        
        StreamReader reader = File.OpenText(logPath);
        
        string jsonString = reader.ReadToEnd();
        reader.Close();

        JSON json = JSON.ParseString(jsonString);
        GenerationSave generationSave = json.Deserialize<GenerationSave>();


        generationNumber = generationSave.generation;
        simulationSteps = generationSave.simulationStepsPerGeneration;
        
        individuals = new List<RocketIndividual>();
        foreach (RocketIndividualSave individualSave in generationSave.bestIndividualsSave)
        {
            RocketIndividual individual = SpawnIndividual(individualPrefab, simulationSteps);
            individual.SetId(individualSave.ID);
            individual.SetGenotype(individualSave.Genotype);
        }
    }
    
}

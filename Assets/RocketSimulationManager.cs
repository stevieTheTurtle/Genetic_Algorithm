using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class RocketSimulationManager : MonoBehaviour
{
    [Header("Simulation Parameters")]
    
    [SerializeField]
    private int populationSize = 1000;
    [SerializeField]
    private int genotypeGenesNumber = 100;
    [SerializeField]
    private float mutationRate = 0.01f;
    [SerializeField]
    private float generationEvolutionMillisec = 1000;
    
    [Header("Simulation Environment")]
    [SerializeField]
    public GameObject rocketIndividualPrefab;
    [SerializeField]
    private Transform rocketTarget;

    [Header("DEBUG")]
    [SerializeField]
    private float averageFitness;
    [SerializeField]
    private Individual bestIndividual;
    [SerializeField]
    private int generationNumber;
    
    private GameObject populationObject;
    
    [SerializeField] 
    private RocketPopulation population;
    [SerializeField]
    private RocketPopulation oldPopulation;
    
    private float timeCounter = 0f;
    
    // Start is called before the first frame update
    void Start()
    {
        populationObject = new GameObject("PopulationManager");
        population = populationObject.AddComponent<RocketPopulation>();
        population.SpawnPopulation(rocketIndividualPrefab, populationSize);
    }

    private void FixedUpdate()
    {
        if (timeCounter >= generationEvolutionMillisec)
        {
            //Fitness calculation of current generation
            CalculateFitness();
            
            //DEBUG
            averageFitness = population.AverageFitness();
            bestIndividual = population.BestIndividual();
            generationNumber = population.generationNumber;
            
            //Making next generation through parent selection and reproduction
            PerformSelection();
            KillOldPopulation();

            timeCounter = 0f;
        }

        timeCounter += Time.fixedDeltaTime * 1000f;
    }
    
    void OnGUI()
    {
        GUIStyle fontSize = new GUIStyle(GUI.skin.GetStyle("label"));
        fontSize.fontSize = 18;
        GUI.Label(new Rect(100, 150, 400, 50), "bestIndividual = " + new string(bestIndividual.genotype.ToString()), fontSize);
        GUI.Label(new Rect(100, 200, 400, 50), "averageFitness = " + averageFitness.ToString(), fontSize);
        GUI.Label(new Rect(100, 250, 400, 50), "generationNumber = " + generationNumber.ToString(), fontSize);
        GUI.Label(new Rect(100, 300, 400, 50), "MillsecUntilNextGeneration = " + (generationEvolutionMillisec - timeCounter).ToString(), fontSize);
    }

    private void CalculateFitness()
    {
        foreach (var individual in population.individuals)
        {
            float d = Vector3.Distance(rocketTarget.transform.position, individual.transform.position);
            individual.SetFitness(Mathf.Pow(1 / d, 2));
        }
    }
    
    public void PerformSelection()
    {
        oldPopulation = population;
        
        //Construct the mating pool, giving more probability to higher fitness individual to reproduce
        List<RocketIndividual> matingPool = new List<RocketIndividual>();
        
        for (int i = 0; i < populationSize; i++)
        {
            int n = (int) (population.individuals[i].Fitness * 100f);
            for (int j = 0; j < n; j++)
            {
                matingPool.Add(population.individuals[i]);
            }
        }

        if (matingPool.Count == 0)
        {
            Debug.LogError("MatingPool count is zero!");
            return;
        }
        
        //Make next generation using crossover
        population.Reset();

        for (int i = 0; i < populationSize; i++)
        {
            int a = 0;
            int b = 0;
            while (a == b)
            {
                a = Random.Range(0, matingPool.Count);
                b = Random.Range(0, matingPool.Count);
            }

            DNA genotypeA = matingPool[a].genotype;
            DNA genotypeB = matingPool[b].genotype;
            
            DNA childGenotype = genotypeA.Crossover(genotypeB);
            
            childGenotype.Mutate(mutationRate);
        }
        
        generationNumber++;
    }

    private void KillOldPopulation()
    {
        foreach (var rocketIndividual in oldPopulation.individuals)
        {
            Destroy(rocketIndividual.gameObject);
        }
        population = null;
    }
}

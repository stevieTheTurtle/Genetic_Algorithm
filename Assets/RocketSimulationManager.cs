using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.Profiling;

public class RocketSimulationManager : MonoBehaviour
{
    [Header("Simulation Parameters")] [SerializeField]
    private int populationSize = 500;
    
    [SerializeField] private float mutationRate = 0.01f;
    [SerializeField] public int generationEvolutionFixedFrames = 1000;
    [SerializeField] public float timeScale = 1;
    
    [Header("Simulation Environment")]
    [SerializeField] public GameObject rocketIndividualPrefab;
    [SerializeField] private Transform rocketTarget;

    [Header("DEBUG")]
    [SerializeField] private float averageFitness;
    [SerializeField] private RocketIndividual bestRocketIndividual;
    [SerializeField] private int generationNumber;
    
    private GameObject populationObject;
    
    [SerializeField] private RocketPopulation population;
    
    private int _frameCounter = 0;

    public int GetPopulationSize()
    {
        return populationSize;
    }
    
    // Start is called before the first frame update
    void Awake()
    {
        Physics.simulationMode = SimulationMode.Script;
        Time.timeScale = timeScale;
        
        populationObject = new GameObject("PopulationManager");
        population = populationObject.AddComponent<RocketPopulation>();
        population.SpawnPopulationRandom(rocketIndividualPrefab, populationSize);
    }

    private void FixedUpdate()
    {
        if (_frameCounter >= generationEvolutionFixedFrames)
        {
            //Fitness calculation of current generation
            Profiler.BeginSample("CalculateFitness");
            CalculateFitness();
            Profiler.EndSample();
            
            //DEBUG
            averageFitness = population.AverageFitness();
            bestRocketIndividual = population.BestIndividual();
            
            //Making next generation through parent selection and reproduction
            Profiler.BeginSample("SelectMatingPool()");
            List<RocketIndividual> matingPool = SelectMatingPool();
            Profiler.EndSample();
            
            Profiler.BeginSample("PerformReproduction()");
            PerformReproduction(matingPool);
            Profiler.EndSample();
            
            Profiler.BeginSample("ResetPopulation()");
            ResetPopulation();
            Profiler.EndSample();

            _frameCounter = 0;
        }

        _frameCounter++;
    }
    
    void OnGUI()
    {
        GUIStyle fontSize = new GUIStyle(GUI.skin.GetStyle("label"));
        fontSize.fontSize = 18;
        GUI.Label(new Rect(100, 150, 400, 50), "bestIndividual = " + new string(bestRocketIndividual.genotype.ToString()), fontSize);
        GUI.Label(new Rect(100, 200, 400, 50), "averageFitness = " + averageFitness, fontSize);
        GUI.Label(new Rect(100, 250, 400, 50), "generationNumber = " + generationNumber, fontSize);
        GUI.Label(new Rect(100, 300, 400, 50), "FramesUntilNextGeneration = " + (generationEvolutionFixedFrames - _frameCounter), fontSize);
    }

    private void CalculateFitness()
    {
        foreach (var rocketIndividual in population.individuals)
        {
            float d = Vector3.Distance(rocketTarget.transform.position, rocketIndividual.transform.position);
            rocketIndividual.SetFitness(1000/d);
            //rocketIndividual.SetFitness(Mathf.Pow(1000 / d, 2));
        }
    }
    
    
    private List<RocketIndividual> SelectMatingPool()
    {
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

        return matingPool;
    }
    
    
    private void ResetPopulation()
    {
        foreach (var rocketIndividual in population.individuals)
        {
            rocketIndividual.Reset();
        }
    }
    
    
    public void PerformReproduction(List<RocketIndividual> matingPool)
    {

        if (matingPool.Count == 0)
        {
            Debug.LogError("MatingPool count is zero!");
            return;
        }
        
        //Make next generation using crossover
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
            
            Profiler.BeginSample("Crossover()");
            DNA childGenotype = genotypeA.Crossover(genotypeB);
            Profiler.EndSample();
            
            Profiler.BeginSample("Mutate()");
            childGenotype.Mutate(mutationRate);
            Profiler.EndSample();
            
            Profiler.BeginSample("ChangeIndividualGenotype()");
            population.ChangeIndividualGenotype(population.individuals[i], childGenotype);
            Profiler.EndSample();
        }
        
        generationNumber++;
    }
    
}

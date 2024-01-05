using System;
using System.Collections.Generic;
using UnityEngine;
using Random = Unity.Mathematics.Random;
using UnityEngine.Profiling;
using UnityEngine.Serialization;

public class RocketSimulationManager : MonoBehaviour
{
    [Header("Simulation Parameters")] [SerializeField]
    private int populationSize = 500;
    
    [SerializeField] private float mutationRate = 0.01f;
    [SerializeField] public int generationEvolutionFixedFrames = 1000;
    [SerializeField] [Range(0.1f, 50f)] public float timeScale = 1;
    
    [Header("Simulation Environment")]
    [SerializeField] public GameObject rocketIndividualPrefab;
    [SerializeField] private Transform rocketTarget;

    [Header("DEBUG")]
    [SerializeField] private float averageFitness;
    [SerializeField] private RocketIndividual bestRocketIndividual;
    [SerializeField] private float bestRocketIndividualFitness;
    [SerializeField] private int generationNumber;
    
    private GameObject populationObject;
    
    [SerializeField] private RocketPopulation population;
    
    private int _frameCounter = 0;
    
    private Random _random;

    public int GetPopulationSize()
    {
        return populationSize;
    }
    
    // Start is called before the first frame update
    void Awake()
    {
        Physics.simulationMode = SimulationMode.Script;
        Time.timeScale = timeScale;
        
        _random = new Random((uint) System.DateTime.Now.Ticks);
        
        populationObject = new GameObject("PopulationManager");
        population = populationObject.AddComponent<RocketPopulation>();
        population.SpawnPopulationRandom(rocketIndividualPrefab, populationSize);
    }

    private void Update()
    {
        Time.timeScale = timeScale;
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
            bestRocketIndividualFitness = bestRocketIndividual.Fitness;
            
            //Making next generation through parent selection and reproduction
            Profiler.BeginSample("SelectMatingPool()");
            List<RocketIndividual> matingPool = SelectMatingPool();
            Profiler.EndSample();
            
            //Debug.Log("Mating Pool individuals number (Generation #"+generationNumber+") = "+matingPool.Count);
            
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
            //DISTANCE FROM TARGET
            float d = Vector3.Distance(rocketTarget.position, rocketIndividual.transform.position) - rocketTarget.localScale.x;
            //normalize d parameter to be between 0f and 1f
            d /= Vector3.Distance(rocketTarget.position, Vector3.zero) - rocketTarget.localScale.x;
            d = Mathf.Clamp(d, 0f, 1f);
            
            //DIRECTION TO TARGET
            Vector3 distanceVector = rocketTarget.position - rocketIndividual.transform.position;
            float dirAffinity = Vector3.Dot(rocketIndividual.GetVelocity().normalized, distanceVector);
            dirAffinity = Mathf.Clamp(dirAffinity, 0f, 1f);
            
            //FINAL FITNESS FUNCTION
            rocketIndividual.SetFitness(Mathf.Pow((1-d) * dirAffinity, 0.5f));
        }
    }
    
    
    private List<RocketIndividual> SelectMatingPool()
    {
        population.individuals.Sort((x, y) => y.Fitness.CompareTo(x.Fitness));
        
        List<RocketIndividual> matingPool = new List<RocketIndividual>();
        float probabilityCounter = populationSize / 10f;
        
        for (int i = 0; i < populationSize / 10f; i++)
        {
            probabilityCounter /= 3;
            for (int j = 0; j < probabilityCounter; j++)
            {
                matingPool.Add(population.individuals[i]);
            }
        }
        
        return matingPool;
        
        /*
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
        */
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
                a = _random.NextInt(0, matingPool.Count);
                b = _random.NextInt(0, matingPool.Count);
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

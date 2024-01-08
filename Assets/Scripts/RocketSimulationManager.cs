using System;
using System.Collections.Generic;
using UnityEngine;
using Random = Unity.Mathematics.Random;
using UnityEngine.Profiling;

[Serializable]
public class RocketSimulationManager : MonoBehaviour
{
    [Header("Simulation Parameters")] [SerializeField]
    private int populationSize = 1000;
    
    [SerializeField] private float mutationRate = 0.01f;
    [SerializeField] public int simulationStepsPerGeneration = 1000;
    [SerializeField] [Range(0.1f, 50f)] public float timeScale = 1;
    
    [Header("Simulation Environment")]
    [SerializeField] public GameObject rocketIndividualPrefab;
    [SerializeField] private Transform rocketTarget;

    [Header("DEBUG")]
    [SerializeField] private int generationNumber;
    [SerializeField] private float averageFitness;
    [SerializeField] private float bestRocketIndividualFitness;
    [SerializeField] private List<RocketIndividual> bestRocketIndividuals;
    
    private GameObject populationObject;
    private RocketPopulation population;
    
    private int _frameCounter;
    private Random _random;

    public int GetPopulationSize()
    {
        return populationSize;
    }
    
    void Awake()
    {
        Time.timeScale = timeScale;
        _frameCounter = 0;
        _random = new Random((uint) System.DateTime.Now.Ticks);
        
        populationObject = new GameObject("PopulationManager");
        population = populationObject.AddComponent<RocketPopulation>();
        population.SpawnPopulationRandom(rocketIndividualPrefab, populationSize);
    }

    private void Update()
    {
        Time.timeScale = timeScale;
        
        //For trail renderer DEBUG purposes
        CalculateFitness();
    }

    private void FixedUpdate()
    {
        if (_frameCounter >= simulationStepsPerGeneration)
        {
            //Fitness calculation of current generation
            Profiler.BeginSample("CalculateFitness");
            CalculateFitness();
            Profiler.EndSample();
            
            //SAVE INFO TO FILE
            population.SaveGenerationToFile(simulationStepsPerGeneration, generationNumber, 5);
            
            //DEBUG
            averageFitness = population.AverageFitness();
            bestRocketIndividuals = population.BestIndividuals(5);
            
            //Making next generation through parent selection and reproduction
            Profiler.BeginSample("SelectMatingPool()");
            List<RocketIndividual> matingPool = SelectMatingPool();
            Profiler.EndSample();
            
            Debug.Log("Mating Pool individuals number (Generation #"+generationNumber+") = "+matingPool.Count);
            
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

    private void CalculateFitness()
    {
        foreach (var rocketIndividual in population.individuals)
        {
            //DISTANCE FROM TARGET
            float d = Vector3.Distance(rocketTarget.position, rocketIndividual.transform.position) - rocketTarget.localScale.x;
            float pathDistance = Vector3.Distance(rocketTarget.position, Vector3.zero) - rocketTarget.localScale.x;
            //normalize d parameter to be between 0f and 1f
            d = d / pathDistance;
            d = Mathf.Clamp(d, 0f, 1f);
            
            //DIRECTION TO TARGET
            Vector3 distanceVector = rocketTarget.position - rocketIndividual.transform.position;
            float dirAffinity = Vector3.Dot(rocketIndividual.GetVelocity().normalized, distanceVector.normalized);
            dirAffinity = (dirAffinity + 1f) / 2f;
            
            //VELOCITY & PROXIMITY
            //float velocityProximityModifier = 1f / (rocketIndividual.GetVelocity().magnitude * d);
            
            //OBSTACLE HIT
            float obstacleHitModifier = 1f;
            if (rocketIndividual.HasCrashed()) 
                obstacleHitModifier = 0.3f;
            
            //TARGET HIT
            float targetHitModifier = 1f;
            if (rocketIndividual.HasArrived())
                targetHitModifier = 2f;
            
            //FINAL FITNESS FUNCTION
            float fitness = Mathf.Pow(((1 - d) + dirAffinity) / 2f, 3f) * obstacleHitModifier * targetHitModifier;
            rocketIndividual.SetFitness(fitness);
        }
    }
    
    
    private List<RocketIndividual> SelectMatingPool()
    {
        population.individuals.Sort((x, y) => y.GetFitness().CompareTo(x.GetFitness()));
        
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
        
        //1 individual to reproduce is the one with the best fitness
        population.individuals[0] = bestRocketIndividuals[0];
        //Make next generation using crossover
        for (int i = 1; i < populationSize; i++)
        {
            int a = 0;
            int b = 0;
            while (a == b)
            {
                a = _random.NextInt(0, matingPool.Count);
                b = _random.NextInt(0, matingPool.Count);
            }

            DNA genotypeA = matingPool[a].GetGenotype();
            DNA genotypeB = matingPool[b].GetGenotype();
            
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

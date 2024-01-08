using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RocketDemoManager : MonoBehaviour
{
    [Header("Simulation Environment")]
    [SerializeField] public GameObject rocketIndividualPrefab;
    [SerializeField] private Transform rocketTarget;

    [Header("DEMO parameters")]
    [SerializeField] int generationNumberToLoad;
    [SerializeField] [Range(0.1f, 50f)] public float timeScale = 1;
    [SerializeField] public int simulationSteps;
    
    private int _frameCounter;
    private GameObject populationObject;
    private RocketPopulation population;
    
    void Start()
    {
        Time.timeScale = timeScale;
        _frameCounter = 0;
        
        populationObject = new GameObject("PopulationManager");
        population = populationObject.AddComponent<RocketPopulation>();
        
        population.LoadGenerationFromFile(generationNumberToLoad,out simulationSteps);
        population.SpawnPopulation(rocketIndividualPrefab);
    }

    private void Update()
    {
        Time.timeScale = timeScale;
        
        //For trail renderer DEBUG purposes
        CalculateFitness();
    }

    private void FixedUpdate()
    {
        if(_frameCounter >= simulationSteps)
        {
            ResetPopulation();
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
    
    private void ResetPopulation()
    {
        foreach (var rocketIndividual in population.individuals)
        {
            rocketIndividual.Reset();
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimulationManager : MonoBehaviour
{
    [SerializeField]
    private int populationSize = 1000;

    [SerializeField] 
    private string stringTarget = "to be or not to be";
    private DNA target;
    [SerializeField]
    private float generationEvolutionMillisec = 1000;
    
    private float timeCounter = 0f;

    [SerializeField] 
    private List<DNA> population;
    
    // Start is called before the first frame update
    void Start()
    {
        target = new DNA(stringTarget);
        population = new List<DNA>();
        
        for (int i = 0; i < populationSize; i++)
        {
            population.Add(new DNA());
        }
    }

    private void FixedUpdate()
    {
        if (timeCounter >= generationEvolutionMillisec)
        {
            CalculateFitness();
            Selection();

            timeCounter = 0f;
        }

        timeCounter += Time.fixedDeltaTime * 1000f;
    }
    
    void OnGUI()
    {
        GUIStyle fontSize = new GUIStyle(GUI.skin.GetStyle("label"));
        fontSize.fontSize = 24;
        GUI.Label(new Rect(100, 300, 200, 50), "MillsecUntilNextGeneration = " + (generationEvolutionMillisec - timeCounter).ToString(), fontSize);
    }

    private void CalculateFitness()
    {
        foreach (var individual in population)
        {
            individual.CalculateFitness(target);
        }
    }

    private void Selection()
    {
        //Construct the mating pool, giving more probability to higher fitness individual to reproduce
        List<DNA> matingPool = new List<DNA>();
        
        for (int i = 0; i < populationSize; i++)
        {
            int n = (int) (population[i].Fitness * 100f);
            for (int j = 0; j < n; j++)
            {
                matingPool.Add(population[i]);
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
        
        population = newPopulation;
    }
}

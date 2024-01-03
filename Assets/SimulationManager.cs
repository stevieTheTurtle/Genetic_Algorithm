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

    [Header("Simulation DEBUG")]
    [SerializeField]
    private float averageFitness;
    [SerializeField]
    private DNA bestIndividual;
    [SerializeField]
    private int generationNumber;
    
    [SerializeField] 
    private Population population;
    
    private float timeCounter = 0f;
    
    // Start is called before the first frame update
    void Start()
    {
        target = new DNA(stringTarget);
        population = new Population(populationSize);
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
            
            //Selection of parents to make next generation
            population.Selection();

            timeCounter = 0f;
        }

        timeCounter += Time.fixedDeltaTime * 1000f;
    }
    
    void OnGUI()
    {
        GUIStyle fontSize = new GUIStyle(GUI.skin.GetStyle("label"));
        fontSize.fontSize = 18;
        GUI.Label(new Rect(100, 150, 400, 50), "bestIndividual = " + new string(bestIndividual.Genes.ToString()), fontSize);
        GUI.Label(new Rect(100, 200, 400, 50), "averageFitness = " + averageFitness.ToString(), fontSize);
        GUI.Label(new Rect(100, 250, 400, 50), "generationNumber = " + generationNumber.ToString(), fontSize);
        GUI.Label(new Rect(100, 300, 400, 50), "MillsecUntilNextGeneration = " + (generationEvolutionMillisec - timeCounter).ToString(), fontSize);
    }

    private void CalculateFitness()
    {
        foreach (var individual in population.individuals)
        {
            individual.CalculateFitness(target);
        }
    }
}

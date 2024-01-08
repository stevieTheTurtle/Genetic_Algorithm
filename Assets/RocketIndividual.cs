using System;
using UnityEngine;

[Serializable]
public class RocketIndividual : MonoBehaviour
{
    [Header("Simulation Parameters")]
    [SerializeField] private float mass;
    [SerializeField] private LayerMask boundsLayer;
    [SerializeField] private LayerMask obstaclesLayer;
    [SerializeField] private LayerMask targetLayer;

    private TrailRenderer _trailRenderer;
    //private Rigidbody _rigidBody;

    [Header("DEBUG")] 
    [SerializeField] private Vector3 acceleration;
    [SerializeField] private Vector3 velocity;
    [SerializeField] private bool hasCrashed;
    [SerializeField] private bool hasArrived;
    [SerializeField] private float Fitness;
    
    [SerializeField] private int id;
    [SerializeField] private int genotypeIndex;
    [SerializeField] private DNA genotype;
    [SerializeField] private Planet[] _planets;

    private void Start()
    {
        //DEBUG
        //Debug.Log((int)obstaclesLayer+" "+(int)targetLayer);
        
        if(boundsLayer.Equals(new LayerMask()))
            Debug.LogError("Missing boundsLayer!");
        if(obstaclesLayer.Equals(new LayerMask()))
            Debug.LogError("Missing obstaclesLayer!");
        if(targetLayer.Equals(new LayerMask()))
            Debug.LogError("Missing targetLayer!");

        //_rigidBody = GetComponent<Rigidbody>();
        _trailRenderer = GetComponentInChildren<TrailRenderer>();

        _planets = FindObjectsByType<Planet>(FindObjectsSortMode.None);
        
        Reset();
    }

    public void SetFitness(float fitness)
    {
        this.Fitness = fitness;
    }
    
    public float GetFitness()
    {
        return this.Fitness;
    }

    public void Reset()
    {
        _trailRenderer.Clear();
        
        Fitness = 0f;
        genotypeIndex = 0;

        this.transform.position = Vector3.zero;
        this.transform.rotation = Quaternion.identity;
        
        acceleration = Vector3.zero;
        velocity = Vector3.zero;
        hasCrashed = false;
        hasArrived = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == Mathf.Log(boundsLayer, 2))
        {
            //Debug.Log(gameObject.name+ " has reached environment bounds!");
            hasCrashed = true;
            //_rigidBody.constraints = RigidbodyConstraints.FreezeAll;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        
        //Debug.Log(gameObject.name + " collided with "+other.gameObject.name+" in layer "+other.gameObject.layer);

        if (other.gameObject.layer == Mathf.Log(obstaclesLayer, 2))
        {
            //Debug.Log(gameObject.name+ " has crashed!");
            hasCrashed = true;
            //_rigidBody.constraints = RigidbodyConstraints.FreezeAll;
        }
        else if (other.gameObject.layer == Mathf.Log(targetLayer, 2))
        {
            Debug.Log(gameObject.name+ " has arrived!");
            hasArrived = true;
        }
    }

    private void Update()
    {
        if(velocity.magnitude >= 0.01f)
            this.transform.forward = velocity.normalized;

        Color trailColor = Color.Lerp(Color.red, Color.green, Fitness);
        _trailRenderer.material.color = trailColor;
        _trailRenderer.material.SetColor("_EmissionColor", trailColor);
    }

    private void FixedUpdate()
    {
        if (hasCrashed || hasArrived) return;
        
        if (genotypeIndex >= genotype.genes.Length)
        {
            Debug.LogError("Genes array out of bound!");
            return;
        }

        acceleration = Vector3.zero;
        
        //PHENOTYPE DEFINITION, which is force applied every simulation step
        AddForce(genotype.genes[genotypeIndex]);
        genotypeIndex++;
        
        /*
        Vector3 r;
        //Adding PLANETS FORCEs
        foreach (Planet p in _planets)
        {
            r = p.transform.position - this.transform.position;
            AddForce(6.67e-11f * p.GetMass() * this.mass / Mathf.Pow(r.magnitude, 2f) * r.normalized);
            //Debug.DrawLine(this.transform.position, p.transform.position);
        }
        */
        
        //Apply motion, calculating velocity and position
        this.velocity += acceleration * Time.fixedDeltaTime;
        this.transform.position += this.velocity * Time.fixedDeltaTime;
    }

    public void SetId(int newId)
    {
        id = newId;
    }
    
    public int GetId()
    {
        return id;
    }

    public DNA GetGenotype()
    {
        return genotype;
    }
    
    public void SetGenotype(DNA newGenotype)
    {
        genotype = newGenotype;
    }

    public Vector3 GetVelocity()
    {
        return velocity;
    }

    public bool HasCrashed()
    {
        return hasCrashed;
    }
    
    public bool HasArrived()
    {
        return hasArrived;
    }

    public void AddForce(Vector3 force)
    {
        acceleration += force / mass;
    }
}

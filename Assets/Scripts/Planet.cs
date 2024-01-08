using UnityEngine;

public class Planet : MonoBehaviour{
    
    [SerializeField] private float baseMass = 1e8f;
    
    private float mass;

    private void Start()
    {
        mass = baseMass * this.transform.localScale.x;
    }

    public float GetMass()
    {
        return mass;
    }
}

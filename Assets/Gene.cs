using UnityEngine;

[System.Serializable]
public class Gene
{
    public Vector3 Allele;
    
    //maximum force that thrusters can apply
    public float maxForce = 30f;
    
    public Gene()
    {
        System.Random rnd = new System.Random();
        
        float x = ((float) rnd.NextDouble() - 0.5f) * 2f;
        float y = ((float) rnd.NextDouble() - 0.5f) * 2f;
        float z = ((float) rnd.NextDouble() - 0.5f) * 2f;
        Vector3 randomVector = new Vector3(x, y, z);
        Allele = randomVector.normalized * Random.Range(0f, maxForce);
    }
    
}

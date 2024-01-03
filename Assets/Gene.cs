using UnityEngine;

[System.Serializable]
public class Gene
{
    public Vector3 Allele;
    
    public Gene()
    {
        float x = Random.Range(-1f,1f);
        float y = Random.Range(-1f,1f);
        float z = Random.Range(-1f,1f);
        Vector3 randomVector = new Vector3(x, y, z);
        Allele = randomVector.normalized;
    }

    public bool Equals(Gene otherGene)
    {
        if (Allele.Equals(otherGene.Allele))
            return true;
        else
            return false;
    }

    public override string ToString()
    {
        return Allele.ToString();
    }
}

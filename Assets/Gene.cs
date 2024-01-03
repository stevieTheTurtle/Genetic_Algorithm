
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

[System.Serializable]
public class Gene
{
    public char Allele;
    
    public Gene()
    {
        Allele = (char) Random.Range(97, 125);
        if (Allele.Equals((char)124))
            Allele = (char)32;
    }

    public bool Equals(Gene otherGene)
    {
        if (Allele.Equals(otherGene.Allele))
            return true;
        else
            return false;
    }
}

using UnityEngine;

[System.Serializable]
public class Gene
{
    public char Allele;
    
    public Gene()
    {
        System.Random rnd = new System.Random();
        Allele = (char) rnd.Next(97, 124);
        if (Allele.Equals((char)123))
            Allele = (char)32;
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

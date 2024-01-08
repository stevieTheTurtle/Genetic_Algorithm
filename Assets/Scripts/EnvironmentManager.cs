using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    [SerializeField] private GameObject planetPrefab;
    [SerializeField] private int planetsNum = 15;
    [SerializeField] private int minSize = 5;
    [SerializeField] private int maxSize = 15;

    public void SetupRandomEnvironment()
    {
        Collider col = this.GetComponentInChildren<BoxCollider>();

        foreach (Planet p in FindObjectsOfType<Planet>())
        {
            DestroyImmediate(p.gameObject);
        }

        GameObject planet;
        Vector3 randomPos;
        float x, y, z, scale;
        for (int i = 0; i < planetsNum; i++)
        {
            x = Random.Range(col.bounds.min.x, col.bounds.max.x);
            y = Random.Range(col.bounds.min.y, col.bounds.max.y);
            z = Random.Range(col.bounds.min.z, col.bounds.max.z);
            scale = Random.Range(minSize, maxSize);
            
            randomPos = new Vector3(x, y, z);
            planet = Instantiate(planetPrefab, randomPos, Quaternion.identity);
            planet.transform.SetParent(this.transform);
            planet.transform.localScale = new Vector3(scale, scale, scale);
        }
    }

}

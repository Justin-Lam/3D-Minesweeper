using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGeneration : MonoBehaviour
{
    public GameObject grassBlock;
    public float gridSpacing = 1f;
    public float blockSize = 1f;
    public int gridX;
    public int gridZ;
    public Vector3 gridOrigin = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        for (int x = 0; x < gridX; x++)
        {
            for (int z = 0; z < gridZ; z++)
            {
                if (x == 0 && z == 0) continue;
                float randomHeight = Random.Range(0.0f, 0.13f);
                Vector3 spawnPosition = new Vector3(x * (gridSpacing + blockSize), randomHeight, z * (gridSpacing + blockSize)) + gridOrigin;
                GameObject grassClone = Instantiate(grassBlock, spawnPosition, Quaternion.identity);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{

    public GameObject[] tilePrefabs;
    public float zSpown = 0;
    public float tileLength = 40;
    public int numberOfTiles = 5;
    public float safeZone = 75;
    public List<GameObject> activeTiles = new List<GameObject>();
    public Transform playerTransform;
      
    void Start()
    {
        for (int i = 0; i < numberOfTiles; i++) 
        {
            if ( i == 0 )
            {
                SpawnTile( 0 );
            }
        SpawnTile(Random.Range(0, tilePrefabs.Length));
        } 
    }

    
    void Update()
    {
        if (playerTransform.position.z - safeZone  > zSpown - (numberOfTiles * tileLength))
        {
            SpawnTile(Random.Range(0, tilePrefabs.Length));
            DeleteTile();
        }
    }

    public void SpawnTile(int tileIndex)
    {
       
        GameObject gameOb = Instantiate(tilePrefabs[tileIndex], transform.forward * zSpown, transform.rotation);
        activeTiles.Add(gameOb);     
        zSpown += tileLength; 
    }

    public void DeleteTile()
    {
        Destroy(activeTiles[0]);
        activeTiles.RemoveAt(0);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public GameObject[] tilePrefabs;
    public float zSpawn = 50;
    public float tileLength = 30;
    public int numberOfTiles = 3;

    public Transform playerTransform;

    // maintain only some number of active tiles
    private List<GameObject> activeTiles = new List<GameObject>();
    private float safeDistance;
    private float tilePosX, tilePosY;
    // Start is called before the first frame update
    void Start()
    {
        tilePosX = 3.43f;
        // tile's surface is 23.7f units below the tile position, therefore sliding it up to match player's position
        tilePosY = 23.7f;
        safeDistance = tileLength + 5;
        SpawnTile(0);
        for(int i=0; i<numberOfTiles; i++)
        {
            SpawnTile(Random.Range(1, tilePrefabs.Length));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(playerTransform.position.z - safeDistance > zSpawn - (numberOfTiles * tileLength))
        {
            SpawnTile(Random.Range(0, tilePrefabs.Length));
            DeleteLastTile();
        }
    }

    public void SpawnTile(int tileIndex)
    {
        GameObject newTile = Instantiate(tilePrefabs[tileIndex], new Vector3(tilePosX, tilePosY, tileIndex == 0 ? zSpawn :transform.forward.z * zSpawn), transform.rotation);
        activeTiles.Add(newTile);
        zSpawn += tileLength;
    }

    private void DeleteLastTile()
    {
        Destroy(activeTiles[0]);
        activeTiles.RemoveAt(0);
    }
}

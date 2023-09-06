using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ObjectPlacer_Random : BaseObjectPlacer {
    [SerializeField] float TargetDensity = 0.2f;
    [SerializeField] int MaxSpawnCount = 3000;
    [SerializeField] List<GameObject> Prefabs;


    public override void Execute(Transform objectRoot, int mapResolution, float[,] heightMap, Vector3 heightMapScale, float[,] slopeMap, float[,,] alphaMaps, int alphaMapResolution, byte[,] biomeMap = null, int biomeIndex = -1, BiomeConfigSO biome = null) {
        // get potential spawn locations
        List<Vector3> candidateLocations = GetAllLocationsForBiome(mapResolution, heightMap, heightMapScale, biomeMap, biomeIndex);

        int numToSpawn = Mathf.FloorToInt(Mathf.Min(MaxSpawnCount, candidateLocations.Count * TargetDensity));
        for (int index = 0; index < numToSpawn; index++) {
            // pick a random location to spawn at
            int randomLocationIndex = Random.Range(0, candidateLocations.Count);
            Vector3 spawnLocation = candidateLocations[randomLocationIndex];
            candidateLocations.RemoveAt(randomLocationIndex);

            // instentiate the prefab
            ExpandBiomeObject(objectRoot, spawnLocation, slopeMap);
        }
    }

    public override void ExpandBiomeObject(Transform objectRoot, Vector3 spawnLocation, float[,] slopeMap) {
        // Get the slope value from the slope map
        int x = Mathf.Clamp(Mathf.FloorToInt(spawnLocation.x), 0, slopeMap.GetLength(0) - 1);
        int y = Mathf.Clamp(Mathf.FloorToInt(spawnLocation.z), 0, slopeMap.GetLength(1) - 1);
        float slope = slopeMap[x, y];
        // Quaternion rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), slope * 10f);

        Quaternion rotation = Quaternion.Euler(0f, Random.Range(-50f, 50f), slope * 15f);
        
        GameObject newObject = Instantiate(Prefabs[(int)Random.Range(0, Prefabs.Count - 1)], spawnLocation, rotation, objectRoot);
    }
}

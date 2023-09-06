using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProcGenManager : MonoBehaviour{

    [SerializeField] ProcGenConfigSO Config;
    [SerializeField] public Terrain TargetTerrain;

    int mapResolution;
    int mapLowRes;

    byte[,] BiomeMap_LowResolution;
    float[,] BiomeStrengths_LowResolution;

    public byte[,] BiomeMap;
    float[,] BiomeStrengths;
    byte[,] BiomeMap_Backup;

    public Texture2D BiomeMapImage;
    public Texture2D BiomeMapImage_Start_LowRes;
    public Texture2D BiomeMapImage_Start_HighRes;

    [SerializeField] RawImage biomeMapImage;

    float[,] SlopeMap;
    float[,,] AlphaMaps;
    int AlphaMapResolution;
    float[,] HeightMap;
    Vector3 HeightMapScale;
    public Color[] hues = new Color[] {
        Color.HSVToRGB(0.18f, 0.49f, 1f),
        Color.HSVToRGB(0.09f, 0.55f, 1f),
        Color.HSVToRGB(0f, 0.55f, 1f),
        Color.HSVToRGB(0.58f, 0.65f, 1f),
        Color.HSVToRGB(0.8333f, 0.39f, 1f),
        Color.HSVToRGB(0.2f, 0.37f, 0.81f),
        Color.HSVToRGB(0f, 0f, 1f)
    };

    public bool isSimulationRunning;

    private float timeSinceLastUpdate = 0f;
    private int simulationTime = 0;
    private float updateDelay = 1f;

    public int numOfSteps;

    // Start is called before the first frame update
    void Start(){
        isSimulationRunning = false;

        biomeMapImage.color = Color.black;

        mapResolution = TargetTerrain.terrainData.heightmapResolution;
        mapLowRes = (int)Config.BiomeMapResolution;

        numOfSteps = 1;

        RegenerateWorld();
    }

    void Update(){
        if(isSimulationRunning && BiomeMap.Length > 0){
            timeSinceLastUpdate += Time.deltaTime;
            if(timeSinceLastUpdate >= updateDelay) {
                for(int i = 0; i < numOfSteps; i++) Perform_BiomeSimulation();
                
                simulationTime++;

                stopSimulation();
                timeSinceLastUpdate = 0f;
            }
        }
    }

    public void stopSimulation(){
        SaveBiomeImage(mapResolution, "Assets/BiomeMap.png", BiomeMap, BiomeMapImage);
        isSimulationRunning = false;
        simulationTime = 0;
    }

    public void RegenerateWorld(){

        AlphaMapResolution = TargetTerrain.terrainData.alphamapResolution;

        // generate low res biome map
        Perform_BiomeGeneration_LowResolution();
        SaveBiomeImage((int)Config.BiomeMapResolution, "Assets/BiomeMap_Start_LowResolution.png", BiomeMap_LowResolution, BiomeMapImage_Start_LowRes);

        // generate the high res biome map
        Perform_BiomeGeneration_HighResolution();
        SaveBiomeImage(mapResolution, "Assets/BiomeMap_Start_HighResolution.png", BiomeMap, BiomeMapImage_Start_HighRes);
        SaveBiomeImage(mapResolution, "Assets/BiomeMap.png", BiomeMap, BiomeMapImage);

        // get slopemap and alphamapresolution
        HeightMap = TargetTerrain.terrainData.GetHeights(0, 0, mapResolution, mapResolution);
        AlphaMaps = TargetTerrain.terrainData.GetAlphamaps(0, 0, AlphaMapResolution, AlphaMapResolution);
        HeightMapScale = TargetTerrain.terrainData.heightmapScale;

        SlopeMap = new float[mapResolution, mapResolution];
        for (int y = 0; y < mapResolution; y++) {
            for (int x = 0; x < mapResolution; x++) {
                SlopeMap[x, y] = TargetTerrain.terrainData.GetInterpolatedNormal((float) x / AlphaMapResolution, (float) y / AlphaMapResolution).y;
            }
        }

        Perform_ObjectPlacement();
    }

    void Perform_BiomeGeneration_LowResolution(){
        // allocate the biome map and strengths map
        BiomeMap_LowResolution = new byte[mapLowRes, mapLowRes];
        BiomeStrengths_LowResolution = new float[mapLowRes, mapLowRes];

        // select space for the seed points
        int numSeedPoints = Mathf.FloorToInt(mapLowRes * mapLowRes * Config.BiomeSeedPointDensity);
        List<byte> biomesToSpawn = new List<byte>(numSeedPoints);

        // populate the biomes to spawn based on weightings
        for(int biomeIndex = 0; biomeIndex < Config.NumBiomes; biomeIndex++){
           int numEntries = Mathf.RoundToInt(numSeedPoints * Config.Biomes[biomeIndex].Weighting / Config.TotalWeighting);
           // Debug.Log("Spawning " + numEntries + " seedpoints for " + Config.Biomes[biomeIndex].Biome.Name);

           for (int entryIndex = 0; entryIndex < numEntries; entryIndex++) biomesToSpawn.Add((byte)biomeIndex);
        }

        // spawn the individual biomes
        while(biomesToSpawn.Count > 0){
            // pick a random seedpoint
            int seedPointIndex = Random.Range(0, biomesToSpawn.Count);
            // extract the biome index
            byte biomeIndex = biomesToSpawn[seedPointIndex];
            // remove seed point
            biomesToSpawn.RemoveAt(seedPointIndex);

            Perform_SpawnIndividualBiome(biomeIndex);
        }
    }

    Vector2Int[] NeighbourOffsets = new Vector2Int[] {
        new Vector2Int(0, 1),
        new Vector2Int(0, -1),
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int(1, 1),
        new Vector2Int(1, -1),
        new Vector2Int(-1, 1),
        new Vector2Int(-1, -1)
    };

    Vector2Int[] GetNeighbours(Vector2Int location, int mapRes) {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        foreach (Vector2Int offset in NeighbourOffsets) {
            Vector2Int neighborLocation = location + offset;
            if (IsValidPosition(neighborLocation, mapRes)) {
                neighbors.Add(neighborLocation);
            }
        }

        return neighbors.ToArray();
    }

    // Ooze-based generation: https://www.procjam.com/tutorials/en/ooze/

    void Perform_SpawnIndividualBiome(byte biomeIndex) {
        BiomeConfigSO biomeConfig = Config.Biomes[biomeIndex].Biome;

        // Pick spawn location
        Vector2Int currentLocation = new Vector2Int(Random.Range(0, mapLowRes), Random.Range(0, mapLowRes));

        // Pick starting intensity
        float currentIntensity = Random.Range(biomeConfig.MinIntensity, biomeConfig.MaxIntensity);

        bool[,] visited = new bool[mapLowRes, mapLowRes];

        // Drunken walk
        for (int step = 0; step < 50; step++) {
            // Set biome and intensity
            BiomeMap_LowResolution[currentLocation.x, currentLocation.y] = biomeIndex;
            BiomeStrengths_LowResolution[currentLocation.x, currentLocation.y] = currentIntensity;

            // Mark as visited
            visited[currentLocation.x, currentLocation.y] = true;

            // Randomly select a neighboring cell
            Vector2Int[] neighbors = GetNeighbours(currentLocation, mapLowRes);
            Vector2Int nextLocation = neighbors[Random.Range(0, neighbors.Length)];

            // Calculate new intensity
            float nextIntensity = currentIntensity - Random.Range(biomeConfig.MinDecayRate, biomeConfig.MaxDecayRate);

            // Update current position and intensity
            currentLocation = nextLocation;
            currentIntensity = Mathf.Max(nextIntensity, 0f);
        }
    }

    byte CalculateHighResBiomeIndex(int lowResX, int lowResY, float fractionX, float fractionY){
        float A = BiomeMap_LowResolution[lowResX, lowResY];
        float B = (lowResX + 1) >=  mapLowRes ? A : BiomeMap_LowResolution[lowResX + 1, lowResY];
        float C = (lowResY + 1) >=  mapLowRes ? A : BiomeMap_LowResolution[lowResX, lowResY + 1];
        float D = (lowResX + 1) >=  mapLowRes ? C
                  : (lowResY + 1) >=  mapLowRes ? B
                  : BiomeMap_LowResolution[lowResX + 1, lowResY + 1];

        // bilinear filtering
        float filteredIndex = A * (1 - fractionX) * (1 - fractionY) + B * fractionX * (1 - fractionY) *
                      C * (1 - fractionX) * fractionY + D * fractionX * fractionY;

        // find closest neighbour
        float[] candidateBiomes = new float [] {A, B, C, D};
        float bestBiome = -1f;
        float bestDelta = float.MaxValue;

        for(int biomeIndex = 0; biomeIndex < candidateBiomes.Length; biomeIndex++){
            float delta = Mathf.Abs(filteredIndex - candidateBiomes[biomeIndex]);
            if(delta < bestDelta){
                bestDelta = delta;
                bestBiome = candidateBiomes[biomeIndex];
            }
        }

        return (byte)Mathf.RoundToInt(bestBiome);
    }

    void Perform_BiomeGeneration_HighResolution(){
        // allocate the biome map and strengths map
        BiomeMap = new byte[mapResolution, mapResolution];
        BiomeStrengths = new float[mapResolution, mapResolution];

        // calculate the high res map
        float mapScale = (float)mapLowRes / (float)mapResolution;
        
        for(int y = 0; y < mapResolution; y++){
            int lowResY = Mathf.FloorToInt(y * mapScale);
            float yFraction = y * mapScale - lowResY;
            for(int x = 0; x < mapResolution; x++){
                int lowResX = Mathf.FloorToInt(x * mapScale);
                float xFraction = x * mapScale - lowResX;
                BiomeMap[x, y] = CalculateHighResBiomeIndex(lowResX, lowResY, xFraction, yFraction);
            }
        }

        BiomeMap_Backup = (byte[,])BiomeMap.Clone();
    }
    static bool ArrayContainsValue(float[] array, float value)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == value)
            {
                return true;
            }
        }
        return false;
    }
    public void SaveBiomeImage(int mapRes, string imageName, byte[,] SaveMap, Texture2D biomeMap) {
        biomeMap = new Texture2D(mapRes, mapRes, TextureFormat.RGB24, false);
        for (int y = 0; y < mapRes; y++) {
            for (int x = 0; x < mapRes; x++) {
                float hue = ((float)SaveMap[x, y] / (float)Config.NumBiomes);
                biomeMap.SetPixel(x, y, hues[(int)SaveMap[x,y]]);
            }
        }
        biomeMap.Apply();

        biomeMapImage.texture = biomeMap;
        biomeMapImage.color = new Color(0.553f, 0.722f, 0.682f, 1.0f);

        System.IO.File.WriteAllBytes(imageName, biomeMap.EncodeToPNG());
    }

    bool IsEdgeCell(int x, int y, int mapRes) {
        return x == 0 || y == 0 || x == mapRes - 1 || y == mapRes - 1;
    }

    bool IsValidPosition(Vector2Int position, int mapRes) {
        return position.x >= 0 && position.y >= 0 && position.x < mapRes && position.y < mapRes;
    }

    float CalculateScoreForExpansion(int biomeIndex, Vector2Int location) {
        BiomeConfigSO biomeConfig = Config.Biomes[biomeIndex].Biome;

        // Calculate score
        float score = 0.25f * biomeConfig.GrowthRateAdvantage + 0.15f * biomeConfig.TemperatureSensitivity + 0.15f * biomeConfig.LightRequirements
                    + 0.25f * biomeConfig.CompetitiveAbilities + 0.2f * biomeConfig.Resilience + Random.Range(0f, 0.5f); // Add a random value between 0 and 1

        return score;
    }

    private void DeleteObjectAtLocationIfExists(Vector3 location) {
        Collider[] colliders = Physics.OverlapSphere(location, 1f);

        foreach (Collider collider in colliders) {   
            if (collider.gameObject.name.Contains("Coral")) {
                //Debug.Log(collider.gameObject.name);
                Destroy(collider.gameObject);
                break;
            }
        }
    }

    public void Perform_BiomeSimulation() {
        for (int y = 0; y < mapResolution; y++) {
            for (int x = 0; x < mapResolution; x++) {
                    int currentBiomeIndex = BiomeMap[x, y];
                    if(currentBiomeIndex == 6) continue;

                    // Calculate scores for each neighboring cell
                    Vector2Int[] neighbours = GetNeighbours(new Vector2Int(x, y), mapResolution);
                    foreach (Vector2Int neighbourLocation in neighbours) {
                        //Vector2Int neighbourLocation = new Vector2Int(x, y) + neighbour;
                        if (IsValidPosition(neighbourLocation, mapResolution)) {
                            int neighbourBiomeIndex = BiomeMap[neighbourLocation.x, neighbourLocation.y];

                            if (neighbourBiomeIndex != currentBiomeIndex) {
                                float scoreCurrent = CalculateScoreForExpansion(currentBiomeIndex, neighbourLocation);
                                float scoreNeighbour = CalculateScoreForExpansion(neighbourBiomeIndex, neighbourLocation);

                                if (scoreCurrent > scoreNeighbour + Random.Range(0f, 0.8f)) {
                                    BiomeMap[neighbourLocation.x, neighbourLocation.y] = (byte)currentBiomeIndex;
                                    Vector3 neighbourSpawnLocation = new Vector3 (neighbourLocation.y * HeightMapScale.z, HeightMap[neighbourLocation.x, neighbourLocation.y] * HeightMapScale.y, neighbourLocation.x * HeightMapScale.x);
                                    if(neighbourBiomeIndex != 6) DeleteObjectAtLocationIfExists(neighbourSpawnLocation);
                                    if(Random.Range(0f, 1f) >= 0.95f) {
                                        var biomeConfig = Config.Biomes[currentBiomeIndex].Biome;
                                        BaseObjectPlacer[] modifiers = biomeConfig.ObjectPlacer.GetComponents<BaseObjectPlacer>();
                                        foreach(var modifier in modifiers){
                                            modifier.ExpandBiomeObject(TargetTerrain.transform, neighbourSpawnLocation, SlopeMap);
                                        }
                                    }
                                }
                            }

                            
                        }
                    }
            }
        }
    }

    void Perform_ObjectPlacement(){
        // clear out any previously-spawned objects
        for(int childIndex = TargetTerrain.transform.childCount - 1; childIndex >=0; --childIndex){
            Destroy(TargetTerrain.transform.GetChild(childIndex).gameObject);
        }

        // run object placement for each biome
        for (int biomeIndex = 0; biomeIndex < Config.NumBiomes; biomeIndex++) {
            var biomeConfig = Config.Biomes[biomeIndex].Biome;
            if(biomeConfig.ObjectPlacer == null) continue;

            BaseObjectPlacer[] modifiers = biomeConfig.ObjectPlacer.GetComponents<BaseObjectPlacer>();

            foreach(var modifier in modifiers){
                modifier.Execute(TargetTerrain.transform, mapResolution, HeightMap, HeightMapScale, SlopeMap, AlphaMaps, AlphaMapResolution, BiomeMap, biomeIndex, biomeConfig);
            }
        }
    }
}

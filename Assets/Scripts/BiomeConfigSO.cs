using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Biome Config", menuName = "Procedural Generation/Biome Configuration", order = -1)]
public class BiomeConfigSO : ScriptableObject {
    public string Name;
    public GameObject ObjectPlacer;

    public float GrowthRateAdvantage = 0.5f;
    public float TemperatureSensitivity = 0.5f;
    public float LightRequirements = 0.5f;
    public float CompetitiveAbilities = 0.5f;
    public float Resilience = 0.5f;

    [Range(0f, 1f)] public float MinIntensity = 0.5f;
    [Range(0f, 1f)] public float MaxIntensity = 1f;

    [Range(0f, 1f)] public float MinDecayRate = 0.01f;
    [Range(0f, 1f)] public float MaxDecayRate = 0.02f;
}

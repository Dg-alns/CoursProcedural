using Components.ProceduralGeneration;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using VTools.Grid;
using static Unity.Burst.Intrinsics.X86.Avx;

[CreateAssetMenu(menuName = "Procedural Generation Method/TerrainNoise")]
public class TerrainNoise : Noise
{
    [SerializeField] int width = 200;
    [SerializeField] int height = 200;

    [SerializeField] int depth = 20;

    [NonSerialized] bool isInstance = false;
    public Terrain terrain;
    protected override async UniTask ApplyGeneration(CancellationToken cancellationToken)
    {

        FastNoiseLite noise = new FastNoiseLite(RandomService.Seed);

        InitNoise(noise);

        terrain.terrainData = CreateTerrain(noise, terrain.terrainData);

        if (isInstance == false)
        {
            Instantiate(terrain);
            isInstance = true;
        }


    }

    TerrainData CreateTerrain(FastNoiseLite noise , TerrainData baseTerrain)
    {
        baseTerrain.size = new(width, depth, height);

        int resolution = baseTerrain.heightmapResolution;

        baseTerrain.SetHeights(0, 0, GenerateAllHeights(noise, resolution));

        return baseTerrain;
    }

    float[,] GenerateAllHeights(FastNoiseLite noise, int resolution)
    {
        float[,] heights = new float[resolution, resolution];
        for (int x = 0; x < resolution; x++)
        {
            for (int y = 0; y < resolution; y++)
            {
                heights[x, y] = GetNoiseData(noise, x, y);
            }

        }

        return heights;
    }


}

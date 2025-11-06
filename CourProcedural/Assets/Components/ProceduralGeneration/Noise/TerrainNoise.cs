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

    public Terrain terrain;
    protected override async UniTask ApplyGeneration(CancellationToken cancellationToken)
    {
        FastNoiseLite noise = new FastNoiseLite(RandomService.Seed);

        InitNoise(noise);

        terrain.terrainData = CreateTerrain(noise, terrain.terrainData);
        Instantiate(terrain);


    }

    TerrainData CreateTerrain(FastNoiseLite noise , TerrainData baseTerrain)
    {
        baseTerrain.size = new(width, depth, height);

        baseTerrain.SetHeights(0, 0, GenerateAllHeights(noise));

        return baseTerrain;
    }

    float[,] GenerateAllHeights(FastNoiseLite noise)
    {
        float[,] heights = new float[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                heights[x, y] = GetNoiseData(noise, x, y);
            }

        }

        return heights;
    }


}

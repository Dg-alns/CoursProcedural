using Components.ProceduralGeneration;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;
using VTools.Grid;
using VTools.RandomService;
using VTools.ScriptableObjectDatabase;
using static UnityEngine.Rendering.DebugUI.Table;

[CreateAssetMenu(menuName = "Procedural Generation Method/CellularAutomata")]
public class CellularAutomata : ProceduralGenerationMethod
{
    [SerializeField, Range(1, 100), Tooltip("Chance of water spawn")]
     int noiseDensity = 50;

    [SerializeField, Range(0, 8), Tooltip("Chance of water spawn")]
    int nbGrassAround = 4;

    protected override async UniTask ApplyGeneration(CancellationToken cancellationToken)
    {

        var time = DateTime.Now;
        //float start = Time.;
        for (int i = 0; i < Grid.Width; i++)
        {
            for (int j = 0; j < Grid.Lenght; j++)
            {
                CreateNoise(i, j);
            }
            await UniTask.Delay(GridGenerator.StepDelay, cancellationToken: cancellationToken);
        }

        Debug.Log($"Generation noise completed in {(DateTime.Now - time).TotalSeconds: 0.00} seconds.");


        //float end = Time.time;

        //Debug.Log($"Time for creat Grid of {Grid.Width} {Grid.Lenght} in {end - start}");

        List<List<string>> tmpGrid = new();

        for (int i = 0; i < _maxSteps; i++)
        {
            time = DateTime.Now;
            tmpGrid.Clear();

            ChangeAllCell(tmpGrid);

            for (int x = 0; x < Grid.Width; x++)
            {
                for (int y = 0; y < Grid.Lenght; y++)
                {
                    UpdateGrid(tmpGrid, x, y);
                }
                await UniTask.Delay(GridGenerator.StepDelay, cancellationToken: cancellationToken);
            }

            Debug.Log($"Generation step {i} completed in {(DateTime.Now - time).TotalSeconds: 0.00} seconds.");

        }







    }

    void CreateNoise(int x, int y)
    {
        if (Grid.TryGetCellByCoordinates(x, y, out Cell cell))
        {
            string type = (RandomService.Range(0, 100 + 1) <= noiseDensity) ? WATER_TILE_NAME : GRASS_TILE_NAME;

            AddTileToCell(cell, type, true);
        }        
    }

    void ChangeAllCell(List<List<string>> cells)
    {
        for (int i = 0; i < Grid.Width; i++)
        {
            List<string> row = new();
            for (int j = 0; j < Grid.Lenght; j++)
            {
                if (Grid.TryGetCellByCoordinates(i, j, out Cell cell))
                {
                    row.Add(ChangeCell(cell.Coordinates));
                }
            }

            cells.Add(row);
        }
    }

    void UpdateGrid(List<List<string>> cells, int x, int y)
    {
        if (Grid.TryGetCellByCoordinates(x, y, out Cell cell))
        {
            AddTileToCell(cell, cells[x][y], true);
        }
    }

    string ChangeCell(Vector2Int coordinates)
    {
        int nbWater = 0;
        int nbGrass = 0;

        Cell cell;

        for (int i = -1; i < 2; i++)
        {

            DetectTypeCell(coordinates.x + i, coordinates.y + 1, ref nbGrass, ref nbWater); // top

            DetectTypeCell(coordinates.x + i, coordinates.y - 1, ref nbGrass, ref nbWater); // bot

            if (new Vector2Int(coordinates.x - 1, coordinates.y) == coordinates)
                continue;

            DetectTypeCell(coordinates.x - 1, coordinates.y, ref nbGrass, ref nbWater); // middle
        }

        if (nbGrass >= nbGrassAround)
            return GRASS_TILE_NAME;
        else
            return WATER_TILE_NAME;
    }

    void DetectTypeCell(int x, int y, ref int nbGrass, ref int nbWater)
    {
        if (Grid.TryGetCellByCoordinates(x, y, out Cell cell))
        {
            if (cell.GridObject.Template.Name == GRASS_TILE_NAME)
                nbGrass++;
            else
                nbWater++;
        }
    }




}

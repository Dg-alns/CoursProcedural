using Components.ProceduralGeneration;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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

    [NonSerialized] RulesTypeCell rules  = new();

    protected override async UniTask ApplyGeneration(CancellationToken cancellationToken)
    {
        var time = DateTime.Now;

        Cell[,] cells = new Cell[Grid.Width, Grid.Lenght];
        string[,] types = new string[Grid.Width, Grid.Lenght];

        for (int i = 0; i < Grid.Width; i++)
        {
            for (int j = 0; j < Grid.Lenght; j++) {
                if (Grid.TryGetCellByCoordinates(i, j, out Cell cell))
                    cells[i, j] = cell;
            }
        }


        for (int i = 0; i < Grid.Width; i++)
        {
            for (int j = 0; j < Grid.Lenght; j++)
            {
                CreateNoise(cells[i, j]);
            }
            await UniTask.Delay(GridGenerator.StepDelay, cancellationToken: cancellationToken);
        }

        Debug.Log($"Generation noise completed in {(DateTime.Now - time).TotalSeconds: 0.00} seconds.");



        for (int i = 0; i < _maxSteps; i++)
        {
            time = DateTime.Now;

            for (int x = 0; x < Grid.Width; x++)
            {
                for (int y = 0; y < Grid.Lenght; y++)
                {
                    types[x, y] = ChangeCell(cells[x, y].Coordinates, cells);

                }

            }
            //ChangeAllCell(types, cells);

            for (int x = 0; x < Grid.Width; x++)
            {
                for (int y = 0; y < Grid.Lenght; y++)
                {
                    UpdateGrid(types[x, y], cells[x, y]);
                }
                await UniTask.Delay(GridGenerator.StepDelay, cancellationToken: cancellationToken);
            }

            Debug.Log($"Generation step {i} completed in {(DateTime.Now - time).TotalSeconds: 0.00} seconds.");

        }







    }

    void CreateNoise(Cell cell)
    {
        string type = (RandomService.Range(0, 100 + 1) <= noiseDensity) ? WATER_TILE_NAME : GRASS_TILE_NAME;

        AddTileToCell(cell, type, true);
           
    }

    void ChangeAllCell(string[,] cellsType, Cell[,] cells)
    {
    }

    void UpdateGrid(string cellsType, Cell cell)
    {
        if(cell.GridObject.Template.Name != cellsType)
          AddTileToCell(cell, cellsType, true);
        
    }

    string ChangeCell(Vector2Int coordinates, Cell[,] cells)
    {
        int nbWater = 0;
        int nbGrass = 0;

        //rules.ResetAllType();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                DetectTypeCell(cells, coordinates.x + x, coordinates.y + y, ref nbGrass, ref nbWater);
            }
        }

        return (nbGrass >= nbGrassAround) ? GRASS_TILE_NAME : WATER_TILE_NAME;

        //return rules.ApplyRules(cells[coordinates.x, coordinates.y]);
    }

    void DetectTypeCell(Cell[,] cells, int x, int y, ref int nbGrass, ref int nbWater)
    {
        if (x < 0 || y < 0 || x >= Grid.Width || y >= Grid.Lenght)
            return;

        Cell cell = cells[x, y];
        if (cell == null)
            return;

        //rules.AddTypeCell(cell.GridObject.Template.Name);

        if (cell.GridObject.Template.Name == GRASS_TILE_NAME)
            nbGrass++;
        else
            nbWater++;

    }




}

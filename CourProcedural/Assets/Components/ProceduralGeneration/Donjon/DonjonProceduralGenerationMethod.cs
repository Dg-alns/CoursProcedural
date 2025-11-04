using Components.ProceduralGeneration;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;
using VTools.Grid;
using VTools.ScriptableObjectDatabase;

[CreateAssetMenu(menuName = "Procedural Generation Method/Donjon Generation")]
public class DonjonGenerationMethod : ProceduralGenerationMethod
{
    protected override async UniTask ApplyGeneration(CancellationToken cancellationToken)
    {
        await UniTask.Delay(GridGenerator.StepDelay, cancellationToken: cancellationToken);
    }

    protected void BuildGround()
    {
        var groundTemplate = ScriptableObjectDatabase.GetScriptableObject<GridObjectTemplate>("Grass");

        // Instantiate ground blocks
        for (int x = 0; x < Grid.Width; x++)
        {
            for (int z = 0; z < Grid.Lenght; z++)
            {
                if (!Grid.TryGetCellByCoordinates(x, z, out var chosenCell))
                {
                    Debug.LogError($"Unable to get cell on coordinates : ({x}, {z})");
                    continue;
                }

                GridGenerator.AddGridObjectToCell(chosenCell, groundTemplate, false);
            }
        }
    }


    protected void BuildRoom(RectInt room)
    {
        for (int x = room.position.x; x < room.position.x + room.width; x++)
        {
            for (int y = room.position.y; y < room.position.y + room.height; y++)
            {

                if (Grid.TryGetCellByCoordinates(x, y, out var cell))
                {
                    AddTileToCell(cell, ROOM_TILE_NAME, false);
                }
            }
        }
    }
}

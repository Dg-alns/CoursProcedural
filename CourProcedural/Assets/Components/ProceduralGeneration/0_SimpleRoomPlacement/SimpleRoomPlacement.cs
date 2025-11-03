using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VTools.Grid;
using VTools.ScriptableObjectDatabase;

namespace Components.ProceduralGeneration.SimpleRoomPlacement
{
    [CreateAssetMenu(menuName = "Procedural Generation Method/Simple Room Placement")]
    public class SimpleRoomPlacement : ProceduralGenerationMethod
    {
        [Header("Room Parameters")]
        [SerializeField] private int _maxRooms = 10;
        
        protected override async UniTask ApplyGeneration(CancellationToken cancellationToken)
        {
            // Declare variables here
            // ........

            int curentRoomsNb = 0;

            for (int i = 0; i < _maxSteps; i++)
            {
                // Check for cancellation
                cancellationToken.ThrowIfCancellationRequested();

                // Your algorithm here
                // .......

                if (curentRoomsNb >= _maxRooms)
                    break;

                int rdmX = RandomService.Range(0, Grid.Width +1);
                int rdmY = RandomService.Range(0, Grid.Lenght +1);

                int rdmW = RandomService.Range(5, 10);
                int rdmL = RandomService.Range(4, 12);

                if (Grid.TryGetCellByCoordinates(rdmX, rdmY, out var e) == false)
                {
                    continue;
                }



                RectInt room = new(new(rdmX, rdmY), new(rdmW, rdmL));

                if (CanPlaceRoom(room, 1))
                {
                    BuildRoom(room);

                    curentRoomsNb++;
                }




                // Waiting between steps to see the result.
                await UniTask.Delay(GridGenerator.StepDelay, cancellationToken : cancellationToken);
            }
            
            // Final ground building.
            BuildGround();
        }
        
        private void BuildGround()
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


        private void BuildRoom(RectInt room)
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
}
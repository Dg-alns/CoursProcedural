using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;
using VTools.Grid;
using VTools.ScriptableObjectDatabase;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
class Room
{
    public int id;
    public bool haveCorridor;
    public RectInt roomRect;

    public Dictionary<string, bool> cardinalPointsLink;

    public Room() 
    { 
        id = -1;
        roomRect = new RectInt();
        haveCorridor = false;

        cardinalPointsLink = new() { ["North"] = false, ["South"] = false, ["East"] = false, ["West"] = false };
    }

    public Room(int id, RectInt room)
    {
        this.id = id;
        roomRect = room;
        haveCorridor = false;

        cardinalPointsLink = new() { ["North"] = false, ["South"] = false, ["East"] = false, ["West"] = false };
    }

}

public enum TYPECORRIDOR
{
    North,
    South,
    West,
    East,

    None
}

namespace Components.ProceduralGeneration.SimpleRoomPlacement
{
    [CreateAssetMenu(menuName = "Procedural Generation Method/Simple Room Placement")]
    public class SimpleRoomPlacement : DonjonGenerationMethod
    {
        [Header("Room Parameters")]
        [SerializeField] private int _maxRooms = 10;

        protected override async UniTask ApplyGeneration(CancellationToken cancellationToken)
        {
            // Declare variables here
            // ........

            int curentRoomsNb = 0;
            List<Room> allRooms = new();

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

                if (CanPlaceRoom(room, 3))
                {
                    BuildRoom(room);

                    curentRoomsNb++;

                    allRooms.Add(new(curentRoomsNb, room));
                }

                // Waiting between steps to see the result.
                await UniTask.Delay(GridGenerator.StepDelay, cancellationToken : cancellationToken);
            }
            //Create Corridor

            Room start = GetFirstLeftRoom(allRooms);

            Room end = GetRoomWithMinusDistance(start, allRooms);

            for (int i = 0; i < allRooms.Count; i++)
            {

                if (end.id == -1)
                    break;


                start.haveCorridor = true;
                BuildCorridor(start, end);


                start = end;

                end = GetRoomWithMinusDistance(start, allRooms);



                await UniTask.Delay(GridGenerator.StepDelay, cancellationToken: cancellationToken);
            }



            // Final ground building.
            BuildGround();


        }



        Room GetFirstLeftRoom(List<Room> rooms)
        {
            if (rooms.Count == 0)
                return new();

            List<Room> result = new();

            Room current = rooms[0];

            // Detect one room left
            foreach (Room room in rooms)
            {
                if (room.haveCorridor)
                    continue;

                if(room.roomRect.position.x <=  current.roomRect.position.x)
                    current = room;
            }

            result.Add(current);

            foreach (Room room in rooms)
            {
                if (room.haveCorridor)
                    continue;

                if (room.roomRect.position.x == current.roomRect.position.x && current.id != room.id)
                    result.Add(room);
            }


            if (result.Count > 1)
                return GetFirstUpRoom(result);
            else
                return result[0];
        }

        Room GetFirstUpRoom(List<Room> rooms)
        {
            Room current = rooms[0];

            foreach (Room room in rooms)
            {
                if (room.haveCorridor)
                    continue;

                if (room.roomRect.position.y > current.roomRect.position.y)
                    current = room;
            }

            return current;
        }


        Room GetRoomWithMinusDistance(Room start, List<Room> rooms)
        {
            if(rooms.Count == 0) throw new System.Exception("Error size rooms");


            Room current = new();

            foreach (Room room in rooms)
            {
                if (room.haveCorridor)
                    continue;

                if (room.id != start.id)
                {
                    current = room;
                    break;
                }
            }


            foreach (Room room in rooms)
            {
                if (room.haveCorridor)
                    continue;

                if (room.id == start.id)
                    continue;

                if (Vector2.Distance(room.roomRect.position, start.roomRect.position) < Vector2.Distance(current.roomRect.position, start.roomRect.position))
                    current = room;
            }

            return current;
        }



        void BuildCorridor(Room start, Room end)
        {
            TYPECORRIDOR horizontal = TYPECORRIDOR.None;
            TYPECORRIDOR vertical = TYPECORRIDOR.None;


            horizontal = (start.roomRect.position.x > end.roomRect.position.x) ? TYPECORRIDOR.West : (start.roomRect.position.x == end.roomRect.position.x) ? TYPECORRIDOR.None : TYPECORRIDOR.East;

            vertical = (start.roomRect.position.y > end.roomRect.position.y) ? TYPECORRIDOR.South : (start.roomRect.position.y == end.roomRect.position.y) ? TYPECORRIDOR.None : TYPECORRIDOR.North;


            if (horizontal == TYPECORRIDOR.None && vertical == TYPECORRIDOR.None) throw new System.Exception("Error Type are none");


            // create simulation corridor

            CreateCorridor(start.roomRect.center, end.roomRect.center, GetLinkOffRoom(end, horizontal, vertical), horizontal, vertical);
        }


        TYPECORRIDOR GetLinkOffRoom(Room end, TYPECORRIDOR horizontal, TYPECORRIDOR vertical)
        {
            if(vertical != TYPECORRIDOR.None)
            {
                if (horizontal != TYPECORRIDOR.None && end.cardinalPointsLink[horizontal.ToString()] == false)
                {
                    ActivePoint(horizontal, end.cardinalPointsLink);
                    return horizontal;
                }

                ActivePoint(vertical, end.cardinalPointsLink);
                return vertical;
            }

            if(horizontal != TYPECORRIDOR.None)
            {
                if (vertical != TYPECORRIDOR.None && end.cardinalPointsLink[vertical.ToString()] == false)
                {
                    ActivePoint(vertical, end.cardinalPointsLink);
                    return vertical;
                }

                ActivePoint(horizontal, end.cardinalPointsLink);
                return horizontal;
            }

            return TYPECORRIDOR.None;
        }

        TYPECORRIDOR GetOpposite(TYPECORRIDOR type)
        {
            switch (type)
            {
                case TYPECORRIDOR.North:
                    return TYPECORRIDOR.South;
                case TYPECORRIDOR.South:
                    return TYPECORRIDOR.North;
                case TYPECORRIDOR.West:
                    return TYPECORRIDOR.East;
                case TYPECORRIDOR.East:
                    return TYPECORRIDOR.West;
                default:
                    return TYPECORRIDOR.None;
            }
        }

        void ActivePoint(TYPECORRIDOR type, Dictionary<string, bool> cardinalPointsLink)
        {
            cardinalPointsLink[type.ToString()] = true;
        }


        void CreateCorridor(Vector2 centerStart, Vector2 CenterEnd, TYPECORRIDOR link, TYPECORRIDOR horizontal, TYPECORRIDOR vertical)
        {
            int offsetX = (int)Mathf.Abs(centerStart.x - CenterEnd.x);

            int offsetY = (int)Mathf.Abs(centerStart.y - CenterEnd.y);


            // Creat Road to start at link

            Vector2Int direction = link switch
            {
                TYPECORRIDOR.North => new(0, 1),
                TYPECORRIDOR.South => new(0, -1),

                TYPECORRIDOR.West => new(1, 0),
                TYPECORRIDOR.East => new(-1, 0),

                _ => Vector2Int.zero
            };


            int distance = (direction.x != 0) ? offsetX : offsetY;


            for (int i = 0; i <= distance; i++)
            {
                int x = (int)CenterEnd.x + direction.x * i;
                int y = (int)CenterEnd.y + direction.y * i;

                if (Grid.TryGetCellByCoordinates(x, y, out var cell))
                {
                    AddTileToCell(cell, CORRIDOR_TILE_NAME, false);
                }
            }



            // Create second part off corridor

            if (vertical == TYPECORRIDOR.None || horizontal == TYPECORRIDOR.None)
                return;

            direction = ((link == horizontal) ? vertical : horizontal) switch
            {
                TYPECORRIDOR.North => new(0, 1),
                TYPECORRIDOR.South => new(0, -1),

                TYPECORRIDOR.West => new(1, 0),
                TYPECORRIDOR.East => new(-1, 0),

                _ => Vector2Int.zero
            };


            distance = (direction.x != 0) ? offsetX : offsetY;


            for (int i = 0; i <= distance; i++)
            {
                int x = (int)centerStart.x + direction.x * i;
                int y = (int)centerStart.y + direction.y * i;

                if (Grid.TryGetCellByCoordinates(x, y, out var cell))
                {
                    AddTileToCell(cell, CORRIDOR_TILE_NAME, false);
                }
            }

        }
    }
}
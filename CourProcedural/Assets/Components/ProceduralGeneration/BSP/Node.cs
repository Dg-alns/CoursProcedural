using System.Collections.Generic;
using UnityEngine;
using VTools.RandomService;

public class Node
{
    RectInt room;
    RectInt visualRoom;

    Node? child1;
    Node? child2;

    RandomService randomService;

    bool isRoot;

    Vector2Int minSize = new(8, 8);


    public Node(RandomService randomService, RectInt room, bool isRoot, Node child1 = null, Node child2 = null)
    {
        this.randomService = randomService;
        this.room = room;
        this.child1 = child1;
        this.child2 = child2;
        this.isRoot = isRoot;
    }

    public Node(RandomService randomService, RectInt room, Node child1 = null, Node child2 = null)
    {
        this.randomService = randomService;
        this.room = room;
        this.child1 = child1;
        this.child2 = child2;
        isRoot = false;
    }

    public RandomService RandomService => randomService;
    public Node Child1 => child1;
    public Node Child2 => child2;
    public RectInt Room => room;
    public RectInt VisualRoom => visualRoom;
    public Vector2Int MinSize => minSize;

    public Node GetLastChild()
    {
        if (child1 == null)
            return this;

        return child1.GetLastChild();
    }


    public void Cut()
    {
        int cutSens = SensOffCut();

        int posCut = PositionOffCut(cutSens);

        if (cutSens == 0) // horizontal
        {

            RectInt Child1Room = new(room.position.x, room.position.y, room.width, posCut);
            RectInt Child2Room = new(room.position.x, room.position.y + posCut, room.width, (room.height - posCut));

            child1 = new(RandomService, Child1Room);
            child2 = new(RandomService, Child2Room);
        }

        if (cutSens == 1) // vertical
        {
            RectInt Child1Room = new(room.position.x, room.position.y, posCut, room.height);
            RectInt Child2Room = new(room.position.x + posCut, room.position.y, (room.width - posCut), room.height);

            child1 = new(RandomService, Child1Room);
            child2 = new(RandomService, Child2Room);
        }
    }

    public int SensOffCut()
    {
        bool value = RandomService.Chance(0.5f);

        if (value)
            return 0;
        else
            return 1;
    }

    public int PositionOffCut(int sens)
    {
        if (sens == 0)
        {
            int min = minSize.y;
            int max = room.height - minSize.y;

            if(max <= min)
                return room.height / 2;

            return RandomService.Range(min, max);
        }

        else
        {
            int min = minSize.x;
            int max = room.width - minSize.x;

            if (max <= min)
                return room.width / 2;

            return RandomService.Range(min, max);
        }
    }

    public RectInt CreateRoom()
    {
        int x = room.x + RandomService.Range(1, Mathf.Max(2, room.width / 4));
        int y = room.y + RandomService.Range(1, Mathf.Max(2, room.height / 4));

        int width = room.width - (x - room.x) - RandomService.Range(1, Mathf.Max(2, room.width / 4));
        int height = room.height - (y - room.y) - RandomService.Range(1, Mathf.Max(2, room.height / 4));

        visualRoom = new RectInt(x, y, width, height);
        return visualRoom;
    }


    public (List<int>, int) DetecteSamePositionOffChildren(Node _child1, Node _child2)
    {
        RectInt child1 = _child1.visualRoom;
        RectInt child2 = _child2.visualRoom;



        bool sameCenterX = _child1.room.center.x == _child2.room.center.x;
        bool sameCenterY = _child1.room.center.y == _child2.room.center.y;

        Debug.Log($"{child1.center.x} == {child2.center.x}");
        Debug.Log($"{child1.center.y} == {child2.center.y}");

        Debug.Log(sameCenterX);
        Debug.Log(sameCenterY);

        if (sameCenterX)
        {
            List<int> allPosChild1 = new();
            List<int> same = new();

            for (int x = child1.xMin; x < child1.xMax; x++)
            {
                allPosChild1.Add(x);
            }

            for (int x = child2.xMin; x < child2.xMax; x++)
            {
                if (allPosChild1.Contains(x))
                {
                    same.Add(x);
                }
            }

            return (same, 0); // top->bot ou bot->top
        }

        if (sameCenterY)
        {
            List<int> allPosChild1 = new();
            List<int> same = new();

            for (int y = child1.yMin; y < child1.yMax; y++)
            {
                allPosChild1.Add(y);
            }

            for (int y = child2.yMin; y < child2.yMax; y++)
            {
                if (allPosChild1.Contains(y))
                {
                    same.Add(y);
                }
            }

            return (same, 1); // right->left ou left->right
        }



        return (new(), -1);

    }



}

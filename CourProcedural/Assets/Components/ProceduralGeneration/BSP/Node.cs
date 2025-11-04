using System.Collections.Generic;
using UnityEngine;
using VTools.RandomService;

public class Node
{
    RectInt room;

    Node? child1;
    Node? child2;

    RandomService randomService;

    bool isRoot;

    int spacing = 3;

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


    bool HaveChildren() => child1 != null && child2 != null;



    public void CutRoom()
    {
        int cutSens = SensOffCut();

        if (cutSens == 0)// horizontal
        {
            int positionOffCut = PositionOffCut(cutSens);
            Debug.Log("Horizontal");
            Debug.Log($"Position cut => {positionOffCut}");

            RectInt Child1Room = new(room.x, room.y, positionOffCut, room.size.y);
            RectInt Child2Room = new(room.x + positionOffCut, room.y, (room.size.x - positionOffCut), room.size.y);

            Debug.Log($"Child1 => {Child1Room}");
            Debug.Log($"Child1 => {Child2Room}");

            child1 = new(RandomService, Child1Room);
            child2 = new(RandomService, Child2Room);
        }

        if (cutSens == 1) // vertical
        {
            int positionOffCut = PositionOffCut(cutSens);
            Debug.Log("Vertical");

            Debug.Log($"Position cut => {positionOffCut}");

            RectInt Child1Room = new(room.x, room.y, room.size.x, positionOffCut);
            RectInt Child2Room = new(room.x, room.y + positionOffCut, room.size.x, (room.size.y - positionOffCut));

            Debug.Log($"Child1 => {Child1Room}");
            Debug.Log($"Child1 => {Child2Room}");

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
        if(sens == 0)
            return RandomService.Range(room.x + spacing, room.x + room.size.x - spacing);

        else
            return RandomService.Range(room.y + spacing, room.y + room.size.y - spacing);
    }


    public RectInt CreateRoom()
    {
        int space = 1;

        Debug.Log("Room");
        Debug.Log($"x => {room.position.x}");
        Debug.Log($"y => {room.position.y}");

        Debug.Log($"width => {room.size.x}");
        Debug.Log($"height => {room.size.y}");


        int rdmPosX = RandomService.Range(room.x + space, room.size.x - space);
        int rdmPosY = RandomService.Range(room.y + space, room.size.y - space);

        int rdmW = RandomService.Range(space, room.size.x - rdmPosX);
        int rdmH = RandomService.Range(space, room.size.y - rdmPosY);

        return new(rdmPosX, rdmPosY, rdmW, rdmH);
    }

}

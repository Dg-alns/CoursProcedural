using Components.ProceduralGeneration;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;
using VTools.RandomService;
using static UnityEngine.Rendering.DebugUI.Table;

[CreateAssetMenu(menuName = "Procedural Generation Method/BSP")]
public class BSP : DonjonGenerationMethod
{
    [SerializeField] int nbLeafs;

    [NonSerialized] List<List<Node>> nodes = new();

    [NonSerialized] int currentLeafs = 0;

    protected override async UniTask ApplyGeneration(CancellationToken cancellationToken)
    {
        nodes = new();
        currentLeafs = 0;

        RectInt GridInt = new(0, 0, Grid.Width, Grid.Lenght);
        Node gridNode = new(RandomService, GridInt, true);

        nodes.Add(new() { gridNode });

        for (int i = 0; i < nbLeafs; i++)
        {
            // Check for cancellation
            cancellationToken.ThrowIfCancellationRequested();

            //Debug.Log($"{currentLeafs} >= {nbLeafs} ");

            if (currentLeafs >= nbLeafs)
                break;

            //Debug.Log($"{i} > {nodes.Count} ");

            if (i >= nodes.Count)
                break;

            SplitGeneration(i);
            await UniTask.Delay(GridGenerator.StepDelay, cancellationToken: cancellationToken);
        }


        List<Node> Leafs = nodes[nodes.Count - 1];


        for (int j = 0; j < Leafs.Count; j++)
        {
            Node currentNode = Leafs[j];

            RectInt room = currentNode.CreateRoom();

            BuildRoom(room);


            await UniTask.Delay(GridGenerator.StepDelay, cancellationToken: cancellationToken);
        }

        for (int l = 0; l < nodes[nodes.Count - 2].Count; l++)
        {
            Node currentNode = nodes[nodes.Count - 2][l];

            CreateCorridorLeaf(currentNode);
            await UniTask.Delay(GridGenerator.StepDelay, cancellationToken: cancellationToken);
        }


        for (int i = 0; i < nodes.Count - 2; i++)
        {
            for (int l = 0; l < nodes[i].Count; l++)
            {
                Node currentNode = nodes[i][l];
                Node ChildA1 = currentNode.Child1.GetLastChild();
                Node ChildB1 = null;

                if (l + 1 < nodes[i].Count && nodes[i][l + 1] != null)
                {
                    ChildB1 = nodes[i][l + 1].Child1?.GetLastChild();
                }
                else
                {
                    ChildB1 = currentNode.Child2?.GetLastChild();
                }



                if (ChildA1 == null || ChildB1 == null)
                {
                    continue;
                }


                CreateOtherCorridor(currentNode, ChildA1, ChildB1);

                await UniTask.Delay(GridGenerator.StepDelay, cancellationToken: cancellationToken);
            }
        }







        BuildGround();
    }


    void SplitGeneration(int nbGeneration)
    {
        Debug.Log($"Generation {nbGeneration}");

        List<Node> childrend = new();

        for (int i = 0; i < nodes[nbGeneration].Count; i++)
        {
            Node currentNode = nodes[nbGeneration][i];

            if (currentNode.Room.width <= currentNode.MinSize.x || currentNode.Room.height <= currentNode.MinSize.y)
            {
                return;
            }


            currentNode.Cut();

            childrend.Add(currentNode.Child1);
            childrend.Add(currentNode.Child2);

            currentLeafs = childrend.Count;
        }

        if (childrend.Count <= 0)
            return;

        nodes.Add(childrend);
    }

    void CreateOtherCorridor(Node node, Node ChildA1, Node ChildB1)
    {
        CreateCorridor(node, ChildA1, ChildB1);
    }


    void CreateCorridorLeaf(Node node)
    {
        CreateCorridor(node, node.Child1, node.Child2);

    }

    void CreateCorridor(Node node, Node Child1, Node Child2)
    {

        (List<int>, int) samepos = node.DetecteSavePositionOffChildren(Child1, Child2);

        int start;
        int end;
        int step;

        int point = SelectPointinLst(samepos.Item1);


        if (samepos.Item2 == 1 || samepos.Item2 == -1)
        {

            start = (int)Child1.Room.center.y;
            end = (int)Child2.Room.center.y;
            step = start < end ? 1 : -1;


            for (int i = start; i != end + step; i += step)
            {

                if (Grid.TryGetCellByCoordinates(point, i, out var cell))
                {
                    AddTileToCell(cell, CORRIDOR_TILE_NAME, false);
                }
            }
        }


        else if (samepos.Item2 == 10 || samepos.Item2 == -10)
        {
            start = (int)Child1.Room.center.x;
            end = (int)Child2.Room.center.x;
            step = start < end ? 1 : -1;

            for (int i = start; i != end + step; i += step)
            {

                if (Grid.TryGetCellByCoordinates(i, point, out var cell))
                {
                    AddTileToCell(cell, CORRIDOR_TILE_NAME, false);
                }
            }

        }
    }

    int SelectPointinLst(List<int> lst)
    {
        if (lst == null || lst.Count == 0) return 0;

        int idx = RandomService.Range(0, lst.Count);

        return lst[idx];
    }

}

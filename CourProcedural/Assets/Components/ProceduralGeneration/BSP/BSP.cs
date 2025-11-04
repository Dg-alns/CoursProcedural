using Components.ProceduralGeneration;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using VTools.RandomService;

[CreateAssetMenu(menuName = "Procedural Generation Method/BSP")]
public class BSP : DonjonGenerationMethod
{
    [SerializeField] int nbGeneration;

    [NonSerialized] List<List<Node>> nodes = new();

    protected override async UniTask ApplyGeneration(CancellationToken cancellationToken)
    {
        RectInt GridInt = new(0, 0, Grid.Width, Grid.Lenght);
        Node gridNode = new(RandomService, GridInt, true);

        nodes.Add(new() { gridNode });

        for (int i = 0; i < nbGeneration + 1; i++)
        {
            // Check for cancellation
            cancellationToken.ThrowIfCancellationRequested();

            SplitGeneration(i);
        }

        //List<Node> Leafs = nodes[nodes.Count - 1];

        //for (int j = 0; j < Leafs.Count; j++)
        //{
        //    Node currentNode = Leafs[j];

        //    RectInt room = currentNode.CreateRoom();

        //    BuildRoom(room);


        //    await UniTask.Delay(GridGenerator.StepDelay, cancellationToken: cancellationToken);
        //}




        BuildGround();
    }


    void SplitGeneration(int nbGeneration)
    {
        List<Node> childrend = new();

        for (int i = 0; i < nodes[nbGeneration].Count; i++)
        {
            Debug.Log(nodes[nbGeneration][i].Room);

            nodes[nbGeneration][i].CutRoom();

            childrend.Add(nodes[nbGeneration][i].Child1);
            childrend.Add(nodes[nbGeneration][i].Child2);
        }

        nodes.Add(childrend);
    }

}

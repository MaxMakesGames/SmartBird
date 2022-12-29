using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AI : MonoBehaviour
{
    public NeuralNetwork network;
    public static int AICount;
    public float randomization = 0.5f;

    void Start()
    {
        network = NetworkSaver.LoadNetworkFromFile("Network.txt"); //try to load previous best
        if (network == null)
        {
            //if there is no saved data, create a fresh random network
            print("First network run");
            network = new NeuralNetwork(new int[] { 2, 1 });
            network.RandomizeLayers(2.0f);
        }
        else if(AICount > 0)
        {
            //else if its the first AI ( AICount = 0 ) keep the saved data and if AICount > 0 randomize a bit around saved data

            //(optional) have a chance to create a new completely random AI, this can prevent your network getting stuck forever
            if (Random.value < 0.05f)
            {
                network.RandomizeLayers(2.0f);
            }
            else
            {
                network.SlightlyRandomizeLayers(randomization);
            }
        }
        AICount++;
        network.FixFroms();
    }
}

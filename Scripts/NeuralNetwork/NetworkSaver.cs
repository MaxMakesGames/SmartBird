using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkSaver
{
    public int[] layerSizes;
    public Node[] nodes;

    public NeuralNetwork LoadNetwork()
    {
        NeuralNetwork network = new NeuralNetwork(layerSizes);
        int nodeindex = 0;
        for(int i = 0; i < network.layers.Length; i++)
        {
            for(int n = 0; n < network.layers[i].nodes.Length; n++)
            {
                network.layers[i].nodes[n] = nodes[nodeindex];
                nodeindex++;
            }
        }
        return network;
    }

    public static NeuralNetwork LoadNetworkFromFile(string path)
    {
        try
        {
            System.IO.StreamReader reader = new System.IO.StreamReader(path);
            string data = reader.ReadToEnd();
            reader.Close();
            return UnityEngine.JsonUtility.FromJson<NetworkSaver>(data).LoadNetwork();
        }
        catch
        {
            return null;
        }
    }

    public static void SaveToFile(NeuralNetwork network, string path)
    {
        System.IO.StreamWriter writer = new System.IO.StreamWriter(path);
        string s = NetworkToString(network);
        writer.Write(s);
        writer.Close();
    }
    public static string NetworkToString(NeuralNetwork network)
    {
        NetworkSaver saver = new NetworkSaver();
        saver.layerSizes = network.layerSizes;
        saver.nodes = new Node[network.nodeCount];
        int nodeindex = 0;
        foreach(Layer layer in network.layers)
        {
            for(int n = 0; n < layer.nodes.Length; n++)
            {
                saver.nodes[nodeindex] = layer.nodes[n];
                nodeindex++;
            }
        }
        return UnityEngine.JsonUtility.ToJson(saver);
    }
}

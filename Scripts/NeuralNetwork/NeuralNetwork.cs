using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Connection
{
    public double weight;
    public Node from;

    public Connection(double weight, Node from)
    {
        this.weight = weight;
        this.from = from;
    }
}

[Serializable]
public class Node
{
    public int id;
    public double bias;
    public Connection[] inConnections;
    public double output;

    public Node(int id, double bias, Node[] connectionsNodess)
    {
        this.id = id;
        this.bias = bias;
        inConnections = new Connection[connectionsNodess.Length];
        for (int i = 0; i < connectionsNodess.Length; i++)
        {
            inConnections[i] = new Connection(1.0f, connectionsNodess[i]);
        }
        output = 0;
    }
}

public class Layer
{
    public Node[] nodes;

    public Layer(Node[] nodes)
    {
        this.nodes = nodes;
    }

    double Activate(double weightedInput)
    {
        return 1 / (1 + Mathf.Exp(-((float)weightedInput)));
    }

    public void CalculateOutputsFromInput(double[] inputs)
    {
        for(int n = 0; n < nodes.Length; n++)
        {
            double weightedInput = inputs[n] + nodes[n].bias;
            nodes[n].output = Activate(weightedInput);
        }
    }

    public void CalculateOutputs(NeuralNetwork network)
    {
        for(int n = 0; n < nodes.Length; n++)
        {
            double weightedInput = nodes[n].bias;
            for(int i = 0; i < nodes[n].inConnections.Length; i++)
            {
                weightedInput += nodes[n].inConnections[i].from.output * nodes[n].inConnections[i].weight;
            }
            nodes[n].output = Activate(weightedInput);
        }
    }

    public double[] GetOutputs()
    {
        double[] outputs = new double[nodes.Length];
        for(int n = 0; n < nodes.Length; n++)
        {
            outputs[n] = nodes[n].output;
        }
        return outputs;
    }

    public void Randomize(float range)
    {
        foreach(Node n in nodes)
        {
            foreach(Connection c in n.inConnections)
            {
                c.weight = Random.Range(-range, range);
            }
            n.bias = Random.Range(-range, range);
        }
    }

    public void SlightlyRandomize(float range)
    {
        foreach (Node n in nodes)
        {
            foreach (Connection c in n.inConnections)
            {
                c.weight += Random.Range(-range, range);
            }
            n.bias += Random.Range(-range, range);
        }
    }
}

public class NeuralNetwork
{
    public Layer[] layers;
    public int[] layerSizes;
    public int nodeCount;

    public NeuralNetwork(int[] layerSizes)
    {
        this.layerSizes = layerSizes;
        layers = new Layer[layerSizes.Length];

        ConnectAll();
    }

    public Node GetNodeWithID(int id)
    {
        foreach(Layer layer in layers)
        {
            foreach(Node n in layer.nodes)
            {
                if (n.id == id)
                    return n;
            }
        }
        return null;
    }

    public void FixFroms()
    {
        foreach (Layer layer in layers)
        {
            foreach (Node n in layer.nodes)
            {
                foreach(Connection c in n.inConnections)
                {
                    c.from = GetNodeWithID(c.from.id);
                }
            }
        }
    }

    public double[] GetOutputs(double[] inputs)
    {
        layers[0].CalculateOutputsFromInput(inputs);
        for(int i = 1; i < layers.Length; i++)
        {
            layers[i].CalculateOutputs(this);
        }
        return layers[layers.Length - 1].GetOutputs();
    }

    public void ConnectAll()
    {
        for(int i = 0; i < layers.Length; i++)
        {
            Node[] nodes = new Node[layerSizes[i]];
            for(int n = 0; n < nodes.Length; n++)
            {
                Node[] connections;
                if(i <= 0)
                {
                    connections = new Node[0];
                }
                else
                {
                    connections = new Node[layerSizes[i-1]]; //previous layer
                    for(int k = 0; k < layerSizes[i - 1]; k++)
                    {
                        Node connectNode = layers[i - 1].nodes[k];
                        connections[k] = connectNode;
                    }
                }

                Node node = new Node(nodeCount, 0, connections);
                nodeCount++;
                nodes[n] = node;
            }
            layers[i] = new Layer(nodes);
        }
    }

    public void RandomizeLayers(float range)
    {
        foreach(Layer layer in layers)
        {
            layer.Randomize(range);
        }
    }

    public void SlightlyRandomizeLayers(float range)
    {
        foreach (Layer layer in layers)
        {
            layer.SlightlyRandomize(range);
        }
    }
}

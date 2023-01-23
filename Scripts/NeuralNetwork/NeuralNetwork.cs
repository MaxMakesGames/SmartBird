using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Connection
{
    public double weight;
    public NeuralNode from;

    public Connection(double weight, NeuralNode from)
    {
        this.weight = weight;
        this.from = from;
    }
}

[Serializable]
public class NeuralNode
{
    public int id;
    public double bias;
    public Connection[] inConnections;
    public double output;

    public NeuralNode(int id, double bias, NeuralNode[] connectionsNodess)
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
    public NeuralNode[] nodes;

    public Layer(NeuralNode[] nodes)
    {
        this.nodes = nodes;
    }

    double Activate(double weightedInput)
    {
        return 1 / (1 + Mathf.Exp(-((float)weightedInput)));
    }
    
#region Output calculations
    /// <summary>
    /// Calculate outputs for the first layers
    /// </summary>
    /// <param name="inputs"></param>
    public void CalculateOutputsFromInputs(double[] inputs)
    {
        for (int nodeIndex = 0; nodeIndex < nodes.Length; nodeIndex++)
        {
            double weightedInput = inputs[nodeIndex] + nodes[nodeIndex].bias;
            nodes[nodeIndex].output = Activate(weightedInput);
        }
    }
    /// <summary>
    /// Calculate outputs for the hidden layers
    /// </summary>
    public void CalculateOutputs()
    {
        for (int nodeIndex = 0; nodeIndex < nodes.Length; nodeIndex++)
        {
            double weightedInput = nodes[nodeIndex].bias;
            Connection[] connections = nodes[nodeIndex].inConnections;
            for (int connectionIndex = 0; connectionIndex < connections.Length; connectionIndex++)
            {
                Connection connection = connections[connectionIndex];
                weightedInput += connection.from.output * connection.weight;
            }
            nodes[nodeIndex].output = Activate(weightedInput);
        }
    }
    public double[] GetLayerOutputs()
    {
        double[] outputs = new double[nodes.Length];
        for(int n = 0; n < nodes.Length; n++)
        {
            outputs[n] = nodes[n].output;
        }
        return outputs;
    }
#endregion
#region Randomizing
    public void Randomize(float range)
    {
        foreach(NeuralNode n in nodes)
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
        foreach (NeuralNode n in nodes)
        {
            foreach (Connection c in n.inConnections)
            {
                c.weight += Random.Range(-range, range);
            }
            n.bias += Random.Range(-range, range);
        }
    }
#endregion
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
#region Unnecessary functions?
    public NeuralNode GetNodeWithID(int id)
    {
        foreach(Layer layer in layers)
        {
            foreach(NeuralNode n in layer.nodes)
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
            foreach (NeuralNode n in layer.nodes)
            {
                foreach(Connection c in n.inConnections)
                {
                    c.from = GetNodeWithID(c.from.id);
                }
            }
        }
    }
#endregion
#region Getting Outputs by the neural network  
    /// <summary>
    /// Calculate the outputs of the neural network
    /// </summary>
    /// <param name="inputs"></param>
    /// <returns></returns>
    public double[] GetOutputs(double[] inputs)
    {
        layers[0].CalculateOutputsFromInput(inputs);
        for(int i = 1; i < layers.Length; i++)
        {
            layers[i].CalculateOutputs(this);
        }
        return layers[layers.Length - 1].GetLayerOutputs();
    }
#endregion
#region Connecting nodes of this layer with previous layer
    /// <summary>
    /// For each layer, connect all nodes with the previous layer
    /// </summary>
    public void ConnectAll(){
        for (int layerIndex = 0; layerIndex < layers.Length; layerIndex++)
        {
            NeuralNode[] currentLayersNewNodes = new NeuralNode[layerSizes[layerIndex]];
            NeuralNode[] connectingNodes = GetConnectingNodesWithPreviousLayer(layerIndex);

            for (int nodeIndex = 0; nodeIndex < currentLayersNewNodes.Length; nodeIndex++)
            {
                int newNodeId = nodeCount++;
                int newNodeBias = 0;
                NeuralNode node = new NeuralNode(newNodeId, newNodeBias, connectingNodes);
                currentLayersNewNodes[nodeIndex] = node;
            }
            layers[layerIndex] = new Layer(currentLayersNewNodes);
        }
    }
    
    /// <summary>
    /// Get all the nodes of the previous layer
    /// </summary>
    /// <param name="layerIndex">The current index layer</param>
    /// <returns></returns>
    private NeuralNode[] GetConnectingNodesWithPreviousLayer(int layerIndex)
    {
        if (layerIndex == 0) return new NeuralNode[0];

        int previousLayerLenght = layerSizes[layerIndex - 1];
        NeuralNode[] connectingNodes = new NeuralNode[previousLayerLenght];
        
        for (int connectionIndex = 0; connectionIndex < previousLayerLenght; connectionIndex++)
        {
            NeuralNode connectNode = layers[layerIndex - 1].nodes[connectionIndex];
            connectingNodes[connectionIndex] = connectNode;
        }

        return connectingNodes;
    }
#endregion 
#region Randomizing
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
#endregion
}

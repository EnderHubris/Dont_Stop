using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Node
{
    public Vector3 position;
    public Node leftNeighbor, rightNeighbor, topNeighbor, bottomNeighbor;
    public Node parent;
    public bool disregard = false;
    public bool root = false;

    public Node(Node par, Vector3 pos, bool isRoot = false)
    {
        parent = par;
        position = pos;
        root = isRoot;
    }

    public void FormEdges()
    {
        if (leftNeighbor != null) ChunkGraph.Instance.AddEdge(new Edge(leftNeighbor.position, position));
        if (rightNeighbor != null) ChunkGraph.Instance.AddEdge(new Edge(rightNeighbor.position, position));
        if (topNeighbor != null) ChunkGraph.Instance.AddEdge(new Edge(topNeighbor.position, position));
        if (bottomNeighbor != null) ChunkGraph.Instance.AddEdge(new Edge(bottomNeighbor.position, position));
    }

    // Add a neighbor in a random direction
    public void Spread()
    {
        // do not create a Node if its position is on its parent
        // just mark the parent as the associated neighbor
        if (Random.Range(0f,1f) < 0.35f)
        {
            bool ok = CreateNode(position + new Vector3(-ChunkGraph.Instance.offsetX,0,0)); // left
            if (!ok) leftNeighbor = parent;
        }

        if (Random.Range(0f,1f) < 0.15f)
        {
            bool ok = CreateNode(position + new Vector3(ChunkGraph.Instance.offsetX,0,0)); // right
            if (!ok) rightNeighbor = parent;
        }

        if (Random.Range(0f,1f) < 0.5f)
        {
            bool ok = CreateNode(position + new Vector3(0,ChunkGraph.Instance.offsetY,0)); // top
            if (!ok) topNeighbor = parent;
        }

        if (Random.Range(0f,1f) < 0.45f)
        {
            bool ok = CreateNode(position + new Vector3(0,-ChunkGraph.Instance.offsetY,0)); // bottom
            if (!ok) bottomNeighbor = parent;
        }
    }

    bool CreateNode(Vector3 pos)
    {
        if (parent != null && pos == parent.position) return false;
        Debug.Log("Created new Node!");
        Node node = new Node(this, pos);
        ChunkGraph.Instance.AddNode(node);
        return true;
    }

    public int GetDegree()
    {
        int degree = 0;

        if (leftNeighbor != null) ++degree;
        if (rightNeighbor != null) ++degree;
        if (topNeighbor != null) ++degree;
        if (bottomNeighbor != null) ++degree;

        return degree;
    }

    public void Adopt(Node node)
    {
        // find the child that was disregarded
        // and replace it for node
        if (leftNeighbor != null && leftNeighbor.disregard)
        {
            leftNeighbor = node;
            return;
        }

        if (rightNeighbor != null && rightNeighbor.disregard)
        {
            rightNeighbor = node;
            return;
        }

        if (topNeighbor != null && topNeighbor.disregard)
        {
            topNeighbor = node;
            return;
        }

        if (bottomNeighbor != null && bottomNeighbor.disregard)
        {
            bottomNeighbor = node;
            return;
        }
    }
}

public struct Edge
{
    public Vector3 pointA;
    public Vector3 pointB;
    
    public Edge(Vector3 a, Vector3 b)
    {
        pointA = a;
        pointB = b;
    }
}

public class ChunkGraph : Singleton<ChunkGraph>
{
    Dictionary<Vector3, Node> nodes = new Dictionary<Vector3, Node>();
    List<Edge> edges = new List<Edge>();
    Node root;

    [Header("Metadata of Graph")]
    public int offsetX, offsetY;
    public int MaxNumberOfNodes = 100;
    
    [Header("Draw Nodes of Graph")]
    [SerializeField] int nodeCount = 0;
    public bool CreateNodes = false;
    
    [Header("Draw Edges of Graph")]
    [SerializeField] int edgeCount = 0;
    public bool RenderGraph = false;

    public void AddNode(Node node)
    {
        if (nodeCount >= MaxNumberOfNodes) return;

        bool conflict = !nodes.TryAdd(node.position, node);
        if (conflict)
        {
            Debug.Log("Conflict when adding Node!");

            // delete node and form an edge between the node.parent and the node
            // at node.position within our dictionary
            Node nodeParent = node.parent;
            Node existingNode = nodes[node.position];
            node.disregard = true;

            if (nodeParent == null) return;
            nodeParent.Adopt(existingNode); // spreading terminates when we manually connect nodes with an edge
        } else
            {
                Debug.Log("Expanding from Node!");
                // allow node to spread out
                ++nodeCount;
                node.Spread();
            }
    }

    Dictionary<Vector3, Node> visited = new Dictionary<Vector3, Node>();

    public void AddEdge(Edge e)
    {
        // edge connects to a visited node, ignore this edge as it already exists
        if (visited.ContainsKey(e.pointA) && visited.ContainsKey(e.pointB))
        edges.Add(e);
        ++edgeCount;
    }

//=====================================================

    void LoadNodes()
    {
        nodes.Clear();
        nodeCount = 0;
    
        // create root node
        root = new Node(null, Vector3.zero, true);
        AddNode(root);
        Debug.Log("Created Root!");
    }

    bool canBeDrawn = false;
    void LoadEdges()
    {
        visited.Clear();
        edges.Clear();
        edgeCount = 0;

        foreach (var item in nodes)
        {
            Node node = item.Value;
            visited.Add(node.position, node);
            node.FormEdges();
        }
    }

    void OnDrawGizmos()
    {
        if (CreateNodes)
        {
            CreateNodes = false;
            LoadNodes();
        }

        if (RenderGraph)
        {
            RenderGraph = false;
            LoadEdges();
        }

        DrawGraph();
    }
    void DrawGraph()
    {
        foreach (var item in nodes)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(item.Key, 5);
        }

        if (edges.Count == 0) return;

        foreach (Edge e in edges)
        {   
            Gizmos.color = Color.white;
            Gizmos.DrawLine(e.pointA, e.pointB);
        }
    }
}//EndScript
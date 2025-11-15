using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;
using Random = UnityEngine.Random;


public class PathfindingManager : MonoBehaviour
{
    public List<PathfindingConnection> connections;
    private PathfindingNode[] nodes;

    public int testCount = 1;

    [SerializeField] private LayerMask raycastContactFilter;


    private void Start()
    {
        nodes = FindObjectsByType<PathfindingNode>(FindObjectsSortMode.None);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (PathfindingConnection connection in connections)
        {
            if (connection.source == null || connection.dest == null)
            {
                continue;
            }
            Gizmos.DrawLine(connection.source.transform.position, connection.dest.transform.position);
        }
    }

    private void OnDrawGizmosSelected()
    {
        nodes = FindObjectsByType<PathfindingNode>(FindObjectsSortMode.None);
        PathfindTo(transform.position, transform.GetChild(0).position);
        for (int i = 0; i < connections.Count; i++)
        {
            if (connections[i].dest == null || connections[i].source == null)
            {
                connections.RemoveAt(i);
                i--;
            }
        }
    }

    public PathfindingNode[] GetAllNodes()
    {
        return nodes;
    }

    public PathfindingNode GetRandomNode()
    {
        return nodes[Random.Range(0, nodes.Length)];
    }

    public List<PathfindingConnection> GetConnectionsInvolvingNode(PathfindingNode node)
    {
        var connectionsToReturn = new List<PathfindingConnection>();
        foreach (PathfindingConnection connection in connections)
        {
            if (connection.dest == node || connection.source == node)
            {
                connectionsToReturn.Add(connection);
            }
        }

        return connectionsToReturn;
    }
    
    public PathfindingConnection? GetConnectionInvolvingNodes(PathfindingNode nodeA, PathfindingNode nodeB)
    {
        foreach (PathfindingConnection connection in connections)
        {
            if ((connection.dest == nodeA && connection.source == nodeB) || (connection.source == nodeA && connection.dest == nodeB))
            {
                return connection;
            }
        }
        return null;
    }

    public PathfindingNode GetNearestNode(Vector3 position, float castRadius = .2f)
    {
        float nearestDist = 99999999;
        PathfindingNode nearestNode = nodes[0];
        foreach (PathfindingNode node in nodes)
        {
            float dist = (position - node.transform.position).magnitude;
            if (dist < nearestDist)
            {
                RaycastHit2D hit = Physics2D.CircleCast(position, castRadius, node.transform.position - position, (node.transform.position - position).magnitude, raycastContactFilter);
                if (hit.collider is not null)
                {
                    continue;
                }
                nearestDist = dist;
                nearestNode = node;
            }
        }

        return nearestNode;
    }

    private Vector3 texOffset = new Vector3(.25f, .25f, 0);
    private Vector3 texOffsetB = new Vector3(.25f, -.25f, 0);
    // ReSharper disable Unity.PerformanceAnalysis
    public Stack<PathfindingNode> PathfindTo(Vector3 currentPos, Vector3 goalPos)
    {
        Handles.color = Color.white;
        
        PathfindingNode destNode = GetNearestNode(goalPos);
        PathfindingNode startNearest = GetNearestNode(currentPos);

        if (destNode == startNearest)
        {
            // You messed up, somehow.
            Debug.LogWarning("TRYING TO PATHFIND TO SAME PLACE!");
            var fake = new Stack<PathfindingNode>();
            fake.Push(destNode);
            return fake;
        }

        var searchNodes = nodes.Select(
            node => new SearchNode
            {
                cost = float.PositiveInfinity, 
                node = node, 
                prevNode = null, 
                state = SearchNodeState.Unsearched
            }).ToList();

        // Setup first node
        SearchNode currentlySearching = searchNodes.First(n => n.node == startNearest);
        currentlySearching.state = SearchNodeState.Searching;
        currentlySearching.cost = 0;
        currentlySearching.prevNode = null;

        int c = 0;
        // For every connection to current search node
        while (c < testCount)
        {
            c++;
            // Get all nodes connected to current searching
            foreach (var connection in GetConnectionsInvolvingNode(currentlySearching.node))
            {
                SearchNode otherNode = searchNodes.First(n => n.node ==
                                                              (connection.dest == currentlySearching.node ? connection.source : connection.dest));
                if (otherNode.node == destNode)
                {
                    // Time to start building the path;
                    otherNode.prevNode = currentlySearching.node;
                    var path = new Stack<PathfindingNode>();
                    SearchNode rewindNode = otherNode;
                    //Gizmos.color = Color.cyan;
                    while (rewindNode is not null)
                    {
                        path.Push(searchNodes.First(n => n.node == rewindNode.node).node);
                        
                        #if UNITY_EDITOR
                        if (rewindNode.prevNode is not null)
                        {
                            //Gizmos.DrawLine(rewindNode.node.transform.position, rewindNode.prevNode.transform.position);
                        }
                        #endif
                        
                        rewindNode = rewindNode.prevNode == null ? null : searchNodes.First(n => n.node == rewindNode.prevNode);
                    }
                    return path;
                }
                
                switch (otherNode.state)
                {
                    case SearchNodeState.Unsearched:
                        otherNode.state = SearchNodeState.ToSearch;
                        otherNode.prevNode = currentlySearching.node;
                        otherNode.cost = (otherNode.node.transform.position - currentlySearching.node.transform.position).magnitude + currentlySearching.cost;
                        break;
                }
                currentlySearching.state = SearchNodeState.Searched;
            }

            currentlySearching = searchNodes.Where(n => n.state == SearchNodeState.ToSearch).OrderBy(n => n.cost).First();
        }
#if UNITY_EDITOR
        foreach (SearchNode node in searchNodes)
        {
            Gizmos.color = node.state switch
            {
                SearchNodeState.Searching => new Color(1f, 165f / 255, 0),
                SearchNodeState.ToSearch => Color.blue,
                SearchNodeState.Unsearched => Color.white,
                SearchNodeState.Searched => Color.red,
                _ => Color.black
            };
            
            Gizmos.DrawWireSphere(node.node.transform.position, 1);
            
            Handles.color = Color.white;
            Handles.Label(node.node.transform.position + texOffset, node.cost.ToString());
            if(node.prevNode is not null)
                Handles.Label(node.node.transform.position + texOffsetB, node.prevNode.gameObject.name);
        }
        #endif

        
        Destroy(gameObject);
        Debug.LogError("FATAL ERROR: INFINITE LOOP DETECTED ON PATHFINDING");
        return null;
    }
}

enum SearchNodeState
{
    /// Orange
    Searching, 
    /// White w/ blue ring
    ToSearch,
    /// Red
    Searched, 
    /// White
    Unsearched 
}

internal class SearchNode
{
    public float cost;
    public PathfindingNode node;
    [CanBeNull] public PathfindingNode prevNode;
    public SearchNodeState state;
}

[System.Serializable]
public struct PathfindingConnection
{
    public PathfindingNode source;
    public PathfindingNode dest;

    public PathfindingConnection(PathfindingNode source, PathfindingNode dest)
    {
        this.source = source;
        this.dest = dest;
    }
}

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class PathfindingNode : MonoBehaviour
{
    #if UNITY_EDITOR
    public PathfindingNode editor_otherNode;

    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, .1f);

        Handles.color = Color.white;
        Handles.Label(transform.position + new Vector3(.1f, -.1f, 0), gameObject.name);
    }

    private void OnDrawGizmosSelected()
    {
        if (!editor_otherNode) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(editor_otherNode.transform.position, .2f);
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(PathfindingNode))]
public class PathfindingNodeDrawer : Editor
{
    private SerializedProperty otherNode;

    private PathfindingManager _managerManager;

    private void OnEnable()
    {
        otherNode = serializedObject.FindProperty("editor_otherNode");
        _managerManager = FindAnyObjectByType<PathfindingManager>();
    }

    public override void OnInspectorGUI()
    {
        //DrawDefaultInspector();
        
        EditorGUILayout.PropertyField(otherNode);

        PathfindingConnection? connection =
            _managerManager.GetConnectionInvolvingNodes(serializedObject.targetObject as PathfindingNode, otherNode.objectReferenceValue as PathfindingNode);
        
        if (connection is null)
        {
            if (GUILayout.Button("Add Connection"))
            {
                _managerManager.connections.Add(new PathfindingConnection(serializedObject.targetObject as PathfindingNode, otherNode.objectReferenceValue as PathfindingNode));
            }
        }
        else
        {
            if (GUILayout.Button("Remove Connection"))
            {
                _managerManager.connections.Remove((PathfindingConnection)connection);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
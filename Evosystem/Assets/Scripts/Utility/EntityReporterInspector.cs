using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CustomEditor(typeof(EntityReporter))]
public class EntityReporterInspector : Editor
{
    public override void OnInspectorGUI()
    {
        EntityReporter reporter = (EntityReporter)target;

        EditorGUILayout.Space();
        DrawDefaultInspector();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField($"Species count: {reporter.populationCount.Count}", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        foreach (KeyValuePair<Species, int> pair in reporter.populationCount)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"{pair.Key.ToString()}", EditorStyles.boldLabel);
            //GUILayout.Space(10);
            EditorGUILayout.LabelField(pair.Value.ToString());
            EditorGUILayout.EndHorizontal();
        }
    }
}
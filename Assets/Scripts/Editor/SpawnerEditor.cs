using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Spawner))]

public class SpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Spawner spawner = (Spawner)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Runtime Tools", EditorStyles.boldLabel);

        if (GUILayout.Button("Spawn Now (Editor Test)"))
        {
            spawner.SpawnNow();
        }
    }


}

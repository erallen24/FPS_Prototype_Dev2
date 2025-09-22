
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Spawner))]

public class SpawnerEditor : Editor
{
    private SerializedProperty triggerCollider;





    private SerializedProperty autoSpawnProp;
    private SerializedProperty autoSpawnObjects;
    private SerializedProperty autoSpawnCount;
    private SerializedProperty autoSpawnObjAtATime;
    private SerializedProperty autoSpawnRate;
    private SerializedProperty autoSpawnRadius;

    private SerializedProperty mainGroup;
    private SerializedProperty secondaryGroup;
    private SerializedProperty tertiaryGroup;
    private SerializedProperty quaternaryGroup;

    private SerializedProperty isBossSpawnerProp;
    private SerializedProperty bossObjects;
    private SerializedProperty bossSpawnPositions;
    private SerializedProperty bossSpawnRate;
    private SerializedProperty bossesAtATime;
    private SerializedProperty spawnAtCompletionProgess;


    private void OnEnable()
    {
        triggerCollider = serializedObject.FindProperty("triggerCollider");
        autoSpawnProp = serializedObject.FindProperty("autoSpawn");
        autoSpawnObjects = serializedObject.FindProperty("autoSpawnObjects");
        autoSpawnCount = serializedObject.FindProperty("autoSpawnCount");
        autoSpawnObjAtATime = serializedObject.FindProperty("autoSpawnObjAtATime");
        autoSpawnRate = serializedObject.FindProperty("autoSpawnRate");
        autoSpawnRadius = serializedObject.FindProperty("autoSpawnRadius");

        mainGroup = serializedObject.FindProperty("mainGroup");
        secondaryGroup = serializedObject.FindProperty("secondaryGroup");
        tertiaryGroup = serializedObject.FindProperty("tertiaryGroup");
        quaternaryGroup = serializedObject.FindProperty("quaternaryGroup");

        isBossSpawnerProp = serializedObject.FindProperty("isBossSpawner");
        bossObjects = serializedObject.FindProperty("bossObjects");
        bossSpawnPositions = serializedObject.FindProperty("bossSpawnPositions");
        bossSpawnRate = serializedObject.FindProperty("bossSpawnRate");
        bossesAtATime = serializedObject.FindProperty("bossesAtATime");
        spawnAtCompletionProgess = serializedObject.FindProperty("spawnAtCompletionProgess");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(triggerCollider);
        EditorGUILayout.PropertyField(autoSpawnProp);



        if (autoSpawnProp.boolValue)
        {
            EditorGUILayout.LabelField("Auto Spawn Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(autoSpawnObjects);
            EditorGUILayout.PropertyField(autoSpawnCount);
            EditorGUILayout.PropertyField(autoSpawnObjAtATime);
            EditorGUILayout.PropertyField(autoSpawnRate);
            EditorGUILayout.PropertyField(autoSpawnRadius);
        }

        if (!autoSpawnProp.boolValue)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Manual Spawn Groups", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(mainGroup);
            EditorGUILayout.PropertyField(secondaryGroup);
            EditorGUILayout.PropertyField(tertiaryGroup);
            EditorGUILayout.PropertyField(quaternaryGroup);
        }

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(isBossSpawnerProp);
        if (isBossSpawnerProp.boolValue)
        {
            EditorGUILayout.HelpBox("This spawner is configured for boss spawning. Ensure boss settings are properly set.", MessageType.Info);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Boss Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(bossObjects);
            EditorGUILayout.PropertyField(bossSpawnPositions);
            EditorGUILayout.PropertyField(bossSpawnRate);
            EditorGUILayout.PropertyField(bossesAtATime);
            EditorGUILayout.PropertyField(spawnAtCompletionProgess);
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Runtime Tools", EditorStyles.boldLabel);
        if (GUILayout.Button("Spawn Now (Editor Test)"))
        {
            ((Spawner)target).SpawnNow();
        }

        Spawner spawner = (Spawner)target;

        if (spawner.TriggerCollider != null)
        {
            // Convert local center to world position
            Transform triggerTransform = spawner.TriggerCollider.transform;
            Vector3 worldCenter = triggerTransform.TransformPoint(spawner.TriggerCollider.center);

            // Draw and move handle
            EditorGUI.BeginChangeCheck();
            Vector3 newWorldCenter = Handles.PositionHandle(worldCenter, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(spawner.TriggerCollider, "Move Trigger Center");
                spawner.TriggerCollider.center = triggerTransform.InverseTransformPoint(newWorldCenter);
                EditorUtility.SetDirty(spawner.TriggerCollider);
            }
        }
        serializedObject.ApplyModifiedProperties();



    }


}

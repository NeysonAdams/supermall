# if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelBuilderSetting))]
public class LevelBuilderSettingsEditor : Editor
{
    SerializedProperty ground_colors;
    SerializedProperty names;
    SerializedProperty road_marking_textures;
    SerializedProperty parking_marking_textures;

    private bool colors_show = true;
    private bool marking_show = true;

    void OnEnable()
    {
        ground_colors = serializedObject.FindProperty("ground_colors");
        names = serializedObject.FindProperty("names");
        road_marking_textures = serializedObject.FindProperty("road_marking_textures");
        parking_marking_textures = serializedObject.FindProperty("parking_marking_textures");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        colors_show = EditorGUILayout.BeginFoldoutHeaderGroup(colors_show, "Tale Colors");
        if (colors_show)
        {
            for (int i = 0; i < ground_colors.arraySize; i++)
            {
                string name = names.GetArrayElementAtIndex(i).stringValue;
                EditorGUILayout.PropertyField(ground_colors.GetArrayElementAtIndex(i), new GUIContent(name));
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        marking_show = EditorGUILayout.BeginFoldoutHeaderGroup(marking_show, "Road Marking");
        if (marking_show)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(road_marking_textures.GetArrayElementAtIndex(0), new GUIContent("R1"));
            EditorGUILayout.PropertyField(road_marking_textures.GetArrayElementAtIndex(1), new GUIContent("R2"));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(road_marking_textures.GetArrayElementAtIndex(2), new GUIContent("R3"));
            EditorGUILayout.PropertyField(road_marking_textures.GetArrayElementAtIndex(3), new GUIContent("R4"));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(road_marking_textures.GetArrayElementAtIndex(4), new GUIContent("Zebra Horizontal"));
            EditorGUILayout.PropertyField(road_marking_textures.GetArrayElementAtIndex(5), new GUIContent("Zebra Vertical"));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(parking_marking_textures.GetArrayElementAtIndex(0), new GUIContent("0"));
            EditorGUILayout.PropertyField(parking_marking_textures.GetArrayElementAtIndex(1), new GUIContent("90"));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(parking_marking_textures.GetArrayElementAtIndex(2), new GUIContent("180"));
            EditorGUILayout.PropertyField(parking_marking_textures.GetArrayElementAtIndex(3), new GUIContent("270"));
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        serializedObject.ApplyModifiedProperties();
    }
}
#endif

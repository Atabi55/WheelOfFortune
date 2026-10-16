
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(RewardPool))]
public class RewardPoolEditor : Editor
{
    private const string ImageFolder = "Assets/Art/Rewards";

    private Sprite[] availableSprites;

    private void OnEnable()
    {
        RefreshSprites();
    }

    private void RefreshSprites()
    {
        availableSprites = AssetDatabase
            .FindAssets("t:Sprite", new[] { ImageFolder })
            .Select(guid => AssetDatabase.LoadAssetAtPath<Sprite>(
                AssetDatabase.GUIDToAssetPath(guid)))
            .Where(sprite => sprite != null)
            .OrderBy(sprite => sprite.name)
            .ToArray();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty rewards =
            serializedObject.FindProperty("rewards");

        EditorGUILayout.LabelField(
            "Available Reward Images",
            EditorStyles.boldLabel
        );

        EditorGUILayout.LabelField(
            $"Found {availableSprites.Length} sprites in {ImageFolder}"
        );

        if (GUILayout.Button("Refresh Image List"))
            RefreshSprites();

        EditorGUILayout.Space();

        if (availableSprites.Length == 0)
        {
            EditorGUILayout.HelpBox(
                "No sprites found. Check the folder path and " +
                "make sure the images are imported as Sprites.",
                MessageType.Warning
            );
        }

        while (rewards.arraySize < 8)
            rewards.InsertArrayElementAtIndex(rewards.arraySize);

        while (rewards.arraySize > 8)
            rewards.DeleteArrayElementAtIndex(rewards.arraySize - 1);

        for (int i = 0; i < 8; i++)
        {
            SerializedProperty reward = rewards.GetArrayElementAtIndex(i);
            Sprite current = reward.objectReferenceValue as Sprite;

            List<GUIContent> options = new List<GUIContent>
            {
                new GUIContent("-- None --")
            };

            options.AddRange(
                availableSprites.Select(s => new GUIContent(s.name))
            );

            int selectedIndex = current == null
                ? 0
                : System.Array.IndexOf(availableSprites, current) + 1;

            if (selectedIndex < 0 || selectedIndex >= options.Count)
                selectedIndex = 0;

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField($"Reward {i + 1}", GUILayout.Width(80));

            int newIndex = EditorGUILayout.Popup(
                selectedIndex,
                options.ToArray()
            );

            if (newIndex != selectedIndex)
            {
                reward.objectReferenceValue = newIndex == 0
                    ? null
                    : availableSprites[newIndex - 1];
            }

            EditorGUILayout.EndHorizontal();

            if (current != null)
                EditorGUILayout.ObjectField(current, typeof(Sprite), false);
        }

        serializedObject.ApplyModifiedProperties();
    }
}

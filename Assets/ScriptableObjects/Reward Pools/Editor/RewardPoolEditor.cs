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

        SerializedProperty rewards = serializedObject.FindProperty("rewards");
        SerializedProperty amounts = serializedObject.FindProperty("amounts");

        EditorGUILayout.LabelField("Available Reward Images", EditorStyles.boldLabel);
        EditorGUILayout.LabelField($"Found {availableSprites.Length} sprites in {ImageFolder}");

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

        // Force both arrays to be exactly 8 elements long
        while (rewards.arraySize < 8) rewards.InsertArrayElementAtIndex(rewards.arraySize);
        while (rewards.arraySize > 8) rewards.DeleteArrayElementAtIndex(rewards.arraySize - 1);

        if (amounts != null)
        {
            while (amounts.arraySize < 8) amounts.InsertArrayElementAtIndex(amounts.arraySize);
            while (amounts.arraySize > 8) amounts.DeleteArrayElementAtIndex(amounts.arraySize - 1);
        }

        for (int i = 0; i < 8; i++)
        {
            SerializedProperty reward = rewards.GetArrayElementAtIndex(i);
            Sprite current = reward.objectReferenceValue as Sprite;

            List<GUIContent> options = new List<GUIContent> { new GUIContent("-- None --") };
            options.AddRange(availableSprites.Select(s => new GUIContent(s.name)));

            int selectedIndex = current == null
                ? 0
                : System.Array.IndexOf(availableSprites, current) + 1;

            if (selectedIndex < 0 || selectedIndex >= options.Count)
                selectedIndex = 0;

            EditorGUILayout.BeginHorizontal();

            // 1. Label
            EditorGUILayout.LabelField($"Slot {i + 1}", GUILayout.Width(45));

            // 2. Sprite Dropdown
            int newIndex = EditorGUILayout.Popup(selectedIndex, options.ToArray());

            if (newIndex != selectedIndex)
            {
                reward.objectReferenceValue = newIndex == 0
                    ? null
                    : availableSprites[newIndex - 1];
            }

            // 3. Amount Input Field (if amounts array exists)
            if (amounts != null)
            {
                SerializedProperty amountProp = amounts.GetArrayElementAtIndex(i);
                EditorGUILayout.LabelField("Amt:", GUILayout.Width(30));
                amountProp.intValue = EditorGUILayout.IntField(amountProp.intValue, GUILayout.Width(50));
            }

            EditorGUILayout.EndHorizontal();

            // Preview the sprite below the selection
            if (current != null)
                EditorGUILayout.ObjectField(current, typeof(Sprite), false);

            EditorGUILayout.Space(5); // Add some room between slots
        }

        serializedObject.ApplyModifiedProperties();
    }
}
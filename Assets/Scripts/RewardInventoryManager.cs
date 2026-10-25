using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Include this for TextMeshPro

public class RewardInventoryManager : MonoBehaviour
{
    [Tooltip("The parent layout group for the reward icons")]
    [SerializeField] private Transform inventoryContainer;

    [Tooltip("A UI Prefab with an Image AND a child TMP_Text component")]
    [SerializeField] private GameObject rewardIconPrefab;

    // Track the total amount for each sprite
    private Dictionary<Sprite, int> rewardTotals = new Dictionary<Sprite, int>();

    // Track the instantiated UI GameObject for each sprite to update its text later
    private Dictionary<Sprite, GameObject> rewardUIElements = new Dictionary<Sprite, GameObject>();

    public void AddReward(Sprite rewardSprite, int amountToAdd)
    {
        if (rewardTotals.ContainsKey(rewardSprite))
        {
            // Reward exists: increase the amount
            rewardTotals[rewardSprite] += amountToAdd;
            UpdateUIAmount(rewardSprite);
        }
        else
        {
            // New reward: create the UI element and add to dictionaries
            rewardTotals.Add(rewardSprite, amountToAdd);

            GameObject newIcon = Instantiate(rewardIconPrefab, inventoryContainer);
            rewardUIElements.Add(rewardSprite, newIcon);

            Image iconImage = newIcon.GetComponent<Image>();
            if (iconImage != null)
            {
                iconImage.sprite = rewardSprite;
                iconImage.preserveAspect = true;
            }

            UpdateUIAmount(rewardSprite);
        }
    }

    private void UpdateUIAmount(Sprite rewardSprite)
    {
        GameObject uiElement = rewardUIElements[rewardSprite];
        TMP_Text amountText = uiElement.GetComponentInChildren<TMP_Text>();

        if (amountText != null)
        {
            amountText.text = rewardTotals[rewardSprite].ToString();
        }
    }

    public void ClearInventory()
    {
        rewardTotals.Clear();
        rewardUIElements.Clear();
        foreach (Transform child in inventoryContainer)
        {
            Destroy(child.gameObject);
        }
    }
}
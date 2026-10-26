using UnityEngine;
using TMPro;

public class NumberRecycler : MonoBehaviour
{
    [SerializeField] private Transform layoutContainer;
    [SerializeField] private int totalNumbers = 60;

    [Header("Colors & Opacity")]
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color multipleOf5Color = Color.green;
    [SerializeField] private Color multipleOf30Color = new Color(1f, 0.84f, 0f); // Golden Yellow
    [Range(0f, 1f)][SerializeField] private float fadedAlpha = 0.3f; // Transparency for numbers on the left

    private int lowestNumberShown = 1;
    private int highestNumberShown;
    private Transform[] originalItems;

    void Awake()
    {
        highestNumberShown = layoutContainer.childCount;
        originalItems = new Transform[highestNumberShown];
        for (int i = 0; i < highestNumberShown; i++)
        {
            originalItems[i] = layoutContainer.GetChild(i);
            UpdateItemText(originalItems[i], i + 1);
        }
    }

    public bool RecycleFirstToEnd()
    {
        if (highestNumberShown >= totalNumbers) return false;

        Transform firstItem = layoutContainer.GetChild(0);
        firstItem.SetAsLastSibling();

        highestNumberShown++;
        lowestNumberShown++;
        UpdateItemText(firstItem, highestNumberShown);
        return true;
    }

    public bool RecycleLastToStart()
    {
        if (lowestNumberShown <= 1) return false;

        Transform lastItem = layoutContainer.GetChild(layoutContainer.childCount - 1);
        lastItem.SetAsFirstSibling();

        lowestNumberShown--;
        highestNumberShown--;
        UpdateItemText(lastItem, lowestNumberShown);
        return true;
    }

    public void ResetRecycler()
    {
        lowestNumberShown = 1;
        highestNumberShown = originalItems.Length;

        for (int i = 0; i < originalItems.Length; i++)
        {
            originalItems[i].SetSiblingIndex(i);
            UpdateItemText(originalItems[i], i + 1);
        }
    }

    // Updates alphas based on what the current center/indicated count is
    public void UpdateAlphas(int currentCount)
    {
        for (int i = 0; i < layoutContainer.childCount; i++)
        {
            Transform child = layoutContainer.GetChild(i);
            TMP_Text text = child.GetComponent<TMP_Text>();
            if (text != null)
            {
                int childNumber = lowestNumberShown + i;
                Color col = text.color;

                // If the number is to the left of the center count, fade it out
                if (childNumber < currentCount)
                {
                    col.a = fadedAlpha;
                }
                else
                {
                    col.a = 1.0f; // Full opacity for center and right numbers
                }
                text.color = col;
            }
        }
    }

    private void UpdateItemText(Transform item, int number)
    {
        TMP_Text text = item.GetComponent<TMP_Text>();
        if (text != null)
        {
            text.text = number.ToString();

            Color baseColor = defaultColor;
            if (number > 0 && number % 30 == 0)
            {
                baseColor = multipleOf30Color;
            }
            else if (number == 1 || (number > 0 && number % 5 == 0))
            {
                baseColor = multipleOf5Color;
            }

            // Keep base color while maintaining full alpha
            baseColor.a = 1f;
            text.color = baseColor;
        }
    }
}
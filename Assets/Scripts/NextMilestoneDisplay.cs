using UnityEngine;
using TMPro;

public class NextMilestoneDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpinCounter spinCounter;
    [SerializeField] private TMP_Text nextMultipleOf5Text;
    [SerializeField] private TMP_Text nextMultipleOf30Text;

    private void Start()
    {
        UpdateMilestones();
    }

    public void UpdateMilestones()
    {
        if (spinCounter == null) return;

        int currentCount = spinCounter.SpinCount;

        // Calculate the next multiple strictly greater than the current count
        int next5 = ((currentCount / 5) + 1) * 5;
        int next30 = ((currentCount / 30) + 1) * 30;

        if (nextMultipleOf5Text != null)
            nextMultipleOf5Text.text = next5.ToString();

        if (nextMultipleOf30Text != null)
            nextMultipleOf30Text.text = next30.ToString();
    }
}
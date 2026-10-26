using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SpinCountSlider : MonoBehaviour
{
    [SerializeField] private SpinCounter spinCounter;
    [SerializeField] private NumberRecycler recycler;
    [SerializeField] private float slideDuration = 0.4f;
    [SerializeField] private int emptySlotsAtStart = 6;

    private RectTransform numberRow;
    private float initialX;
    private float stepWidth;
    private Tween slideTween;

    private int displayedCount;
    private int currentRecycleCount = 0;

    private void Awake()
    {
        numberRow = GetComponent<RectTransform>();

        if (spinCounter == null || numberRow == null || recycler == null)
        {
            Debug.LogError("SpinCountSlider references are missing!", this);
            enabled = false;
            return;
        }

        initialX = numberRow.anchoredPosition.x;
        displayedCount = spinCounter.SpinCount;

        HorizontalLayoutGroup layout = GetComponent<HorizontalLayoutGroup>();
        RectTransform firstChild = numberRow.GetChild(0).GetComponent<RectTransform>();

        LayoutRebuilder.ForceRebuildLayoutImmediate(numberRow);
        stepWidth = firstChild.rect.width + layout.spacing;

        UpdatePosition(false);
    }

    public void UpdateSpinCountSlider()
    {
        UpdatePosition(true);
    }

    private void UpdatePosition(bool animate)
    {
        displayedCount = spinCounter.SpinCount;

        recycler.UpdateAlphas(displayedCount);

        float targetX = initialX - (displayedCount - 1) * stepWidth + (currentRecycleCount * stepWidth);

        if (animate)
        {
            slideTween = numberRow.DOAnchorPosX(targetX, slideDuration)
                .SetEase(Ease.OutCubic)
                .OnComplete(() =>
                {
                    HandleRecycling();
                    recycler.UpdateAlphas(displayedCount);
                });
        }
        else
        {
            numberRow.anchoredPosition = new Vector2(targetX, numberRow.anchoredPosition.y);
            HandleRecycling();
            recycler.UpdateAlphas(displayedCount);
        }
    }

    private void HandleRecycling()
    {
        int requiredRecycles = Mathf.Max(0, displayedCount - 1 - emptySlotsAtStart);
        bool layoutChanged = false;

        if (currentRecycleCount < requiredRecycles)
        {
            if (recycler.RecycleFirstToEnd())
            {
                currentRecycleCount++;
                numberRow.anchoredPosition += new Vector2(stepWidth, 0);
                layoutChanged = true;
            }
        }
        else if (currentRecycleCount > requiredRecycles)
        {
            if (recycler.RecycleLastToStart())
            {
                currentRecycleCount--;
                numberRow.anchoredPosition -= new Vector2(stepWidth, 0);
                layoutChanged = true;
            }
        }

        if (layoutChanged)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(numberRow);
        }
    }

    public void ResetSliderToStart()
    {
        // Kill any active slide animation
        slideTween?.Kill();

        // Reset recycler order and numbers
        recycler.ResetRecycler();

        // Reset internal tracking states completely
        currentRecycleCount = 0;
        displayedCount = spinCounter.SpinCount;

        // Snap position instantly back to start without animation
        numberRow.anchoredPosition = new Vector2(initialX, numberRow.anchoredPosition.y);

        // Force the layout group to immediately recalculate child positions
        LayoutRebuilder.ForceRebuildLayoutImmediate(numberRow);

        // Reset alphas for the starting state
        recycler.UpdateAlphas(displayedCount);
    }

    private void OnDestroy()
    {
        slideTween?.Kill();
    }
}
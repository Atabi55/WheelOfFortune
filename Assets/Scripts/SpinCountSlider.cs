using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class SpinCountSlider : MonoBehaviour
{
    [SerializeField] private SpinCounter spinCounter;
    [SerializeField] private float slideDuration = 0.4f;

    private RectTransform numberRow;
    private float initialX;
    private float stepWidth;
    private Tween slideTween;
    private int displayedCount;

    private void Awake()
    {
        numberRow = GetComponent<RectTransform>();

        if (spinCounter == null || numberRow == null)
        {
            Debug.LogError("SpinCountSlider references are missing!", this);
            enabled = false;
            return;
        }
        initialX = numberRow.anchoredPosition.x;
        Debug.Log($"initialX={initialX}");
        displayedCount = spinCounter.SpinCount;

        HorizontalLayoutGroup layout = GetComponent<HorizontalLayoutGroup>();
        RectTransform firstChild = numberRow.GetChild(0).GetComponent<RectTransform>();

        stepWidth = firstChild.rect.width + layout.spacing;

        Debug.Log($"stepwidth={stepWidth}");
        UpdatePosition(false);
    }

    public void UpdateSpinCountSlider()
    {
        displayedCount = spinCounter.SpinCount;
        UpdatePosition(true);
    }

    private void UpdatePosition(bool animate)
    {
        slideTween?.Kill();

        float targetX = initialX - (displayedCount - 1) * stepWidth;

        if (animate)
        {
            slideTween = numberRow.DOAnchorPosX(targetX, slideDuration)
                .SetEase(Ease.OutCubic);
        }
        else
        {
            numberRow.anchoredPosition =
                new Vector2(targetX, numberRow.anchoredPosition.y);
        }
    }

    private void OnDestroy()
    {
        slideTween?.Kill();
    }
}


using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RotateImage : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private RectTransform imageTransform;
    [SerializeField] private float rotationAngle = 90f;
    [SerializeField] private float duration = 0.5f;

    private Tween rotationTween;

    private void OnValidate()
    {
        // Automatically assign the Button component.
        if (button == null)
        {
            button = GetComponent<Button>();
        }
    }

    private void Awake()
    {
        // Ensure the reference is available at runtime too.
        if (button == null)
        {
            button = GetComponent<Button>();
        }

        button.onClick.AddListener(Rotate);
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(Rotate);
        }

        rotationTween?.Kill();
    }

    private void Rotate()
    {
        if (imageTransform == null)
        {
            Debug.LogWarning("Image Transform is not assigned.", this);
            return;
        }

        rotationTween?.Kill();

        rotationTween = imageTransform
            .DORotate(
                new Vector3(0f, 0f, rotationAngle),
                duration,
                RotateMode.FastBeyond360
            )
            .SetRelative()
            .SetEase(Ease.OutCubic);
    }
}

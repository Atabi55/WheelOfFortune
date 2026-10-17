using UnityEngine;
using DG.Tweening;

public class RevolverSpin : MonoBehaviour
{
    [Header("Spin Settings")]
    [SerializeField] private int slotCount = 8;
    [SerializeField] private float fastSpinDuration = 3f;
    [SerializeField] private int fastRotations = 8;
    [SerializeField] private float slowSpinDuration = 3f;
    [SerializeField] private int extraRotations = 2;

    private bool isSpinning;

    public int LastSelectedSlot { get; private set; } = -1;

    public void Spin()
    {
        // Prevent another spin while the current one is running.
        if (isSpinning)
            return;

        isSpinning = true;

        // Choose a random slot: 0 through 7.
        LastSelectedSlot = Random.Range(0, slotCount);

        float slotAngle = 360f / slotCount;

        // Fast spin, then slow down and land on the chosen slot.
        Sequence sequence = DOTween.Sequence();

        sequence.Append(
            transform.DOLocalRotate(
                new Vector3(0f, 0f, 360f * fastRotations),
                fastSpinDuration,
                RotateMode.FastBeyond360
            )
            .SetRelative()
            .SetEase(Ease.Linear)
        );

        sequence.Append(
            transform.DOLocalRotate(
                new Vector3(
                    0f,
                    0f,
                    360f * extraRotations +
                    slotAngle * LastSelectedSlot
                ),
                slowSpinDuration,
                RotateMode.FastBeyond360
            )
            .SetRelative()
            .SetEase(Ease.OutCubic)
        );

        sequence.OnComplete(() =>
        {
            isSpinning = false;
            Debug.Log($"Spin complete! Selected slot: {LastSelectedSlot}");
        });

        // Also release the lock if the tween is killed.
        sequence.OnKill(() =>
        {
            isSpinning = false;
        });
    }
}

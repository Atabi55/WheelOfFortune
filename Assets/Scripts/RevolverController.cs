using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class RevolverController : MonoBehaviour
{
    [Header("Spin Settings")]
    [SerializeField] private int slotCount = 8;
    [SerializeField] private float fastSpinDuration = 3f;
    [SerializeField] private int fastRotations = 8;
    [SerializeField] private float slowSpinDuration = 3f;
    [SerializeField] private int extraRotations = 2;

    [Header("Reward Pools")]
    [SerializeField] private RewardPool rewardPoolBronze;
    [SerializeField] private RewardPool rewardPoolSilver;
    [SerializeField] private RewardPool rewardPoolGold;

    [SerializeField] private Sprite bombSprite;
    [SerializeField] private BombPopupController bombPopup;
    [SerializeField] private SpinCounter spinCounter;

    private bool isSpinning;
    private int bombSlotIndex = 0;

    public int currentSlot { get; private set; } = 0;
    private int selectedSlot = 0;

    private Image[] childImages;

    private void Awake()
    {
        //find slot index of bomb in bronze reward pool
        for(int i = 0; i < slotCount; i++)
        {
            if (rewardPoolBronze.rewards[i] == bombSprite)
                bombSlotIndex = i;
        }
        Debug.Log($"Bomb slot is {bombSlotIndex}");
    }

    private void Start()
    {
        RewardsSetup();
        SetRewards();
    }

    private void RewardsSetup()
    {
        childImages = new Image[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            childImages[i] = transform.GetChild(i).GetComponent<Image>();

            if (childImages[i] == null)
            {
                Debug.LogError(
                    $"Child {i} ({transform.GetChild(i).name}) has no Image component.",
                    transform.GetChild(i).gameObject
                );
            }
        }

        if (childImages.Length != 8)
        {
            Debug.LogWarning(
                $"Expected 8 child Images, but found {childImages.Length}.",
                this
            );
        }
    }

    public void SetRewards()
    {
        RewardPool currentRewardPool;
        if (spinCounter.SpinCount == 0)
        {
            currentRewardPool = rewardPoolBronze;
        }
        else if (spinCounter.SpinCount % 30 == 0)
        {
            currentRewardPool = rewardPoolGold;
        }
        else if (spinCounter.SpinCount % 5 == 0)
        {
            currentRewardPool = rewardPoolSilver;
        }
        else
        {
            currentRewardPool = rewardPoolBronze;
        }

        for (int i = 0; i < childImages.Length; i++)
        {
            childImages[i].sprite =
                currentRewardPool.rewards[i % currentRewardPool.rewards.Length];

            childImages[i].preserveAspect = true;
            childImages[i].color = Color.white;
        }
    }

    public void Spin()
    {
        // Prevent another spin while the current one is running.
        if (isSpinning)
            return;

        isSpinning = true;

        // Choose a random slot: 0 through 7.
        //selectedSlot = Random.Range(0, slotCount);
        // always spin to bomb for testing purposes
        selectedSlot = 3;
        Debug.Log($"Wheel supposed to stop at: {selectedSlot}");

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
                new Vector3(0f,0f,360f * extraRotations +
                    slotAngle * ((selectedSlot - currentSlot + slotCount) % slotCount)
                ),
                slowSpinDuration,
                RotateMode.FastBeyond360
            )
            .SetRelative()
            .SetEase(Ease.OutCubic)
        );

        sequence.OnComplete(() =>
        {
            currentSlot = selectedSlot;

            if (selectedSlot == bombSlotIndex)
            {
                bombPopup.Show();
            }

            spinCounter.RegisterSpin();
            Debug.Log($"Spin complete! Selected slot: {selectedSlot}");
            isSpinning = false;
        });

        // Also release the lock if the tween is killed.
        sequence.OnKill(() =>
        {
            isSpinning = false;
        });
    }
}

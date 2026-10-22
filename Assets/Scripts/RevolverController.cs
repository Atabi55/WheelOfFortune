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

    [SerializeField] private SpinCounter spinCounter;
    [SerializeField] private SpinCountSlider spinCountSlider;
    [SerializeField] private Sprite[] revolverSprites, indicatorSprites;
    [SerializeField] private GameObject indicator;

    [SerializeField] private Sprite bombSprite;
    [SerializeField] private BombPopupController bombPopup;
    

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
            gameObject.GetComponent<Image>().sprite = revolverSprites[0];
            indicator.GetComponent<Image>().sprite = indicatorSprites[0];
        }
        else if (spinCounter.SpinCount % 30 == 0)
        {
            currentRewardPool = rewardPoolGold;
            gameObject.GetComponent<Image>().sprite = revolverSprites[2];
            indicator.GetComponent<Image>().sprite = indicatorSprites[2];
        }
        else if (spinCounter.SpinCount % 5 == 0)
        {
            currentRewardPool = rewardPoolSilver;
            gameObject.GetComponent<Image>().sprite = revolverSprites[1];
            indicator.GetComponent<Image>().sprite = indicatorSprites[1];
        }
        else
        {
            currentRewardPool = rewardPoolBronze;
            gameObject.GetComponent<Image>().sprite = revolverSprites[0];
            indicator.GetComponent<Image>().sprite = indicatorSprites[0];
        }

        for (int i = 0; i < childImages.Length; i++)
        {
            childImages[i].sprite =
                currentRewardPool.rewards[i % currentRewardPool.rewards.Length];

            childImages[i].preserveAspect = true;
            childImages[i].color = Color.white;
        }
    }

    public void Reset()
    {
        spinCounter.ResetCount();
        SetRewards();
        spinCountSlider.UpdateSpinCountSlider();
        transform.rotation = Quaternion.identity;
    }

    public void Spin()
    {
        // Prevent another spin while the current one is running.
        if (isSpinning)
            return;

        isSpinning = true;

        // Choose a random slot: 0 through 7.
        //selectedSlot = Random.Range(0, slotCount);
        // always spin to bomb for testing
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
            Debug.Log($"currentslot={currentSlot}, selectedslot={selectedSlot}, bombslotindex={bombSlotIndex}");

            if (selectedSlot == bombSlotIndex)
            {
                bombPopup.Show();
            }

            spinCounter.RegisterSpin();
            spinCountSlider.UpdateSpinCountSlider();
            Debug.Log($"Spin complete! Selected slot: {selectedSlot}");
            SetRewards();
            isSpinning = false;
        });

        // Also release the lock if the tween is killed.
        sequence.OnKill(() =>
        {
            isSpinning = false;
        });
    }
}

using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    [SerializeField] private RewardInventoryManager inventoryManager;
    [SerializeField] private NextMilestoneDisplay milestoneDisplay;

    [SerializeField] private Sprite bombSprite;
    [SerializeField] private BombPopupController bombPopup;


    private bool isSpinning;

    public bool IsSpinning => isSpinning;
    private int bombSlotIndex = 0;

    private RewardPool currentRewardPool;

    public int currentSlot { get; private set; } = 0;
    private int selectedSlot = 0;

    private Image[] childImages; //reward sprite display objects
    private TMP_Text[] childTexts; //reward amount display objects

    private void Awake()
    {
        // Find slot index of bomb in bronze reward pool
        for (int i = 0; i < slotCount; i++)
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
        childTexts = new TMP_Text[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform slotChild = transform.GetChild(i);
            childImages[i] = slotChild.GetComponent<Image>();
            childTexts[i] = slotChild.GetComponentInChildren<TMP_Text>();

            if (childImages[i] == null)
            {
                Debug.LogError(
                    $"Child {i} ({slotChild.name}) has no Image component.",
                    slotChild.gameObject
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
        if (spinCounter.SpinCount == 1)
        {
            currentRewardPool = rewardPoolSilver;
            gameObject.GetComponent<Image>().sprite = revolverSprites[1];
            indicator.GetComponent<Image>().sprite = indicatorSprites[1];
            Debug.Log("First spin, silver");
        }
        else if (spinCounter.SpinCount % 30 == 0)
        {
            currentRewardPool = rewardPoolGold;
            gameObject.GetComponent<Image>().sprite = revolverSprites[2];
            indicator.GetComponent<Image>().sprite = indicatorSprites[2];
            Debug.Log("Gold");
        }
        else if (spinCounter.SpinCount % 5 == 0)
        {
            currentRewardPool = rewardPoolSilver;
            gameObject.GetComponent<Image>().sprite = revolverSprites[1];
            indicator.GetComponent<Image>().sprite = indicatorSprites[1];
            Debug.Log("Silver");
        }
        else
        {
            currentRewardPool = rewardPoolBronze;
            gameObject.GetComponent<Image>().sprite = revolverSprites[0];
            indicator.GetComponent<Image>().sprite = indicatorSprites[0];
            Debug.Log("Bronze");
        }

        for (int i = 0; i < childImages.Length; i++)
        {
            int rewardIndex = i % currentRewardPool.rewards.Length;

            // Set the slot sprite
            childImages[i].sprite = currentRewardPool.rewards[rewardIndex];
            childImages[i].preserveAspect = true;
            childImages[i].color = Color.white;

            // Set the slot amount text
            if (childTexts[i] != null)
            {
                //set empty text if the slot has bomb
                bool isBronzeBomb = (currentRewardPool == rewardPoolBronze && i == bombSlotIndex);
                if (isBronzeBomb)
                {
                    childTexts[i].text = string.Empty;
                }
                else
                {
                    childTexts[i].text = currentRewardPool.amounts[rewardIndex].ToString();
                }
            }
        }
    }

    public void Reset()
    {
        currentSlot = 0;
        selectedSlot = 0;

        spinCounter.ResetCount();
        SetRewards();
        milestoneDisplay.UpdateMilestones();

        if (spinCountSlider != null)
        {
            spinCountSlider.ResetSliderToStart();
        }

        if (inventoryManager != null)
        {
            inventoryManager.ClearInventory();
        }

        transform.rotation = Quaternion.identity;
    }

    public void Spin()
    {
        if (isSpinning)
            return;

        isSpinning = true;

        // Choose a random slot: 0 through 7.
        selectedSlot = Random.Range(0, slotCount);
        Debug.Log($"Wheel supposed to stop at: {selectedSlot}");

        float slotAngle = 360f / slotCount;

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
                new Vector3(0f, 0f, 360f * extraRotations +
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

            bool isBomb = (selectedSlot == bombSlotIndex && currentRewardPool == rewardPoolBronze);

            if (isBomb)
            {
                bombPopup.Show();
            }
            else
            {
                Sprite wonSprite = currentRewardPool.rewards[selectedSlot];
                int wonAmount = currentRewardPool.amounts[selectedSlot];

                inventoryManager.AddReward(wonSprite, wonAmount);
            }

            spinCounter.RegisterSpin();
            spinCountSlider.UpdateSpinCountSlider();
            SetRewards();
            milestoneDisplay.UpdateMilestones();
            isSpinning = false;
        });

        sequence.OnKill(() =>
        {
            isSpinning = false;
        });
    }
}
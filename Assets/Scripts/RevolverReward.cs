
using UnityEngine;
using UnityEngine.UI;

public class RevolverRewards : MonoBehaviour
{
    [SerializeField] private RewardPool rewardPool;

    private void Start()
    {
        SetRewards();
    }

    public void SetRewards()
    {
        if (rewardPool == null)
        {
            Debug.LogError("RewardPool is not assigned!", this);
            return;
        }

        Image[] childImages = new Image[transform.childCount];

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

        if (rewardPool.rewards == null || rewardPool.rewards.Length == 0)
        {
            Debug.LogError("RewardPool contains no rewards!", this);
            return;
        }

        for (int i = 0; i < childImages.Length; i++)
        {
            childImages[i].sprite =
                rewardPool.rewards[i % rewardPool.rewards.Length];

            childImages[i].preserveAspect = true;
        }
    }
}

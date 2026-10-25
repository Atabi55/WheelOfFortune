
using UnityEngine;

[CreateAssetMenu(
    fileName = "NewRewardPool",
    menuName = "Fortune Wheel/Reward Pool"
)]
public class RewardPool : ScriptableObject
{
    public Sprite[] rewards = new Sprite[8];
    [SerializeField] public int[] amounts = new int[8];
}

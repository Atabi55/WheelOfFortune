
using UnityEngine;

[CreateAssetMenu(
    fileName = "NewRewardPool",
    menuName = "Fortune Wheel/Reward Pool"
)]
public class RewardPool : ScriptableObject
{
    public Sprite[] rewards = new Sprite[8];
}

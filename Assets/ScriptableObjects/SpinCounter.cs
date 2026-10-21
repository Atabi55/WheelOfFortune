using UnityEngine;

[CreateAssetMenu(
    fileName = "SpinCounter",
    menuName = "Fortune Wheel/Spin Counter"
)]
public class SpinCounter : ScriptableObject
{
    [SerializeField] private int spinCount = 0;

    public int SpinCount => spinCount;

    private void OnEnable()
    {
        spinCount = 0;
    }

    public void RegisterSpin()
    {
        spinCount++;
    }

    public void ResetCount()
    {
        spinCount = 0;
    }
}

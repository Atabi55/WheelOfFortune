
using System;
using UnityEngine;
using UnityEngine.UI;

public class BombPopupController : MonoBehaviour
{
    [Header("Popup")]
    [SerializeField] private GameObject popupPanel;

    [Header("Popup Buttons")]
    [SerializeField] private Button giveupButton;
    [SerializeField] private Button reviveButton;

    [SerializeField] private RevolverController revolver;

    private void Awake()
    {
        if (popupPanel == null)
            popupPanel = gameObject;

        if (giveupButton != null)
            giveupButton.onClick.AddListener(OnGiveup);

        if (reviveButton != null)
            reviveButton.onClick.AddListener(OnRevive);
    }

    public void Show()
    {
        popupPanel.SetActive(true);
    }

    public void Hide()
    {
        popupPanel.SetActive(false);
    }

    private void OnRevive()
    {
        Debug.Log("Revive button pressed.");
        Hide();
    }

    private void OnGiveup()
    {
        Debug.Log("Give Up pressed, resetting.");
        revolver.Reset();
        Hide();
    }

    private void OnDestroy()
    {
        if (giveupButton != null)
            giveupButton.onClick.RemoveListener(Hide);

        if (reviveButton != null)
            reviveButton.onClick.RemoveListener(OnRevive);
    }
}

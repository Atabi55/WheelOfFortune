using UnityEngine;
using UnityEngine.UI;

public class ExitMenuManager : MonoBehaviour
{
    [Header("Main HUD")]
    [SerializeField] private Button mainExitButton;

    [Header("Exit Panel (Starts Inactive)")]
    [SerializeField] private GameObject exitPanel;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button goBackButton;

    [Header("References")]
    [SerializeField] private RevolverController revolver;

    private void Awake()
    {
        // Ensure the exit panel starts disabled
        if (exitPanel != null)
            exitPanel.SetActive(false);

        // Hook up the always-active main exit button
        if (mainExitButton != null)
            mainExitButton.onClick.AddListener(OnMainExitClicked);

        // Hook up the panel's internal buttons
        if (confirmButton != null)
            confirmButton.onClick.AddListener(OnConfirmClicked);

        if (goBackButton != null)
            goBackButton.onClick.AddListener(OnGoBackClicked);
    }

    private void OnMainExitClicked()
    {
        // Only open the exit panel if the wheel is NOT spinning
        if (revolver != null && !revolver.IsSpinning)
        {
            if (exitPanel != null)
                exitPanel.SetActive(true);
        }
    }

    private void OnConfirmClicked()
    {
        // Reset the game exactly like the bomb popup "give up" button
        if (revolver != null)
        {
            revolver.Reset();
        }

        // Hide/close the exit panel after resetting
        if (exitPanel != null)
            exitPanel.SetActive(false);
    }

    private void OnGoBackClicked()
    {
        // Simply hide the panel without resetting
        if (exitPanel != null)
            exitPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (mainExitButton != null) mainExitButton.onClick.RemoveListener(OnMainExitClicked);
        if (confirmButton != null) confirmButton.onClick.RemoveListener(OnConfirmClicked);
        if (goBackButton != null) goBackButton.onClick.RemoveListener(OnGoBackClicked);
    }
}
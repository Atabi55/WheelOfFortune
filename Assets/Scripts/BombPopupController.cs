
using UnityEngine;
using UnityEngine.UI;

public class BombPopupController : MonoBehaviour
{
    [Header("Popup")]
    [SerializeField] private GameObject popupPanel;

    [Header("Popup Buttons")]
    [SerializeField] private Button closeButton;
    [SerializeField] private Button continueButton;

    private void Awake()
    {
        if (popupPanel == null)
            popupPanel = gameObject;

        popupPanel.SetActive(false);

        if (closeButton != null)
            closeButton.onClick.AddListener(Hide);

        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinue);
    }

    public void Show()
    {
        popupPanel.SetActive(true);
    }

    public void Hide()
    {
        popupPanel.SetActive(false);
    }

    private void OnContinue()
    {
        Hide();

        // Add your continue behavior here.
        Debug.Log("Continue button pressed.");
    }

    private void OnDestroy()
    {
        if (closeButton != null)
            closeButton.onClick.RemoveListener(Hide);

        if (continueButton != null)
            continueButton.onClick.RemoveListener(OnContinue);
    }
}

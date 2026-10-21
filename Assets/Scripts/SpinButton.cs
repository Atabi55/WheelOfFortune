
using UnityEngine;
using UnityEngine.UI;

public class SpinButton : MonoBehaviour
{
    [SerializeField] private RevolverController revolverController;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();

        if (button == null)
        {
            Debug.LogError("No Button component found!", this);
            return;
        }

        if (revolverController == null)
        {
            revolverController = FindFirstObjectByType<RevolverController>();
        }

        if (revolverController == null)
        {
            Debug.LogError("No revolverController component found!", this);
            return;
        }

        button.onClick.AddListener(StartSpin);
    }

    private void StartSpin()
    {
        revolverController.Spin();
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(StartSpin);
        }
    }
}

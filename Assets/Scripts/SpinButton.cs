
using UnityEngine;
using UnityEngine.UI;

public class SpinButton : MonoBehaviour
{
    [SerializeField] private RevolverSpin revolverSpin;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();

        if (button == null)
        {
            Debug.LogError("No Button component found!", this);
            return;
        }

        if (revolverSpin == null)
        {
            revolverSpin = FindFirstObjectByType<RevolverSpin>();
        }

        if (revolverSpin == null)
        {
            Debug.LogError("No RevolverSpin component found!", this);
            return;
        }

        button.onClick.AddListener(StartSpin);
    }

    private void StartSpin()
    {
        revolverSpin.Spin();
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(StartSpin);
        }
    }
}

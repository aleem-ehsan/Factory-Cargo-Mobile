using UnityEngine;
using UnityEngine.InputSystem;

public class MobileInputEnabler : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;

    void Awake()
    {
        if (inputActions == null)
        {
            Debug.LogError("Input Actions asset not assigned to MobileInputEnabler.");
            return;
        }

        // Enable all action maps in the asset
        foreach (var actionMap in inputActions.actionMaps)
        {
            actionMap.Enable();
        }

        

        Debug.Log("Mobile Input Enabler: Touch scheme enabled, others disabled.");
    }

    void OnDestroy()
    {
        // Disable the action maps when the object is destroyed
        if (inputActions != null)
        {
            foreach (var actionMap in inputActions.actionMaps)
            {
                actionMap.Disable();
            }
        }
    }
} 
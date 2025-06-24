using UnityEngine;

public class CamsController : MonoBehaviour
{
    public GameObject Train_Cam;
    public GameObject Gameplay_Cam;


    // --------------------- Singleton ---------------
    public static CamsController Instance { get; private set; }

    void Awake()
    {
        Debug.Log("CamsController Awake called");
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("CamsController instance set");
        }
        else
        {
            Debug.Log("Destroying duplicate CamsController");
            Destroy(gameObject);
            return;
        }

       
    }

    public void SetGameplayCamActive(bool isActive)
    {
        if (Gameplay_Cam != null)
        {
            Debug.Log("Gameplay_Cam switched.");

            Gameplay_Cam.SetActive(isActive);
        }
        else
        {
            Debug.LogError("Gameplay_Cam is not assigned in CamsController.");
        }
    }

}

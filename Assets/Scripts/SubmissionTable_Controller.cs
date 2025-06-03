using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;



public class SubmissionTable_Controller : MonoBehaviour
{
    public List<GameObject> Required_Resources;
    private Vector3 CurrentPlacingPosition;
    [SerializeField] private Transform PlaceHolder;

    private int placementCount = 0;
    private Dictionary<ResourceType, GameObject> resourceUIItems = new Dictionary<ResourceType, GameObject>();

    [Header("Required Resources")]
    public RequiredResource[] requiredResources;

    [Header("UI Elements")]
    [Tooltip("Gameobbject for the required resource list item")]
    [SerializeField] private Transform ListContainer;
    [SerializeField] private GameObject PrefabListItem; // * Prefab of the UI list-item 




    // --------------- Singelton ---------------
    public static SubmissionTable_Controller Instance { get; private set; }


    // -------------- Events ----------------





    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }



        // Initialize required resources if not set in inspector
        // InitailizeRequiredItems(); // We will remove this call as LevelManager will set the requirements




        // check if the Platform is Android or IOS
        #if UNITY_ANDROID || UNITY_IOS
            // Initialize the required resources for mobile platforms
        
        Debug.Log("Mobile Platform Detected. Initializing required resources for mobile.");
        #endif
    }



/// <summary>
/// Initialize the Required Items of this Level
/// Update the UI
/// </summary>
/// <param name="requiredResources"></param>
    public void InitailizeRequiredItems(RequiredResource[] requiredResources)
    {
         this.requiredResources = requiredResources;

        // Clear existing UI items
        resourceUIItems.Clear();
        // Clear the ListContainer
        foreach (Transform child in ListContainer)
        {
            Destroy(child.gameObject);
        }
        // Populate the UI with required resources
        foreach (RequiredResource resource in requiredResources)
        {
            GameObject uiItem = Instantiate(PrefabListItem, ListContainer);
            TextMeshProUGUI textComponent = uiItem.GetComponentInChildren<TextMeshProUGUI>();
            SpriteRenderer spriteRenderer = uiItem.GetComponentInChildren<SpriteRenderer>();

            if (textComponent != null)
                // textComponent.text = $"{resource.type}: {resource.quantity}";
                textComponent.text = $": {resource.quantity}";

            if (spriteRenderer != null)
                spriteRenderer.sprite = Resources.Load($"Sprites/{resource.type}", typeof(Sprite)) as Sprite;

            resourceUIItems[resource.type] = uiItem; // Store the UI item for later updates
        }
        // Initialize the current placing position
        CurrentPlacingPosition = PlaceHolder.position;
        placementCount = 0; // Reset the placement count
        Debug.Log("Required resources initialized and UI updated.");


    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CurrentPlacingPosition = PlaceHolder.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Resource"))
        {
            Debug.Log("Resource Detected by Table");
            
            // Check for MetalBar
            if (other.TryGetComponent<MetalBar>(out MetalBar metalBar))
            {
                // if(metalBar.resourceType == ResourceType.MetalBar)
                // {
                //     ProcessResource(ResourceType.MetalBar, metalBar.gameObject);
                // }else if(metalBar.resourceType == ResourceType.TexturedBar)
                // {
                //     ProcessResource(ResourceType.TexturedBar, metalBar.gameObject);   
                // }
                // else if(metalBar.resourceType == ResourceType.StoneBlock)
                // {
                //     ProcessResource(ResourceType.StoneBlock, metalBar.gameObject);   
                // }

                ProcessResource(metalBar.resourceType, metalBar.gameObject);
            }
            // Check for WoodPlank
           
        }
    }

    private void ProcessResource(ResourceType resourceType, GameObject resourceObject)
    {
        Debug.Log($"Processing resource: {resourceType}");
        // Find and update the required resource
        for (int i = 0; i < requiredResources.Length; i++)
        {
            if (requiredResources[i].type == resourceType && requiredResources[i].quantity > 0)
            {
                // Reduce the quantity
                RequiredResource updatedResource = requiredResources[i];
                updatedResource.quantity--;
                requiredResources[i] = updatedResource;

                // Update UI
                UpdateUI(resourceType, updatedResource.quantity);

                // Place the resource
                PlaceResourceOnTable(resourceObject);

                // Check if all resources are collected
                CheckLevelCompletion(); // TODO: TESTING --- Uncomment this when want to check for level completion
                break;
            }
        }
    }

    private void UpdateUI(ResourceType resourceType, int quantity)
    {
        if (resourceUIItems.TryGetValue(resourceType, out GameObject uiItem))
        {
            TextMeshProUGUI quantityText = uiItem.GetComponentInChildren<TextMeshProUGUI>();
            if (quantityText != null)
            {
                quantityText.text = ": "+ quantity.ToString();
            }
        }
    }


    private void PlaceResourceOnTable(GameObject resourceObject)
    {
        // Disable components
        resourceObject.GetComponent<BoxCollider>().enabled = false;
        if (resourceObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // Animate placement
        resourceObject.transform.DOJump(CurrentPlacingPosition, 0.5f, 1, 0.5f);
         // Set parent and rotate
        resourceObject.transform.DOLocalRotate(new Vector3(0, 270,0  ), 0.5f);  // * Do a local Rotation because the resourceObject is a child of the Table



        resourceObject.transform.SetParent(transform);


        placementCount++;

        // Update position for next resource
        CurrentPlacingPosition.z += 0.5f;
        if (placementCount == 4)
        {
            placementCount = 0;
            CurrentPlacingPosition.z -= 2;
            CurrentPlacingPosition.y += 0.5f;
        }
    }

    public void CheckLevelCompletion()
    {
        bool allResourcesCollected = true;
        foreach (RequiredResource resource in requiredResources)
        {
            if (resource.quantity > 0)
            {
                allResourcesCollected = false;
                break;
            }
        }

        if (allResourcesCollected)
        {
            Debug.Log("Level Win! All required resources have been collected!");
            // OnLevelWin?.Invoke(); // Trigger level completion event
            My_UIManager.Instance.ShowGameWinPanel(); // Show the game win panel
            LevelManager.Instance.CurrentLevelCompleted(); // Save the level progress

            // * Stop the Time
            Time.timeScale = 0;

        }else{
            // * Check if the time is up then the level is lost as Resources not collected
            if(TimerController.Instance.isTimeUp){
                Debug.Log("Level Lose! Not all required resources have been collected!");
                My_UIManager.Instance.ShowGameLosePanel(); // Show the game win panel
            }
        }
    }


// Time Up Function
/*
    // public void TimeEnded(){  // this is only checked when the time is up for a Level
    //      bool allResourcesCollected = true;
    //     foreach (RequiredResource resource in requiredResources)
    //     {
    //         if (resource.quantity > 0)
    //         {
    //             allResourcesCollected = false;
    //             break;
    //         }
    //     }

    //     if (allResourcesCollected)
    //     {
    //         Debug.Log("Level Win! All required resources have been collected!");
    //         // OnLevelWin?.Invoke(); // Trigger level completion event
    //         // My_UIManager.Instance.ShowGameWinPanel(); // Show the game win panel
    //         Debug.Log("Now invoking OnAllRequirementsCompleted event");
    //         OnAllRequirementsCompleted?.Invoke(); // Notify that all requirements are completed
    //     }else{
           
    //     }
    // }
*/

    public ResourceType[] GetRequiredResourcesTypenames(){

        ResourceType[] resourceTypes = new ResourceType[requiredResources.Length];
        for (int i = 0; i < requiredResources.Length; i++)
        {
            resourceTypes[i] = requiredResources[i].type;
        }
        return resourceTypes;
    }

}

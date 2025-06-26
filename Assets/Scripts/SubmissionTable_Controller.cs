using System;
using System.Collections;
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

    public bool is_allResourcesCollected { get; private set; } = false; // Flag to check if all resources are collected

    [Space(10)]
    [Header("UI Elements")]
    [Tooltip("Gameobbject for the required resource list item")]
    [SerializeField] private Robot_SubmissionTable robotSubmissionTable; // * Reference to the Robot Submission Table


     public List<Transform> ProductsOnTable = new List<Transform>(); // List to keep track of products on the table



    // --------------- Singelton ---------------
    public static SubmissionTable_Controller Instance { get; private set; }


    // -------------- Events ----------------





    void Awake()
    {
        if (Instance == null || Instance != this)
        {
            Instance = this;
        }

            // Get the Robot Submission Table component from the children
        if(robotSubmissionTable == null)
            robotSubmissionTable = GetComponentInChildren<Robot_SubmissionTable>();
        
    


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
                ProcessResource(metalBar.resourceType, metalBar.gameObject);
            }
            // Check for WoodPlank
           
        }
    }

    private void ProcessResource(ResourceType resourceType, GameObject resourceObject)
    {

         // ! important --->> Check if Product is Already On the Table     ||     // ! important --->> Check if the Level is Started or not
        if (ProductsOnTable.Contains(resourceObject.transform) || LevelManager.isGameplayStarted == false)
            return;

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
                // #if !UNITY_EDITOR
                    CheckLevelCompletion(); // TODO: TESTING --- Uncomment this when want to check for level completion
                // # endif
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


        // * Play Collection Sound
        AudioManager_Script.Instance.Play(SoundName.CollectProduct); // Play the collect product sound

        // Disable components
        // resourceObject.GetComponent<BoxCollider>().enabled = false;
        if (resourceObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = true;
            // rb.useGravity = false;
        }
        // Set the parent 
        resourceObject.transform.SetParent(transform);

        // Animate placement
        resourceObject.transform.DOJump(PlaceHolder.position, 0.5f, 1, 0.5f);
        // Set parent and rotate
        // resourceObject.transform.DOLocalRotate(new Vector3(0, 0,0  ), 0.5f);  // * Do a local Rotation because the resourceObject is a child of the Table

        resourceObject.transform.rotation = Quaternion.Euler(0, 0, 0); // Reset rotation to avoid any unwanted rotations



        IncrememntPlaceHolderPosition();

        ProductsOnTable.Add(resourceObject.transform); // Add the resource to the list of products on the table
        robotSubmissionTable.ProductAdded();


    }

/// <summary>
/// Increment the Placeholder position for the next resource placement on the Table.
/// </summary>
    private void IncrememntPlaceHolderPosition()
    {
        placementCount++;

        // Update position for next resource
        // PlaceHolder.position.z += 0.5f;
        PlaceHolder.localPosition += new Vector3(0.5f, 0, 0); // Move the placeholder position forward
        if (placementCount == 4)
        {
            placementCount = 0;
            PlaceHolder.localPosition -= new Vector3(2f, 0, 0);
            PlaceHolder.localPosition += new Vector3(0, 0, 1.5f);
        }
    }

    // TODO : call it from Robot when Dropping

    public void DecrementPlaceHolderPosition()
    {
        if(placementCount < 1)  // * if Placement Count is Smaller than Zero
            return;

        
        placementCount--;

        // Update position for next resource
        PlaceHolder.localPosition -= new Vector3(0.5f, 0, 0); // Move the placeholder position backward
        if (placementCount < 0)
        {
            placementCount = 3;
            PlaceHolder.localPosition += new Vector3(2f, 0, 0);
            PlaceHolder.localPosition -= new Vector3(0, 0, 1.5f);
        }
    }


    public void CheckLevelCompletion()
    {
        is_allResourcesCollected = true;
        foreach (RequiredResource resource in requiredResources)
        {
            if (resource.quantity > 0)
            {
                is_allResourcesCollected = false;
                break;
            }
        }

        if (is_allResourcesCollected)
        {
            Debug.Log("Level Win! All required resources have been collected!");
            // OnLevelWin?.Invoke(); // Trigger level completion event

            // TODO: Move this to the Robot_SubmissionTable when all the resources Dropped onto the TRAIN
            // My_UIManager.Instance.ShowGameWinPanel(); // Show the game win panel
            // LevelManager.Instance.CurrentLevelCompleted(); // Save the level progress
            

            // // * Stop the Time
            // StartCoroutine(StopTime(0.5f));
        }
        else{
            // * Check if the time is up then the level is lost as Resources not collected
            if(TimerController.Instance.isTimeUp){
                LevelManager.Instance.LevelFailed();
            }
        }
    }

    public IEnumerator StopTime(float seconds){
        yield return new WaitForSeconds(seconds);
        Time.timeScale = 0; // Stop the time
        
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

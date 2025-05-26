using System;
using System.Collections.Generic;
using DG.Tweening;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;

public enum ResourceType
{
    MetalBar,
    WoodPlank,
    StoneBlock
}

[System.Serializable]
public struct RequiredResource
{
    public ResourceType type;
    public int quantity;

    public RequiredResource(ResourceType type, int quantity)
    {
        this.type = type;
        this.quantity = quantity;
    }
}

public class SubmissionTable_Controller : MonoBehaviour
{
    public List<GameObject> Required_Resources;
    private Vector3 CurrentPlacingPosition;
    [SerializeField] private Transform PlaceHolder;

    private int placementCount = 0;
    private Dictionary<ResourceType, GameObject> resourceUIItems = new Dictionary<ResourceType, GameObject>();

    [Header("Required Resources")]
    [SerializeField] private RequiredResource[] requiredResources;

    [Header("UI Elements")]
    [Tooltip("Gameobbject for the required resource list item")]
    [SerializeField] private Transform ListContainer;
    [SerializeField] private GameObject PrefabListItem;




    // --------------- Singelton ---------------
    public static SubmissionTable_Controller Instance { get; private set; }


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }



        // Initialize required resources if not set in inspector
        InitailizeRequiredItems();
    }

    private void InitailizeRequiredItems()
    {
        if (requiredResources == null || requiredResources.Length == 0)
        {
            requiredResources = new RequiredResource[]
            {
                new RequiredResource(ResourceType.MetalBar, 2),
                // new RequiredResource(ResourceType.WoodPlank, 3),
                // new RequiredResource(ResourceType.StoneBlock, 1)
            };

            foreach (RequiredResource resource in requiredResources)
            {
                GameObject newItem = Instantiate(PrefabListItem, ListContainer);
                newItem.GetComponentInChildren<TextMeshProUGUI>().text = resource.quantity.ToString();
                newItem.GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/" + resource.type.ToString());
                resourceUIItems[resource.type] = newItem;
            }
        }
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
                if(metalBar.resourceType == ResourceType.MetalBar)
                {
                    ProcessResource(ResourceType.MetalBar, metalBar.gameObject);
                }else if(metalBar.resourceType == ResourceType.WoodPlank)
                {
                    ProcessResource(ResourceType.WoodPlank, metalBar.gameObject);   
                }
                else if(metalBar.resourceType == ResourceType.StoneBlock)
                {
                    ProcessResource(ResourceType.StoneBlock, metalBar.gameObject);   
                }

            }
            // Check for WoodPlank
           
        }
    }

    private void ProcessResource(ResourceType resourceType, GameObject resourceObject)
    {
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
                CheckLevelCompletion();
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
                quantityText.text = quantity.ToString();
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
        }

        // Animate placement
        resourceObject.transform.DOJump(CurrentPlacingPosition, 1f, 1, 0.5f);
        placementCount++;

        // Update position for next resource
        CurrentPlacingPosition.z += 0.5f;
        if (placementCount == 4)
        {
            placementCount = 0;
            CurrentPlacingPosition.z -= 2;
            CurrentPlacingPosition.y += 0.5f;
        }

        // Set parent and rotate
        resourceObject.transform.SetParent(transform);
        resourceObject.transform.DORotate(new Vector3(0, 90, 0), 0.5f);
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
        }
    }


    public void TimeEnded(){  // this is only checked when the time is up for a Level
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
        }else{
            Debug.Log("Level Lose! Not all required resources have been collected!");
            My_UIManager.Instance.ShowGameLosePanel(); // Show the game win panel
        }
    }
}

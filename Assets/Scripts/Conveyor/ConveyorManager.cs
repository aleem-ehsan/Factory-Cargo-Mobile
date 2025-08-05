using UnityEngine;
using System.Collections.Generic;
using System;
namespace Hypertonic.GridPlacement.Example.BasicDemo
{
    public enum ConveyorType
    {
        Long,
        Straight,
        Short,
        Curved,
        Up,
        Bumper,
    }

    public class ConveyorManager : MonoBehaviour
    {
        public static ConveyorManager Instance { get; private set; }

        [System.Serializable]
        public class ConveyorTypeCount
        {
            public ConveyorType conveyorType;
            public int maxCount;
            public int currentCount;
        }

        [SerializeField] private List<ConveyorTypeCount> AllConveyorTypeCounts = new List<ConveyorTypeCount>();

        // Holds the currently selected Conveyor on the grid. Null if none is selected.
        public Conveyor selectedConveyor = null;

        public static ConveyorType lastCreatedConveyorType = ConveyorType.Long; // Default value, can be changed as needed

        private GameObject lastCreatedConveyor_GameObject; // Reference to the last created conveyor prefab, if needed


        /// <summary>
        /// New Chain of Conveyors Logics
        /// </summary>

        public Vector2Int NextGridCellIndex = new(0, 0);
        public Vector3 NextValidPos = Vector3.zero; // This will be the next valid position for the conveyor to be placed on the grid


        private int TotalPlacedConveyorsCount = 0;


        // Additional Offset for different Conveyor Types
    public Dictionary<ConveyorType, Vector3> ConveyorTypeOffests = new Dictionary<ConveyorType, Vector3>
        {
            { ConveyorType.Long, new Vector3(0, 0, 0) },
            { ConveyorType.Straight, new Vector3(0.5f, 0, 1.0f) },
            { ConveyorType.Short, new Vector3(0, 0, 0) },
            { ConveyorType.Curved, new Vector3(1.9f, 0, 0f) },
            { ConveyorType.Up, new Vector3(0, 0, 0) },
            { ConveyorType.Bumper, new Vector3(-0.2f, 0, 0) }
        };

    
// ------------ EVENT  -------------
    public static event System.Action<ConveyorType> OnConveyorMaxLimitReached;
    public static event System.Action<ConveyorType> OnConveyorCanceledOrDeleted;




        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                Debug.Log("ConveyorManager initialized");
            }
            else
            {
                Destroy(gameObject);
            }
        }


        void OnEnable()
        {
            Button_CancelPlacement.OnCancelPlacementPressed += HandleCancelPlacement;
            ExampleGridObject.OnObjectSelected += ConveyorIsBeingSelected;

            Button_Delete.OnDeletePressed += ConveyorDeleted;
            ExampleGridObject.OnConveyorDeleted += ConveyorDeleted;
        }
        void OnDisable()
        {
            Button_CancelPlacement.OnCancelPlacementPressed -= HandleCancelPlacement;
            ExampleGridObject.OnObjectSelected -= ConveyorIsBeingSelected;

            Button_Delete.OnDeletePressed -= ConveyorDeleted;
            ExampleGridObject.OnConveyorDeleted -= ConveyorDeleted;

        }

        private void ConveyorDeleted()
        {
            Debug.Log("Conveyor Deleted Called");
            if (selectedConveyor != null)
            {
                DecreaseConveyor_CurrentCount(selectedConveyor.conveyorType);
                selectedConveyor = null;
                OnConveyorCanceledOrDeleted?.Invoke(lastCreatedConveyorType); // to update the Count-Text of the Button

                
            }
            else
            {
                Debug.LogError("No conveyor is currently selected to delete.");
            }

            // Decrease the Total Placed Conveyors Count
            ConveyorManager.Instance.DecreaseTotalPlacedConveyorsCount();
        }

        private void HandleCancelPlacement()
        {
            DecreaseConveyor_CurrentCount(lastCreatedConveyorType);
            OnConveyorCanceledOrDeleted?.Invoke(lastCreatedConveyorType); // to update the Count-Text of the Button
        }

        // ----------------- Sample Functions ------------------------
        /*
                public bool CanPlaceConveyor(ConveyorType conveyorType)
                {
                    Debug.Log($"Checking if can place conveyor type: {conveyorType}");
                    var conveyorCount = conveyorCounts.Find(c => c.conveyorType == conveyorType);
                    if (conveyorCount == null)
                    {
                        Debug.Log($"No count configuration found for conveyor type: {conveyorType}");
                        return false;
                    }

                    Debug.Log($"Current count for {conveyorType}: {conveyorCount.currentCount}/{conveyorCount.maxCount}");
                    bool canPlace = conveyorCount.currentCount < conveyorCount.maxCount;
                    if (!canPlace)
                    {
                        Debug.Log($"Cannot place more {conveyorType} conveyors. Current: {conveyorCount.currentCount}, Max: {conveyorCount.maxCount}");
                    }
                    return canPlace;
                }




                public void ResetConveyorCounts()
                {
                    foreach (var conveyorCount in conveyorCounts)
                    {
                        conveyorCount.currentCount = 0;
                    }
                    Debug.Log("Conveyor counts have been reset.");
                }
        */

        public void IncreaseConveyor_CurrentCount(ConveyorType conveyorType)
        {
            Debug.Log($"Attempting to decrease count for conveyor type: {conveyorType}");
            var conveyorCount = AllConveyorTypeCounts.Find(c => c.conveyorType == conveyorType);
                conveyorCount.currentCount++;
            if(conveyorCount.currentCount >= conveyorCount.maxCount)
            {
                Debug.Log("Cannot Place more conveyors of this Type: " + conveyorType);
                OnConveyorMaxLimitReached?.Invoke(conveyorType);
            }

            lastCreatedConveyorType = conveyorType; // Update the last created conveyor type
        }

        public void DecreaseConveyor_CurrentCount(ConveyorType conveyorType){
            // decrement the current count of the lastCreatedConveyorType Conveyor from the conveyorCounts list
            var conveyorCount = AllConveyorTypeCounts.Find(c => c.conveyorType == conveyorType);
            if (conveyorCount != null && conveyorCount.currentCount > 0)
            {
                conveyorCount.currentCount--;
                Debug.Log($"Decreased count for {lastCreatedConveyorType}. Current count: {conveyorCount.currentCount}");
            }
            else
            {
                Debug.Log($"Cannot decrease count for {lastCreatedConveyorType}. It may not exist or is already at zero.");
            }
        }
        public int GetRemainingCount(ConveyorType conveyorType)
        {
            var conveyorCount = AllConveyorTypeCounts.Find(c => c.conveyorType == conveyorType);
            if (conveyorCount == null) return 0;
            return conveyorCount.maxCount - conveyorCount.currentCount;
        }

        public void ConveyorIsBeingSelected(GameObject selectedObject){
            Debug.Log($"ConveyorManager: Conveyor is being selected: {selectedObject.name}");
            if (selectedObject.TryGetComponent<Conveyor>(out Conveyor conveyor))
            {
                selectedConveyor = conveyor;
                Debug.Log($"Selected Conveyor: {conveyor.name} of type {conveyor.conveyorType}");
            }
            else
            {
                Debug.Log("Selected object is not a Conveyor.");
            }
        }


    // Set Conveyor Quantites from the LevelManager
    public void SetAllConveyorQuantities(ConveyorStats[] conveyorStats){
        // * reset the List of Conveyor Count
        AllConveyorTypeCounts.Clear();

        // conveyorCounts = new List<ConveyorTypeCount>();
        if (AllConveyorTypeCounts != null)
            {
            
            foreach (var conveyorStat in conveyorStats)
            {
                // Add new conveyor type to the list
                AllConveyorTypeCounts.Add(new ConveyorTypeCount 
                { 
                    conveyorType = conveyorStat.conveyorType,
                    maxCount = conveyorStat.maxCount,
                    currentCount = 0
                });
                Debug.Log($"Added new conveyor type {conveyorStat.conveyorType} with max count {conveyorStat.maxCount}");


                // * Create the button in the UI canvas
                CreateButtonInCanvas(conveyorStat.conveyorType, conveyorStat.maxCount);
            }
        }
    }


/// <summary>
/// Adding a new Conveyor Type and its Quantity to the AllConveyorTypeCounts list.
/// </summary>
/// <param name="conveyorType"></param>
/// <param name="quantity"></param>
    public void AddConveyorTypeAndQuantity(ConveyorStats conveyorStats)
        {
            // Check if the conveyor type already exists
            var existingConveyor = AllConveyorTypeCounts.Find(c => c.conveyorType == conveyorStats.conveyorType);


            // TODO :            // ! if the Conveyor Exists, then LOGIC is not Fully implemented 
            if (existingConveyor != null)  // ! if the Conveyor Exists, then LOGIC is not Fully implemented
            {
                Debug.Log($"Conveyor type {conveyorStats.conveyorType} already exists. Updating quantity.");
                existingConveyor.maxCount += conveyorStats.maxCount; // Increase the max count
                
                // ! update the Button's Quantity UI COUNT
            }
            else  // * if the Conveyor Type does not exist, then add it to the list
            {
                // Add a new conveyor type with the specified quantity
                AllConveyorTypeCounts.Add(new ConveyorTypeCount 
                { 
                    conveyorType = conveyorStats.conveyorType, 
                    maxCount = conveyorStats.maxCount, 
                    currentCount = 0 
                });
                Debug.Log($"Added new conveyor type {conveyorStats.conveyorType} with max count {conveyorStats.maxCount}");

                
                // Create a button in the UI canvas for this conveyor type
                CreateButtonInCanvas(conveyorStats.conveyorType, conveyorStats.maxCount);
            }


    }


/// <summary>
/// Creates a button in the UI canvas using the My_UIManager.
/// </summary>
/// <param name="conveyorType"></param>
/// <param name="quantity"></param>
    private void CreateButtonInCanvas(ConveyorType conveyorType, int quantity)
        {
           Vector3 offset =  ConveyorTypeOffests[conveyorType];
            // Create a button in the UI canvas for the conveyor type
            My_UIManager.Instance.CreateGridPlacementButton(conveyorType, quantity , offset);
        }


    public List<ConveyorTypeCount> GetAllConveyorTypeCounts()
        {
            return AllConveyorTypeCounts;
        }


    public GameObject GetLastCraetedConveyor(){
        if(lastCreatedConveyor_GameObject == null)
        {
            Debug.LogError("Last created conveyor GameObject is null. Please set it before accessing.");
            return null;
        }
            return lastCreatedConveyor_GameObject;
    }


    /// <summary>
    /// Called from Conveyor_Button_GridObjectSelectionOption to set the LAST-CREATED-CONVEYOR GameObject. So that it can be used later for DELETION or other operations.
    /// </summary>
    /// <param name="conveyorGameObject"></param>
    public void SetLastCreatedConveyor(GameObject conveyorGameObject)
        {
            lastCreatedConveyor_GameObject = conveyorGameObject;
            Debug.Log($"Last created conveyor set to: {conveyorGameObject.name}");

            // set the Selected Conveyor to this conveyorGameObject
            if (conveyorGameObject.TryGetComponent<Conveyor>(out Conveyor conveyor))
            {
                selectedConveyor = conveyor;
                Debug.Log($"Selected Conveyor set to: {conveyor.name} of type {conveyor.conveyorType}");
            }
            else
            {
                Debug.LogError("The provided GameObject does not have a Conveyor component.");
            }
        }


    // Update the Next Grid Cell Index
    public void UpdateNextGridCellIndex(Conveyor conveyor)
    {
        Vector3 validPos = conveyor.nextConveyorGridCellPosition.position;

        NextValidPos = validPos; // Update the NextValidPos to the position of the next conveyor grid cell


        // get Grid Cell index from the position of the ValidPos position 
        var gridManager = Hypertonic.GridPlacement.GridManagerAccessor.GridManager;
        var gridSettings = gridManager.GridSettings;
        float gridRotation = gridManager.RuntimeGridRotation;
        Vector3 gridPosition = gridManager.RuntimeGridPosition;
        
        NextGridCellIndex = Hypertonic.GridPlacement.PlacementGrid.GetCellIndexFromWorldPosition(validPos, gridSettings, gridRotation, gridPosition);

        Debug.Log($"Next conveyor grid cell index from {conveyor.name}: {NextGridCellIndex}");
    }



    public Vector2Int GetNextValidCellPosForConveyor(){
        if(NextGridCellIndex == null)
        {
            Debug.LogError("NextGridCellIndex is null. Please update it before accessing.");
            return Vector2Int.zero;
        }
        return NextGridCellIndex;
    }
    public Vector3 GetNextValidCellPosWorldPos(){
        return NextValidPos;
    }


    public void IncreaseTotalPlacedConveyorsCount()
    {
        TotalPlacedConveyorsCount++;
    }
    public void DecreaseTotalPlacedConveyorsCount()
    {
        if(TotalPlacedConveyorsCount > 0)
        TotalPlacedConveyorsCount--;
    }

    public int GetTotalPlacedConveyorsCount()
    {
        return TotalPlacedConveyorsCount;
    }


    }
}

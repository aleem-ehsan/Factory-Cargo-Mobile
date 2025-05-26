using UnityEngine;
using System.Collections.Generic;
using System;

namespace Hypertonic.GridPlacement.Example.BasicDemo
{
    public enum ConveyorType
    {
        Long,
        Straight,
        Short
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

        [SerializeField] private List<ConveyorTypeCount> conveyorCounts = new List<ConveyorTypeCount>();

        // Holds the currently selected Conveyor on the grid. Null if none is selected.
        public Conveyor selectedConveyor = null;

        public static ConveyorType lastCreatedConveyorType = ConveyorType.Long; // Default value, can be changed as needed



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
            Button_Delete.OnDeletePressed += ConveyorDeleted;
            ExampleGridObject.OnObjectSelected += ConveyorIsBeingSelected;
        }
        void OnDisable()
        {
            Button_CancelPlacement.OnCancelPlacementPressed -= HandleCancelPlacement;
            Button_Delete.OnDeletePressed -= ConveyorDeleted;
            ExampleGridObject.OnObjectSelected -= ConveyorIsBeingSelected;

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
            var conveyorCount = conveyorCounts.Find(c => c.conveyorType == conveyorType);
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
            var conveyorCount = conveyorCounts.Find(c => c.conveyorType == conveyorType);
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
            var conveyorCount = conveyorCounts.Find(c => c.conveyorType == conveyorType);
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
    }
}

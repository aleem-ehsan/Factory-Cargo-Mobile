using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Hypertonic.GridPlacement.Example.BasicDemo
{
    /// <summary>
    /// Manages the chain of placed conveyors and handles automatic deletion of downstream conveyors
    /// when a specific conveyor in the chain is deleted.
    /// </summary>
    public class ConveyorChainController : MonoBehaviour
    {
        public static ConveyorChainController Instance { get; private set; }

        [Header("Chain Management")]
        [SerializeField] private Stack<Conveyor> conveyorChain = new Stack<Conveyor>();
        
        [Header("Debug Info")]
        [SerializeField] private List<Conveyor> conveyorChainList = new List<Conveyor>(); // For inspector viewing

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                Debug.Log("ConveyorChainController initialized");
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
        }

        void OnEnable()
        {
            // Subscribe to events
            if (ConveyorManager.Instance != null)
            {
                ExampleGridObject.OnConveyorConfirmed += OnConveyorConfirmed;
                // Subscribe to conveyor deletion
                ExampleGridObject.OnConveyorDeleted += OnConveyorDeleted;
                // We'll subscribe to events in Start to ensure ConveyorManager is initialized
            }
        }
        void OnDisable()
        {
            // // Unsubscribe from events
            // if (ExampleGridObject.OnConveyorConfirmed != null
                ExampleGridObject.OnConveyorConfirmed -= OnConveyorConfirmed;
            // if (ExampleGridObject.OnConveyorDeleted != null)
                ExampleGridObject.OnConveyorDeleted -= OnConveyorDeleted;
        }

        /// <summary>
        /// Called when a conveyor is successfully placed and confirmed
        /// </summary>
        private void OnConveyorConfirmed()
        {
            if (ConveyorManager.Instance != null && ConveyorManager.Instance.selectedConveyor != null)
            {
                AddConveyorToChain(ConveyorManager.Instance.selectedConveyor);
            }
        }

        /// <summary>
        /// Called when a conveyor is deleted
        /// </summary>
        private void OnConveyorDeleted()
        {
            // This will be handled by the specific deletion logic
            Debug.Log("ConveyorChainController: Conveyor deleted event received");
        }

        /// <summary>
        /// Adds a conveyor to the chain
        /// </summary>
        /// <param name="conveyor">The conveyor to add</param>
        public void AddConveyorToChain(Conveyor conveyor)
        {
            if (conveyor == null) return;

            // Check if this conveyor is already in the chain
            if (conveyorChain.Any(c => c == conveyor))
            {
                Debug.LogWarning($"Conveyor {conveyor.name} is already in the chain!");
                return;
            }

            conveyorChain.Push(conveyor);
            UpdateInspectorList();
            
            Debug.Log($"Added conveyor {conveyor.name} to chain. Total conveyors in chain: {conveyorChain.Count}");
        }

        /// <summary>
        /// Removes a specific conveyor from the chain and deletes all downstream conveyors
        /// </summary>
        /// <param name="conveyorToDelete">The conveyor to delete</param>
        public void RemoveConveyorFromChain(Conveyor conveyorToDelete)
        {
            if (conveyorToDelete == null) return;

            Debug.Log($"Removing conveyor {conveyorToDelete.name} from chain and deleting downstream conveyors");

            // Convert stack to list for easier manipulation
            List<Conveyor> chainList = conveyorChain.ToList();
            
            // Find the index of the conveyor to delete
            int deleteIndex = chainList.FindIndex(c => c == conveyorToDelete);
            
            if (deleteIndex == -1)
            {
                Debug.LogWarning($"Conveyor {conveyorToDelete.name} not found in chain!");
                return;
            }

            // Delete all conveyors from the delete index onwards (downstream conveyors)
            for (int i = deleteIndex; i < chainList.Count; i++)
            {
                Conveyor conveyorToRemove = chainList[i];
                if (conveyorToRemove != null && conveyorToRemove.gameObject != null)
                {
                    Debug.Log($"Deleting downstream conveyor: {conveyorToRemove.name}");
                    
                    // Update conveyor count for each deleted conveyor
                    if (ConveyorManager.Instance != null)
                    {
                        ConveyorManager.Instance.DecreaseConveyor_CurrentCount(conveyorToRemove.conveyorType);
                        ConveyorManager.Instance.DecreaseTotalPlacedConveyorsCount();
                    }
                    
                    // Remove from the chain
                    RemoveConveyorFromChainInternal(conveyorToRemove);
                    
                    // Delete from grid and destroy
                    if (GridManagerAccessor.GridManager != null)
                    {
                        GridManagerAccessor.GridManager.DeleteObject(conveyorToRemove.gameObject);
                    }
                    else
                    {
                        Destroy(conveyorToRemove.gameObject);
                    }
                }
            }

            // Clear the chain and rebuild it with remaining conveyors
            conveyorChain.Clear();
            for (int i = 0; i < deleteIndex; i++)
            {
                if (chainList[i] != null && chainList[i].gameObject != null)
                {
                    conveyorChain.Push(chainList[i]);
                }
            }

            UpdateInspectorList();
            Debug.Log($"Chain updated. Remaining conveyors: {conveyorChain.Count}");
        }

        /// <summary>
        /// Removes a single conveyor from the chain without deleting downstream conveyors
        /// </summary>
        /// <param name="conveyor">The conveyor to remove</param>
        private void RemoveConveyorFromChainInternal(Conveyor conveyor)
        {
            if (conveyor == null) return;

            // Convert stack to list, remove the conveyor, and rebuild the stack
            List<Conveyor> tempList = conveyorChain.ToList();
            tempList.Remove(conveyor);
            
            conveyorChain.Clear();
            foreach (var c in tempList)
            {
                conveyorChain.Push(c);
            }
        }

        /// <summary>
        /// Gets the current chain of conveyors as a list (for inspection purposes)
        /// </summary>
        /// <returns>List of conveyors in the chain</returns>
        public List<Conveyor> GetConveyorChain()
        {
            return conveyorChain.ToList();
        }

        /// <summary>
        /// Gets the number of conveyors in the chain
        /// </summary>
        /// <returns>Number of conveyors</returns>
        public int GetChainLength()
        {
            return conveyorChain.Count;
        }

        /// <summary>
        /// Gets the last placed conveyor in the chain
        /// </summary>
        /// <returns>The last placed conveyor or null if chain is empty</returns>
        public Conveyor GetLastPlacedConveyor()
        {
            return conveyorChain.Count > 0 ? conveyorChain.Peek() : null;
        }

        /// <summary>
        /// Updates the inspector list for debugging purposes
        /// </summary>
        private void UpdateInspectorList()
        {
            conveyorChainList = conveyorChain.ToList();
            conveyorChainList.Reverse(); // Reverse to show in chronological order
        }

        /// <summary>
        /// Clears the entire chain (useful for level reset)
        /// </summary>
        public void ClearChain()
        {
            conveyorChain.Clear();
            UpdateInspectorList();
            Debug.Log("Conveyor chain cleared");
        }

        /// <summary>
        /// Checks if a conveyor is in the chain
        /// </summary>
        /// <param name="conveyor">The conveyor to check</param>
        /// <returns>True if the conveyor is in the chain</returns>
        public bool IsConveyorInChain(Conveyor conveyor)
        {
            return conveyor != null && conveyorChain.Any(c => c == conveyor);
        }

        /// <summary>
        /// Gets the position of a conveyor in the chain (0 = first placed, Count-1 = last placed)
        /// </summary>
        /// <param name="conveyor">The conveyor to find</param>
        /// <returns>The position in the chain, or -1 if not found</returns>
        public int GetConveyorPositionInChain(Conveyor conveyor)
        {
            if (conveyor == null) return -1;
            
            List<Conveyor> chainList = conveyorChain.ToList();
            return chainList.FindIndex(c => c == conveyor);
        }
    }
}

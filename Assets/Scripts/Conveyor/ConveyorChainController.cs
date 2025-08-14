using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using DG.Tweening;

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
        [SerializeField] private List<Conveyor> conveyorChain = new List<Conveyor>();
        
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
                // Subscribe to conveyor deletion
                // We'll subscribe to events in Start to ensure ConveyorManager is initialized
            }
        }
        void OnDisable()
        {
            // // Unsubscribe from events
            // if (ExampleGridObject.OnConveyorDeleted != null)
        }

        public void OnConveyorDeleted()
        {
            // Handle conveyor deletion event
            Debug.Log("ConveyorChainController: Conveyor deleted event received");
            
            // Get the last placed conveyor
            Conveyor lastConveyor = GetLastPlacedConveyor();
            RemoveConveyorFromChain(lastConveyor);
            
       
        
        }




        /// <summary>
        /// Called when a conveyor is deleted
        /// </summary>
        public void DeleteAConveyor( Conveyor conveyorToDelete)
        {
             // Update conveyor count for each deleted conveyor
                    if (ConveyorManager.Instance != null && conveyorToDelete != null)
                    {
                        // Vibrate the conveyor for 0.5 seconds before deleting
                        VibrateAndDeleteConveyor(conveyorToDelete);
                    }
                    
                    else
                    {
                        Debug.Log(" ConveyorToDelete is Null, so not deleting it from the grid");
                        Destroy(conveyorToDelete.gameObject);
                    }
            // This will be handled by the specific deletion logic
            Debug.Log("ConveyorChainController: Conveyor deleted event received");
        }

        /// <summary>
        /// Vibrates the conveyor for 0.5 seconds using DOTween rotation, then deletes it
        /// </summary>
        /// <param name="conveyorToDelete">The conveyor to vibrate and delete</param>
        private void VibrateAndDeleteConveyor(Conveyor conveyorToDelete)
        {
            if (conveyorToDelete == null || conveyorToDelete.gameObject == null) return;

            // Store the original rotation
            Vector3 originalRotation = conveyorToDelete.transform.eulerAngles;
            
            // Create a vibration sequence using DOTween
            Sequence vibrationSequence = DOTween.Sequence();
            
            // Add rotation shake animation for 0.5 seconds
            vibrationSequence.Append(conveyorToDelete.transform.DOShakeRotation(0.5f, 5f, 40, 20, false));
            
            // After vibration completes, delete the conveyor
            vibrationSequence.OnComplete(() => {
                // Set resources to fall
                conveyorToDelete.SetMovingResourceToFall();
                
                // Delete the conveyor object from the GRID
                if (GridManagerAccessor.GridManager != null)
                {
                    GridManagerAccessor.GridManager.DeleteObject(conveyorToDelete.gameObject);
                }
                else
                {
                    Destroy(conveyorToDelete.gameObject);
                }
                
                Debug.Log($"Conveyor {conveyorToDelete.name} rotation vibrated and deleted successfully");
            });
            
            Debug.Log($"Starting rotation vibration for conveyor {conveyorToDelete.name}");
        }

        /// <summary>
        /// Adds a conveyor to the chain
        /// </summary>
        /// <param name="conveyor">The conveyor to add</param>
        public void AddConveyorToChain(Conveyor conveyor)
        {
            if (conveyor == null) return;

            // Check if this conveyor is already in the chain   ||  or if it's a bumper conveyor
            if (conveyorChain.Contains(conveyor)  || conveyor.conveyorType == ConveyorType.Bumper)
            {
                Debug.LogWarning($"Conveyor {conveyor.name} is already in the chain!");
                return;
            }

            conveyorChain.Add(conveyor);
            
            Debug.Log($"Added conveyor {conveyor.name} to chain at index {conveyorChain.Count - 1}. Total conveyors in chain: {conveyorChain.Count}");
        }

        /// <summary>
        /// Removes a specific conveyor from the chain and deletes all downstream conveyors
        /// </summary>
        /// <param name="conveyorToDelete">The conveyor to delete</param>
        public void RemoveConveyorFromChain(Conveyor conveyorToDelete)
        {
            if (conveyorToDelete == null || conveyorToDelete.conveyorType == ConveyorType.Bumper){ 
                return;
            }

            Debug.Log($"Removing conveyor {conveyorToDelete.name} from chain and deleting downstream conveyors");


            // Find the index of the conveyor to delete
            int deleteIndex = conveyorChain.IndexOf(conveyorToDelete);
            Debug.Log($"A conveyor is Deleted which is at index: {deleteIndex}");
            
            if (deleteIndex == -1)
            {
                Debug.LogWarning($"Conveyor {conveyorToDelete.name} not found in chain!");
                return;
            }

            // Delete all conveyors from the delete-index+1 onwards (downstream conveyors)
            for (int i = deleteIndex+1; i < conveyorChain.Count; i++) // * +1 because deleteIndex is already deleted and Count is updated
            {
                Conveyor conveyorToRemove = conveyorChain[i];

                if (conveyorToRemove != null && conveyorToRemove.gameObject != null)
                {
                    Debug.Log($"Deleting downstream conveyor: {conveyorToRemove.name}");
                    
                   DeleteAConveyor(conveyorToRemove);
                   ConveyorManager.Instance.DecreaseConveyor_CurrentCount(conveyorToRemove.conveyorType); // Update conveyor count for each deleted conveyor
                }
            }

            // Remove all conveyors from the delete index onwards from the list
            conveyorChain.RemoveRange(deleteIndex, conveyorChain.Count - deleteIndex);

            Debug.Log($"Chain updated. Remaining conveyors: {conveyorChain.Count}");
        }

        /// <summary>
        /// Removes a single conveyor from the chain without deleting downstream conveyors
        /// </summary>
        /// <param name="conveyor">The conveyor to remove</param>
        private void RemoveConveyorFromChainInternal(Conveyor conveyor)
        {
            if (conveyor == null) return;

            conveyorChain.Remove(conveyor);
        }

        /// <summary>
        /// Gets the current chain of conveyors as a list (for inspection purposes)
        /// </summary>
        /// <returns>List of conveyors in the chain</returns>
        public List<Conveyor> GetConveyorChain()
        {
            return new List<Conveyor>(conveyorChain);
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
            return conveyorChain.Count > 0 ? conveyorChain[^1] : null;
        }


        /// <summary>
        /// Clears the entire chain (useful for level reset)
        /// </summary>
        public void ClearChain()
        {
            conveyorChain.Clear();
            Debug.Log("Conveyor chain cleared");
        }

        /// <summary>
        /// Checks if a conveyor is in the chain
        /// </summary>
        /// <param name="conveyor">The conveyor to check</param>
        /// <returns>True if the conveyor is in the chain</returns>
        public bool IsConveyorInChain(Conveyor conveyor)
        {
            return conveyor != null && conveyorChain.Contains(conveyor);
        }

        /// <summary>
        /// Gets the position of a conveyor in the chain (0 = first placed, Count-1 = last placed)
        /// </summary>
        /// <param name="conveyor">The conveyor to find</param>
        /// <returns>The position in the chain, or -1 if not found</returns>
        public int GetConveyorPositionInChain(Conveyor conveyor)
        {
            if (conveyor == null) return -1;
            
            return conveyorChain.IndexOf(conveyor);
        }
    }
}

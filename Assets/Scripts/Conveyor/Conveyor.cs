using UnityEngine;
using Hypertonic.GridPlacement.GridObjectComponents;
using System.Collections.Generic;
using Hypertonic.GridPlacement.Example.BasicDemo;
using System;
using Hypertonic.GridPlacement;







[RequireComponent(typeof(CustomValidator))]
public class Conveyor : MonoBehaviour
{
    private CustomValidator _customValidator;
    public bool isPlaced = false;

        [SerializeField] private BoxCollider[] TriggerColliders;
        [SerializeField] public ConveyorType conveyorType; // Set this in the inspector for each conveyor prefab

        // create a List of Resources moving on me
        // so that when I am InPlacementMode, I can set them to FALL
        [SerializeField]private List<MetalBar> resourcesMovingOnMe = new List<MetalBar>();


        private short numberOfCollisionsWithMachine = 0; 


    [Header("Manual Conveyor Placement Settings")]
    [Tooltip("If Convyeor is Placed Manually, this Should be True")]
    [SerializeField] private bool isManuallyPlaced = false;



    [Header("Connector")]
    [Tooltip("If this conveyor is a connector, it should be true")]
    [SerializeField] private ConveyorConnector thisConnector;


    [Header("Next Conveyor Grid Cell Position")]
    [Tooltip("This is the position of the next Conveyor's Grid Cell")]
    [SerializeField] public Transform nextConveyorGridCellPosition;

    [Header("This Conveyor's Entry Controller")]
    public ConveyorEntryController thisEntryController;
   
        private void Awake()
        {
            _customValidator = GetComponent<CustomValidator>();

            // Only try to increase conveyor count if ConveyorManager exists
            if (ConveyorManager.Instance != null)
            {
                Debug.Log("Im Created for 1st time");

                // ! if a Conveyor is Manually Placed, It should not be added in the ConveyorManager's count
                if(isManuallyPlaced == false)
                {
                    ConveyorManager.Instance.IncreaseConveyor_CurrentCount(conveyorType);

                    
                }
            }
            else
            {
                Debug.Log("Conveyor created without ConveyorManager - this is a manually placed conveyor");
                // For manually placed conveyors, we can skip the ConveyorManager functionality
                isPlaced = true; // Mark as placed since it's manually placed
                foreach (var trigger in TriggerColliders)
                {
                    trigger.enabled = true;
                }
            }

            if(thisConnector == null)
            {
                thisConnector = GetComponentInChildren<ConveyorConnector>();
            }

        }

        private void Start()
        {
            
            // * Increase the Total Placed Conveyors Count
            ConveyorManager.Instance.IncreaseTotalPlacedConveyorsCount();

            // // * Add the Conveyor in the ChainController
            // ConveyorChainController.Instance.AddConveyorToChain(this);
            
        }
/// <summary>
/// When Conveyor is in Placement Mode, it will not accept any resources moving on it
/// </summary>
        public void InPlacementMode()
        {
            isPlaced = false;
            Debug.Log("Conveyor In Placement Mode");

           // * Entry/Exit Colliders
            foreach (var trigger in TriggerColliders)
            {
                // trigger.gameObject.SetActive(false); // Disabling gameobjects enables having Trigger Events
                trigger.enabled = false;
            }

            // * Disable the Connector
                thisConnector.gameObject.SetActive(false);
            
            // Add any visual feedback or behavior changes when not placed
            // For example, you could change the material or disable certain components

            // TODO: make the resourcesMovingOnMe to fall 
            SetMovingResourceToFall();


            ResetConnectors();


        }
        
        /// <summary>
        /// When the conveyor is placed, it will enable the colliders and set the isPlaced flag to true.
        /// </summary>
        public void Placed()
        {
            Debug.Log("Conveyor In PLACE NOW");
            isPlaced = true;
            foreach (var trigger in TriggerColliders)
            {
                // trigger.gameObject.SetActive(true); // Enabling gameobjects enables having Trigger Events
                trigger.enabled = true;
            }

                thisConnector.gameObject.SetActive(true);

            // * Set the Next Conveyor Grid Cell Position
            ConveyorManager.Instance.UpdateNextGridCellIndex(this);

        
        //     // * Check if Conveyor's Entry is Connected with a CONNECTOR, 
           Invoke(nameof(CheckIfConnectionBuilt) , 0.3f) ;
        }

        public void CheckIfConnectionBuilt(){

            if(conveyorType == ConveyorType.Bumper){   // * Bumper Conveyor does not need to check for connection
                return;
            }

            // Check if Conveyor's Entry is Connected with a CONNECTOR, 
            if(thisEntryController.connectedConnector == null){

                SetMovingResourceToFall(); // Set the resources moving on me to fall

                Debug.Log("NO Connection Built. ");
                // ConveyorManager.Instance.IncreaseConveyor_CurrentCount(conveyorType);
                ConveyorManager.Instance.HandleCancelPlacement(); // Handle the cancel placement logic in ConveyorManager


                _customValidator.SetValidation(false); // Set validation to false if no connection is built


                // * Further Deleting the Connected Conveyors
                // if(thisConnector.isConnected){
                //     // Delete the Conveyor connected with it's CONNECTOR
                //         ConveyorEntryController NextConveyors_EntryController =  thisConnector.ConnectedConveyorEntryController;
                //         NextConveyors_EntryController._conveyor.CheckIfConnectionBuilt(); // Disconnect the entry controller from the connector
                // }else{
                //     Debug.Log("No Connected Conveyor to delete.");
                // }


                GridManagerAccessor.GridManager.DeleteObject(this.gameObject); // Delete the conveyor if no connection is built
                // Selected
                
            }else{
                Debug.Log("Connection Built");
                _customValidator.SetValidation(true); // Set validation to false if no connection is built

            }
        }







        /// <summary>
        /// New resources moving on me will be added to the list
        /// </summary>
        public void AddMovingResource(MetalBar resource){
            if(conveyorType == ConveyorType.Bumper)
            {
                Debug.Log("Bumper Conveyor does not accept resources moving on it.");
                return; // Bumper conveyors do not accept resources
            }
            if (resourcesMovingOnMe.Contains(resource) == false)
            {
                resourcesMovingOnMe.Add(resource);
                Debug.Log($"Added metal bar to conveyor {name}. Total resources: {resourcesMovingOnMe.Count}");
            }
            else
            {
                Debug.Log($"Metal bar already exists in conveyor {name}. Total resources: {resourcesMovingOnMe.Count}");
            }
        }

        /// <summary>
        /// moving resources on me will be removed from the list
        /// </summary>
        public void RemoveMovingResource(MetalBar resource)
        {
            if(conveyorType == ConveyorType.Bumper)
            {
                Debug.Log("Bumper Conveyor does not accept resources moving on it.");
                return; // Bumper conveyors do not accept resources
            }
            if (resourcesMovingOnMe.Contains(resource))
            {
                resourcesMovingOnMe.Remove(resource);
                Debug.Log($"Removed metal bar from conveyor {name}. Total resources: {resourcesMovingOnMe.Count}");
            }
            else
            {
                Debug.Log($"Metal bar not found in conveyor {name} when trying to remove. Total resources: {resourcesMovingOnMe.Count}");
            }
        }

        /// <summary>
        /// When the conveyor is in placement mode, the Resources on it will fall
        /// </summary>
        public void SetMovingResourceToFall()  
        {
            Debug.Log("Wasting the Resources Moving on Me");
            foreach (var resource in resourcesMovingOnMe.ToArray()) // ! iterate over a copy to avoid runtime Errors
            {
                if (resource != null)
                {
                    resource.WasteMySelf();
                }
            }
            resourcesMovingOnMe.Clear();
        }

        /// <summary>
        /// We will check what object we hit. If it a wall object we'll set the 
        /// custom validation to be invalid.
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($"Conveyor Triggered with {other.name}");
            // To avoid adding a "wall" tag to the package, we will check the object
            // we've collided with based on if it has the demo wall component. You could also check 
            // which object it is by checking the name or however else you wish.
            // if(other.GetComponent<Machine>() != null)
            if(other.GetComponent<Prop>() != null){
                HandleEnteredPropArea();
            }
            else if(other.CompareTag("Machine"))
            {
                Debug.Log("Conveyor ENTER Triggered with Machine: " + numberOfCollisionsWithMachine);
                HandleEnteredMachineArea();
            }
            else if (other.GetComponent<SubmissionTable_Controller>() != null)
            {
                Debug.Log("Conveyor EXIT Triggered with Submission Table");
                // If the conveyor exits the submission table, we can set the validation to true
                // as it is no longer colliding with a machine.
                HandleEnteredMachineArea();
            }
            
        }

        private void OnTriggerExit(Collider other)
        {
            if(other.GetComponent<Prop>() != null){
                HandleExitedPropArea();
            }
            else if (other.CompareTag("Machine"))
            {
                HandleExitedMachineArea();
            }
            else if (other.GetComponent<SubmissionTable_Controller>() != null)
            {
                Debug.Log("Conveyor EXIT Triggered with Submission Table");
                // If the conveyor exits the submission table, we can set the validation to true
                // as it is no longer colliding with a machine.
                HandleExitedMachineArea();
            }
        }

//   ------ Machine Trigger Handling Functions -----
        private void HandleEnteredMachineArea()
        {
            numberOfCollisionsWithMachine++;
            _customValidator.SetValidation(false);
        }
        private void HandleExitedMachineArea()
        {
            // Check if still colliding with a Machine, and if so, return
               // ! : The error is When Conveyor is in Collision with Two machines, at that Time when it exits Collider of 1 Machine, It enables the 
                                                // ! : CustomValidator, but it should not, as it is still colliding with the other Machine
                                                // ! : So, we need to check if it is still colliding with a Machine, and if so, return   
                
            numberOfCollisionsWithMachine--;
            if (numberOfCollisionsWithMachine < 0)
            {
                numberOfCollisionsWithMachine = 0; // Ensure it doesn't go negative
            }

            if(!IsStillCollidingWithMachine())
            {
                _customValidator.SetValidation(true);
            }

            Debug.Log("HandleExitedMachineArea Conveyor ENTERED Machine Area: " + numberOfCollisionsWithMachine);

        }



//   ------ Props Trigger Handling Functions -----
        private void HandleEnteredPropArea(){
            Debug.Log("Conveyor ENTERED Wall Area");
            numberOfCollisionsWithMachine++;
            _customValidator.SetValidation(false);
        }
        private void HandleExitedPropArea()
        {
            numberOfCollisionsWithMachine--;
        Debug.Log("Conveyor EXITED Prop Area: " + numberOfCollisionsWithMachine);
            if(numberOfCollisionsWithMachine <= 0)
            {
                numberOfCollisionsWithMachine = 0; // Ensure it doesn't go negative
                _customValidator.SetValidation(true);
            }


        }


    private bool IsStillCollidingWithMachine()
    {
        return numberOfCollisionsWithMachine > 0;
    }


    private void ResetConnectors(){
        // Reset the connector state
        if (thisConnector != null)
        {
            thisConnector.DisconnectEntry();
        }
        else
        {
            Debug.Log("No ConveyorConnector found to reset.");
        }
        
        // Also disconnect any other connectors that might be connected to this conveyor's entry controllers
        DisconnectOtherConnectorsFromThisConveyor();
    }
    
    private void DisconnectOtherConnectorsFromThisConveyor()
    {
        // Find all entry controllers on this conveyor
        ConveyorEntryController[] entryControllers = GetComponentsInChildren<ConveyorEntryController>();
        
        foreach (var entryController in entryControllers)
        {
            if (entryController.connectedConnector != null)
            {
                Debug.Log($"Disconnecting other connector from entry: {entryController.name}");
                // Tell the other connector to disconnect from this entry
                entryController.connectedConnector.DisconnectEntry();
            }
        }
    }

    /// <summary>
    /// Re-enables all colliders and connectors for this conveyor. Call after level reload.
    /// </summary>
    public void ResetCollidersAndConnectors()
    {
        foreach (var trigger in TriggerColliders)
        {
            trigger.enabled = true;
        }
        if (thisConnector != null)
        {
            thisConnector.gameObject.SetActive(true);
        }
    }
}

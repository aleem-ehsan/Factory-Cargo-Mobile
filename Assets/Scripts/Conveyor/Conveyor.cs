using UnityEngine;
using Hypertonic.GridPlacement.GridObjectComponents;
using System.Collections.Generic;
using Hypertonic.GridPlacement.Example.BasicDemo;






[RequireComponent(typeof(CustomValidator))]
public class Conveyor : MonoBehaviour
{
    private CustomValidator _customValidator;
    private bool isPlaced = false;

        [SerializeField] private BoxCollider[] TriggerColliders;
        [SerializeField] public ConveyorType conveyorType; // Set this in the inspector for each conveyor prefab

        // create a List of Resources moving on me
        // so that when I am InPlacementMode, I can set them to FALL
        [SerializeField]private List<MetalBar> resourcesMovingOnMe = new List<MetalBar>();



        private void Awake()
        {
            _customValidator = GetComponent<CustomValidator>();

            Debug.Log("Im Created for 1st time");
            ConveyorManager.Instance?.IncreaseConveyor_CurrentCount(conveyorType);
        }

        private void Start()
        {
            // Subscribe to the placement mode event in Start instead of Awake
        }

        public void InPlacementMode()
        {
            isPlaced = false;
            Debug.Log("Conveyor In Placement Mode");
            foreach (var trigger in TriggerColliders)
            {
                trigger.enabled = false;
            }
            // Add any visual feedback or behavior changes when not placed
            // For example, you could change the material or disable certain components

            // TODO: make the resourcesMovingOnMe to fall 
            SetMovingResourceToFall();
        }

        /// <summary>
        /// New resources moving on me will be added to the list
        /// </summary>
        public void AddMovingResource(MetalBar resource){
            if (!resourcesMovingOnMe.Contains(resource))
            {
                resourcesMovingOnMe.Add(resource);
            }
        }

        /// <summary>
        /// moving resources on me will be removed from the list
        /// </summary>
        public void RemoveMovingResource(MetalBar resource)
        {
            if (resourcesMovingOnMe.Contains(resource))
            {
                resourcesMovingOnMe.Remove(resource);
            }
        }

        /// <summary>
        /// When the conveyor is in placement mode, the Resources on it will fall
        /// </summary>
        public void SetMovingResourceToFall()  
        {
            foreach (var resource in resourcesMovingOnMe)
            {
                if (resource != null)
                {
                    resource.Kill();
                }
            }
            resourcesMovingOnMe.Clear();
        }

        public void Placed()
        {
            Debug.Log("Conveyor In PLACE NOW");
            isPlaced = true;
            foreach (var trigger in TriggerColliders)
            {
                trigger.enabled = true;
            }
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
            if(other.GetComponent<Machine>() != null)
            {
                HandleEnteredMachineArea();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<Machine>() != null)
            {
                HandleExitedMachineArea();
            }
        }

        private void HandleEnteredMachineArea()
        {
            _customValidator.SetValidation(false);
        }

        private void HandleExitedMachineArea()
        {
            _customValidator.SetValidation(true);
        }
    }

using Hypertonic.GridPlacement.GridObjectComponents;
using Hypertonic.GridPlacement.Models;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Hypertonic.GridPlacement.Example.BasicDemo
{
    /// <summary>
    /// This is an example of a component that should be added to an object you place on the grid.
    /// This simple implementation just fires of an event so the demo scene knows that the player wants to
    /// modify the position of the grid object.
    /// </summary>
    public class ExampleGridObject : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
    {
        public delegate void OnObjectSelectedEvent(GameObject gameObject);
        public static event OnObjectSelectedEvent OnObjectSelected;

        [SerializeField] private CustomValidator _customValidator;
        [SerializeField] private Conveyor _conveyor;



    // ----------- Events -------------
    public static event System.Action OnConveyorDeleted;

        //--------------- Event to Confirm the Placement of the Conveyor ---------------
        public static event System.Action OnConveyorConfirmed;

        void Awake()
        {
            _customValidator = GetComponent<CustomValidator>();
            _conveyor = GetComponent<Conveyor>();
        }



        public void OnPointerUp(PointerEventData eventData)
        {
            Debug.Log("OnPointerUp ExampleGridObject - Always Called");

            if (GridManagerAccessor.GridManager.IsObjectPlacementValid().Valid)
            {
               OnConveyorConfirmed?.Invoke();
                Debug.Log("ExampleGridObject: Placement is Valid, invoking OnConveyorConfirmed event.");
            }
            else // * DELETE THE OBJECT AS PLACED INVALID 
            {
                /*
                if (_has_Prev_ValidPosition)
                {
                    // First, free up the current grid cells
                    if (_gridObjectInfo != null)
                    {
                        GridManagerAccessor.GridManager.HandleGridObjectRotated();
                    }

                    // Restore position and rotation
                    transform.position = _lastValidPosition;
                    transform.rotation = _lastValidRotation;

                    // Restore grid cell index if available
                    if (_gridObjectInfo != null)
                    {
                        _gridObjectInfo.Setup(_gridObjectInfo.GridKey, _lastValidGridCellIndex, Enums.ObjectAlignment.CENTER);
                        
                        // Update the grid manager to reflect the new position
                        GridManagerAccessor.GridManager.HandleGridObjectRotated();
                        
                        // Get the current placement response and update the highlight
                        var placementResponse = GridManagerAccessor.GridManager.IsObjectPlacementValid();
                        GridManagerAccessor.GridManager.UpdatePlacementIndicator(placementResponse);
                    }
                    else
                    {
                        Debug.LogError("ExampleGridObject: _gridObjectInfo is null. Cannot restore grid cell index.");
                    }
                }
                _has_Prev_ValidPosition = false; // Reset valid position flag
                return;
            }

            // Always invoke the event, regardless of placement validity
            OnConveyorConfirmed?.Invoke();
            Conveyor_Button_GridObjectSelectionOption.SetAnyButtonPressable(true);
                */
                Debug.Log("ExampleGridObject: Placement is Invalid, restoring to last valid position.");



                // GridManagerAccessor.GridManager.DeleteObject(this.gameObject);
                // GridManagerAccessor.GridManager.DeleteObject(gameObject);
                // OnConveyorDeleted?.Invoke();
                HandleConveyorDeleted(); // Call the static method to handle deletion logic


            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("ExampleGridObject: Pointer Down");
            // This handler declaration is needed if the OnPointerUp Handler is implemented.
            if(!_conveyor.isPlaced){
                Debug.Log("ExampleGridObject:   Conveyor is Not Placed but Again PointDown");
                return;
            }
            OnObjectSelected?.Invoke(gameObject);
            Debug.Log("ObjectSelecion Event invoked");
        }


        public static void HandleConveyorDeleted()
        {
            Debug.Log("ExampleGridObject: HandleConveyorDeleted called");
            // * Subscribed by the ConveyorManager 
            OnConveyorDeleted?.Invoke();
            // Here you can add any additional logic needed when a conveyor is deleted
            // For example, you might want to update the UI or notify other components
            // !: instead of Adding Code here, Use the Static Events to subscribe to the events in the ConveyorManager

        }
    }
}
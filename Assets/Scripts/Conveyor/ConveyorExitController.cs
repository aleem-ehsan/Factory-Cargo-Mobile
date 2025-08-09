using UnityEngine;
using Hypertonic.GridPlacement.Example.BasicDemo;
public class ConveyorExitController : MonoBehaviour
{
    public Conveyor ParentConveyor;
    public float OutForce = 2;
    
    [Header("Connector System")]
    [SerializeField] private ConveyorConnector connector;

    [Tooltip("The previous metal bar that exited this conveyor, used to handle connections correctly.")]
    private MetalBar previousMetalBar; 


    [Header("Conveyor Type")]
    public bool isMachine = false;

    void Awake()
    {
        if(!isMachine)
            ParentConveyor = GetComponentInParent<Conveyor>();
        
        // Find connector if not assigned
        if (connector == null)
            Debug.LogError("ConveyorExitController: Connector is not assigned. Please assign a ConveyorConnector in the inspector.");

        previousMetalBar = null;
        isMachine = connector.isMachine; // Get the isMachine value from the parent ConveyorEntryController
    }
    
    public void HandleMetalBarExit(MetalBar metalBar)
    {
        
        // ! Prevent double exit handling
        if(previousMetalBar == metalBar)
            return; 


        previousMetalBar = metalBar; // Store the last exited metal bar

        // ! This Removal is now Performed in the MetalBar itself when the MoveOnConveyor is called
        // if(isMachine == false){ // ! if not Machine then remove resources from the Conveyor
        //     ParentConveyor.RemoveMovingResource(metalBar);
        // }

        // Remove from current conveyor
        // *Leave the current conveyor initially
        metalBar.LeaveCurrentConveyor();

        // Check if we have a connection
        if (connector.IsConnected )
        {
            // Debug.Log($"Conveyor Exit has a Connection {gameObject.name} to {connector.ConnectedConveyorEntryController.name}");
            // Pass to connected conveyor
            ConveyorEntryController connectedEntry = connector.ConveyorEntryController;
            if (connectedEntry != null)
            {
                // leave the current conveyor with no Force
                metalBar.MoveOnConveyor(connectedEntry);
                Debug.Log($"ConveyorExitController: Metal bar {metalBar.name} moved to connected conveyor entry {connectedEntry.name}.");
                // Note: MoveOnConveyor now handles adding the metal bar to the conveyor's resources list when appropriate
            }
        }
        else
        {
             metalBar.movementController.EnablePhyscis();
            Debug.Log($"MetalBar exited with no connection");

            // No connection - make it fall
        }
    }
}
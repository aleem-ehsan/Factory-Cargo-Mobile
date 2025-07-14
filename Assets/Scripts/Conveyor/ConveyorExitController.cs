using UnityEngine;

public class ConveyorExitController : MonoBehaviour
{
    public Machine connectedMachine;
    public Conveyor ParentConveyor;
    public float OutForce = 2;
    
    [Header("Connector System")]
    [SerializeField] private ConveyorConnector connector;

    [Tooltip("The previous metal bar that exited this conveyor, used to handle connections correctly.")]
    private MetalBar previousMetalBar; 


    

    void Awake()
    {
        ParentConveyor = GetComponentInParent<Conveyor>();
        
        // Find connector if not assigned
        if (connector == null)
            connector = GetComponentInChildren<ConveyorConnector>();
    }
    
    public void HandleMetalBarExit(MetalBar metalBar)
    {
        // ! Prevent double exit handling
        if(previousMetalBar == metalBar)
            return; 
        

        // Remove from current conveyor
        ParentConveyor.RemoveMovingResource(metalBar);
        // *Leave the current conveyor initially
        metalBar.LeaveCurrentConveyor();
        previousMetalBar = metalBar; // Store the last exited metal bar
        // Check if we have a connection
        if (connector != null && connector.IsConnected)
        {
            // Pass to connected conveyor
            ConveyorEntryController connectedEntry = connector.ConveyorEntryController;
            if (connectedEntry != null)
            {
                // leave the current conveyor with no Force
                metalBar.MoveOnConveyor(connectedEntry);
                // Note: MoveOnConveyor now handles adding the metal bar to the conveyor's resources list when appropriate
            }
        }
        else
        {
             metalBar.movementController.EnablePhyscis();
            // No connection - make it fall
            Debug.Log($"MetalBar from {ParentConveyor.name} has no connection - falling");
        }
    }
}
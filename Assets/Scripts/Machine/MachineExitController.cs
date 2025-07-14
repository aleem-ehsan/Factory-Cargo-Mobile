using UnityEngine;

public class MachineExitController : MonoBehaviour
{
    [Header("Connector System")]
    [SerializeField] private ConveyorConnector connector;
    [SerializeField] private Machine parentMachine;
    
    [Header("Exit Settings")]
    [SerializeField] private float exitForce = 1.2f;
    
    private void Awake()
    {
        // Find parent machine if not assigned
        if (parentMachine == null)
            parentMachine = GetComponentInParent<Machine>();
            
        // Find connector if not assigned
        if (connector == null)
            connector = GetComponentInChildren<ConveyorConnector>();
    }
    
    public void HandleMetalBarExit(MetalBar metalBar)
    {
        Debug.Log($"Machine {parentMachine.name} handling metal bar exit");
        
        // *Leave the current conveyor initially
        metalBar.LeaveCurrentConveyor(); 

        // Check if we have a connection
        if (connector != null && connector.IsConnected)
        {
            // Pass to connected conveyor
            ConveyorEntryController connectedEntry = connector.ConnectedConveyorEntryController;
            if (connectedEntry != null)
            {
                // Move the metal bar to the connected conveyor
                metalBar.MoveOnConveyor(connectedEntry);
                // Note: MoveOnConveyor now handles adding the metal bar to the conveyor's resources list when appropriate
                Debug.Log($"MetalBar passed from Machine {parentMachine.name} to Conveyor {connectedEntry._conveyor.name}");
            }else{
                Debug.LogError("No Connector found in MachineExitController");
            }
        }
        else
        {
            // No connection - make it fall
            metalBar.movementController.EnablePhyscis();
            Debug.Log($"MetalBar from Machine {parentMachine.name} has no connection - falling");
        }
    }
}
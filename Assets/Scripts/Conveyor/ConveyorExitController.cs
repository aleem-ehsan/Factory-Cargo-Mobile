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


        // Determine if there is a valid next conveyor connection that is DIFFERENT from the current/parent conveyor
        bool hasValidConnection = connector.IsConnected && connector.ConveyorEntryController != null;
        ConveyorEntryController connectedEntry = hasValidConnection ? connector.ConveyorEntryController : null;

        if (hasValidConnection)
        {
            // Guard against looping back to the same conveyor
            if (!connectedEntry.isMachine && connectedEntry._conveyor == ParentConveyor)
            {
                hasValidConnection = false; // Treat as no connection to avoid re-attaching to the same conveyor
            }
        }

        // Leave the current conveyor first
        previousMetalBar = metalBar; // Store the last exited metal bar
        metalBar.LeaveCurrentConveyor();

        if (hasValidConnection)
        {
            // Additional safeguard: avoid reattaching to same conveyor if metal bar believes it is still on it
            if (!connectedEntry.isMachine && metalBar.currentConveyor != null && connectedEntry._conveyor == metalBar.currentConveyor)
            {
                metalBar.movementController.EnablePhyscis();
                Debug.Log("Connection points to the current conveyor; enabling physics instead to avoid loop.");
                return;
            }
            metalBar.MoveOnConveyor(connectedEntry);
            Debug.Log($"ConveyorExitController: Metal bar {metalBar.name} moved to connected conveyor entry {connectedEntry.name}.");
        }
        else
        {
            // No valid connection or would loop back to the same conveyor -> enable physics (fall)
            metalBar.movementController.EnablePhyscis();
            Debug.Log("MetalBar exited with no valid connection. Physics enabled.");
        }
    }
}
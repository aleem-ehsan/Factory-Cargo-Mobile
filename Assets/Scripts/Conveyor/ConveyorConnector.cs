using UnityEngine;

public class ConveyorConnector : MonoBehaviour
{
    [Header("Connection Settings")]
    [SerializeField] private Conveyor parentConveyor;

    [Tooltip("Next Connected Conveyor to Transfer Resources to.")]
     public ConveyorEntryController ConnectedConveyorEntryController;

    public bool isConnected = false;
    
    [Tooltip("Set to true if this connector is part of a Machine, false if it's part of a Conveyor.")]
    public bool isMachine = false;
    
    private void Awake()
    {
        if (parentConveyor == null)
            parentConveyor = GetComponentInParent<Conveyor>();
        
        // Ensure a Rigidbody is present and set up for trigger events
        var rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.isKinematic = true;
        rb.detectCollisions = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.useGravity = false;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // Check if we're connecting to another conveyor's entry point
        if (other.CompareTag("Conveyor"))
        {
            if (other.TryGetComponent<ConveyorEntryController>(out var entryController))
            {
                if (entryController.isMachine)
                {
                    ConnectEntry(entryController);
                }
                else
                {
                    // check if the entryController's Conveyor is in Placement Mode
                    if (entryController._conveyor.isPlaced)
                    {
                        ConnectEntry(entryController);
                    }
                }
            }
        }
        
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Conveyor"))
        {
            ConveyorEntryController entryController = other.GetComponent<ConveyorEntryController>();
            if (entryController == ConnectedConveyorEntryController)
            {
                // Disconnect from the Conveyor entry
                DisconnectEntry();
            }
        }
    }
    
    /// <summary>
    /// Connects the connector to a Conveyor Entry Controller.
    /// </summary>
    /// <param name="entryController"></param>
    public void ConnectEntry(ConveyorEntryController entryController)
    {
        entryController.ConnectorAttached(this);
        ConnectedConveyorEntryController = entryController;
        isConnected = true;
    }
/// <summary>
/// Disconnects the connector from the connected Conveyor Entry.
/// </summary>
    public void DisconnectEntry()
    {
        if (ConnectedConveyorEntryController == null){
            Debug.Log("Attempted to disconnect but no Conveyor Entry is connected.");
            return;
        }

        Debug.Log($"Disconnecting from Conveyor Entry: {ConnectedConveyorEntryController.name}");

        // Store reference before clearing
        ConveyorEntryController entryToDisconnect = ConnectedConveyorEntryController;
        
        // Clear our reference first
        ConnectedConveyorEntryController = null;
        isConnected = false;
        
        // Then tell the entry controller to detach from us
        if (entryToDisconnect != null)
        {
            entryToDisconnect.DetachFromConnector();
        }
    }
    
    
    public bool IsConnected => isConnected;
    public ConveyorEntryController ConveyorEntryController => ConnectedConveyorEntryController;
}
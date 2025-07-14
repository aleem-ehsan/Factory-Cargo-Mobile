using UnityEngine;

public class MachineConnector : MonoBehaviour
{
    [Header("Connection Settings")]
    [SerializeField] private Machine parentMachine;
    [SerializeField] private ConveyorEntryController connectedEntryController;

    // TODO: an instance of MachineEntryController if separate entryControllers

    [SerializeField] private bool isConnected = false;
    
    private void Awake()
    {
        if (parentMachine == null)
            parentMachine = GetComponentInParent<Machine>();
            
    }
    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Machine Connector Connected {parentMachine.name} trigger with {other.name}");
        // Check if we're connecting to a conveyor's entry point
        if (other.CompareTag("Conveyor"))
        {
            ConveyorEntryController entryController = other.GetComponent<ConveyorEntryController>();
            if (entryController != null)
            {
                ConnectAnEntryController(entryController);
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"Machine Connector DisConnected {parentMachine.name}  trigger with {other.name}");

        if (other.CompareTag("Conveyor"))
        {
            ConveyorEntryController entryController = other.GetComponent<ConveyorEntryController>();
            if (entryController == connectedEntryController)
            {
                DisconnectEntryController();
            }
        }
    }
    
    public void ConnectAnEntryController(ConveyorEntryController entryController)
    {
        connectedEntryController = entryController;
        isConnected = true;
        
            
        Debug.Log($"Machine {parentMachine.name} connected to Conveyor {entryController._conveyor.name}");
    }
    
    public void DisconnectEntryController()
    {
        connectedEntryController = null;
        isConnected = false;
        
            
        Debug.Log($"Machine {parentMachine.name} disconnected");
    }
    
    public bool IsConnected => isConnected;
    public ConveyorEntryController ConnectedEntryController => connectedEntryController;
}
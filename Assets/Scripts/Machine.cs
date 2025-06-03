using UnityEngine;

public class Machine : MonoBehaviour
{
    // TODO: The first Machine in the entire Level should instantiate Metal Bars


    [Header("Products")]
    [Tooltip("These Prfabs will be initialized from Resources folder")]
    [SerializeField] private GameObject Product;


// ------------------------------------- -------------------------------------
    [SerializeField] private Transform metalSpawnPoint;


    [SerializeField] private ConveyorEntryController machineEntryController; // Reference to the MachineEntryController script

    public bool isFirstMachine = false; // Flag to check if this is the first machine in the level


//   ------------------- Singleton -------------------
    public static Machine Instance;

    private void Awake()
    {
        // if(!isFirstMachine) // !: Ensure only one instance of Machine exists
        //     return;


        if(!isFirstMachine){ // !: Ensuring that only 1 Machine can Create Products from START
            return;
        }
        
        // Remove singleton enforcement so all Machine instances remain in the scene
        Instance = this;

        InitializeProducts();

        machineEntryController = GetComponentInChildren<ConveyorEntryController>();
    }

    private void InitializeProducts()
    {
        Product = (GameObject)Resources.Load("Products/Product");
    }


    void Start(){
        // if(isFirstMachine)  // ? if not disabled, Spawned Metal will instantly Trigger Collisions
        //     DisableEntry();
    }

    public void DisableEntry(){
        if (machineEntryController != null)
        {
        //   destroy the MachineEntryController component to disable it
            Destroy(machineEntryController.gameObject);
        }
        else
        {
            Debug.LogError("MachineEntryController not found on: " + gameObject.name);
        }
    }


    public void SpawnStartingProduct()
    {   

        if (!isFirstMachine)  // * if it is not the first machine, do not spawn metal
            return;

        Debug.Log("Spawning Product called");

        GameObject spawnedMetalBar = Instantiate(Product, metalSpawnPoint.position, metalSpawnPoint.rotation);
        spawnedMetalBar.transform.SetParent(metalSpawnPoint);

        MetalBar metalBar;
        spawnedMetalBar.TryGetComponent<MetalBar>(out metalBar);

        if (metalBar != null)
        {

            // 


            // metalBar.moveDirection = transform.right; 
            Debug.Log("Machine Assigning the Spline to the Metal Bar");
            spawnedMetalBar.transform.localRotation = Quaternion.Euler(0, 90, 0);

            // Tryget the MovementController from the MetalBar
            MovementController movementController;
            spawnedMetalBar.TryGetComponent<MovementController>(out movementController);

            if (movementController != null)
            {
                movementController.isMovingOnConveyor = false;
            }

            metalBar.MoveWithConveyor(machineEntryController);
        }
        else
        {
            Debug.LogError("MetalBar component not found on the spawned object.");
        }
    }


    public ConveyorEntryController GetMachineEntryController()
    {
        if (machineEntryController == null)
        {
            Debug.LogError("MachineEntryController is not assigned or found.");
        }
        return machineEntryController;
    }



}

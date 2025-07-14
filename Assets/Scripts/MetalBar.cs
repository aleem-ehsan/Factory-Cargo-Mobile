using UnityEngine;
using DG.Tweening;




// Types of Resources a Product can have
public enum ResourceType
{
    MetalBar,
    TexturedBar,
    EmmisiveBar
}


public class MetalBar : MonoBehaviour
{

    private BoxCollider triggerCollider;


    public ResourceType resourceType;


    [Header("Product Shapes")]
    [Tooltip("All shapes that this Product can have")]
    [SerializeField] private GameObject StartingShape; 
    [SerializeField] private GameObject MetalShape; 
    [SerializeField] private GameObject TextureShape; 
    [SerializeField] private GameObject EmmisiveShape; 


    public MovementController movementController; // Reference to the movement controller script

    private void Awake()
    {
       

        triggerCollider = GetComponent<BoxCollider>();

        movementController = GetComponent<MovementController>();
    }

    private void Start()
    {
        CalculateTheAmountOfShapes();
    }



    private void Update()
    {
        // if (canMoveOnConveyor)
        // {
        //     MoveMetalBar();
        // }
    }

    private void MoveMetalBar(){
       
            // rb.transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Machine"))
        {
            // check if the gameobject has Script "MachineExitController"
            if (other.TryGetComponent<MachineExitController>(out var machineExitController))
            {
                Debug.Log("Metal Bar Triggered with Machine Exit");
                // LeaveCurrentConveyor();

                machineExitController.HandleMetalBarExit(this); // Call the method to handle the exit logic

                // * Jump a little bit forward in the direction of the machine exit
                // transform.DOJump(transform.position + machineExitController.transform.forward * 2f, 0.4f, 1, 0.5f).SetEase(Ease.Linear); // Jump forward in the direction of the machine exit
                transform.DOMove(transform.position + machineExitController.transform.forward * 1.2f, 0.5f).SetEase(Ease.Linear); // Move forward in the direction of the machine exit

            }


            // *----------------  {"MachineEntryController"} Script is not Used any more / {"ConveyorEntryController"} is used for Machines  ----------------
            //! now no Entry is detected using MetalBar - CONNECTORs will handle transfer of MetalBars onto next Conveyor or Machine

            // else if (other.TryGetComponent<ConveyorEntryController>(out var conveyorEntryController)){
            //         // kill any DOTween animations that are running on this GameObject
            //     DOTween.Kill(gameObject); // Kill any running animations on this GameObject

            //     Debug.Log("Metal Bar Triggered with Machine Entry");
            //     MoveOnConveyor(conveyorEntryController);
            // }
            // ?--------------------------------------------

        }
        else if(other.CompareTag("Conveyor"))
        {

                // TODO: this Conveyor ENTRY logic is managed through the CONVEYOR-CONNECTOR 
            // *ENTRY CONTROLLER  -- No Need to detect by MetalBar as OnExit from previous will decide to enter MetalBar on Next based on Connector
            // if (other.TryGetComponent<ConveyorEntryController>(out var conveyorEntryController))
            // {
            //     DOTween.Kill(gameObject); // Kill any running animations on this GameObject
            //     Debug.Log("Metal Bar Triggered with Conveyor Entry");


            //     // if(movementController.isMovingOnConveyor) //! If already on a conveyor, do nothing 
            //     //     return; 


            //         // MoveOnConveyor(conveyorEntryController);
            //         // conveyorEntryController._conveyor.AddMovingResource(this);  // ! very important to assign resource to the Conveyor
            // }

            // TODO: this Conveyor EXIT logic is managed through the CONVEYOR-EXITController 
            // *EXIT CONTROLLER
            if (other.TryGetComponent<ConveyorExitController>(out var conveyorExitController)){
                
                // if(!movementController.isMovingOnConveyor) return; // ? If not moving on a conveyor, do nothing otherwise Repel the Metal Bar
                // LeaveCurrentConveyor();

                // ! Remove the resource from the conveyor only for the Conveyors , not the Machines because they dont keep Moving Resources in Cache
                // conveyorExitController._conveyor.RemoveMovingResource(this);
                    Debug.Log("Metal Bar Triggered with Conveyor Exit");
                
                // * Use the new CONNECTOR System
                conveyorExitController.HandleMetalBarExit(this);


                // * Jump a little bit forward in the direction of the machine exit
                // transform.DOJump(transform.position + conveyorExitController.transform.forward * conveyorExitController.OutForce, 0.1f, 1, 0.5f).SetEase(Ease.Linear); // Jump forward in the direction of the machine exit
                transform.DOMove(transform.position + conveyorExitController.transform.forward * conveyorExitController.OutForce, 0.5f).SetEase(Ease.Linear); // Move forward in the direction of the machine exit

                }

            // }else if(other.CompareTag("Converter"))
            }
            // * CONVERTER
            else if(other.TryGetComponent<ResourceConverter>(out var resourceConverter))
            {
                Debug.Log("Metal Bar Triggered with Converter");
            ChangeShape(resourceConverter.outputType);
            }
            // * BUMPER
            else if(other.CompareTag("Bumper"))
            {
                Debug.Log("Metal Bar Triggered with Bumper");
                // add force in the forward Direction of the Bumper
                movementController.JumpTheResource( other.transform.up , other);
            }
            
            
        }

        void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject.CompareTag("Floor"))
            {
                // disable this Script 
                triggerCollider.enabled = false; // ! so that no more collisions are detected as the metal bar is on the floor
                DelayWasteMySelf(); // Call the WasteMySelf method to disable the metal bar
                Destroy(gameObject, 2f); // Destroy the metal bar after 2 seconds
            }
            movementController.isJumping = false; // Reset the jumping flag when colliding with the floor
        }



    /// <summary>
    /// Handle Movement for Both Machine Conveyors and Simple Conveyors 
    /// </summary>
    /// <param name="EntryController"></param>
        public void MoveOnConveyor(ConveyorEntryController conveyorEntryController)
        {
        if (movementController.isMovingOnConveyor) {
            Debug.Log("Metal Bar is already Moving with Conveyor, do nothing");
            return;
            } // If already on a conveyor, do nothing
        Debug.Log("Moving with Conveyor");


    // * Add the Machine's EntryController also the Script of ConveyorEntryController
            // EntryController.TryGetComponent<ConveyorEntryController>(out var conveyorEntryController);
            if (conveyorEntryController == null)
            {
                Debug.LogError("ConveyorEntryController not found on the EntryController GameObject.");
                return;
            }

        movementController.AttachToSpline(conveyorEntryController.spline);
        movementController.isMovingOnConveyor = true;
        
        // ! Only add to conveyor's resources list if it's not a machine
        if (conveyorEntryController._conveyor != null && !conveyorEntryController.isMachine)
        {
            conveyorEntryController._conveyor.AddMovingResource(this);
            Debug.Log($"Added metal bar to conveyor: {conveyorEntryController._conveyor.name}");
        }
    }
    

    public void LeaveCurrentConveyor()
    {
       
        if (!movementController.isMovingOnConveyor) {
            Debug.Log("Metal Bar is not Moving with Conveyor, do nothing");
            return;
            } // If already outside, do nothing
        Debug.Log("Leaving the machine");

        movementController.DetachFromSpline();  // Detach from the spline if attached
        movementController.isMovingOnConveyor = false;

    }




    private void CalculateTheAmountOfShapes()
    {
        // check how many child Objects this GameObject has
        int childCount = transform.childCount;
        if (childCount > 0)
        {
            // Disable all child objects first
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
            // Enable the first child object
            Transform firstShape = transform.GetChild(0);
            firstShape.gameObject.SetActive(true);  
        }
    }

    public void ChangeShape(ResourceType type)
    {
        /*
         check how many child Objects this GameObject has
        int childCount = transform.childCount;
        if (childCount > 0)
        {
            // enable the second child object
            Transform nextShape = transform.GetChild(1);
            nextShape.gameObject.SetActive(true); 


            // Destroy the first child object
            Transform currentShape = transform.GetChild(0);
            Destroy(currentShape.gameObject);
        }
        */
        // check which type of Converter it is, then Change the shape accordingly

        // * Play conversion sound
        AudioManager_Script.Instance.Play(SoundName.Conversion);

        switch (type)
        {
            case ResourceType.MetalBar:
                StartingShape.SetActive(false);
                MetalShape.SetActive(true);
                TextureShape.SetActive(false);
                EmmisiveShape.SetActive(false);
                break;
            case ResourceType.TexturedBar:
                StartingShape.SetActive(false);
                MetalShape.SetActive(false);
                TextureShape.SetActive(true);
                EmmisiveShape.SetActive(false);
                break;
            case ResourceType.EmmisiveBar:
                StartingShape.SetActive(false);
                MetalShape.SetActive(false);
                TextureShape.SetActive(false);
                EmmisiveShape.SetActive(true);
                break;
            default:
                Debug.LogWarning("Unknown resource type: " + type);
                break;
        }

        resourceType = type; // Update the resource type

    }

    public void WasteMySelf(){
        // Disable the metal bar

        // give a shrink affect using DoTween

        // also give a vibration effect using DoTween
        transform.DOShakeRotation(0.5f, new Vector3(0, 10, 0), 22, 90, false).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            // Destroy the metal bar after the shake effect
            transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.OutCubic);
        });

        // Disable the collider
        triggerCollider.enabled = false; // ! so that no more collisions are detected as the metal bar is on the floor
        movementController.DisablePhsyics();
        movementController.isMovingOnConveyor = false; // Disable movement on the conveyor
        
        // Destroy the metal bar after 2 seconds
        Destroy(gameObject, 1f);
    }

    public void DelayWasteMySelf(){
        // Delay the WasteMySelf method by 2 seconds
        Invoke(nameof(WasteMySelf), 2f);
    }


} 
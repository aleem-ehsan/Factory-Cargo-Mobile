using UnityEngine;
using DG.Tweening;




// Types of Resources a Product can have
public enum ResourceType
{
    MetalBar,
    TexturedBar,
    StoneBlock
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
    [SerializeField] private GameObject StoneShape; 


    [SerializeField] private MovementController movementController; // Reference to the movement controller script

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
                LeaveCurrentConveyor(null);

                // * Jump a little bit forward in the direction of the machine exit
                transform.DOJump(transform.position + machineExitController.transform.forward * 2f, 0.4f, 1, 0.5f).SetEase(Ease.Linear); // Jump forward in the direction of the machine exit

            }


            // *----------------  {"MachineEntryController"} Script is not Used any more / {"ConveyorEntryController"} is used for Machines  ----------------
            else if (other.TryGetComponent<ConveyorEntryController>(out var conveyorEntryController)){
                Debug.Log("Metal Bar Triggered with Machine Entry");
                MoveWithConveyor(conveyorEntryController);
            }
            // ?--------------------------------------------

        }else if(other.CompareTag("Conveyor"))
        {
            // *ENTRY CONTROLLER
            if (other.TryGetComponent<ConveyorEntryController>(out var conveyorEntryController))
            {
                Debug.Log("Metal Bar Triggered with Conveyor Entry");


                if(movementController.isMovingOnConveyor) return; // If already on a conveyor, do nothing
                    MoveWithConveyor(conveyorEntryController);
                    conveyorEntryController._conveyor.AddMovingResource(this);  // ! very important to assign resource to the Conveyor
            }
            // *EXIT CONTROLLER
            else if (other.TryGetComponent<ConveyorExitController>(out var conveyorExitController)){
                
                // if(!movementController.isMovingOnConveyor) return; // ? If not moving on a conveyor, do nothing otherwise Repel the Metal Bar
                LeaveCurrentConveyor(conveyorExitController);

                // ! Remove the resource from the conveyor only for the Conveyors , not the Machines because they dont keep Moving Resources in Cache
                conveyorExitController._conveyor.RemoveMovingResource(this);
                    Debug.Log("Metal Bar Triggered with Conveyor Exit");

                // * Jump a little bit forward in the direction of the machine exit
                transform.DOJump(transform.position + conveyorExitController.transform.forward * 2f, 0.1f, 1, 0.5f).SetEase(Ease.Linear); // Jump forward in the direction of the machine exit

                }

            // }else if(other.CompareTag("Converter"))
            }else if(other.TryGetComponent<ResourceConverter>(out var resourceConverter))
            {
                Debug.Log("Metal Bar Triggered with Converter");
            ChangeShape(resourceConverter.outputType);
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject.CompareTag("Floor"))
            {
                // disable this Script 
                triggerCollider.enabled = false; // ! so that no more collisions are detected as the metal bar is on the floor
                Destroy(gameObject, 2f); // Destroy the metal bar after 2 seconds
            }
        }



    /// <summary>
    /// Handle Movement for Both Machine Conveyors and Simple Conveyors 
    /// </summary>
    /// <param name="EntryController"></param>
        public void MoveWithConveyor(ConveyorEntryController conveyorEntryController)
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
        
/*
        // Use DoTween to smoothly move and rotate the metal bar to the conveyor's position and orientation
        // Vector3 targetPosition = EntryController.transform.position;
        // Vector3 conveyorRotation = EntryController.transform.rotation.eulerAngles;
        // conveyorRotation.z = 0;
        // Quaternion targetRotation = Quaternion.Euler(0, conveyorRotation.y - 90, 0);

        // Animate position and rotation over 0.5 seconds
        transform.DOMove(targetPosition, 0.5f).SetEase(Ease.OutCubic);
        transform.DORotateQuaternion(targetRotation, 0.5f).SetEase(Ease.OutCubic);
        canMoveOnConveyor = true; // Set to true to allow movement on the conveyor


*/

        // // set rotation of Z axis to 0
        // Vector3 rotation = transform.rotation.eulerAngles;
        // rotation.z = 0;
        // transform.rotation = Quaternion.Euler(rotation);

    }
    

        public void LeaveCurrentConveyor( ConveyorExitController conveyorExitController)
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

        switch (type)
        {
            case ResourceType.MetalBar:
                StartingShape.SetActive(false);
                MetalShape.SetActive(true);
                TextureShape.SetActive(false);
                StoneShape.SetActive(false);
                break;
            case ResourceType.TexturedBar:
                StartingShape.SetActive(false);
                MetalShape.SetActive(false);
                TextureShape.SetActive(true);
                StoneShape.SetActive(false);
                break;
            case ResourceType.StoneBlock:
                StartingShape.SetActive(false);
                MetalShape.SetActive(false);
                TextureShape.SetActive(false);
                StoneShape.SetActive(true);
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
        Destroy(gameObject, 2f);
    }


} 
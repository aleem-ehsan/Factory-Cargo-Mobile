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
    [SerializeField] private float moveSpeed = 2f;

    private Rigidbody rb;
    [SerializeField] private bool canMoveOnConveyor = true; // Initialize as true since it starts in machine

    [SerializeField] public Vector3 moveDirection = Vector3.right; // Default move direction

    private BoxCollider triggerCollider;


    public ResourceType resourceType;


    [Header("Product Shapes")]
    [Tooltip("All shapes that this Product can have")]
    [SerializeField] private GameObject StartingShape; 
    [SerializeField] private GameObject MetalShape; 
    [SerializeField] private GameObject TextureShape; 
    [SerializeField] private GameObject StoneShape; 



    private void Awake()
    {
        // Get or add Rigidbody if it doesn't exist
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // Disable physics for the metal bar

        triggerCollider = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        CalculateTheAmountOfShapes();
    }

    public void MoveTheMetalInMachine()
    {
        rb.isKinematic = true; // Disable physics when inside the machine
        canMoveOnConveyor = true;
    }

    private void Update()
    {
        if (canMoveOnConveyor)
        {
            MoveMetalBar();
        }
    }

    private void MoveMetalBar(){
       
            rb.transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    public void LeaveCurrentConveyor( ConveyorExitController conveyorExitController)
    {
       
        if (!canMoveOnConveyor) return; // If already outside, do nothing
        Debug.Log("Leaving the machine");

        canMoveOnConveyor = false;
        rb.isKinematic = false; // Enable physics when leaving the machine
        rb.AddForce(moveDirection + 2 * moveSpeed * Vector3.up, ForceMode.Impulse);
        // rb.transform.SetParent(null); // Detach the metal bar from the machine

        if(conveyorExitController != null){
            conveyorExitController._conveyor.RemoveMovingResource(this); // Remove this metal bar from the conveyor's list of moving resources
        }
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
            }else if (other.TryGetComponent<MachineEntryController>(out var machineEntryController2)){
                Debug.Log("Metal Bar Triggered with Machine Entry");
                MoveWithConveyor(other.gameObject);
            }
        }else if(other.CompareTag("Conveyor"))
        {
            if (other.TryGetComponent<ConveyorEntryController>(out var conveyorEntryController))
            {
                Debug.Log("Metal Bar Triggered with Conveyor Entry");

                MoveWithConveyor(other.gameObject);
                conveyorEntryController._conveyor.AddMovingResource(this);  // ! very important to assign resource to the Conveyor
            }else if (other.TryGetComponent<ConveyorExitController>(out var conveyorExitController)){
                LeaveCurrentConveyor(conveyorExitController);
                Debug.Log("Metal Bar Triggered with Conveyor Exit");
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


    private void MoveWithConveyor(GameObject EntryController)
    {
        Debug.Log("Metal Bar Moving with conveyor");
        rb.isKinematic = true; // Disable physics when on the conveyor
        // Use DoTween to smoothly move and rotate the metal bar to the conveyor's position and orientation
        Vector3 targetPosition = EntryController.transform.position;
        Vector3 conveyorRotation = EntryController.transform.rotation.eulerAngles;
        conveyorRotation.z = 0;
        Quaternion targetRotation = Quaternion.Euler(0, conveyorRotation.y - 90, 0);

        // Animate position and rotation over 0.5 seconds
        transform.DOMove(targetPosition, 0.5f).SetEase(Ease.OutCubic);
        transform.DORotateQuaternion(targetRotation, 0.5f).SetEase(Ease.OutCubic);
        canMoveOnConveyor = true; // Set to true to allow movement on the conveyor

        // // set rotation of Z axis to 0
        // Vector3 rotation = transform.rotation.eulerAngles;
        // rotation.z = 0;
        // transform.rotation = Quaternion.Euler(rotation);

       

        moveDirection =  EntryController.transform.forward;   


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

    public void Kill(){
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
        rb.isKinematic = true; // Disable physics when on the conveyor
        canMoveOnConveyor = false; // Disable movement on the conveyor
        
        // Destroy the metal bar after 2 seconds
        Destroy(gameObject, 2f);
    }


} 
using UnityEngine;

public class ConveyorEntryController : MonoBehaviour
{

    public Machine connectedMachine;
    public Conveyor _conveyor;


    [SerializeField] private GameObject ForwardDirection_GameObject;

   [HideInInspector] public Vector3  ForwardDirection;



    void Awake()
    {
        _conveyor = GetComponentInParent<Conveyor>();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
        {
            ForwardDirection = ForwardDirection_GameObject.transform.forward;
        }


    // void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("MachineExit"))
    //     {
    //         if (other.TryGetComponent<MachineExitController>(out var machineExitController))
    //         {
    //            Debug.Log("Conveyor ENTRY:    Connected to machine Exit point: " + machineExitController.name);
    //         }

    //     }
    //     else if (other.CompareTag("Conveyor"))
    //     {
    //         if (other.TryGetComponent<ConveyorExitController>(out var conveyorExitController))
    //         {
    //             Debug.Log("Conveyor ENTRY:       Connected to conveyor: " + conveyorExitController.name);
    //         }
    //     }
    // }
    

    // void OnTriggerExit(Collider other)
    // {
    //     if (other.CompareTag("Machine"))
    //     {
    //         connectedMachine = null;
    //         Debug.Log("ENTRY:       Disconnected from machine");
    //     }
    //     else if (other.CompareTag("Conveyor"))
    //     {
    //         connectedConveyor = null;
    //         Debug.Log("ENTRY:       Disconnected from conveyor");
    //     }
    // }

}

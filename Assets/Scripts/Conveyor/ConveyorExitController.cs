using UnityEngine;

public class ConveyorExitController : MonoBehaviour
{

    public Machine connectedMachine;
    public Conveyor _conveyor;
    public int OutForce = 2;


    void Awake()
    {
        _conveyor = GetComponentInParent<Conveyor>();   
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }


    // void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("Machine"))
    //     {
    //        MachineEntryController machineEntryPoint = other.GetComponent<MachineEntryController>();
    //         if (machineEntryPoint != null)
    //         {
    //            Debug.Log("Conveyor EXIT:    Connected to machine entry point: " + machineEntryPoint.name);
    //         }

    //     }
    //     else if (other.CompareTag("Conveyor"))
    //     {
    //         ConveyorEntryController conveyorEntryController = other.GetComponent<ConveyorEntryController>();
    //         if (conveyorEntryController != null)
    //         {
    //             Debug.Log("Conveyor EXIT:    Connected to conveyor: " + connectedConveyor.name);
    //         }
    //     }
    // }

    // void OnTriggerExit(Collider other)
    // {
    //     if (other.CompareTag("Machine"))
    //     {
    //         connectedMachine = null;
    //         Debug.Log("EXIT:    Disconnected from machine");
    //     }
    //     else if (other.CompareTag("Conveyor"))
    //     {
    //         connectedConveyor = null;
    //         Debug.Log("EXIT:    Disconnected from conveyor");
    //     }
    // }


}

using UnityEngine;
using Dreamteck.Splines;

public class ConveyorEntryController : MonoBehaviour
{

    public Machine connectedMachine;
    public Conveyor _conveyor;


    [Header("Spline ")]
    public SplineComputer spline; // Reference to the spline component if needed


    [Tooltip("Type of Entry Controller   | MACHINE or CONVEYOR")]
    public bool isMachine = false; // Set this in the inspector for each entry controller

    [Header("Connector System")]
    public ConveyorConnector connectedConnector;

    protected void Awake()
    {
        _conveyor = GetComponentInParent<Conveyor>();


    }


    public void ConnectorAttached(ConveyorConnector connector)
    {
        connectedConnector = connector;
    }

    public void DetachFromConnector()
    {   
        // The connector has already cleared its references, so we just clear ours
        connectedConnector = null;
    }



}

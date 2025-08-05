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


    public void ConnectorAttached(ConveyorConnector connector)
    {
        connectedConnector = connector;
    }

    public void DetachFromConnector()
    {   
        // The connector has already cleared its references, so we just clear ours
        connectedConnector = null;
    }


    void OnTriggerEnter( Collider other)
    {
        // Check if we're connecting to a conveyor connector
        if (other.CompareTag("Conveyor"))
        {
            if (other.TryGetComponent<ConveyorConnector>(out var connector))
            {
                    // Connect to the entry controller
                    ConnectorAttached(connector);
                    connector.isConnected = true;
            }
        }

    }

}

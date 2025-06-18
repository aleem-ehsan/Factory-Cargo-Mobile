using UnityEngine;
using Dreamteck.Splines;

public class ConveyorEntryController : MonoBehaviour
{

    public Machine connectedMachine;
    public Conveyor _conveyor;


    [SerializeField] private GameObject ForwardDirection_GameObject;

   [HideInInspector] public Vector3  ForwardDirection;

    [Header("Spline ")]
        [SerializeField] public SplineComputer spline; // Reference to the spline component if needed


    protected void Awake()
    {
        _conveyor = GetComponentInParent<Conveyor>();


    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
        {
            // ?: TO BE REMOVED
            // ForwardDirection = ForwardDirection_GameObject.transform.forward;
        }


}

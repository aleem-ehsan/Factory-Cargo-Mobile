using System;
using System.Collections;
using DG.Tweening;
using Dreamteck.Splines;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    [SerializeField] private SplineFollower splineFollower; // Reference to the spline component if needed

    [SerializeField] private float moveSpeed = 2f;

    private Rigidbody rb;
    public bool isMovingOnConveyor = false; // Initialize as true since it starts in machine


    public bool isJumping = false; // Flag to check if the resource is jumping

    private Collider LastBumperCollided;

    void Awake()
    {
        // Ensure the SplineFollower component is attached
        if (splineFollower == null)
        {
            splineFollower = GetComponent<SplineFollower>();
            if (splineFollower == null)
            {
                Debug.LogError("SplineFollower component is missing on this GameObject.");
            }
        }

         // Get or add Rigidbody if it doesn't exist
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // Disable physics for the metal bar


    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }



    public void AttachToSpline(SplineComputer spline)
    {
        // Attach this object to the provided spline
        if (spline != null)
        {
            Debug.Log("Attaching to spline: " + spline.name);

            splineFollower.spline = spline; // Set the spline for the SplineFollower
            splineFollower.follow = false;

            // Project the current position onto the spline to get the correct starting point
            SplineSample sample = new SplineSample();
            spline.Project(transform.position, ref sample);
            
            // Gradually move to the projected position and rotation over 1 second
            // MoveToSplinePosition(sample.position, sample.rotation, 0.5f);



            splineFollower.SetPercent(sample.percent); // Set the follower's position on the spline
            splineFollower.follow = true;
            
            

            // disable Physics
            DisablePhsyics();
        }
        else
        {
            Debug.LogError("Provided spline is null. Cannot attach.");
        }
    }


    public void DetachFromSpline(){
        // Detach this object from the spline
        if (splineFollower == null)
        {
            Debug.LogWarning("SplineFollower component is missing. Cannot detach.");
            return;
        }
        splineFollower.spline = null; // Clear the spline reference
        splineFollower.follow = false; // Disable following the spline

        EnablePhyscis(); // Enable physics when detaching from the spline
        rb.transform.SetParent(null); // Detach the metal bar from the machine


        Debug.Log("Detaching from spline.");
        // Example: spline.Detach(this);
    }

    public void EnablePhyscis(){
        rb.isKinematic = false; // Enable physics when leaving the machine
    }


    public void DisablePhsyics(){
        rb.isKinematic = true; // Disable physics when inside the machine
    }

    public void JumpTheResource(Vector3 JumpDirection , Collider Bumper){

        if (isJumping && LastBumperCollided == Bumper)
            return;

        // This method can be used to make the resource jump or perform an action
        // For now, we will just log a message
        Debug.Log("Jumping the resource!");
            rb.isKinematic = false; // Enable physics for the jump action
            rb.AddForce(JumpDirection.normalized *moveSpeed * 6.5f , ForceMode.VelocityChange);
            rb.AddTorque(Vector3.up * 2f, ForceMode.VelocityChange);

        isJumping = true; // Set the jumping flag to true

        LastBumperCollided = Bumper; // Store the last bumper collided with
    }

}

using System.Collections.Generic;
using UnityEngine;

public class Robot_SubmissionTable : MonoBehaviour
{

    [SerializeField] private Transform productContainer; // Container for products on the submission table
    public Animator robotAnimator; // Animator for the robot

   public SubmissionTable_Controller submissionTable_Controller;

    private Transform CurrentPickableObject; // Transform of the product to be picked up

    public static event System.Action<Transform> OnProductDropped; // Event to notify when a product is added

    void Awake(){
        if (robotAnimator == null)
        {
            robotAnimator = GetComponent<Animator>();
            if (robotAnimator == null)
            {
                Debug.LogError("Robot Animator is not assigned or found on the GameObject.");
            }
        }

        if (productContainer == null)
        {
            Debug.LogError("Product container is not assigned in the inspector.");
        }

        if (submissionTable_Controller == null)
        {
            submissionTable_Controller = GetComponentInParent<SubmissionTable_Controller>();
            if (submissionTable_Controller == null)
            {
                Debug.LogError("SubmissionTable_Controller not found in the scene.");
            }
        }
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }


/// <summary>
/// Function to add a product to the pickable list on the table.
/// </summary>
/// <param name="product"></param>
    public void ProductAdded()
    {
        
            Debug.Log("Product added to the pickable list.");

            // * play the pickup animation
            robotAnimator.SetTrigger("Pickup"); // Trigger the pickup animation

    }

/// <summary>
/// Function to pickup the product from the table.  
/// * Called by AnimationEvent ONLY
/// </summary>
    public void PickupProductFromTable(){
        CurrentPickableObject = submissionTable_Controller.ProductsOnTable[0]; // Get the first product in the list
        // This could involve checking if the product is valid, moving it to the train, etc.
        if (CurrentPickableObject != null && productContainer != null)
        {
            CurrentPickableObject.SetParent(productContainer);
            CurrentPickableObject.localPosition = Vector3.zero; // Reset position relative to the container
            Debug.Log("Product picked by ROBOT");
        }
        else
        {
            Debug.LogError("Invalid transform or product container.");
        }
    }
    /// <summary>
    /// Function to drop the product on the Train Boggy.
    /// * Called by AnimationEvent ONLY
    /// </summary>
    public void DropTheProduct(){
        // Logic to pick up product from train
        if (CurrentPickableObject != null && productContainer != null)
        {
            CurrentPickableObject.SetParent(null); // Detach from the container
            Debug.Log("Product dropped on the Train Boggy");

             // * Notify subscribers that a product has been dropped
            OnProductDropped?.Invoke(CurrentPickableObject); // Notify subscribers that a product has been dropped


            RemovePickedProduct(); // Remove the product from the table after picking it up


            submissionTable_Controller.DecrementPlaceHolderPosition(); // Decrement the placeholder position after dropping the product

           

                // * check if Count of Products in List is greater than 0 ==> then Pick the Product Again
                if(submissionTable_Controller.ProductsOnTable.Count > 0)
                {
                    // ? play the drop animation   &   PickupProductFromTable() will be automatically called by Animation   when reached the AnimationEvent
                    robotAnimator.SetTrigger("Pickup"); // Trigger the drop animation
                }
                else{    // * If no products left on the table
                        
                        if(submissionTable_Controller.is_allResourcesCollected){ // * , check if all resources have been collected

                            AllItemsLoadedOnTrain();
                            Debug.Log("All products have been collected from the table.");
                            Debug.Log("Move The Train to the next station.");

                            
                        }

                    }
        }
        else
        {
            Debug.LogError("Invalid transform or product container.");
        }
    }

/// <summary>
/// A Function to implement Logic when Train is Ready to GO..   || Level is Completed || Tutorial is Completed
/// </summary>
    private void AllItemsLoadedOnTrain(){
        TrainController.Instance.TrainReadyToMove(); // Start the train movement to the next station
        if(LevelManager.Instance != null)
            LevelManager.Instance.CurrentLevelCompleted(); // Save the level progress
        else
            TutorialManager.Instance.TutorialCompleted(); // Save the level progress
    }





    private void RemovePickedProduct(){
        // Logic to remove the product from the table after it has been picked up
        if (submissionTable_Controller.ProductsOnTable.Count > 0)
        {
            // // * Activate the Collider and set the rigidbody to Non-Kinematic
            // if (CurrentPickableObject.TryGetComponent<Collider>(out Collider collider))
            // {
            //     collider.enabled = true; // Enable the collider of the product
            // }
            // if (CurrentPickableObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
            // {
            //     rb.isKinematic = false;
            // }

            submissionTable_Controller.ProductsOnTable.Remove(CurrentPickableObject); // Remove the product from the list
            CurrentPickableObject = null; // Clear the current pickable object
            Debug.Log("Product removed from the pickable list.");

        }
        else
        {
            Debug.LogWarning("No products to remove from the table.");
        }
    }


}

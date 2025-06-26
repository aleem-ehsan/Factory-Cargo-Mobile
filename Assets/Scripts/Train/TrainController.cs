using System;
using System.Collections.Generic;
using DG.Tweening;
using Dreamteck.Splines;
using UnityEngine;

public class TrainController : MonoBehaviour
{
    [Header("Train Spline Follower")]
    [SerializeField] private SplineFollower splineFollower; // Reference to the SplineFollower component
    [SerializeField] private float speed = 5f; // Speed of the train

    [Header("Boggy's Spline Follower")]
    [Tooltip("Assign the following Boggy's Spline Follower here.")]
    [SerializeField] private SplineFollower BoggySplineFollower; // Reference to the SplineFollower component for the boggy



    //...................... Singleton Instance ......................
    public static TrainController Instance { get; private set; } // Singleton instance of TrainController



    //--------------------- Events -------------------
    public static event Action OnTrainStoppedAtDoor; // Event to notify when the train is ready to move

    void Awake(){
        // Ensure only one instance of TrainController exists
        if (Instance == null || Instance != this)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // * Play Audio of Train Running
        AudioManager_Script.Instance.Play(SoundName.TrainRunning); // Play the train running sound
        if (splineFollower == null)
        {
            splineFollower = GetComponent<SplineFollower>();
        }

        CamsController.Instance.SetGameplayCamActive(false); // * Disable the gameplay camera at the start

        // Set the initial speed of the train
        splineFollower.followSpeed = speed;
        BoggySplineFollower.followSpeed = speed;
    }




    
    public void TrainReadyToMove(){
        CamsController.Instance.SetGameplayCamActive(false); // * Disable the gameplay camera at the start

        StartCoroutine(DelayTrainMovement(1f)); // Start the train movement after a 2-second delay
    }
      private System.Collections.IEnumerator DelayTrainMovement(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified delay
        StartTrainMovement(); // Start the train movement after the delay
    }

/// <summary>
/// Called when The Level is Completed
/// </summary>
    public void StartTrainMovement(){
        if (splineFollower != null)
        {
            // splineFollower.followSpeed = speed; // Set the speed of the train
            // BoggySplineFollower.followSpeed = speed; // Set the speed of the boggy

             DOTween.To(() => splineFollower.followSpeed, x => splineFollower.followSpeed = x, speed, 1f); // speed to Zero in 1 Second
            DOTween.To(() => BoggySplineFollower.followSpeed, x => BoggySplineFollower.followSpeed = x, speed, 1f);
        }
         // * Play Audio of Train Running
        AudioManager_Script.Instance.Play(SoundName.TrainRunning); // Play the train running sound

        // * Show Loading Panel after Delay
        LoadingPanelController.Instance.ShowLoadingPanelDelay(1.2f , LoadingState.Win); // Show the loading panel 

        AudioManager_Script.Instance.StopSoundAfterDelay(SoundName.TrainRunning, 1.2f); // Stop the train running sound after 1.2 seconds
    }



  

    public void StopTrainAtDoor()
    {
        // * Play the Train Whistle Sound
        AudioManager_Script.Instance.Play(SoundName.TrainWhistle); // Play the train stop sound

        CamsController.Instance.SetGameplayCamActive(true); // * Disable the gameplay camera at the start

        if (splineFollower != null)
        {
            DOTween.To(() => splineFollower.followSpeed, x => splineFollower.followSpeed = x, 0f, 1f); // speed to Zero in 1.5 Second
            DOTween.To(() => BoggySplineFollower.followSpeed, x => BoggySplineFollower.followSpeed = x, 0f, 1f);
            Debug.Log("Train has stopped at the door.");
        }

        // * Show the Gameplay panel and Initialize the Conveyors
        OnTrainStoppedAtDoor?.Invoke(); // Notify subscribers that the train has stopped at the door
        
    }


    void OnTriggerEnter(Collider collider)
    {
        // Check if the train collides with a door or any other object
        if (collider.CompareTag("Door")) // Assuming the door has a tag "Door"
        {
            Debug.Log("Train collided with the door: " + collider.name);
            StopTrainAtDoor(); // Stop the train when it collides with the door
        }
    }

}
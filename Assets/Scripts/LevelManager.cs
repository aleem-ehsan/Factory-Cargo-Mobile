using System.Collections.Generic;
using Hypertonic.GridPlacement.Example.BasicDemo;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    [Header("Test Level to Play")]
    public int levelToLoad = 1; // Set this in the inspector to specify which level to load

    [Space(10)]

    public List<GameObject> Levels; 

    [Header("Level Requirements Data")]
    public LevelRequirementsData levelRequirementsData; // Reference to the script holding all level's Required Resources
    // This will be used to get the requirements for the active level


    // --------------------- Singelton ---------------
    public static LevelManager Instance { get; private set; }


    void Awake(){

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }



        InitializeLevels();
        GetLevelToLoadIndex();
        EnableActiveLevel();
        
    }

    void Start()
    {

        InitializeLevelRequirements(); // Call this after enabling the active level
        InitializeConveyorQuantites();
        InitializeTimerController(); // Initialize the timer for the active level
    }

    public void InitializeLevels(){
           // get all childs (only first level children)
        Levels = new List<GameObject>();
        // Only get the direct (first-level) children of this GameObject, not deeper descendants
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.parent == transform ) // Ensure only direct children
            {
                Levels.Add(child.gameObject);
            }
        }
    }

    public void InitializeTimerController(){
        

        int levelIndex = GetLevelToLoadIndex();
        LevelData levelData = levelRequirementsData.levels[levelIndex];

        if (levelData != null) {
            TimerController.Instance.SetTimeFromSeconds(levelData.AvailableTimeInSeconds);

            // timerController.LevelMinutes = levelData.AvailableTimeInSeconds / 60; // Convert seconds to minutes
            // timerController.LevelSeconds = levelData.AvailableTimeInSeconds % 60; // Remaining seconds
            // timerController.StartTimer();
        } else {
            Debug.LogError($"Level data not found for level index {levelIndex}");
        }
    }

    public void InitializeConveyorQuantites(){
        // set the Conveyor quantities in the ConveyorManager
        int levelIndex = GetLevelToLoadIndex();
        LevelData levelData = levelRequirementsData.levels[levelIndex];
        if (levelData != null) {
            ConveyorManager.Instance.SetConveyorQuantities(levelData.AllowedConveyors);
        } else {
            Debug.LogError($"Level data not found for level index {levelIndex}");
        }
    }

    public void InitializeLevelRequirements(){

        levelRequirementsData = GetComponent<LevelRequirementsData>();

        if (levelRequirementsData == null) {
            Debug.LogError("LevelRequirementsData is not assigned in LevelManager.");
            return;
        }

        int levelIndex = GetLevelToLoadIndex();
        RequiredResource[] requiredResources = levelRequirementsData.GetRequirementsForLevel(levelIndex);

        if (requiredResources != null) {
            // Find the SubmissionTable_Controller in the active level's children
            // SubmissionTable_Controller submissionTable = Levels[levelIndex].GetComponentInChildren<SubmissionTable_Controller>();
            SubmissionTable_Controller.Instance.InitailizeRequiredItems(requiredResources);
            
        }else{
            Debug.LogError($"Level requirements not found for level index {levelIndex}");
        }
    }

    public int GetLevelToLoadIndex(){
        return levelToLoad - 1; // Convert to zero-based index
    }


    public void EnableActiveLevel(){
        for (int i = 0; i < Levels.Count; i++)
            {
                if (i == GetLevelToLoadIndex())
                {
                    Levels[i].SetActive(true);
                }
                else
                {
                    Levels[i].SetActive(false);
                }
            }
    }
    




} 
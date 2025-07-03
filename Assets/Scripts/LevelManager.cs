using System.Collections.Generic;
using Hypertonic.GridPlacement.Example.BasicDemo;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
    {

        [Header("Test Level to Play")]
        public int levelToLoad = 1; // Set this in the inspector to specify which level to load

        [Space(10)]

        public List<GameObject> Levels; 

        [Header("Level Requirements Data")]
        public LevelRequirementsData levelRequirementsData; // Reference to the script holding all level's Required Resources
        // This will be used to get the requirements for the active level

        [Header("Level Progress Manager")]
        [SerializeField] private LevelProgressManager _levelProgressManager; // Reference to the LevelProgressManager to save the level progress

        // --------------------- Singelton ---------------
        public static LevelManager Instance { get; private set; }


        // -------------------- Events -------------------
        public static event System.Action<int> OnLevelCompleted; // Event to notify when a level is completed
        public static event System.Action<bool> OnGameplayStarted; // Event to notify when a level is completed


        public static bool isGameplayStarted = false; // Flag to check if the gameplay has started

        void Awake()
        {
            Debug.Log("LevelManager Awake called");
            if (Instance == null)
            {
                Instance = this;
                Debug.Log("LevelManager instance set");
            }
            else
            {
                Debug.Log("Destroying duplicate LevelManager");
                Destroy(gameObject);
                return;
            }

             


            InitializeLevels();
            
            // Get or add LevelProgressManager
            _levelProgressManager = GetComponent<LevelProgressManager>();
            if (_levelProgressManager == null)
        {
                Debug.Log("Adding LevelProgressManager component");

                // find Object with LevelProgressManager in the scene
                _levelProgressManager = FindFirstObjectByType<LevelProgressManager>();
                if (_levelProgressManager == null)
                {
                    Debug.LogError("No LevelProgressManager found in the scene. Please add one.");
                    return;
                }
            }
            
            // TODO: Only to Play a specific level 
            // #if !UNITY_EDITOR 
             LoadLastUncompletedLevel();
            // #endif
            EnableActiveLevel();

            // *Hide the loading panel after enabling the active level
            LoadingPanelController.Instance.HideLoadingPanelDelay(1f); 

        }

        void Start()
        {

            InitializeLevelRequirements(); // Call this after enabling the active level
            // ! should not initialize in Start because these are InActive due to Loading Panel Display
            InitializeConveyorQuantites();
            InitializeTimerController();

        }

        void OnEnable(){
                TrainController.OnTrainStoppedAtDoor += GamePlayeStarted; // Subscribe to the event when the train stops at the door
        }
        void OnDisable(){
                TrainController.OnTrainStoppedAtDoor -= GamePlayeStarted; // Subscribe to the event when the train stops at the door
        }



    



        private void LoadLastUncompletedLevel()
        {
            // Check if we're replaying a level
            if (PlayerPrefs.HasKey("ReplayLevel"))
            {
                levelToLoad = PlayerPrefs.GetInt("ReplayLevel");
                PlayerPrefs.DeleteKey("ReplayLevel"); // Clear the replay flag
                PlayerPrefs.Save();
                return;
            }

            int highestCompletedLevel = _levelProgressManager.GetHighestCompletedLevel();
            levelToLoad = highestCompletedLevel + 1;
            
            // Ensure we don't exceed the total number of levels
            if (levelToLoad > Levels.Count)
            {
                levelToLoad = Levels.Count;
            }
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
            Debug.Log("Initializing conveyor quantities for the active level.");
            // set the Conveyor quantities in the ConveyorManager
            int levelIndex = GetLevelToLoadIndex();
            LevelData levelData = levelRequirementsData.levels[levelIndex];
            if (levelData != null) {
                ConveyorManager.Instance.SetAllConveyorQuantities(levelData.AllowedConveyors);
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



// TODO: make this function Efficient by using a for loop
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
            My_UIManager.Instance.UpdateLevelText(levelToLoad); // Update the UI with the current level
        }

        public void DisableCurrentLevel(){
            int currentLevelIndex = GetLevelToLoadIndex();
            if (currentLevelIndex >= 0 && currentLevelIndex < Levels.Count)
            {
                Levels[currentLevelIndex].SetActive(false); // Disable the current level
            }
            else
            {
                Debug.LogError($"Cannot disable level: Invalid level index {currentLevelIndex}");
            }
        }
        


/// <summary>
/// Function to Save the Completed Current Level index in the LevelProgressManager. 
/// </summary>
        public void CurrentLevelCompleted()
        {
            My_UIManager.Instance.ShowGameWinPanel(); // Show the game win panel

            AudioManager_Script.Instance.Play(SoundName.LevelCompleted); // Play the level completed sound

            if (_levelProgressManager == null)
            {
                Debug.LogError("Cannot save level progress: LevelProgressManager is null!");
                return;
            }

            Debug.Log($"LevelManager: Saving progress for level {levelToLoad} as completed.");
            OnLevelCompleted?.Invoke(levelToLoad); // Notify subscribers that the level is completed
            _levelProgressManager.SaveLevelProgress(levelToLoad);


            // check in How Much Time the Level is completed.
            TimerController.Instance.CheckHowMuchTimeIsUsed(); // Stop the timer when the level is completed

            isGameplayStarted = false; // Reset the gameplay started flag
        }


/// <summary>
/// Function to set the gameplay started flag to true.
/// </summary>
        public void GamePlayeStarted(){
            isGameplayStarted = true; // Set the gameplay started flag to true
            OnGameplayStarted?.Invoke(true); // Notify subscribers that the gameplay has started
        }

        public void LevelFailed(){
            isGameplayStarted = false; // Reset the gameplay started flag

            Debug.Log("Level Lose! Not all required resources have been collected!");
            AudioManager_Script.Instance.Play(SoundName.LevelFailed); // Play the level failed sound
            My_UIManager.Instance.ShowGameLosePanel(); // Show the game win panel

             // stop the Time.timeScale
            // Time.timeScale = 0;

            // * Display the Loading Panel
            LoadingPanelController.Instance.ShowLoadingPanelDelay(0.5f , LoadingState.Lose); // Show the loading panel for 1 second
        }




    } 
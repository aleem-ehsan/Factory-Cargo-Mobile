using System.Collections;
using System.Collections.Generic;
using Hypertonic.GridPlacement.Example.BasicDemo;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{

    public bool IsTutorialCompleted = false;
    [SerializeField] private LevelProgressManager levelProgressManager;
    [SerializeField] private LevelRequirementsData _levelRequirementsData;

    // [Space(10)]
    // [SerializeField] private TimerController timerController;
    // [SerializeField] private ConveyorManager conveyorManager;
    


    public int InstructionsToComplete;


    private ConveyorStats[] allowedConveyors;

    public int CreatedConveyorsCount = 0; // Count of created conveyors during the tutorial


    public static bool TutorialInstructions_Completed; 


// ---------------- Singleton Instance ----------------
    public static TutorialManager Instance { get; private set; }

    void Awake(){

            if (Instance == null)
            {
                Instance = this;
                Debug.Log("TutorialManager initialized");
            }
            else
            {
                Destroy(gameObject);
                return; // Exit if an instance already exists
            }



            if(levelProgressManager == null){
                levelProgressManager = LevelProgressManager.Instance;
            }
            _levelRequirementsData = GetComponent<LevelRequirementsData>();

        IsTutorialCompleted = levelProgressManager.IsTutorialCompleted() ;

        // Check if tutorial is completed AND user doesn't want to force replay
        if(IsTutorialCompleted && !levelProgressManager.ShouldForceReplayTutorial()){
            // load the Scene with Build Index 1
            Debug.Log("TutorialManager: Tutorial already completed, skipping initialization.");
            // Here you can load the next scene or perform any other action
            SceneManager.LoadScene(1); // Assuming scene index 1 is the next scene after tutorial
            return;
        } 

         // ! Reset the force replay flag since we're starting the tutorial
        levelProgressManager.ResetForceReplayTutorial();

        Debug.Log("TutorialManager: Initializing tutorial level.");
        
        // ! Reset the tutorial completed flag at the start of the tutorial
        IsTutorialCompleted = false; // Reset the tutorial completed flag at the start of the tutorial
        TutorialInstructions_Completed = false; // Reset the tutorial instructions completed flag at the start of the tutorial
        
       
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize the tutorial level
        InitializeTutorialLevel();

        if(TutorialTextAnimator .Instance == null){
            Debug.LogError("TutorialTextAnimator Instance is null! Make sure it is Active in the Scene");
            return; // Exit if TutorialTextAnimator is not initialized
        }
        InstructionsToComplete = TutorialTextAnimator.Instance.GetNumberOfInstructions(); // Get the total number of instructions from the TutorialTextAnimator
    }




    private void InitializeTutorialLevel(){

        // * Set Available Time
        TimerController.Instance.SetTimeFromSeconds(
            _levelRequirementsData.GetTutorialLevelRequirements().AvailableTimeInSeconds
        );


        // Conveyors will be Set time by time when they are needed
        // // * Set Allowed Conveyors
        // ConveyorManager.Instance.SetConveyorQuantities(
        //     _levelRequirementsData.GetTutorialLevelRequirements().AllowedConveyors
        // );

         // * Set Required Resources in Submission Table
        SubmissionTable_Controller.Instance.InitailizeRequiredItems(_levelRequirementsData.GetTutorialLevelRequirements().requiredResources);

        LevelManager.isGameplayStarted = true; // Set gameplay started to true
    }


    public void GameplayStarted(){
        Debug.Log("TutorialManager: Gameplay started, hiding tutorial instructions.");
        TutorialTextAnimator.Instance.StartShowingTutorialInstructions(); // Hide the tutorial instructions when gameplay starts

        allowedConveyors = _levelRequirementsData.GetTutorialLevelRequirements().AllowedConveyors;
    }


/// <summary>
/// Called when a Conveyor is being Created
/// </summary>
/// <param name="conveyorType"></param>
    public void ConveyorCreated(ConveyorType conveyorType)
    {
        // Move onto the Next Instruction 
            // CompleteCurrentInstruction();

            int index = Mathf.Max(0, CreatedConveyorsCount - 1);
            if (conveyorType == allowedConveyors[index].conveyorType && TutorialInstructions_Completed == false)
            {
                TutorialTextAnimator.Instance.UnHideTutorialInstructions(); // Show the next text in the tutorial
            }



        // Disable that Type of Button using ConveyorManager
    }


/// <summary>
/// Function Holds the Logic for Completing the Current Instruction.
/// </summary>
    public void CompleteCurrentInstruction()
    {
            TutorialTextAnimator.currentTextIndex++;

        Debug.Log($"Completed Instructions: {TutorialTextAnimator.currentTextIndex+1}/{InstructionsToComplete}");

        if (TutorialTextAnimator.currentTextIndex < InstructionsToComplete)
        {
            MoveToNextInstruction();
            // Move to the next instruction
            Debug.Log($"Moving to next instruction: {TutorialTextAnimator.currentTextIndex}");
            // Here you can update the UI or perform any other action for the next instruction
        }else{
            // if(LevelManager.isGameplayStarted == false){  // * OR wait untill all the Resources are fetched by the Train
            //     AllInstructionsCompleted();
            // }
            TutorialInstructions_Completed = true; // * Set the flag to indicate that tutorial instructions are completed
            
            // * Hide the Instructions
            TutorialTextAnimator.Instance.HideTutorialInstructions();
        }

    }

/// <summary>
/// This method is called when all tutorial instructions are completed.
/// </summary>
    private void AllInstructionsCompleted()
    {
        IsTutorialCompleted = true;
        levelProgressManager.MarkTutorialCompleted();
        Debug.Log("Tutorial completed successfully!");


        // * Display the Loading Screen or any other UI to indicate completion
        LoadingPanelController.Instance.ShowLoadingPanel(LoadingState.Loading);

        // Load the next scene or perform any other action
        SceneManager.LoadScene(1); // Assuming scene index 1 is the next scene after tutorial
    }

    private void MoveToNextInstruction(){
        // Show Next Text using TutorialTextAnimator
        TutorialTextAnimator.Instance.ShowNextText();
    }

    public void CreateGridConveyorButton(){
        
            if(CreatedConveyorsCount < allowedConveyors.Length){  // if Created Conveyor Count is Less than the List of Available Conveyors

                // * Create a Single Button using ConveyorManager
                ConveyorManager.Instance.AddConveyorTypeAndQuantity(
                    allowedConveyors[CreatedConveyorsCount]
                );

                // * Increment the Created Conveyor Count
                CreatedConveyorsCount++;
            }


    }



    public void TutorialCompleted(){
        // * Show the Tutorial Win Panel
        // * Show the Tutorial Win Panel

         My_UIManager.Instance.ShowGameWinPanel(); // Show the game win panel

            AudioManager_Script.Instance.Play(SoundName.LevelCompleted); // Play the level completed sound

            if (levelProgressManager == null)
            {
                Debug.LogError("Cannot save level progress: LevelProgressManager is null!");
                return;
            }

            // Debug.Log($"LevelManager: Saving progress for level {levelToLoad} as completed.");
            // OnLevelCompleted?.Invoke(levelToLoad); // Notify subscribers that the level is completed
            // TODO: to keep Testing the Tutorial Scene
            // #if !UNITY_EDITOR
                // * Save the Tutorial as Completed in the LevelProgressManager
                levelProgressManager.MarkTutorialCompleted(); 
            // #endif

            // // check in How Much Time the Level is completed.
            // TimerController.Instance.CheckHowMuchTimeIsUsed(); // Stop the timer when the level is completed

            LevelManager.isGameplayStarted = false; // Reset the gameplay started flag
            // * Cache the Tutorial as Completed
            IsTutorialCompleted = true; // Reset the tutorial completed flag

    }

    public void TutorialFailed(){
        // * Show the Tutorial Win Panel
        // * Show the Tutorial Win Panel

        LevelManager.isGameplayStarted = false; // Reset the gameplay started flag

            Debug.Log("Level Lose! Not all required resources have been collected!");
            AudioManager_Script.Instance.Play(SoundName.LevelFailed); // Play the level failed sound
            My_UIManager.Instance.ShowGameLosePanel(); // Show the game win panel

             // stop the Time.timeScale
            // Time.timeScale = 0;

            // * Display the Loading Panel
            LoadingPanelController.Instance.ShowLoadingPanelDelay(0.5f , LoadingState.Lose); // Show the loading panel for 1 second

            
    }

/// <summary>
/// Reload the Tutorial Scene | Used when the Tutorial is Failed
/// </summary>
    public void ReloadTutorialScene(){
        // ! Set the force replay flag to allow tutorial replay even if completed
        levelProgressManager.SetForceReplayTutorial();

        My_UIManager.Instance.HidePausePanel(); // Hide the pause panel if it is open

        LoadingPanelController.Instance.ShowLoadingPanelForDuration(1.8f,LoadingState.Loading); // Show the loading panel for 1 second
        
        // ! important to RESET the TIME because it is also called from the PAUSE-PANEL
        Time.timeScale = 1; // Resume the time scale

        

        StartCoroutine( DelayReloadTutorialScene(0.5f) );
    }

    private IEnumerator DelayReloadTutorialScene(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(0);
    }



/// <summary>
/// Load the Next Scene | Used when the Tutorial is Completed
/// </summary>
    public void LoadNextScene(){
        // * Load the Next Scene
        SceneManager.LoadScene(1);
    }



}

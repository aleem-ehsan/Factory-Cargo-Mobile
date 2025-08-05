using System.Collections;
using Hypertonic.GridPlacement.Example.BasicDemo;
using TMPro;
using UnityEngine;

public class My_UIManager : MonoBehaviour
{

    [Header("UI Panels")]
    [SerializeField] private GameObject gameWinPanel;
    [SerializeField] private GameObject gameLosePanel;
    [SerializeField] private GameObject PausePanel;
    [SerializeField] private GameObject GamePlayPanel;

    [Tooltip("Grid Object Selection UI, used to select objects in the game, assign in the Inspector")]
    [SerializeField] private GameObject ObjectSelection;


    [Header("Text UI")]
    [Tooltip("Timer text that will be updated continously,  it is here so that the TimerController can access it easily")]
    [SerializeField] public TextMeshProUGUI TimerText;
    [SerializeField] public TextMeshProUGUI LevelNumberText;
[Space(10)]

    [Header("Grid Coneyor Buttons Container")]
    [Tooltip("Container for the conveyor buttons in the UI, so that the ConveyorManager can access it easily")]
    public GameObject ConveyorButtonsContainer;
    [SerializeField] private GameObject ButtonGridPlacementPrefab; // Prefab for the conveyor button grid placement option


[Header("Stars Controller")]
[Tooltip("Controller for the STARS display in the win panel , assign in the Inspector")]
    [SerializeField] private StarsController starsController; // Reference to the StarsController to manage star display in the win panel 




    [Header("Win Panel Buttons")]
    [Tooltip("Buttons to Next-Level/Retry-level in the win panel, assign in the Inspector")]
    [SerializeField] private GameObject Win_NextLevelButton;
    [SerializeField] private GameObject Win_RetryButton;

    [Header("Lose Panel Buttons")]
    [Tooltip("Button to Retry-level in the Lose panel, assign in the Inspector")]
    [SerializeField] private GameObject Lose_RetryButton;





    // ---------------- Singelton ----------------
    public static My_UIManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }



    public void ShowGameWinPanel()
    {
        
        // // stop the Time.timeScale
        // Time.timeScale = 0;

        // Show the game win panel
        gameWinPanel.SetActive(true);

        // * Disabling Buttons
        Win_NextLevelButton.SetActive(false);
        Win_RetryButton.SetActive(false);

        StartCoroutine( DelayActivateGameObject(new GameObject[] { Win_NextLevelButton, Win_RetryButton }, 2.5f));
    }
    public void ShowGameLosePanel()
    {
        // Show the game lose panel
        gameLosePanel.SetActive(true);

        // * Disabling Buttons
        Lose_RetryButton.SetActive(false);

        StartCoroutine(DelayActivateGameObject(new GameObject[] { Lose_RetryButton }, 1.5f));

    }
    private IEnumerator DelayActivateGameObject(GameObject[] gameObjects, float delay)
    {
        yield return new WaitForSeconds(delay);
        foreach (var obj in gameObjects)
        {
            obj.SetActive(true);
        }

    }
    private IEnumerator DelayDectivateGameObject(GameObject gameObject, float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }


    public void ShowPausePanel()
    {
         // stop the Time.timeScale
        Time.timeScale = 0;

        PausePanel.SetActive(true);
        
    }

    public void HidePausePanel()
    {
        // resume the Time.timeScale
        Time.timeScale = 1;

        PausePanel.SetActive(false);
        
    }

    public void RetryButtonPressed(){
        // resume the Time.timeScale
        Time.timeScale = 1;

        // Hide all panels
        gameWinPanel.SetActive(false);
        gameLosePanel.SetActive(false);
        PausePanel.SetActive(false);

        // Reload the current scene or reset the game state
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// 
    /// </summary>
    public void NextLevelButtonPressed(){
        // resume the Time.timeScale
        Time.timeScale = 1;

        // Hide all panels
        gameWinPanel.SetActive(false);
        gameLosePanel.SetActive(false);
        PausePanel.SetActive(false);

        // Reload the current scene And the Next Level will be Played because the LevelProgressManager Saved the Previous Completed Level
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name); 
    }

    /// <summary>
    /// Function to Replay the Current Level Being Played
    /// </summary>
    public void ReplayButtonPressed(){

        
        // * Display the Loading Panel
        LoadingPanelController.Instance.ShowLoadingPanel(LoadingState.Loading);

        
        // resume the Time.timeScale
        Time.timeScale = 1;

        // Hide all panels
        gameWinPanel.SetActive(false);
        gameLosePanel.SetActive(false);
        PausePanel.SetActive(false);

        // Store the current level number to replay
        int currentLevel = LevelManager.Instance.levelToLoad;
        PlayerPrefs.SetInt("ReplayLevel", currentLevel);
        PlayerPrefs.Save();


        // Reload the current scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }



/// <summary>
/// Creates a button in the UI for grid placement of a specific conveyor type.
/// </summary>
/// <param name="conveyorType"></param>
/// <param name="quantity"></param>
    public void CreateGridPlacementButton(ConveyorType conveyorType , int quantity, Vector3 offset)
    {
        // Create a new button for the specified conveyor type
        GameObject button = Instantiate(ButtonGridPlacementPrefab, ConveyorButtonsContainer.transform);
        Conveyor_Button_GridObjectSelectionOption ConveyorButton =  button.GetComponent<Conveyor_Button_GridObjectSelectionOption>();
        ConveyorButton.SetConveyorTypeAndQuantity(conveyorType , quantity);
        ConveyorButton.SetConveyorOffset(offset);
    }



    public void UpdateLevelText(int levelNumber){
        if (LevelNumberText != null)
        {
            LevelNumberText.text = "Level: " + levelNumber.ToString();
        }
        else
        {
            Debug.LogError("LevelNumberText is not assigned in My_UIManager.");
        }
    }

    /// <summary>
    /// Shows the specified number of stars in the win panel
    /// </summary>
    /// <param name="numberOfStars">Number of stars to show (0-3)</param>
    public void ShowStars(short numberOfStars)
    {
        if (starsController == null)
        {
            Debug.LogError("StarsController is not assigned in My_UIManager!");
            return;
        }
        Debug.Log("My_UIManager: Showing " + numberOfStars + " stars in the win panel.");
        starsController.Display_YELLOW_Stars(numberOfStars);

        // Save the Gained Stars in the LevelProgressManager with the completed level index
        int currentLevel = LevelManager.Instance.levelToLoad;
        LevelProgressManager.Instance.SaveLevelStars(currentLevel, numberOfStars);
    }


    public void SetGamePlayPanel(bool value)
    {
        // Hide the gameplay panel
        GamePlayPanel.SetActive(value);
        ObjectSelection.SetActive(value);
    }


}

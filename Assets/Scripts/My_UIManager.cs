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


    [Header("Timer UI")]
    [Tooltip("Timer text that will be updated continously,  it is here so that the TimerController can access it easily")]
    [SerializeField] public TextMeshProUGUI TimerText;
[Space(10)]

    [Header("Grid Coneyor Buttons Container")]
    [Tooltip("Container for the conveyor buttons in the UI, so that the ConveyorManager can access it easily")]
    public GameObject ConveyorButtonsContainer;
    [SerializeField] private GameObject ButtonGridPlacementPrefab; // Prefab for the conveyor button grid placement option



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
        // stop the Time.timeScale
        Time.timeScale = 0;

        // Show the game win panel
        gameWinPanel.SetActive(true);
        
    }
    public void ShowGameLosePanel()
    {
         // stop the Time.timeScale
        Time.timeScale = 0;

        // Show the game win panel
        gameLosePanel.SetActive(true);
        
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

    public void ReplayButtonPressed(){
        // resume the Time.timeScale
        Time.timeScale = 1;

        // Hide all panels
        gameWinPanel.SetActive(false);
        gameLosePanel.SetActive(false);
        PausePanel.SetActive(false);

        // Reload the current scene or reset the game state
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }


    // public void CraeteGridPlacementButtonsInUI(){
    //     // Clear existing buttons in the container
    //     foreach (Transform child in ConveyorButtonsContainer.transform)
    //     {
    //         Destroy(child.gameObject);
    //     }

    //     // Create new buttons based on the ConveyorManager's conveyor types
    //     foreach (var conveyorType in ConveyorManager.Instance.GetAllConveyorTypes())
    //     {
    //         GameObject button = Instantiate(ButtonGridPlacementPrefab, ConveyorButtonsContainer.transform);
    //         button.GetComponent<Conveyor_Button_GridObjectSelectionOption>().SetConveyorType(conveyorType);
    //     }
    // }

/// <summary>
/// Creates a button in the UI for grid placement of a specific conveyor type.
/// </summary>
/// <param name="conveyorType"></param>
/// <param name="quantity"></param>
    public void CreateGridPlacementButton(ConveyorType conveyorType , int quantity)
    {
        // Create a new button for the specified conveyor type
        GameObject button = Instantiate(ButtonGridPlacementPrefab, ConveyorButtonsContainer.transform);
        Conveyor_Button_GridObjectSelectionOption ConveyorButton =  button.GetComponent<Conveyor_Button_GridObjectSelectionOption>();
        ConveyorButton.SetConveyorTypeAndQuantity(conveyorType , quantity);
    }



}

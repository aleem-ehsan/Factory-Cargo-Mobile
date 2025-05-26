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

}

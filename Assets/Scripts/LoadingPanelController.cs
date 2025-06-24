using UnityEngine;
using DG.Tweening;
using System.Collections;

public class LoadingPanelController : MonoBehaviour
{
    [SerializeField] private GameObject BlueTopImage; // Reference to the loading panel GameObject
    [SerializeField] private GameObject YellowDownImage; // Reference to the loading panel GameObject

    [Header("Rect Tranforms Settings")]
    [SerializeField] private RectTransform Blue_RectTransform; // Reference to the loading panel GameObject
    [SerializeField] private RectTransform Yellow_RectTransform; // Reference to the loading panel GameObject

    [Header("Animation Settings")]
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private Ease animationEase = Ease.OutBack;

    // Store original positions
    private Vector2 blueTopOriginalPos;
    private Vector2 yellowDownOriginalPos;
    private Vector2 blueTopHiddenPos; // Position when hidden (above screen)
    private Vector2 yellowDownHiddenPos; // Position when hidden (below screen)

    // --------------------- Singleton ---------------
    public static LoadingPanelController Instance { get; private set; }

    [SerializeField] private Transform LoadingText; // Reference to the loading text transform (if needed)

    // Store the tween so we can kill it
    private Tween loadingTextTween;
    private Vector3 loadingTextOriginalPos;

    void Awake()
    {
        Debug.Log("LoadingPanelController Awake called");
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("LoadingPanelController instance set");
        }
        else if(Instance != this)
        {
            Debug.Log("Destroying duplicate LoadingPanelController");
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject); // Ensure this object persists across scenes
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Store original positions (when loading panel is visible)
        if (Blue_RectTransform != null)
        {
            blueTopOriginalPos = Blue_RectTransform.anchoredPosition;
            // Calculate hidden position (above screen)
            blueTopHiddenPos = new Vector2(blueTopOriginalPos.x, blueTopOriginalPos.y + 1200f); // Move up by 500 units
        }
        
        if (Yellow_RectTransform != null)
        {
            yellowDownOriginalPos = Yellow_RectTransform.anchoredPosition;
            // Calculate hidden position (below screen)
            yellowDownHiddenPos = new Vector2(yellowDownOriginalPos.x, yellowDownOriginalPos.y - 1200f); // Move down by 500 units
        }

        // Store original position of LoadingText
        if (LoadingText != null)
        {
            loadingTextOriginalPos = LoadingText.localPosition;
        }

        Debug.Log($"BlueTopPos = {blueTopOriginalPos}, YellowDownPos = {yellowDownOriginalPos}");
        Debug.Log($"BlueTopHiddenPos = {blueTopHiddenPos}, YellowDownHiddenPos = {yellowDownHiddenPos}");
        
        // Initially hide the loading panel after a short delay
        HideLoadingPanelDelay(1f);
    }

    

    /// <summary>
    /// Shows the loading panel with animation
    /// BlueTopImage animates from TOP (above screen) to Center (visible position)
    /// YellowDownImage animates from Bottom (below screen) to Center (visible position)
    /// </summary>
    public void ShowLoadingPanel()
    {
        Debug.Log("Showing loading panel");
        
        // * Hide gameplay panel when loading
        My_UIManager.Instance.SetGamePlayPanel(false);

        
        if (Blue_RectTransform != null)
        {
            // Move BlueTopImage from above screen to original position
            Blue_RectTransform.DOAnchorPos(blueTopOriginalPos, animationDuration)
                .SetEase(animationEase)
                .OnStart(() => BlueTopImage.SetActive(true));
        }

        if (Yellow_RectTransform != null)
        {
            // Move YellowDownImage from below screen to original position
            Yellow_RectTransform.DOAnchorPos(yellowDownOriginalPos, animationDuration)
                .SetEase(animationEase)
                .OnStart(() => YellowDownImage.SetActive(true));
        }

        // Start LoadingText jump animation
        if (LoadingText != null)
        {
            LoadingText.gameObject.SetActive(true);
            LoadingText.localPosition = loadingTextOriginalPos;
            loadingTextTween?.Kill();
            loadingTextTween = LoadingText.DOLocalMoveY(loadingTextOriginalPos.y + 30f, 0.5f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutQuad);
        }

        Debug.Log("Loading panel shown");
 
    }

    /// <summary>
    /// Hides the loading panel with animation
    /// BlueTopImage animates from Center (visible position) to TOP (above screen)
    /// YellowDownImage animates from Center (visible position) to Bottom (below screen)
    /// </summary>
    public void HideLoadingPanel()
    {
        Debug.Log("Hiding loading panel");
        
        if (Blue_RectTransform != null)
        {
            // Move BlueTopImage from original position to above screen
            Blue_RectTransform.DOAnchorPos(blueTopHiddenPos, animationDuration)
                .SetEase(animationEase)
                .OnComplete(() => BlueTopImage.SetActive(false));
        }

        if (Yellow_RectTransform != null)
        {
            // Move YellowDownImage from original position to below screen
            Yellow_RectTransform.DOAnchorPos(yellowDownHiddenPos, animationDuration)
                .SetEase(animationEase)
                .OnComplete(() => YellowDownImage.SetActive(false));
        }

        // Stop LoadingText jump animation
        if (LoadingText != null)
        {
            loadingTextTween?.Kill();
            LoadingText.localPosition = loadingTextOriginalPos;
            LoadingText.gameObject.SetActive(false);
        }

        Debug.Log("Loading panel hidden");
    }

    /// <summary>
    /// Shows loading panel for a specified duration then hides it
    /// </summary>
    /// <param name="duration">How long to show the loading panel</param>
    public void ShowLoadingPanelForDuration(float duration)
    {
        ShowLoadingPanel();
        DOVirtual.DelayedCall(duration, HideLoadingPanel);
    }

    public void ShowLoadingPanelDelay(float delay){
        StartCoroutine(ShowLoadingPanelCoroutine(delay));
    } 

    private IEnumerator ShowLoadingPanelCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        ShowLoadingPanel();
    }
    public void HideLoadingPanelDelay(float delay){
        StartCoroutine(HideLoadingPanelCoroutine(delay));
    } 

    private IEnumerator HideLoadingPanelCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        HideLoadingPanel();
    }
}

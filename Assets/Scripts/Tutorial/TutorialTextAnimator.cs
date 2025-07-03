using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TutorialTextAnimator : MonoBehaviour, IPointerClickHandler
{

    [Header("Text Animation Settings")]
    [SerializeField] private float animationDuration = 0.5f; // Duration of the animation in seconds
    

    public List<GameObject> tutorialTextObjects; // List of text objects to animate
    public static int currentTextIndex = 0; // Index of the current text object being animated

    public bool isActionRequired = false;


    // ---- Sprites
    [Header("Sprites")]
    [SerializeField] private GameObject WorkerSprite;
    [SerializeField] private Image BlackBGImage;
    [SerializeField] private GameObject PopupTextContainer;



    // * Only the Instructions that require action will be listed here
    private readonly List<int> ActionRequired_InstructionsIndexes = new List<int>
    {
        3, //  3rd Instruction
        4, //  4th Instruction
        6, //  6th Instruction
    };



    //--------------- Singleton Instance ---------------
    public static TutorialTextAnimator Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("TutorialTextAnimator initialized");
        }
        else
        {
            Destroy(gameObject);
            return; // Exit if an instance already exists
        }

        // * Hide Tutorial on Start
        HideTutorialInstructions();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // ! use the TrainController to Start showing the Tutorial when the Train has Stopped 
        // StartShowingTutorialInstructions();
    }

    public int GetNumberOfInstructions(){
        return tutorialTextObjects.Count; // Return the total number of text objects
    } 


    // Called when the user clicks on the UI panel this script is attached to
    public void OnPointerClick(PointerEventData eventData)
    {
        // * if Action is Not Required then Completed this Instruction
        if(isActionRequired == false){
            Debug.Log("TutorialTextAnimator panel clicked");
            TutorialManager.Instance.CompleteCurrentInstruction(); // Notify the TutorialManager that the current instruction is completed
        }else{
            // *  Hide the Instructions and wait untill the Action is Completed 
            HideTutorialInstructions();

            // * Create the Grid Conveyor Button
               TutorialManager.Instance.CreateGridConveyorButton();

               isActionRequired = false;
        }
        // You can call ShowNextText() here if you want to show the next text on click
        // ShowNextText();
    }


    public void StartShowingTutorialInstructions(){
        Debug.Log("Starting to show tutorial instructions");

        WorkerSprite.SetActive(true); // Show the worker sprite
        PopupTextContainer.SetActive(true); // Show the popup text container

        // Reset the current text index to start from the first text object
        currentTextIndex = 0; // Reset the index to start from the first text object
        ShowNextText(); // Show the first text object

        FadeInBlackBG();
    }



    public void ShowNextText()
    {
        // * hide all the Other text objects before showing the next one
        foreach (GameObject textObject in tutorialTextObjects)
        {
            textObject.SetActive(false); // Hide all text objects
        }


        if (currentTextIndex < tutorialTextObjects.Count)
        {
            GameObject textObject = tutorialTextObjects[currentTextIndex];
            textObject.SetActive(true); // Show the text object
            StartCoroutine(FadeInText(textObject));



                // check if the CurrentInstructionIndex is in the actionRequiredOnInstructions list
                if (ActionRequired_InstructionsIndexes.Contains(currentTextIndex  +1)) // +1 because currentTextIndex is 0-based
                {
                    Debug.Log($"Action required for instruction {currentTextIndex +1 }");


                    isActionRequired = true;

                    // // Perform the action required for this instruction
                    //     // * Hide the Tutorial Instructions
                    //     TutorialTextAnimator.Instance.HideTutorialInstructions();

                    //     // * Create the Grid Conveyor Button
                    //     CreateGridConveyorButton();
                }
                else
                {
                    Debug.Log($"No action required for instruction {currentTextIndex +1 }");
                } 
        }
        


    }

    private System.Collections.IEnumerator FadeInText(GameObject textObject)
    {
        CanvasGroup canvasGroup = textObject.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = textObject.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0f; // Start with the text invisible
        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / animationDuration);
            yield return null; // Wait for the next frame
        }

        canvasGroup.alpha = 1f; // Ensure the text is fully visible at the end
    }


    public void HideTutorialInstructions()
    {
        Debug.Log("Hiding tutorial instructions");

        WorkerSprite.SetActive(false); // Hide the worker sprite
        PopupTextContainer.SetActive(false); // Hide the popup text container

        // Hide all text objects
        foreach (GameObject textObject in tutorialTextObjects)
        {
            textObject.SetActive(false);
        }

    // Fade out the BlackBGImage by animating its alpha to 0
        if (BlackBGImage != null)
        {
            FadeOutBlackBG();
        }
    }

    private void FadeOutBlackBG(){
        // BlackBGImage.color = new Color(0, 0, 0, 0.7f);


        BlackBGImage.DOFade(0f, animationDuration).OnComplete(() => {
            BlackBGImage.gameObject.SetActive(false);
        });
    }

    private void FadeInBlackBG(){
        // BlackBGImage.color = new Color(0, 0, 0, 0f);
            BlackBGImage.gameObject.SetActive(true);

            BlackBGImage.DOFade(0.7f, animationDuration);
    }




    public void UnHideTutorialInstructions(){
        Debug.Log("Showing tutorial instructions");

        WorkerSprite.SetActive(true); // Show the worker sprite
        PopupTextContainer.SetActive(true); // Show the popup text container

        currentTextIndex++;  // * so that Next instruction is shown to the User
        ShowNextText(); // Show the first text object

        FadeInBlackBG();
    }


}

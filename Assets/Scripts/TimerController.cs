using System.Collections;
using TMPro;
using UnityEngine;

public class TimerController : MonoBehaviour
{
    public bool isTimerStarted = false;
    public float TotalTime = 0f;
    public TextMeshProUGUI timerText;

    
    [SerializeField] private int LevelMinutes; 
    [SerializeField] private int LevelSeconds; 

    private int minutes_Remaining = 1;
    private int seconds_Remaining = 0;

    private float initialTimeInSeconds;
    private float currentTimeInSeconds;




    void Start()
    {
        StartTimer();
    }

    public void StartTimer()
    {
        // reset the time to the level time
        minutes_Remaining = LevelMinutes;
        seconds_Remaining = LevelSeconds;

        Debug.Log("Timer started with " + minutes_Remaining + " minutes and " + seconds_Remaining + " seconds remaining.");
        isTimerStarted = true;
        // Convert minutes and seconds to total seconds
        initialTimeInSeconds = (minutes_Remaining * 60) + seconds_Remaining;
        currentTimeInSeconds = initialTimeInSeconds;
        

        timerText = My_UIManager.Instance.TimerText;

        if (timerText != null)
        {
            UpdateTimerText();
        }


        StartCoroutine(EndTimeAfterRemainingTime());
    }

    // Update the timerText according to minutes_Remaining and seconds_Remaining
    void UpdateTimerText()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentTimeInSeconds / 60);
            int seconds = Mathf.FloorToInt(currentTimeInSeconds % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isTimerStarted)
        {
            if (currentTimeInSeconds > 0)
            {
                currentTimeInSeconds -= Time.deltaTime;
                minutes_Remaining = Mathf.FloorToInt(currentTimeInSeconds / 60);
                seconds_Remaining = Mathf.FloorToInt(currentTimeInSeconds % 60);
                UpdateTimerText();
            }
            else
            {
                // Timer has reached zero
                currentTimeInSeconds = 0;
                minutes_Remaining = 0;
                seconds_Remaining = 0;
                isTimerStarted = false;
                UpdateTimerText();
                // You can add any additional logic here when timer reaches zero
            }
        }
    }


    public IEnumerator EndTimeAfterRemainingTime(){
        yield return new WaitForSeconds(minutes_Remaining*60 + seconds_Remaining);
        // Time is up
        SubmissionTable_Controller.Instance.TimeEnded();
    }


    public void StarTimeAgain()
    {
        StartTimer();
    }
}

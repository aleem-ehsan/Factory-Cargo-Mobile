using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LevelProgressManager : MonoBehaviour
{
    private const string LEVEL_PROGRESS_KEY = "LevelProgress";
    private const string LEVEL_STARS_KEY = "LevelStars_"; // Prefix for storing stars for each level

    // -------------- Singleton Pattern --------------
    private static LevelProgressManager instance;

    public static LevelProgressManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogWarning("LevelProgressManager instance was null, creating new instance");
                GameObject go = new GameObject("LevelProgressManager");
                instance = go.AddComponent<LevelProgressManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    private void Awake()
    {
        Debug.Log("LevelProgressManager Awake called");
        if (instance != null && instance != this)
        {
            Debug.Log("Destroying duplicate LevelProgressManager");
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        #if UNITY_EDITOR
            // ResetProgress();// TODO: Testing purposes
        #endif
    }

    public void SaveLevelProgress(int levelNumber)
    {
        Debug.Log($"LevelProgressManager: Saving progress for level {levelNumber}");
        int currentProgress = GetHighestCompletedLevel();
        if (levelNumber > currentProgress)
        {
            PlayerPrefs.SetInt(LEVEL_PROGRESS_KEY, levelNumber);
            PlayerPrefs.Save();
            Debug.Log($"LevelProgressManager: Successfully saved progress for level {levelNumber}");
        }
        else
        {
            Debug.Log($"LevelProgressManager: Level {levelNumber} is not higher than current progress {currentProgress}, not saving");
        }
    }

    public int GetHighestCompletedLevel()
    {
        return PlayerPrefs.GetInt(LEVEL_PROGRESS_KEY, 0);
    }

    public bool IsLevelCompleted(int levelNumber)
    {
        return levelNumber <= GetHighestCompletedLevel();
    }

    /// <summary>
    /// Saves the number of stars earned for a specific level
    /// </summary>
    /// <param name="levelNumber">The level number</param>
    /// <param name="stars">Number of stars earned (0-3)</param>
    public void SaveLevelStars(int levelNumber, short stars)
    {
        string key = LEVEL_STARS_KEY + levelNumber;
        short currentStars = GetLevelStars(levelNumber);
        
        // Only save if the new star count is higher than the current one
        if (stars > currentStars)
        {
            PlayerPrefs.SetInt(key, stars);
            PlayerPrefs.Save();
            Debug.Log($"LevelProgressManager: Saved {stars} stars for level {levelNumber}");
        }
        else
        {
            Debug.Log($"LevelProgressManager: Not saving stars for level {levelNumber} as current stars ({currentStars}) are higher than new stars ({stars})");
        }
    }

    /// <summary>
    /// Gets the number of stars earned for a specific level
    /// </summary>
    /// <param name="levelNumber">The level number</param>
    /// <returns>Number of stars earned (0-3)</returns>
    public short GetLevelStars(int levelNumber)
    {
        string key = LEVEL_STARS_KEY + levelNumber;
        return (short)PlayerPrefs.GetInt(key, 0);
    }

    /// <summary>
    /// Function to Reset the Level Progress being Made in the Game
    /// </summary>
    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey(LEVEL_PROGRESS_KEY);
        ResetStars();
        PlayerPrefs.Save();
    }

/// <summary>
/// Function to Reset the Stars gained for each Level
/// </summary>
    private void ResetStars()
    {
        // Also reset all level stars
        int highestLevel = GetHighestCompletedLevel();
        for (int i = 1; i <= highestLevel; i++)
        {
            PlayerPrefs.DeleteKey(LEVEL_STARS_KEY + i);
        }
    }
}

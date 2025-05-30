using System.Collections.Generic;
using Hypertonic.GridPlacement.Example.BasicDemo;
using UnityEngine;

// Ensure LevelData and RequiredResource are accessible
// Assuming LevelData.cs defines them or they are in a shared location

public class LevelRequirementsData : MonoBehaviour
{
    public List<LevelData> levels = new List<LevelData>();



    void Awake(){
         PopulateLevelData();
    }

    private void PopulateLevelData(){
        // Level 1
        levels.Add(new LevelData
        {
            requiredResources = new RequiredResource[]
            {
                new RequiredResource(ResourceType.MetalBar, 2),
            },
            AvailableTimeInSeconds = 50, 

            // * Amount of Conveyors allowed 
            AllowedConveyors = new ConveyorStats[]
            {
                new ConveyorStats(ConveyorType.Straight, 1), // Example conveyor stats
                new ConveyorStats(ConveyorType.Short, 2) // Another example
            }
        });

        // Level 2
        levels.Add(new LevelData
        {
            requiredResources = new RequiredResource[]
            {
                new RequiredResource(ResourceType.TexturedBar, 2)
            },
            AvailableTimeInSeconds = 40,

            // * Amount of Conveyors allowed 
            AllowedConveyors = new ConveyorStats[]
            {
                new ConveyorStats(ConveyorType.Straight, 1), // Example conveyor stats
                new ConveyorStats(ConveyorType.Short, 1) // Another example
            }
        });

        // Level 3
        levels.Add(new LevelData
        {
            requiredResources = new RequiredResource[]
            {
                new RequiredResource(ResourceType.MetalBar, 1),
                new RequiredResource(ResourceType.TexturedBar, 2)
            },
            AvailableTimeInSeconds = 40,

            // * Amount of Conveyors allowed 
            AllowedConveyors = new ConveyorStats[]
            {
                new ConveyorStats(ConveyorType.Straight, 2), // Example conveyor stats
                new ConveyorStats(ConveyorType.Short, 2) // Another example
            }
        });

    }




    // Optional: Add a method to get requirements for a specific level index
    public RequiredResource[] GetRequirementsForLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < levels.Count)
        {
            return levels[levelIndex].requiredResources;
        }
        Debug.LogError($"Level requirements not found for index {levelIndex}");
        return null;
    }


/// <summary>
/// The time returned is in Seconds and 
/// </summary>
/// <param name="levelIndex"></param>
/// <returns></returns>
    public int GetRemainingTimeForLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < levels.Count)
        {
            return levels[levelIndex].AvailableTimeInSeconds;
        }
        Debug.LogError($"Remaining time not found for level index {levelIndex}");
        return 0;
    }
} 
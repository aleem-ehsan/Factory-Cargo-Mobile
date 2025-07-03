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
                new(ResourceType.MetalBar, 10),
            },
            AvailableTimeInSeconds = 70, 

            // * Amount of Conveyors allowed 
            AllowedConveyors = new ConveyorStats[]
            {
                new(ConveyorType.Straight, 2), // Example conveyor stats
                // new(ConveyorType.Short, 2), // Example conveyor stats
            }
        });

        // Level 2
        levels.Add(new LevelData
        {
            requiredResources = new RequiredResource[]
            {
                new(ResourceType.TexturedBar, 8)
            },
            AvailableTimeInSeconds = 70,

            // * Amount of Conveyors allowed 
            AllowedConveyors = new ConveyorStats[]
            {
                new(ConveyorType.Straight, 2), // Example conveyor stats
            }
        });

        // Level 3
        levels.Add(new LevelData
        {
            requiredResources = new RequiredResource[]
            {
                new(ResourceType.MetalBar, 10),
            },
            AvailableTimeInSeconds = 70,

            // * Amount of Conveyors allowed 
            AllowedConveyors = new ConveyorStats[]
            {
                new(ConveyorType.Curved, 1), // Example conveyor stats
                new(ConveyorType.Straight, 2), // Example conveyor stats
            }
        });

        // Level 4
        levels.Add(new LevelData
        {
            requiredResources = new RequiredResource[]
            {
                new(ResourceType.MetalBar, 8),
            },
            AvailableTimeInSeconds = 50,

            // * Amount of Conveyors allowed 
            AllowedConveyors = new ConveyorStats[]
            {
                new(ConveyorType.Curved, 1), // Example conveyor stats
                new(ConveyorType.Straight, 1), // Example conveyor stats
                new(ConveyorType.Bumper, 1), // Example conveyor stats
            }
        });
          
        // Level 5
        levels.Add(new LevelData
        {
            requiredResources = new RequiredResource[]
            {
                new(ResourceType.MetalBar, 4),
            },
            AvailableTimeInSeconds = 40,

            // * Amount of Conveyors allowed 
            AllowedConveyors = new ConveyorStats[]
            {
                new(ConveyorType.Curved, 1), // Example conveyor stats
                new(ConveyorType.Straight, 1), // Example conveyor stats
                new(ConveyorType.Bumper, 1), // Example conveyor stats
            }
        });
  
        // Level 6
        levels.Add(new LevelData
        {
            requiredResources = new RequiredResource[]
            {
                new(ResourceType.EmmisiveBar, 6),
            },
            AvailableTimeInSeconds = 60,

            // * Amount of Conveyors allowed 
            AllowedConveyors = new ConveyorStats[]
            {
                new(ConveyorType.Curved, 1), // Example conveyor stats
                new(ConveyorType.Straight, 1), // Example conveyor stats
            }
        });
        

        // Level 7
        levels.Add(new LevelData
        {
            requiredResources = new RequiredResource[]
            {
                new(ResourceType.MetalBar, 2),
                new(ResourceType.EmmisiveBar, 4),
            },
            AvailableTimeInSeconds = 90,

            // * Amount of Conveyors allowed 
            AllowedConveyors = new ConveyorStats[]
            {
                new(ConveyorType.Curved, 1), // Example conveyor stats
                new(ConveyorType.Straight, 1), // Example conveyor stats
                new(ConveyorType.Bumper, 2), // Example conveyor stats
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

/// <summary>
/// Gets the required resources for the tutorial level
/// </summary>
/// <returns>Array of required resources for tutorial level</returns>
public LevelData GetTutorialLevelRequirements()
{
    LevelData levelRequiredData = new()
    {
            requiredResources = new RequiredResource[]
            {
                new(ResourceType.MetalBar, 4),
            },
            AvailableTimeInSeconds = 90,

            // * Amount of Conveyors allowed 
            AllowedConveyors = new ConveyorStats[]
            {
                new(ConveyorType.Curved, 1), // Example conveyor stats
                new(ConveyorType.Straight, 1), // Example conveyor stats
                new(ConveyorType.Bumper, 2), // Example conveyor stats
            }
        };
    // Tutorial level is typically level 0 or the first level
    return levelRequiredData;
}
} 
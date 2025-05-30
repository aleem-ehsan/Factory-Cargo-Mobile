using System.Collections.Generic;
using Hypertonic.GridPlacement.Example.BasicDemo;
using UnityEngine;

// Make sure the RequiredResource struct is accessible, maybe define it here or in a shared file
// For now, we will define it here for simplicity, assuming it's not needed elsewhere in this file.
[System.Serializable]
public struct RequiredResource
{
    public ResourceType type;
    public int quantity;

    public RequiredResource(ResourceType type, int quantity)
    {
        this.type = type;
        this.quantity = quantity;
    }
}

public struct ConveyorStats
{
    public ConveyorType conveyorType;
    public int maxCount;
    public ConveyorStats(ConveyorType conveyorType, int quantity)
    {
        this.conveyorType = conveyorType;
        this.maxCount = quantity;
    }
}


// Note: ResourceType enum should also be accessible. 
// You might need to move it to a shared file or ensure this script can access SubmissionTable_Controller.cs

[System.Serializable]
public class LevelData
{
    public RequiredResource[] requiredResources;
    public int AvailableTimeInSeconds = 60; // Default time for the level, can be overridden in the inspector
    // public int ProductionRate; // TODO: what type of Prodyuction Rate do we need?  {either limited craetion of Product on Starting Machine}  

    public ConveyorStats[] AllowedConveyors; // List of conveyors and their stats


} 
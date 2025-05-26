using Hypertonic.GridPlacement;
using UnityEngine;

public class MyGridSetup : MonoBehaviour
{
    public GridManager gridManager; // Reference to the GridManager script
    public GridSettings _gridSettings; // Reference to the GridSettings script

    public GameObject testObjectPrefab;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GridSetup();
        PlaceTestingObject();
    }

    private void GridSetup(){
        gridManager.Setup(_gridSettings);
    }

    private void PlaceTestingObject(){
        GameObject gameObject = Instantiate(testObjectPrefab, GridManagerAccessor.GridManager.GetGridPosition(),new Quaternion());
        GridManagerAccessor.GridManager.EnterPlacementMode(testObjectPrefab);
    }

}

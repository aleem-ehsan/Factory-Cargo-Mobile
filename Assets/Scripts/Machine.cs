using UnityEngine;

public class Machine : MonoBehaviour
{
    // TODO: The first Machine in the entire Level should instantiate Metal Bars


    [SerializeField] private GameObject metalPrefab;
    [SerializeField] private Transform metalSpawnPoint;

    private float nextSpawnTime;


//   ------------------- Singleton -------------------
    public static Machine Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void Start()
    {
    }




    public void SpawnMetal()
    {
        Debug.Log("SpawnMetal called");

        GameObject spawnedMetalBar = Instantiate(metalPrefab, metalSpawnPoint.position, metalSpawnPoint.rotation);
        spawnedMetalBar.transform.SetParent(metalSpawnPoint);

        MetalBar metalBar;
        spawnedMetalBar.TryGetComponent<MetalBar>(out metalBar);

        if (metalBar != null)
        {
            metalBar.moveDirection = transform.right; 
            metalBar.MoveTheMetalInMachine();
        }
        else
        {
            Debug.LogError("MetalBar component not found on the spawned object.");
        }
       
    }
}

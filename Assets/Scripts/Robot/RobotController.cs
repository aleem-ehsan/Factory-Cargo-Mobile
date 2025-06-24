using System;
using UnityEngine;

public class RobotController : MonoBehaviour
{

    [SerializeField] private Machine ConnectedMachine; // Reference to the Machine script, if needed

    [SerializeField] private GameObject myMetal;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void SetMetalActive(int value)
    {
        if (value == 1)
        {
            myMetal.SetActive(true);
        }
        else{
            Debug.Log("CraeteMetalBar Invoked");
            Machine.Instance.SpawnStartingProduct();
            // ConnectedMachine.SpawnMetal();
            myMetal.SetActive(false);
        }
        
    }


    



}

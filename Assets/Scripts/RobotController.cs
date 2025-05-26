using System;
using UnityEngine;

public class RobotController : MonoBehaviour
{

    
    




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
            Machine.Instance.SpawnMetal();
            myMetal.SetActive(false);
        }
        
    }


    



}

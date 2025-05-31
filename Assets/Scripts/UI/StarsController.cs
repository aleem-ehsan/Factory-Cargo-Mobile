using System.Collections.Generic;
using UnityEngine;

public class StarsController : MonoBehaviour
{

    public List<GameObject> _YellowStars; // List of star GameObjects to manage

    // Start is called once before the first execution of Update after the MonoBehaviour is created


    // Initialize the stars by setting them to inactive
    private void InitializeStars()
    {
        foreach (GameObject star in _YellowStars)
        {
            star.SetActive(false); // Set all stars to inactive initially
        }
    }

    public void Display_YELLOW_Stars(short numberOfStars){
        if (numberOfStars <= 0 || numberOfStars > _YellowStars.Count)
        {
            Debug.Log("Invalid number of stars: " + numberOfStars);
            return; // Exit if the amount is out of bounds
        }
        Debug.Log("StarsController: Showing " + numberOfStars + " stars in the win panel.");


        for (short i = 0; i < numberOfStars; i++)
        {
            _YellowStars[i].SetActive(true); // Activate the specified number of stars
        }
    }





}
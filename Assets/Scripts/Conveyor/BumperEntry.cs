using UnityEngine;
using Dreamteck.Splines;
using System.Collections;

public class BumperEntry : ConveyorEntryController
{
    [Header("Animation")]
    [SerializeField] private Animator animator;

    protected void Awake()
    {
        base.Awake();
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private IEnumerator PlayBumpAnimation()
    {
        if (animator != null)
        {
            yield return new WaitForSeconds(0.1f); // Wait for a short duration before playing the animation
            animator.SetTrigger("bump");
            Debug.Log("Playing bump animation.");

        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Resource"))
        {
            StartCoroutine(PlayBumpAnimation());
            // Additional logic for when the player enters the bumper
        }
    }
}

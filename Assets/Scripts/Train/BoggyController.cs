using DG.Tweening;
using UnityEngine;

public class BoggyController : MonoBehaviour
{
    [SerializeField] private Transform productPos; // Container for products in the boggy

    private int PlacememntCount=0;

    private Vector3 startingPos; // Starting position of the boggy

    private float productYOffset = 0; // Offset to adjust the product position
    private float productForwardOffset = 0.1f; // Offset to adjust the product position

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startingPos = productPos.localPosition; // Store the initial position of the boggy
    }

    void OnEnable(){
        // Subscribe to the event when a product is dropped
        Robot_SubmissionTable.OnProductDropped += PlaceProductInContainer;
    }
    void OnDisable(){
        // Unsubscribe from the event when a product is dropped
        Robot_SubmissionTable.OnProductDropped -= PlaceProductInContainer;
    }




    public void PlaceProductInContainer(Transform product)
    {
        if (product == null)
        {
            Debug.LogError("Product to place in container is null.");
            return;
        }

        // Assuming the product has a parent container to place it in
        product.SetParent(transform); // Set the parent to this BoggyController's transform
        product.DOMove(productPos.position, 0.5f); // Animate position using DoTween
        product.DORotate(new Vector3(0, 90, 0), 0.5f); // Animate rotation using DOTween
        Debug.Log("Product placed in container.");

        productPos.localPosition += Vector3.forward * 0.8f   +   Vector3.forward * productForwardOffset ; // Adjust the position slightly to avoid overlap

        IncrementPlacememntCount(); // Increment the placement count
    }

    private void IncrementPlacememntCount()
    {
        PlacememntCount++;

        if(PlacememntCount > 2){
            ResetContainerPosition(); // Reset the position after 5 placements
        }
    }

    public void ResetContainerPosition()
    {
        PlacememntCount = 0; // Reset the placement count

        productPos.localPosition = startingPos; // Reset the position of the product container
        productYOffset += 0.2f; // Reset the offset for product positioning
        productPos.position += Vector3.up * productYOffset; // Move the position to the right after 5 placements

        productForwardOffset += 0.2f; // Reset the forward offset for product positioning
    }

}

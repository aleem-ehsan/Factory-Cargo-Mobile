using UnityEngine;
using Hypertonic.GridPlacement.CustomSizing;

public class CurvedConveyorBounds : MonoBehaviour
{

        [SerializeField]
        public Bounds _bounds1 = new Bounds(new Vector3(0.03055191f,0,-1.909308f), new Vector3(2.704933f,0,0.8729744f));
        public Bounds _bounds2 = new Bounds(Vector3.zero, Vector3.one);
        // [SerializeField]
        // public Bounds _bounds2 = new Bounds(Vector3.zero, Vector3.one);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
}

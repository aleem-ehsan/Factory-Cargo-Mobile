using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
namespace Hypertonic.GridPlacement.Example.BasicDemo
{
    [RequireComponent(typeof(Button))]
    public class Button_GridObjectSelectionOption : MonoBehaviour, IPointerDownHandler
    {
        public static event System.Action<GameObject> OnOptionSelected;

        [SerializeField] private GameObject _gridObjectToSpawnPrefab;


        
        void Awake(){
           
        }


        private void Start()
        {
        }


        void OnEnable()
        {
        }
        void OnDisable()
        {
        }



        public void OnPointerDown(PointerEventData eventData)
        {
            
            HandleButtonClicked();
        }

        private void HandleButtonClicked()
        {
            if (_gridObjectToSpawnPrefab == null)
            {
                Debug.LogError("Error. No prefab assigned to spawn on this selection option");
            }

            GameObject objectToPlace = Instantiate(_gridObjectToSpawnPrefab, GridManagerAccessor.GridManager.GetGridPosition(), new Quaternion());

            objectToPlace.name = _gridObjectToSpawnPrefab.name;

            if (!objectToPlace.TryGetComponent(out ExampleGridObject gridObject))
            {
                objectToPlace.AddComponent<ExampleGridObject>();
            }

            OnOptionSelected?.Invoke(objectToPlace);

            GridManagerAccessor.GridManager.EnterPlacementMode(objectToPlace);
        }

    }
}
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


        [SerializeField] public ConveyorType ConveyorType;

        [SerializeField] private bool _isButtonInteractable = true; // Set this to false if you want the button to be disabled initially

        [SerializeField] private int MaxPlaceble = 0; // Set this to false if you want the button to be disabled initially


        private TextMeshProUGUI _countText; // Reference to the TextMeshProUGUI component for displaying the count

        
        void Awake(){
            _countText = GetComponentInChildren<TextMeshProUGUI>();
           
        }


        private void Start()
        {
            // No need for click listener anymore

            MaxPlaceble = ConveyorManager.Instance.GetRemainingCount(ConveyorType);
             if (_countText == null)
            {
                Debug.LogError("TextMeshProUGUI component not found in children. Please ensure it is present.");
            }
            else
            {
                UpdateCountUI(); // Initialize the count UI
            }
        }


        void OnEnable()
        {
            ConveyorManager.OnConveyorMaxLimitReached += HandleConveyorMaxLimitReached;
            ConveyorManager.OnConveyorCanceledOrDeleted += UpdateCount_From_ConveyorManager;
        }
        void OnDisable()
        {
            ConveyorManager.OnConveyorMaxLimitReached -= HandleConveyorMaxLimitReached;
            ConveyorManager.OnConveyorCanceledOrDeleted -= UpdateCount_From_ConveyorManager;

        }



        public void OnPointerDown(PointerEventData eventData)
        {
            if(_isButtonInteractable == false)
            {
                Debug.Log("Button is not interactable. Returning.");
                return; // Exit if the button is not interactable
            }
            HandleButtonClicked();
            MaxPlaceble--;

            UpdateCountUI();
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


        private void UpdateCountUI(){
                _countText.text = $"{MaxPlaceble}"; // Update the text with the current count
        }

        public void HandleConveyorMaxLimitReached(ConveyorType conveyorType)
        {
            if (conveyorType == ConveyorType)
            {
                Debug.Log($"Button Converter Type Matched: {conveyorType}. Disabling button for this conveyor type.");
                // Disable the button if the max limit is reached for this conveyor type
                if (TryGetComponent<Button>(out var button))
                {
                    button.interactable = false;
                    _isButtonInteractable = false; // !! Update the interactable state
                    Debug.Log("Button component FOUND");
                }
                else
                {
                    Debug.LogError("Button component !!! NOT FOUND on this GameObject.");
                }
            }
        }


    public void UpdateCount_From_ConveyorManager(ConveyorType conveyorType){
        MaxPlaceble = ConveyorManager.Instance.GetRemainingCount(ConveyorType);
        UpdateCountUI();
        if(MaxPlaceble <= 0)
        {
            if (TryGetComponent<Button>(out var button))
            {
                button.interactable = false;
                _isButtonInteractable = false; // !! Update the interactable state
            }
        }
        else
        {
            if (TryGetComponent<Button>(out var button))
            {
                button.interactable = true;
                _isButtonInteractable = true; // !! Update the interactable state
            }
        }
    }




    }
}
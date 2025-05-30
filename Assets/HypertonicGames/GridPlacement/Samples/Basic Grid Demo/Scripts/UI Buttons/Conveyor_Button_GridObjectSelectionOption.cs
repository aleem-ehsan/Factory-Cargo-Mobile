using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
namespace Hypertonic.GridPlacement.Example.BasicDemo
{
    [RequireComponent(typeof(Button))]
    public class Conveyor_Button_GridObjectSelectionOption : MonoBehaviour, IPointerDownHandler
    {
        public static event System.Action<GameObject> OnOptionSelected;

        [SerializeField] private GameObject _gridObjectToSpawnPrefab;


        [SerializeField] public ConveyorType ConveyorType;

        [SerializeField] private bool _isButtonInteractable = true; // Set this to false if you want the button to be disabled initially

        [SerializeField] private int MaxPlaceble = 0; // Set this to false if you want the button to be disabled initially


        private TextMeshProUGUI _countText; // Reference to the TextMeshProUGUI component for displaying the count

        [SerializeField] private Image _buttonIconImage;


        /// <summary>
        /// Static variable to control whether any button (including this Button) can be pressed or not.
        /// </summary>
        public static bool canPressAnyButton = true; // Static variable to control button press behavior across instances


        [SerializeField] private Button _thisButton;

        
        // --------------- events ------------

        void Awake(){

                _thisButton = GetComponent<Button>();

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



            SetConveyorIcon();
            InitializeConveyorPrefab();
        }


        private void InitializeConveyorPrefab(){
            GameObject conveyor = (GameObject) Resources.Load($"Conveyors/Conveyor-{ConveyorType}", typeof(GameObject));
            if (conveyor == null)
            {
                Debug.LogError($"Conveyor prefab for type {ConveyorType} not found in Resources/Conveyors.");
                return; // Exit if the conveyor prefab is not found
            }
            _gridObjectToSpawnPrefab = conveyor;

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
            if(_isButtonInteractable == false || canPressAnyButton == false)  // ? if this is NOT Interactable or if another Button is pressed
            {
                Debug.Log("Button is not interactable. Returning.");
                return; // Exit if the button is not interactable
            }

            canPressAnyButton = false;  // * Disable pressing of anyother Button
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
                // if (TryGetComponent<Button>(out var button))
                // {
                    _thisButton.interactable = false;
                    _isButtonInteractable = false; // !! Update the interactable state
                // }
            }
            else
            {
                // if (TryGetComponent<Button>(out var button))
                // {
                    _thisButton.interactable = true;
                    _isButtonInteractable = true; // !! Update the interactable state
                // }
            }
        }



/// <summary>
/// To initialize the maximum conveyor count for each type .
/// </summary>
        public void SetConveyorTypeAndQuantity( ConveyorType conveyorType, int quantity)
        {
            ConveyorType = conveyorType;
            MaxPlaceble = quantity;

            if (_countText != null)
            {
                UpdateCountUI(); // Update the count UI with the new quantity
            }
            else
            {
                Debug.LogError("TextMeshProUGUI component not found in children. Please ensure it is present.");
            }

        }

/// <summary>
/// This Function assigns the Conveyor Icon to the Button from the Resources folder based on the ConveyorType  
/// </summary>
        public void SetConveyorIcon(){

        // !: Execution of below line of code terminated Unity once
            Sprite image = Resources.Load($"Sprites/Conveyors/{ConveyorType}", typeof(Sprite)) as Sprite;
            

            if (image == null)
            {
                Debug.LogError($"Sprite for ConveyorType -{ConveyorType}- not found in Resources/Sprites/Conveyors.");
                return; // Exit if the sprite is not found
            }

            _buttonIconImage.sprite = image;
        }


        /// <summary>
        /// This 
        /// </summary>
        /// <param name="value"></param>
        public static void SetAnyButtonPressable(bool value){
            canPressAnyButton = value; // Set the static variable to control button press behavior


// TODO: Enable/Disable Button      -----   Optional
        // if(value == false){
        //     // disable Button
        // }else{
        //     // Enable Button
        // }
            
            
        }

/// <summary>
/// Fuction to disable this button
/// </summary>
        private void DisableThisButton(){
            _thisButton.interactable = false; // Disable this button
        }



    }
}
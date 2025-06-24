using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
namespace Hypertonic.GridPlacement.Example.BasicDemo
{
    [RequireComponent(typeof(Button))]
    public class Conveyor_Button_GridObjectSelectionOption : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
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
        /// 
        // TODO: Remove this CanPressAnyButton ASAP, after the first Time created Grid Object is Confirmed Placement on PointerUp
        // public static bool canPressAnyButton = true; // Static variable to control button press behavior across instances


        [SerializeField] private Button _thisButton;

        
        // --------------- events ------------
        public static event System.Action OnOptionReleased;


        void Awake(){

                _thisButton = GetComponent<Button>();

            _countText = GetComponentInChildren<TextMeshProUGUI>();
        }


        private void Start()
        {
           

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
            
            // SetButtonInteractable(false); // Set the initial interactable state of the button
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
            LevelManager.OnGameplayStarted += SetButtonInteractable; // Subscribe to the event to update count when gameplay starts   
            ConveyorManager.OnConveyorMaxLimitReached += HandleConveyorMaxLimitReached;
            ConveyorManager.OnConveyorCanceledOrDeleted += HandleConveyorDeleted;
        }
        void OnDisable()
        {
            LevelManager.OnGameplayStarted -= SetButtonInteractable; // Subscribe to the event to update count when gameplay starts   
            ConveyorManager.OnConveyorMaxLimitReached -= HandleConveyorMaxLimitReached;
            ConveyorManager.OnConveyorCanceledOrDeleted -= HandleConveyorDeleted;

        }



        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("Pointer Down on the Button");

            if(_isButtonInteractable == false)  // ? if this is NOT Interactable or if another Button is pressed
            {
                Debug.Log("Button is not interactable. Returning.");
                return; // Exit if the button is not interactable
            }

            // canPressAnyButton = false;  // ! Not Needed anymore : Disable pressing of anyother Button
            HandleButtonClicked();



            // * Set the Selected Conveyor in the ConveyorManager
            
          
        }


        public void OnPointerUp(PointerEventData eventData){
            // * Check if the MaxPlaceble is 0 , Means no More Conveyor of this type can be placed
            if(MaxPlaceble == 0) 
                return;

            Debug.Log("Pointer Up on the Button");
            // Check if placement is valid before confirming
            if (GridManagerAccessor.GridManager.IsObjectPlacementValid().Valid )
            {
                // * Conveyor is placed successfully
                OnOptionReleased?.Invoke();
                MaxPlaceble--;
                UpdateCountUI();


            }else{
                GridManagerAccessor.GridManager.DeleteObject(ConveyorManager.Instance.GetLastCraetedConveyor());
                ExampleGridObject.HandleConveyorDeleted();
            }
        }

        public void ButtonReleased(){
            Debug.Log("Button Released");
            // canPressAnyButton = true; // Enable pressing of any other Button
            OnOptionReleased?.Invoke();
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

            // * Set the last created conveyor reference in ConveyorManager
            ConveyorManager.Instance.SetLastCreatedConveyor(objectToPlace);

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
            Debug.Log($"GRID BUTTON: UpdateCount_From_ConveyorManager with TYPE {conveyorType} called.");
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
/// When a Grid Object is Being deleted, then this function is called to update the count of the corresponding conveyor type.
/// </summary>
/// <param name="conveyorType"></param>
        public void HandleConveyorDeleted(ConveyorType conveyorType){
            UpdateCount_From_ConveyorManager(conveyorType); // Update the count from ConveyorManager when a conveyor is deleted
            // Now we should be able to Press any Button
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
/// Fuction to disable this button
/// </summary>
        // public void DisableThisButton(){
        //     _thisButton.interactable = false; // Disable this button
        // }

        public void SetButtonInteractable(bool isInteractable)
        {
            Debug.Log($"Setting button interactable state to: {isInteractable}");
            _isButtonInteractable = isInteractable;
            _thisButton.interactable = isInteractable; // Set the button interactable state
            _thisButton.enabled = isInteractable; // Enable or disable the button component itself
        }



    }
}
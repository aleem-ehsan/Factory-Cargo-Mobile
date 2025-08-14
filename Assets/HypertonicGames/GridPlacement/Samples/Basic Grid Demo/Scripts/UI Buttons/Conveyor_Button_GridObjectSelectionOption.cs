using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Hypertonic.GridPlacement.Models;
using Hypertonic.GridPlacement.Enums;
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

        private bool is1stConveyorPlaced = false;






        public Vector3 Offset = Vector3.zero; // Offset to apply to the conveyor object when placing it on the grid

        
        // --------------- events ------------
        public static event System.Action OnOptionReleased;


        void Awake(){

                _thisButton = GetComponent<Button>();

            _countText = GetComponentInChildren<TextMeshProUGUI>();
        }


        private void Start()
        {
           
            if(TutorialManager.Instance == null){  // if not in a Tutorial then Get Max Count from the ConveyorManager 
                MaxPlaceble = ConveyorManager.Instance.GetRemainingCount(ConveyorType);
            }
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
            ConveyorManager.OnConveyorMaxLimitReached += HandleConveyorMaxLimitReached;
            ConveyorManager.OnConveyorCanceledOrDeleted += HandleConveyorDeleted;
        }
        void OnDisable()
        {
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
            



            // if(ConveyorManager.Instance.GetTotalPlacedConveyorsCount() == 0){ // * if it is 1st Conveyor
            if(true){ // * TO Make Dragging Every Time for each Conveyor
                Debug.Log("Placing 1st Conveyor");
                is1stConveyorPlaced = true;
                HandleButtonClicked();
            }
            else{// * if it is 2nd or more Conveyor
                Debug.Log("Placing 2nd or more Conveyor");

                is1stConveyorPlaced = false;

                GameObject objectToPlace = Instantiate(_gridObjectToSpawnPrefab, GridManagerAccessor.GridManager.GetGridPosition(), new Quaternion());
            

                //* Use the GridManager to Place the Conveyor manually at the NextValidCellPos From the ConveyorManager
                // Vector2Int validCellPos = ConveyorManager.Instance.GetNextValidCellPosForConveyor();
                Vector3 validCellPos = ConveyorManager.Instance.GetNextValidCellPosWorldPos();
                PlacementSettings   placementSettings = new(validCellPos + Offset); // Create a PlacementSettings object with the valid cell position
                GridManagerAccessor.GridManager.EnterPlacementMode(objectToPlace , placementSettings); // Enter placement mode with the specified prefab and placement settings

                // Set alignment so the object starts at the grid cell index
                GridManagerAccessor.GridManager.ChangeAlignment(ObjectAlignment.UPPER_MIDDLE);


                //  exit placement mode
                GridManagerAccessor.GridManager.ConfirmPlacement();


 // * Set the last created conveyor reference in ConveyorManager
            ConveyorManager.Instance.SetLastCreatedConveyor(objectToPlace);
                return;
            }


            // * Set the Selected Conveyor in the ConveyorManager
            
          
        }


        public void OnPointerUp(PointerEventData eventData){
            // * Check if the MaxPlaceble is 0 , Means no More Conveyor of this type can be placed
            if(MaxPlaceble == 0 || is1stConveyorPlaced == false) 
                return;

            Debug.Log("Pointer Up on the Button");
            // Check if placement is valid before confirming
            if (GridManagerAccessor.GridManager.IsObjectPlacementValid().Valid )
            {
                // * Conveyor is placed successfully
                OnOptionReleased?.Invoke();
                MaxPlaceble--;
                UpdateCountUI();

                    if(TutorialManager.Instance != null){
                        TutorialManager.Instance.ConveyorCreated(ConveyorType); // Notify the TutorialManager that a conveyor has been created
                    }

            }else{// ! Conveyor is Not placed || Canceled the Placement
                Debug.Log("Placement is Invalid, deleting the conveyor object.");
                ConveyorChainController.Instance.OnConveyorDeleted(); // Delete the last created conveyor
                GridManagerAccessor.GridManager.DeleteObject(ConveyorManager.Instance.GetLastCraetedConveyor());
                ConveyorManager.Instance.HandleCancelPlacement(ConveyorType); // Notify the ConveyorManager that the placement was canceled
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

            Debug.Log("Setting Conveyor Type and Quantity: " + conveyorType + ", Quantity: " + quantity);
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


       
  public void SetConveyorOffset(Vector3 offset)
        {
            Offset = offset;
            Debug.Log($"Conveyor Offset set to: {Offset}");
        }




    }
}
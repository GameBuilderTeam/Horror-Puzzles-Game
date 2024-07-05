using UnityEngine;
using UnityEngine.InputSystem;

namespace InventorySystem
{
    public class InventoryInputHandler : MonoBehaviour
    {
        private Inventory _inventory;

        private void Awake()
        {
            _inventory = GetComponent<Inventory>(); 
        }

        private void OnEnable()
        {
            InputActions.Instance.Game.ThrowItem.performed += OnThrowItem;
            InputActions.Instance.Game.NextItem.performed += OnNextItem;
            InputActions.Instance.Game.PreviousItem.performed += OnPreviousItem;
        }

        private void OnDisable()
        {
            InputActions.Instance.Game.ThrowItem.performed -= OnThrowItem;
            InputActions.Instance.Game.NextItem.performed += OnNextItem;
            InputActions.Instance.Game.PreviousItem.performed += OnPreviousItem;
        }

        private void OnThrowItem(InputAction.CallbackContext cxt)
        {
            if(_inventory.GetActiveSlot().HasItem)
            _inventory.RemoveItem(_inventory.ActiveSlotIndex, true);
        }

        private void OnNextItem(InputAction.CallbackContext cxt)
        {
            _inventory.ActiveSlot(_inventory.ActiveSlotIndex + 1);
        }

        private void OnPreviousItem(InputAction.CallbackContext cxt)
        {
            _inventory.ActiveSlot(_inventory.ActiveSlotIndex - 1);
        }
    }
}
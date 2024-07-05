using InventroySystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace InventorySystem.UI
{
    public class UI_inventorySlot : MonoBehaviour
    {
        [SerializeField]
        private Inventory _inventory;

        [SerializeField]
        private int _inventorySlotIndex;

        [SerializeField]
        private Image _itemIcom;

        [SerializeField]
        private Image _activeIndicator;

        [SerializeField]
        private TMP_Text _numberOfItem;

        private InventorySlot _slot;

        private void Start()
        {
            AssignSlot(_inventorySlotIndex); 
        }
        public void AssignSlot(int soltIndex)
        {
            if (_slot != null) _slot.StateChanged -= OnStateChanged;
            _inventorySlotIndex = soltIndex;
            if (_inventory == null) _inventory = GetComponentInParent<UI_Inventory>().Inventory;
            _slot = _inventory.Slots[_inventorySlotIndex];
            _slot.StateChanged += OnStateChanged;
            UpdateViewState(_slot.State, _slot.Active);
        }

        private void UpdateViewState(ItemStack state, bool active)
        {
            _activeIndicator.enabled = active;
            var item = state?.Item;
            var hasItem = item != null;
            var isStackbale = hasItem && item.IsStackable;
            _itemIcom.enabled = hasItem;
            _numberOfItem.enabled = isStackbale;
            if (!hasItem) return;

            _itemIcom.sprite = item.UISprite;
            if (isStackbale) _numberOfItem.SetText(state.NumberOfItems.ToString());
        }
        private void OnStateChanged(object sender, InventorySlotStateChangedArgs args)
        {
            UpdateViewState(args.NewState, args.Active);
        }
    }
}
    

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace InventorySystem.UI
{
    public class UI_Inventory : MonoBehaviour
    {
        [SerializeField]
        private GameObject _inventorySlotPrefab;

        [SerializeField]
        private Inventory _inventory;

        [SerializeField]
        private List<UI_inventorySlot> _slots;

        public Inventory Inventory => _inventory;

        [ContextMenu("Initilize Inventory")]
        private void InitialiazedInventoryUi()
        {
            if (_inventory == null || _inventorySlotPrefab == null) return;

            _slots = new List<UI_inventorySlot>(_inventory.Size);

            for (int i = 0; i < _inventory.Size; i++)
            {
                var uiSlot = PrefabUtility.InstantiatePrefab(_inventorySlotPrefab) as GameObject;//Instantiate(_inventorySlotPrefab);
                uiSlot.transform.SetParent(transform, false);
                var uiSlotScript = uiSlot.GetComponent<UI_inventorySlot>();
                uiSlotScript.AssignSlot(i);
                _slots.Add(uiSlotScript);
            }
        }
    }
}
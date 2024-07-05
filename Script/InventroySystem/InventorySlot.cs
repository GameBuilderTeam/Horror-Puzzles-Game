using InventroySystem;
using System;
using UnityEngine;

namespace InventorySystem
{
    [Serializable]
    public class InventorySlot
    {
        public event EventHandler<InventorySlotStateChangedArgs> StateChanged;

        [SerializeField]
        private ItemStack _state;

        private bool _active;

        public ItemStack State
        {
            get => _state;
            set
            {
                _state = value;
                NotifyAboutStateChange();
            }
        }

        public bool Active
        {
            get => _active;
            set
            {
                _active = value;
                NotifyAboutStateChange();
            }
        }

        public int NumberOfItems
        {
            get => _state.NumberOfItems;
            set 
            {
                _state.NumberOfItems = value;
                NotifyAboutStateChange();
            }
        }

        public void Clear()
        {
            State = null;
        }

        private void NotifyAboutStateChange()
        {
            //?. if StateChanger!=null then Invoke
            StateChanged?.Invoke(this, new InventorySlotStateChangedArgs(_state, _active));
        }

        public bool HasItem => _state?.Item != null;
        public ItemDefinition Item => _state?.Item;

    }
}
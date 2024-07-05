using InventroySystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEngine;
using static UnityEditor.Progress;


namespace InventorySystem
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField]
        private int _size = 8;

        [SerializeField]
        private List<InventorySlot> _slots;

        [SerializeField]
        private GameItemContext _gameItemList;

        private int _activeSlotIndex;

        public int Size => _size;

        public List<InventorySlot> Slots => _slots;

        public int ActiveSlotIndex
        {
            get => _activeSlotIndex;
            private set
            {
                _slots[_activeSlotIndex].Active = false;
                _activeSlotIndex = value < 0 ? _size - 1 : value % Size;
                _slots[_activeSlotIndex].Active = true;
            }
        }

        private void Awake()
        {
            if (_size > 0)
            {
                _slots[0].Active = true;
            }
        }

        private void OnValidate()
        {
            AdjustSize();
        }
        private void AdjustSize()
        {
            _slots ??= new List<InventorySlot>();  // if(_slots == null) _slot = new List<InventorySlot>();

            if (_slots.Count > _size) _slots.RemoveRange(_size, _slots.Count - _size);

            if (_slots.Count < _size) _slots.AddRange(new InventorySlot[_size - _slots.Count]);
        }

        public bool IsFull()
        {
            return _slots.Count(slot => slot.HasItem) >= _size;
        }

        public bool CanAcceptItem(ItemStack itemStack)
        {
            var slotWithStackableItem = FindSlot(itemStack.Item, true);
            return !IsFull() || slotWithStackableItem != null;
        }

        private InventorySlot FindSlot(ItemDefinition item, bool OnlyStackable = false)
        {
            return _slots.FirstOrDefault(slot => slot.Item == item &&
                                                  item.IsStackable ||
                                                  !OnlyStackable);
        }
        public bool HasItem(ItemStack itemStack, bool checkNumberOfItems = false)
        {
            var itemSlot = FindSlot(itemStack.Item);
            if (itemSlot == null) return false;
            if (!checkNumberOfItems) return true;

            if (itemStack.Item.IsStackable)
            {
               return itemSlot.NumberOfItems >= itemStack.NumberOfItems;
            }

            return _slots.Count(slot => slot.Item == itemStack.Item) >= itemStack.NumberOfItems;

        }

        public ItemStack AddItem(ItemStack itemStack)
        {
            var relevantSlot = FindSlot(itemStack.Item, true);
            if (IsFull() && relevantSlot == null)
            {
                throw new InventoryException(InventoryOperation.Add, "Inventory is Full");
            }

            if (relevantSlot != null)
            {
                relevantSlot.NumberOfItems += itemStack.NumberOfItems;
            }
            else
            {
                relevantSlot = _slots.First(slot => !slot.HasItem);
                relevantSlot.State = itemStack;
            }
            return relevantSlot.State;
        }

        public ItemStack RemoveItem(int atIndex, bool spawn = false)
        {
            if (!_slots[atIndex].HasItem)          
                throw new InventoryException(InventoryOperation.Remove, "Slot is Empty");

            if (spawn && TryGetComponent<GameItemSpawner>(out var itemSpawner))
            {
                itemSpawner.SpawnItem(_slots[atIndex].State);
            }
            ClearSlot(atIndex);
            return new ItemStack();
        }

        public ItemStack RemoveItem(ItemStack itemStack)
        {
            var itemSlot = FindSlot(itemStack.Item);
            if (itemSlot == null)           
                throw new InventoryException(InventoryOperation.Remove, "No Item In The Inventory");
            
            if(itemSlot.Item.IsStackable && itemSlot.NumberOfItems < itemStack.NumberOfItems)
                throw new InventoryException(InventoryOperation.Remove, "Not Enough Items");

            itemSlot.NumberOfItems -= itemStack.NumberOfItems;
            if (itemSlot.Item.IsStackable && itemSlot.NumberOfItems > 0)
            {
                return itemSlot.State;
            }
            itemSlot.Clear();
            return new ItemStack();
        }

        public void ClearSlot(int atIndex)
        {
            _slots[atIndex].Clear();
            for (int i = atIndex; i < _slots.Count; i++)
            {
                if (!_slots[i].HasItem && _slots[i + 1].HasItem && i + 1 < _slots.Count)
                {
                    _slots[i].State = _slots[i + 1].State;
                    _slots[i + 1].Clear();
                }
                else if (!_slots[i + 1].HasItem || i + 1 >= _slots.Count)
                    break;

            }
        }
        public void ActiveSlot(int atIndex)
        {
            ActiveSlotIndex = atIndex;
        }

        public InventorySlot GetActiveSlot()
        {
            return _slots[ActiveSlotIndex];
        }
        public void SaveButton()
        {
            SaveByXml("InventoryXml.text");
        }
        private GameSave CreatSaveManager()
        {
            GameSave save = new GameSave();
            save.InventoryIndex = new int[_size];
            save.GameItemId = new int[_size];
            save.GameItemNumber = new int[_size];
            for (int i = 0; i < _size; i++)
            {
                if ( _slots[i].State!=null)
                {
                    save.InventoryIndex[i] = i;
                    save.GameItemId[i] = ItemId(_slots[i].State);
                    save.GameItemNumber[i] = _slots[i].State.NumberOfItems;
                }
                else
                {
                    save.InventoryIndex[i] = i;
                    save.GameItemId[i] = 0;
                    save.GameItemNumber[i] = 0;
                }
            }

            return save;
        }

        private int ItemId(ItemStack itemStack)
        {

            for (int i = 0;i < _gameItemList._gameItems.Length; i++)
            {
                if (_gameItemList._gameItems[i] == itemStack.Item && itemStack!=null)
                {
                    return i;
                }
            }
            return -1;
        }

        private ItemDefinition ReturnItem(int id)
        {
            return _gameItemList._gameItems[id];
        }
        private void SaveByXml(string name)
        {
            GameSave save = CreatSaveManager();
            XmlDocument xmlDocument = new XmlDocument();

            #region CreatXml elements
            XmlElement root = xmlDocument.CreateElement("Save"); // MRRKER <Save> elements </Save>
            root.SetAttribute("FileName", "File_01");

            XmlElement Inventory = xmlDocument.CreateElement("Inventory");
            root.AppendChild(Inventory);

            XmlElement InventoryInFormation = xmlDocument.CreateElement("InventoryInformation");
            Inventory.AppendChild(InventoryInFormation);

            XmlElement InventoryIndex, itemId, itemNumber;

            for (int i = 0; i <_size; i++)
            {
                if (_slots[i] != null)
                {
                    InventoryIndex = xmlDocument.CreateElement("InventoryIndex");
                    itemId = xmlDocument.CreateElement("GameItemId");
                    itemNumber = xmlDocument.CreateElement("GameItemNumber");

                    InventoryIndex.InnerText = save.InventoryIndex[i].ToString();
                    itemId.InnerText = save.GameItemId[i].ToString();
                    itemNumber.InnerText = save.GameItemNumber[i].ToString();

                    InventoryInFormation.AppendChild(InventoryIndex);
                    InventoryInFormation.AppendChild(itemId);
                    InventoryInFormation.AppendChild(itemNumber);
                }
            }
           
            #endregion
            xmlDocument.AppendChild(root);

            xmlDocument.Save(Application.dataPath + name);
            if (File.Exists(Application.dataPath + name))
            {
                Debug.Log(Application.dataPath + name);
            }
        }
        public void LoadButton()
        {
            WaitloadXml();
        }

        public void WaitloadXml()
        {
            LoadByXml(Application.dataPath + "InventoryXml.text");
        }
        private void LoadByXml(string Path)
    {
        if (File.Exists(Path))
        {
            for (int i = 0; i < _size; i++)
            {
                  if(_slots != null)
                     ClearSlot(i);
            }
            GameSave save = new GameSave();

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(Path);
            #region Load SaveXml
            // Get the Save File Data from the File

            for (int i = 0; i < _size; i++)
            {

                 XmlNodeList InventoryIndex = xmlDocument.GetElementsByTagName("InventoryIndex");
                 int inventoryIndex = int.Parse(InventoryIndex[i].InnerText);
                 XmlNodeList GameItemId = xmlDocument.GetElementsByTagName("GameItemId");
                 int gameItemId = int.Parse(GameItemId[i].InnerText);
                 XmlNodeList itemNumber = xmlDocument.GetElementsByTagName("GameItemNumber");
                 int numberOfItem = int.Parse(itemNumber[i].InnerText);
                 if (numberOfItem > 0)
                 {
                        _slots[inventoryIndex].State = new ItemStack(ReturnItem(gameItemId), numberOfItem);
                 }


            }
            #endregion
        }
        else
        {
            Debug.Log("No save Files");
        }
    }
    }
}

using UnityEngine;

namespace InventorySystem
{
    public class ItemCollectionHandler : MonoBehaviour
    {
        private Inventory _inventory;

        private void Awake()
        {
            _inventory = GetComponentInParent<Inventory>();
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.TryGetComponent<GameItem>(out var gameItem) || !_inventory.CanAcceptItem(gameItem.Stack)) return;

            _inventory.AddItem(gameItem.Pick());
        }
    }

}
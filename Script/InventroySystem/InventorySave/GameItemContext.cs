using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace InventorySystem
{
    [CreateAssetMenu(menuName = "GameItemList/GameItemList",fileName = "GameItemList")]
    public class GameItemContext : ScriptableObject
    {
        public ItemDefinition[] _gameItems;
    }
}
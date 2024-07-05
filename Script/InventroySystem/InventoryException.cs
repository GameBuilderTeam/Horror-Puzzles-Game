using System;


namespace InventroySystem
{
    public enum InventoryOperation
    {
        Add,
        Remove

    }
    public class InventoryException : Exception
    {
        public InventoryOperation Operation { get; }
        public InventoryException(InventoryOperation operation,string msg) : base($"{operation} Error:p {msg}") //Inventory is full
        {
            Operation = operation;
        }

    }
}

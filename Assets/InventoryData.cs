using System.Collections.Generic;

[System.Serializable]
public class InventoryData
{
    public List<ItemData> items; // Список предметов в инвентаре

    [System.Serializable]
    public class ItemData
    {
        public int itemID; // ID предмета
        public int itemValue; // Количество предметов
    }
}

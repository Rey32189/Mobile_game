using System.Collections.Generic;

[System.Serializable]
public class InventoryData
{
    public List<ItemData> items; // ������ ��������� � ���������

    [System.Serializable]
    public class ItemData
    {
        public int itemID; // ID ��������
        public int itemValue; // ���������� ���������
    }
}

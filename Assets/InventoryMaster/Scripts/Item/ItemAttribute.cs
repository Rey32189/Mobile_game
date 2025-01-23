using UnityEngine;
using System.Collections;

[System.Serializable]
public class ItemAttribute
{

    public string attributeName;
    public int attributeValue;
    public ItemAttribute(string attributeName, int attributeValue)
    {
        this.attributeName = attributeName;
        this.attributeValue = attributeValue;
    }

    public ItemAttribute() { }

    public ItemAttribute Clone()
    {
        return new ItemAttribute(attributeName, attributeValue); // Создаем новую копию с теми же значениями
    }
}


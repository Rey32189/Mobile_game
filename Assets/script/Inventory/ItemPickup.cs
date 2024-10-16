using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Класс для обработки взаимодействия с предметом
public class ItemPickup : MonoBehaviour
{
    public string itemName; // Имя предмета
    public Sprite itemIcon; // Иконка предмета
    public int quantity = 1; // Количество предметов (по умолчанию 1)

    private InventoryManager inventoryManager; // Ссылка на объект InventoryManager

    void Start()
    {
        // Находим InventoryManager в сцене
        inventoryManager = FindObjectOfType<InventoryManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        // Проверяем, что игрок соприкасается с предметом
        if (other.CompareTag("Player"))
        {
            // Создаем предмет с заданным именем, иконкой и количеством
            StackableItem item = new StackableItem(itemName, itemIcon, quantity);

            // Добавляем предмет в инвентарь
            if (inventoryManager != null)
            {
                // Проверяем, удастся ли добавить предмет в инвентарь
                bool added = inventoryManager.AddItem(item);

                // Удаляем предмет из сцены только если он был успешно добавлен в инвентарь
                if (added)
                {
                    Destroy(gameObject);
                }
                else
                {
                    Debug.Log("Инвентарь полон, предмет не был добавлен: " + itemName);
                }
            }
        }
    }
}

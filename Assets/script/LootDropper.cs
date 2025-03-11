using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootDropper : MonoBehaviour
{
    static ItemDataBaseList inventoryItemList; // Статическая переменная для хранения списка предметов из базы данных

    private void Start()
    {
        // Загружаем базу данных предметов из ресурсов и присваиваем ее переменной inventoryItemList
        inventoryItemList = (ItemDataBaseList)Resources.Load("ItemDatabase");
    }

    [System.Serializable]
    public class LootEntry
    {
        public int itemID; // ID предмета в базе данных
        public float dropChance; // Вероятность выпадения (от 0 до 1)
        public Vector3 forceRange; // Диапазон силы разлета (x, y, z)
        public int minDropAmount; // Минимальное количество предметов
        public int maxDropAmount; // Максимальное количество предметов
    }

    public LootEntry[] lootTable; // Таблица вероятностей
    public int maxDrops = 40; // Максимальное количество выпадающих предметов

    // Метод для генерации выпадения
    public void DropLoot()
    {
        // Создаем новый список возможных предметов, которые могут выпасть
        List<GameObject> possibleDrops = new List<GameObject>();
        Dictionary<int, int> dropCounts = new Dictionary<int, int>(); // Словарь для хранения количества выпавших предметов по ID

        // Создаем список возможных предметов, которые могут выпасть
        foreach (var loot in lootTable)
        {
            if (Random.value < loot.dropChance)
            {
                // Получаем предмет по itemID из базы данных
                Item itemData = inventoryItemList.itemList.Find(item => item.itemID == loot.itemID);
                if (itemData != null && itemData.itemModel != null)
                {
                    // Определяем количество предметов, которые будут выпадать
                    int dropAmount = Random.Range(loot.minDropAmount, loot.maxDropAmount + 1);
                    Debug.Log($"Item ID: {loot.itemID}, Drop Amount: {dropAmount}");
                    if (dropCounts.ContainsKey(loot.itemID))
                    {
                        dropCounts[loot.itemID] += dropAmount; // Увеличиваем количество для существующего предмета
                    }
                    else
                    {
                        dropCounts[loot.itemID] = dropAmount; // Устанавливаем количество для нового предмета
                    }
                }
            }
        }

        // Ограничиваем количество выпадающих предметов
        int numberOfDrops = Mathf.Min(dropCounts.Count, maxDrops);
        int dropsCreated = 0;

        foreach (var drop in dropCounts)
        {
            if (dropsCreated >= numberOfDrops) break;

            // Получаем предмет по itemID из базы данных
            Item itemData = inventoryItemList.itemList.Find(item => item.itemID == drop.Key);
            if (itemData != null && itemData.itemModel != null)
            {
                // Создаем предмет в мире
                GameObject item = Instantiate(itemData.itemModel, transform.position, Quaternion.identity);

                // Добавляем компонент PickUpItem
                PickUpItem pickUpItem = item.GetComponent<PickUpItem>();
                if (pickUpItem == null)
                {
                    pickUpItem = item.AddComponent<PickUpItem>();
                }

                // Используем глубокое клонирование itemData
                Item clonedItemData = itemData.getDeepCopy(); // Здесь вы используете getDeepCopy()
                clonedItemData.itemValue = drop.Value; // Устанавливаем количество предметов
                pickUpItem.item = clonedItemData; // Присваиваем клонированный объект в pickUpItem

                // Получаем компонент Rigidbody предмета
                Rigidbody rb = item.GetComponent<Rigidbody>();

                // Если Rigidbody существует, применяем силу
                if (rb != null)
                {
                    Vector3 force = new Vector3(
                        Random.Range(-lootTable[0].forceRange.x, lootTable[0].forceRange.x),
                        Random.Range(-lootTable[0].forceRange.y, lootTable[0].forceRange.y),
                        Random.Range(-lootTable[0].forceRange.z, lootTable[0].forceRange.z)
                    );

                    rb.AddForce(force, ForceMode.VelocityChange);
                }

                dropsCreated++;
            }
        }

    }
}
   

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootDropper : MonoBehaviour
{
    // скрипт для выпадения предметов
    [System.Serializable]
    public class LootEntry
    {
        public GameObject itemPrefab; // Префаб предмета
        public float dropChance;     // Вероятность выпадения (от 0 до 1)
        public Vector3 forceRange;   // Диапазон силы разлёта (x, y, z)
    }

    public LootEntry[] lootTable; // Таблица вероятностей
    public int maxDrops = 3;       // Максимальное количество выпадающих предметов

    // Метод для генерации выпадения
    public void DropLoot()
    {
        List<LootEntry> possibleDrops = new List<LootEntry>();

        // Создаем список возможных предметов, которые могут выпасть
        foreach (var loot in lootTable)
        {
            if (Random.value < loot.dropChance)
            {
                possibleDrops.Add(loot);
            }
        }

        // Ограничиваем количество выпадающих предметов
        int numberOfDrops = Mathf.Min(possibleDrops.Count, maxDrops);

        for (int i = 0; i < numberOfDrops; i++)
        {
            // Выбираем случайный предмет из списка возможных
            LootEntry selectedLoot = possibleDrops[Random.Range(0, possibleDrops.Count)];

            // Создаем предмет в мире
            GameObject item = Instantiate(selectedLoot.itemPrefab, transform.position, Quaternion.identity);

            // Получаем компонент Rigidbody предмета
            Rigidbody rb = item.GetComponent<Rigidbody>();

            // Если Rigidbody существует, применяем силу
            if (rb != null)
            {
                // Генерируем случайную силу в пределах указанного диапазона
                Vector3 force = new Vector3(
                    Random.Range(-selectedLoot.forceRange.x, selectedLoot.forceRange.x),
                    Random.Range(-selectedLoot.forceRange.y, selectedLoot.forceRange.y),
                    Random.Range(-selectedLoot.forceRange.z, selectedLoot.forceRange.z)
                );

                // Применяем силу к Rigidbody
                rb.AddForce(force, ForceMode.VelocityChange);
            }
        }
    }
}




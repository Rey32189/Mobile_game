using UnityEngine; // Подключаем пространство имен Unity для доступа к его функционалу.
using System.Collections; // Подключаем пространство имен для работы с коллекциями.
using System.Collections.Generic; // Подключаем пространство имен для работы с обобщенными коллекциями.

public class RandomChestLootSystem : MonoBehaviour // Определяем класс RandomChestLootSystem, который наследуется от MonoBehaviour.
{
    public int amountOfChest = 10; // Общее количество сундуков, которые будут созданы.

    public int minItemInChest = 2; // Минимальное количество предметов в каждом сундуке.
    public int maxItemInChest = 10; // Максимальное количество предметов в каждом сундуке.

    static ItemDataBaseList inventoryItemList; // Статическая переменная для хранения списка предметов из базы данных.

    public GameObject storageBox; // Префаб сундука, который будет создан в игре.

    int counter; // Счетчик для отслеживания количества созданных сундуков.
    int creatingItemsForChest = 0; // Счетчик для отслеживания количества предметов, создаваемых для текущего сундука.
    int randomItemNumber; // Переменная для хранения случайного номера предмета из базы данных.

    // Метод, вызываемый при инициализации объекта
    void Start()
    {
        // Загружаем базу данных предметов из ресурсов
        inventoryItemList = (ItemDataBaseList)Resources.Load("ItemDatabase");

        // Пока счетчик меньше общего количества сундуков
        while (counter < amountOfChest)
        {
            counter++; // Увеличиваем счетчик созданных сундуков.

            creatingItemsForChest = 0; // Сбрасываем счетчик предметов для нового сундука.

            // Определяем случайное количество предметов для текущего сундука
            int itemAmountForChest = Random.Range(minItemInChest, maxItemInChest);
            List<Item> itemsForChest = new List<Item>(); // Создаем новый список для хранения предметов для текущего сундука.

            // Пока количество создаваемых предметов меньше, чем определенное
            while (creatingItemsForChest < itemAmountForChest)
            {
                // Генерируем случайный индекс предмета из базы данных
                randomItemNumber = Random.Range(1, inventoryItemList.itemList.Count - 1);
                // Генерируем случайное число для определения шанса выпадения предмета
                int raffle = Random.Range(1, 100);

                // Если случайное число меньше или равно редкости предмета, добавляем его в сундук
                if (raffle <= inventoryItemList.itemList[randomItemNumber].rarity)
                {
                    itemsForChest.Add(inventoryItemList.itemList[randomItemNumber].getCopy()); // Добавляем копию предмета в список.
                    creatingItemsForChest++; // Увеличиваем счетчик созданных предметов.
                }
            }

            Terrain terrain = Terrain.activeTerrain; // Получаем активный террейн (ландшафт) в игре.

            // Генерируем случайные координаты для размещения сундука
            float x = Random.Range(5, terrain.terrainData.size.x - 5);
            float z = Random.Range(5, terrain.terrainData.size.z - 5);

            // Получаем высоту террейна в заданных координатах
            float height = terrain.terrainData.GetHeight((int)x, (int)z);

            // Создаем экземпляр сундука на сцене
            GameObject chest = (GameObject)Instantiate(storageBox);
            StorageInventory sI = chest.GetComponent<StorageInventory>(); // Получаем компонент StorageInventory из созданного сундука.
            sI.inventory = GameObject.FindGameObjectWithTag("Storage"); // Привязываем инвентарь к сундуку.

            // Наполняем сундук предметами
            for (int i = 0; i < itemsForChest.Count; i++)
            {
                // Добавляем предмет в инвентарь сундука по его ID
                sI.storageItems.Add(inventoryItemList.getItemByID(itemsForChest[i].itemID));

                // Генерируем случайное значение для предмета в пределах его максимального запаса
                int randomValue = Random.Range(1, sI.storageItems[sI.storageItems.Count - 1].maxStack);
                sI.storageItems[sI.storageItems.Count - 1].itemValue = randomValue; // Устанавливаем случайное значение для предмета.
            }

            // Устанавливаем позицию сундука на сцене
            chest.transform.localPosition = new Vector3(x, height + 2, z); // Устанавливаем позицию сундука с учетом высоты террейна и небольшого смещения по оси Y.
        }
    }
}

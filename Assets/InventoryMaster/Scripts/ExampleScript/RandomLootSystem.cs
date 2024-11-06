using UnityEngine; 
using System.Collections; 

public class RandomLootSystem : MonoBehaviour 
{
    public int amountOfLoot = 10; // Объявление переменной, определяющей количество лута, которое будет сгенерировано
    static ItemDataBaseList inventoryItemList; // Статическая переменная для хранения списка предметов из базы данных

    int counter = 0; // Переменная-счетчик для отслеживания количества сгенерированного лута

    void Start()
    {
        // Загружаем базу данных предметов из ресурсов и присваиваем ее переменной inventoryItemList
        inventoryItemList = (ItemDataBaseList)Resources.Load("ItemDatabase");

        // Цикл, который продолжается, пока количество сгенерированного лута меньше заданного
        while (counter < amountOfLoot)
        {
            counter++; // Увеличиваем счетчик на 1

            // Генерация случайного числа в диапазоне от 1 до количества предметов в списке (исключая последний элемент)
            int randomNumber = Random.Range(1, inventoryItemList.itemList.Count - 1);

            // Получение активного террейна в сцене
            Terrain terrain = Terrain.activeTerrain;

            // Генерация случайных координат X и Z для размещения предмета на террейне
            float x = Random.Range(5, terrain.terrainData.size.x - 5);
            float z = Random.Range(5, terrain.terrainData.size.z - 5);

            // Проверка, есть ли модель предмета для случайно выбранного элемента
            if (inventoryItemList.itemList[randomNumber].itemModel == null)
                counter--; // Если модели нет, уменьшаем счетчик, чтобы не превышать заданное количество лута
            else
            {
                // Создание экземпляра случайного предмета из базы данных
                GameObject randomLootItem = (GameObject)Instantiate(inventoryItemList.itemList[randomNumber].itemModel);

                // Добавление компонента PickUpItem к созданному предмету
                PickUpItem item = randomLootItem.AddComponent<PickUpItem>();
                item.item = inventoryItemList.itemList[randomNumber]; // Присваивание предмета компоненту PickUpItem

                // Установка позиции созданного предмета на террейне
                randomLootItem.transform.localPosition = new Vector3(x, 0, z);
            }
        }
    }
}


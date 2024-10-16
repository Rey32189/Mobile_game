using System;
using System.Collections;
using System.Collections.Generic; // Для работы со списками
using Unity.VisualScripting;
using UnityEngine; // Основное пространство имён Unity
using UnityEngine.UI; // Для работы с UI элементами
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public GameObject itemPrefab; // Префаб для отображения предметов в инвентаре
    public Transform inventoryPanel; // Панель инвентаря для размещения элементов
    public Transform playerTransform; // Позиция игрока или персонажа
    public List<ItemPrefabEntry> itemPrefabsList;  // Словарь для хранения префабов предметов
    public int maxSlots = 5; // Максимальное количество ячеек в инвентаре

    private Dictionary<string, GameObject> itemWorldPrefabs = new Dictionary<string, GameObject>(); // Словарь для хранения префабов для разных типов предметов
    private List<StackableItem> items = new List<StackableItem>(); // Список для хранения предметов
    void Start()
    {
        // Инициализируйте словарь префабов из списка
        itemWorldPrefabs.Clear();
        foreach (var entry in itemPrefabsList)
        {
            itemWorldPrefabs[entry.itemName] = entry.prefab;
        }

        DisplayItems(); // Отображаем инвентарь (изначально пустой)
    }

    // Метод для добавления предмета в инвентарь
    public bool AddItem(StackableItem newItem)
    {
        // Переменная для отслеживания количества добавленных предметов
        int totalAdded = 0;

        // Поиск предмета в списке
        List<StackableItem> existingStacks = items.FindAll(item => item.itemName == newItem.itemName);

        // Добавляем предметы в существующие стопки, пока не закончатся предметы или стопки
        foreach (var stack in existingStacks)
        {
            if (newItem.quantity == 0) break; // Если предметы закончились, выходим из цикла

            // Сколько предметов можно добавить в эту стопку
            int quantityToAdd = Mathf.Min(newItem.quantity, 3 - stack.quantity);

            // Проверка, чтобы не превышать максимальное количество в ячейке
            if (quantityToAdd > 0)
            {
                stack.quantity += quantityToAdd;
                newItem.quantity -= quantityToAdd;
                totalAdded += quantityToAdd; // Увеличиваем общее количество добавленных предметов
            }
        }

        // Если остались предметы, создаем новые стопки
        while (newItem.quantity > 0 && items.Count < maxSlots)
        {
            int quantityToAdd = Mathf.Min(newItem.quantity, 3); // Максимум 3 предмета в одной ячейке

            // Создаем новую стопку только если есть возможность добавить предметы
            if (quantityToAdd > 0)
            {
                items.Add(new StackableItem(newItem.itemName, newItem.itemIcon, quantityToAdd));
                newItem.quantity -= quantityToAdd;
                totalAdded += quantityToAdd; // Увеличиваем общее количество добавленных предметов
            }
        }

        if (totalAdded == 0)
        {
            return false; // Не удалось добавить ни одного предмета
        }
        DisplayItems(); // Обновляем отображение инвентаря
        return true; // Предметы были успешно добавлены
    }

    // Метод для удаления предмета из инвентаря
    public void RemoveItem(StackableItem itemToRemove, bool destroyImmediately = false)
    {
        if (itemToRemove != null)
        {
            itemToRemove.quantity -= 1;  // Уменьшение количества существующего предмета

            if (itemToRemove.quantity <= 0)
            {
                items.Remove(itemToRemove);// Удаление предмета, если его количество стало ноль или меньше
            }

            if (destroyImmediately)
            {
                DestroyItemInWorld(itemToRemove); // Удаление предмета из мира
            }
        }

        DisplayItems(); // Обновляем отображение инвентаря
    }

    //Метод для создания предмета в мире
    private void DestroyItemInWorld(StackableItem item)
    {
        if (playerTransform != null)
        {
            // Добавляем случайное смещение на расстоянии от 1 до 2 метров
            Vector3 offset = new Vector3(UnityEngine.Random.Range(1f, 2f), 0f, UnityEngine.Random.Range(1f, 2f));

            // Выбираем префаб для создания предмета в мире в зависимости от имени предмета
            if (itemWorldPrefabs.TryGetValue(item.itemName, out GameObject itemWorldPrefab))
            {
                // Создаем предмет в мире на смещенной позиции
                GameObject itemWorldObject = Instantiate(itemWorldPrefab, playerTransform.position + offset, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("No prefab assigned for item: " + item.itemName);
            }
        }
        else
        {
            Debug.LogWarning("Player Transform is not assigned!");
        }
    }

    // Метод для отображения предметов в UI
    void DisplayItems()
    {
        // Удаляем старые элементы из панели
        foreach (Transform child in inventoryPanel)
        {
            Destroy(child.gameObject); // Удаляем старый элемент из панели
        }

        // Создаем новые элементы для каждого предмета
        foreach (var item in items)
        {
            // Создаем новый элемент UI для предмета
            GameObject itemObject = Instantiate(itemPrefab, inventoryPanel);

            // Настраиваем отображение иконки предмета
            Image itemImage = itemObject.GetComponentInChildren<Image>();
            if (itemImage != null)
            {
                itemImage.sprite = item.itemIcon; // Устанавливаем иконку предмета
            }

            // Настраиваем отображение количества предметов
            Text itemText = itemObject.GetComponentInChildren<Text>();
            if (itemText != null)
            {
                itemText.text = $"{item.itemName} ({item.quantity})"; // Устанавливаем текст как имя предмета и количество
            }

            // Добавляем кнопку для выбрасывания предмета в мир

            Dropdown dropdownMenu = itemObject.GetComponentInChildren<Dropdown>(true);
            if (dropdownMenu != null)
            {
                dropdownMenu.onValueChanged.AddListener(delegate
                {
                    DropdownValueChanged(dropdownMenu, item);
                });
            }
        }
    }

    // Новый метод для панели применения предметов
    void DropdownValueChanged(Dropdown change, StackableItem item)
    {
        switch (change.value)
        {
            //case 0:
            //    // Логика для отображения информации о предмете
            //    break;
            case 0:
                // Логика для отображения информации о предмете
                break;
            case 1:
                // Логика для применения предмета
                break;
            case 2:
                // Логика для выбрасывания предмета в мир
                RemoveItem(item, true);
                break;
            default:
                break;
        }
    }
}

[System.Serializable]
public class ItemPrefabEntry
{
    public string itemName;
    public GameObject prefab;
}
// Класс для хранения предметов, которые можно складывать в стопки
[System.Serializable]
public class StackableItem
{
    public string itemName; // Имя предмета
    public Sprite itemIcon; // Иконка предмета
    public int quantity; // Количество предметов в стопке

    // Конструктор для создания стопки предметов
    public StackableItem(string name, Sprite icon, int quantity)
    {
        this.itemName = name; // Устанавливаем имя предмета
        this.itemIcon = icon; // Устанавливаем иконку предмета
        this.quantity = quantity; // Устанавливаем количество предметов
    }
}

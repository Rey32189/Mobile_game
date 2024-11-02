using UnityEngine;                                         // Подключаем пространство имен Unity для работы с игровыми объектами
using System.Collections;                                  // Подключаем пространство имен для работы с коллекциями
using UnityEngine.UI;                                     // Подключаем пространство имен для работы с элементами пользовательского интерфейса
using UnityEngine.EventSystems;                           // Подключаем пространство имен для работы с событиями интерфейса

public class SplitItem : MonoBehaviour, IPointerDownHandler // Определяем класс SplitItem, наследующий от MonoBehaviour и реализующий интерфейс IPointerDownHandler
{                                                         // Начало класса SplitItem
    private bool pressingButtonToSplit;                    // Булева переменная для отслеживания нажатия кнопки для разделения предмета
    public Inventory inv;                                   // Ссылка на скрипт инвентаря
    static InputManager inputManagerDatabase = null;       // Статическая переменная для хранения ссылки на менеджер ввода

    void Update()                                          // Метод, который вызывается каждый кадр
    {
        if (Input.GetKeyDown(inputManagerDatabase.SplitItem)) // Если нажата клавиша для разделения предмета
            pressingButtonToSplit = true;                   // Устанавливаем переменную pressingButtonToSplit в true
        if (Input.GetKeyUp(inputManagerDatabase.SplitItem)) // Если клавиша для разделения предмета отпущена
            pressingButtonToSplit = false;                  // Устанавливаем переменную pressingButtonToSplit в false
    }

    void Start()                                           // Метод, который вызывается при инициализации объекта
    {
        inputManagerDatabase = (InputManager)Resources.Load("InputManager"); // Загружаем ресурс InputManager из папки Resources
    }

    public void OnPointerDown(PointerEventData data)       // Метод, который вызывается при нажатии кнопки мыши
    {
        inv = transform.parent.parent.parent.GetComponent<Inventory>(); // Получаем компонент Inventory из родительского объекта
        // Проверяем, что объект не является горячей панелью, нажата левая кнопка мыши, кнопка для разделения нажата, предметы в инвентаре могут быть сложены и в инвентаре достаточно места
        if (transform.parent.parent.parent.GetComponent<Hotbar>() == null && data.button == PointerEventData.InputButton.Left && pressingButtonToSplit && inv.stackable && (inv.ItemsInInventory.Count < (inv.height * inv.width)))
        {
            ItemOnObject itemOnObject = GetComponent<ItemOnObject>(); // Получаем компонент ItemOnObject у текущего объекта

            // Проверяем, что количество предметов больше 1, чтобы можно было разделить
            if (itemOnObject.item.itemValue > 1)
            {
                int splitPart = itemOnObject.item.itemValue;         // Сохраняем текущее количество предметов в переменной splitPart
                itemOnObject.item.itemValue = (int)itemOnObject.item.itemValue / 2; // Делим количество предметов пополам
                splitPart = splitPart - itemOnObject.item.itemValue; // Вычисляем количество разделенной части

                // Добавляем новый предмет в инвентарь
                inv.addItemToInventory(itemOnObject.item.itemID, splitPart);
                inv.stackableSettings(); // Обновляем настройки для складываемых предметов

                // Проверяем, есть ли дубликат предмета
                if (GetComponent<ConsumeItem>().duplication != null)
                {
                    GameObject dup = GetComponent<ConsumeItem>().duplication; // Получаем дубликат предмета
                    dup.GetComponent<ItemOnObject>().item.itemValue = itemOnObject.item.itemValue; // Устанавливаем значение предмета дубликата
                    dup.GetComponent<SplitItem>().inv.stackableSettings(); // Обновляем настройки дубликата
                }
                inv.updateItemList(); // Обновляем список предметов в инвентаре
            }
        }
    }
}

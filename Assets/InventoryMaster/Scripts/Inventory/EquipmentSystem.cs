using System; 
using UnityEngine; 
using System.Collections; 
using System.Collections.Generic; 
#if UNITY_EDITOR 
using UnityEditor; 
#endif 
using UnityEngine.UI; 
using UnityEngine.EventSystems;

public class EquipmentSystem : MonoBehaviour 
{
    [SerializeField] 
    public int slotsInTotal; // Публичное целочисленное поле для хранения общего количества слотов.

    [SerializeField] 
    public ItemType[] itemTypeOfSlots = new ItemType[999]; // Массив типа ItemType, который будет хранить типы предметов для каждого слота, инициализированный с 999 элементами.

    void Start() 
    {
        ConsumeItem.eS = GetComponent<EquipmentSystem>(); // Присваивает статическому полю eS класса ConsumeItem ссылку на текущий компонент EquipmentSystem.
    }

    public void getSlotsInTotal() // Публичный метод для вычисления общего количества слотов.
    {
        Inventory inv = GetComponent<Inventory>(); // Получает компонент Inventory, связанный с тем же объектом.
        slotsInTotal = inv.width * inv.height; // Вычисляет общее количество слотов, умножая ширину на высоту инвентаря.
    }

#if UNITY_EDITOR // Начало условной компиляции для кода, который будет выполняться только в редакторе.
    [MenuItem("Master System/Create/Equipment")] // Создает пункт меню в редакторе Unity для вызова метода.
    public static void menuItemCreateInventory() // Статический метод для создания инвентаря при выборе пункта меню.
    {
        GameObject Canvas = null; // Объявляет переменную Canvas для хранения ссылки на объект канваса.
        if (GameObject.FindGameObjectWithTag("Canvas") == null) // Проверяет, существует ли объект с тегом "Canvas".
        {
            GameObject inventory = new GameObject(); // Создает новый объект GameObject для инвентаря.
            inventory.name = "Inventories"; // Задает имя созданному объекту "Inventories".
            Canvas = (GameObject)Instantiate(Resources.Load("Prefabs/Canvas - Inventory") as GameObject); // Загружает префаб канваса из ресурсов и создает его экземпляр.
            Canvas.transform.SetParent(inventory.transform, true); // Устанавливает родителем канваса объект inventory.
            GameObject panel = (GameObject)Instantiate(Resources.Load("Prefabs/Panel - EquipmentSystem") as GameObject); // Загружает и создает панель системы оборудования.
            panel.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0); // Устанавливает локальную позицию панели в (0, 0, 0).
            panel.transform.SetParent(Canvas.transform, true); // Устанавливает родителем панели объект Canvas.
            GameObject draggingItem = (GameObject)Instantiate(Resources.Load("Prefabs/DraggingItem") as GameObject); // Загружает и создает объект для перетаскиваемого элемента.
            draggingItem.transform.SetParent(Canvas.transform, true); // Устанавливает родителем перетаскиваемого элемента объект Canvas.
            Instantiate(Resources.Load("Prefabs/EventSystem") as GameObject); // Загружает и создает объект EventSystem для обработки событий.
            Inventory inv = panel.AddComponent<Inventory>(); // Добавляет компонент Inventory к панели.
            panel.AddComponent<InventoryDesign>(); // Добавляет компонент InventoryDesign к панели.
            panel.AddComponent<EquipmentSystem>(); // Добавляет компонент EquipmentSystem к панели.
            inv.getPrefabs(); // Вызывает метод getPrefabs у инвентаря.
        }
        else // Если объект с тегом "Canvas" уже существует.
        {
            GameObject panel = (GameObject)Instantiate(Resources.Load("Prefabs/Panel - EquipmentSystem") as GameObject); // Загружает и создает панель системы оборудования.
            panel.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, true); // Устанавливает родителем панели объект с тегом "Canvas".
            panel.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0); // Устанавливает локальную позицию панели в (0, 0, 0).
            Inventory inv = panel.AddComponent<Inventory>(); // Добавляет компонент Inventory к панели.
            panel.AddComponent<EquipmentSystem>(); // Добавляет компонент EquipmentSystem к панели.
            DestroyImmediate(GameObject.FindGameObjectWithTag("DraggingItem")); // Удаляет объект с тегом "DraggingItem" немедленно.
            GameObject draggingItem = (GameObject)Instantiate(Resources.Load("Prefabs/DraggingItem") as GameObject); // Загружает и создает объект для перетаскиваемого элемента.
            panel.AddComponent<InventoryDesign>(); // Добавляет компонент InventoryDesign к панели.
            draggingItem.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, true); // Устанавливает родителем перетаскиваемого элемента объект с тегом "Canvas".
            inv.getPrefabs(); // Вызывает метод getPrefabs у инвентаря для инициализации префабов.
        }
    }
#endif

}


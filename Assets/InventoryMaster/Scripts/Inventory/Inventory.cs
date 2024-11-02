using UnityEngine; // Подключаем пространство имен Unity для работы с игровыми объектами и компонентами
using System.Collections; // Подключаем пространство имен для работы с коллекциями
#if UNITY_EDITOR // Условная компиляция для редактора Unity
using UnityEditor; // Подключаем пространство имен для работы с редактором Unity
#endif
using UnityEngine.UI; // Подключаем пространство имен для работы с пользовательским интерфейсом
using System.Collections.Generic; // Подключаем пространство имен для работы с обобщенными коллекциями


using System.IO;
using static UnityEditor.Progress;

public class Inventory : MonoBehaviour // Определяем класс Inventory, который наследует от MonoBehaviour
{
    // Prefabs
    [SerializeField] // Позволяет редактировать поле в инспекторе Unity
    private GameObject prefabCanvasWithPanel; // Префаб для канваса с панелью
    [SerializeField]
    private GameObject prefabSlot; // Префаб для слота инвентаря
    [SerializeField]
    private GameObject prefabSlotContainer; // Префаб контейнера для слотов
    [SerializeField]
    private GameObject prefabItem; // Префаб для предмета
    [SerializeField]
    private GameObject prefabDraggingItemContainer; // Префаб контейнера для перетаскиваемого предмета
    [SerializeField]
    private GameObject prefabPanel; // Префаб панели инвентаря

    // Item database
    [SerializeField]
    private ItemDataBaseList itemDatabase; // Ссылка на базу данных предметов

    // GameObjects which are alive
    [SerializeField]
    private string inventoryTitle; // Заголовок инвентаря
    [SerializeField]
    private RectTransform PanelRectTransform; // Прямоугольный трансформ панели инвентаря
    [SerializeField]
    private Image PanelImage; // Изображение панели инвентаря
    [SerializeField]
    private GameObject SlotContainer; // Контейнер для слотов
    [SerializeField]
    private GameObject DraggingItemContainer; // Контейнер для перетаскиваемого предмета
    [SerializeField]
    private RectTransform SlotContainerRectTransform; // Прямоугольный трансформ контейнера слотов
    [SerializeField]
    private GridLayoutGroup SlotGridLayout; // Группа сетки для организации слотов
    [SerializeField]
    private RectTransform SlotGridRectTransform; // Прямоугольный трансформ сетки слотов

    // Inventory Settings
    [SerializeField]
    public bool mainInventory; // Флаг, указывающий, является ли инвентарь основным
    [SerializeField]
    public List<Item> ItemsInInventory = new List<Item>(); // Список предметов в инвентаре
    [SerializeField]
    public int height; // Высота инвентаря
    [SerializeField]
    public int width; // Ширина инвентаря
    [SerializeField]
    public bool stackable; // Флаг, указывающий, может ли инвентарь складывать предметы
    [SerializeField]
    public static bool inventoryOpen; // Статический флаг, указывающий, открыт ли инвентарь

    // GUI Settings
    [SerializeField]
    public int slotSize; // Размер слота инвентаря
    [SerializeField]
    public int iconSize; // Размер иконки предмета
    [SerializeField]
    public int paddingBetweenX; // Отступ между слотами по оси X
    [SerializeField]
    public int paddingBetweenY; // Отступ между слотами по оси Y
    [SerializeField]
    public int paddingLeft; // Левый отступ
    [SerializeField]
    public int paddingRight; // Правый отступ
    [SerializeField]
    public int paddingBottom; // Нижний отступ
    [SerializeField]
    public int paddingTop; // Верхний отступ
    [SerializeField]
    public int positionNumberX; // Позиция по оси X
    [SerializeField]
    public int positionNumberY; // Позиция по оси Y

    InputManager inputManagerDatabase; // Переменная для хранения ссылки на менеджер ввода

    // Event delegates for consuming, gearing
    public delegate void ItemDelegate(Item item); // Делегат для обработки событий с предметами
    public static event ItemDelegate ItemConsumed; // Событие, вызываемое при потреблении предмета
    public static event ItemDelegate ItemEquip; // Событие, вызываемое при экипировке предмета
    public static event ItemDelegate UnEquipItem; // Событие, вызываемое при разэкипировке предмета

    public delegate void InventoryOpened(); // Делегат для обработки событий открытия инвентаря
    public static event InventoryOpened InventoryOpen; // Событие, вызываемое при открытии инвентаря
    public static event InventoryOpened AllInventoriesClosed; // Событие, вызываемое при закрытии всех инвентарей

    public static Inventory Instance { get; private set; } // Статическое свойство для доступа к экземпляру инвентаря

    void Start() // Метод, вызываемый при старте игры
    {
      

        if (transform.GetComponent<Hotbar>() == null) // Проверяем, есть ли компонент Hotbar
            this.gameObject.SetActive(false); // Если нет, отключаем объект инвентаря

        updateItemList(); // Обновляем список предметов в инвентаре

        inputManagerDatabase = (InputManager)Resources.Load("InputManager"); // Загружаем менеджер ввода из ресурсов
    }

    public void sortItems() // Метод для сортировки предметов в инвентаре
    {
        int empty = -1; // Переменная для хранения индекса пустого слота
        for (int i = 0; i < SlotContainer.transform.childCount; i++) // Проходим по всем слотам
        {
            if (SlotContainer.transform.GetChild(i).childCount == 0 && empty == -1) // Если слот пустой и еще не найден пустой индекс
                empty = i; // Сохраняем индекс пустого слота
            else
            {
                if (empty > -1) // Если найден пустой слот
                {
                    if (SlotContainer.transform.GetChild(i).childCount != 0) // Если текущий слот не пустой
                    {
                        RectTransform rect = SlotContainer.transform.GetChild(i).GetChild(0).GetComponent<RectTransform>(); // Получаем RectTransform предмета
                        SlotContainer.transform.GetChild(i).GetChild(0).transform.SetParent(SlotContainer.transform.GetChild(empty).transform); // Перемещаем предмет в пустой слот
                        rect.localPosition = Vector3.zero; // Устанавливаем локальную позицию предмета в ноль
                        i = empty + 1; // Продолжаем с следующего слота
                        empty = i; // Обновляем индекс пустого слота
                    }
                }
            }
        }
    }

    void Update() // Метод, вызываемый каждый кадр
    {
        updateItemIndex(); // Обновляем индексы предметов в инвентаре
    }

    public void setAsMain() // Метод для установки инвентаря как основного
    {
        if (mainInventory) // Если это основной инвентарь
            this.gameObject.tag = "Untagged"; // Убираем тег
        else if (!mainInventory) // Если это не основной инвентарь
            this.gameObject.tag = "MainInventory"; // Устанавливаем тег основного инвентаря
    }

    public void OnUpdateItemList() // Метод для обновления списка предметов
    {
        updateItemList(); // Вызываем метод обновления списка предметов
    }

    public void closeInventory() // Метод для закрытия инвентаря
    {
        this.gameObject.SetActive(false); // Отключаем объект инвентаря
        checkIfAllInventoryClosed(); // Проверяем, закрыты ли все инвентарные окна
    }

    public void openInventory() // Метод для открытия инвентаря
    {
        this.gameObject.SetActive(true); // Включаем объект инвентаря
        if (InventoryOpen != null) // Если событие открытия инвентаря подписано
            InventoryOpen(); // Вызываем событие
    }

    public void checkIfAllInventoryClosed() // Метод для проверки, закрыты ли все инвентарные окна
    {
        GameObject canvas = GameObject.FindGameObjectWithTag("Canvas"); // Находим объект с тегом "Canvas"

        for (int i = 0; i < canvas.transform.childCount; i++) // Проходим по всем дочерним объектам канваса
        {
            GameObject child = canvas.transform.GetChild(i).gameObject; // Получаем дочерний объект по индексу
            if (!child.activeSelf && // Если дочерний объект не активен
                (child.tag == "EquipmentSystem" || child.tag == "Panel" || child.tag == "MainInventory" || child.tag == "CraftSystem")) // И его тег соответствует одному из указанных
            {
                if (AllInventoriesClosed != null && i == canvas.transform.childCount - 1) // Если событие закрытия инвентарей подписано и это последний объект
                    AllInventoriesClosed(); // Вызываем событие закрытия всех инвентарей
            }
            else if (child.activeSelf && // Если дочерний объект активен
                     (child.tag == "EquipmentSystem" || child.tag == "Panel" || child.tag == "MainInventory" || child.tag == "CraftSystem")) // И его тег соответствует одному из указанных
                break; // Выходим из цикла, так как найден активный инвентарь

            else if (i == canvas.transform.childCount - 1) // Если достигли последнего дочернего объекта
            {
                if (AllInventoriesClosed != null) // Если событие закрытия инвентарей подписано
                    AllInventoriesClosed(); // Вызываем событие закрытия всех инвентарей
            }
        }
    }

    public void ConsumeItem(Item item) // Метод для потребления предмета
    {
        if (ItemConsumed != null) // Если событие потребления предмета подписано
            ItemConsumed(item); // Вызываем событие, передавая предмет
    }

    public void EquiptItem(Item item) // Метод для экипировки предмета
    {
        if (ItemEquip != null) // Если событие экипировки предмета подписано
            ItemEquip(item); // Вызываем событие, передавая предмет
    }

    public void UnEquipItem1(Item item) // Метод для разэкипировки предмета
    {
        if (UnEquipItem != null) // Если событие разэкипировки предмета подписано
            UnEquipItem(item); // Вызываем событие, передавая предмет
    }

#if UNITY_EDITOR // Условная компиляция для редактора Unity
    [MenuItem("Master System/Create/Inventory and Storage")] // Создаем пункт меню для создания инвентаря и хранилища
    public static void menuItemCreateInventory() // Метод для создания инвентаря
    {
        GameObject Canvas = null; // Переменная для хранения ссылки на канвас
        if (GameObject.FindGameObjectWithTag("Canvas") == null) // Если канвас не найден
        {
            GameObject inventory = new GameObject(); // Создаем новый объект для инвентаря
            inventory.name = "Inventories"; // Устанавливаем имя объекта
            Canvas = (GameObject)Instantiate(Resources.Load("Prefabs/Canvas - Inventory") as GameObject); // Загружаем и создаем канвас инвентаря
            Canvas.transform.SetParent(inventory.transform, true); // Устанавливаем канвас как дочерний объект инвентаря
            GameObject panel = (GameObject)Instantiate(Resources.Load("Prefabs/Panel - Inventory") as GameObject); // Загружаем и создаем панель инвентаря
            panel.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0); // Устанавливаем позицию панели в (0, 0, 0)
            panel.transform.SetParent(Canvas.transform, true); // Устанавливаем панель как дочерний объект канваса
            GameObject draggingItem = (GameObject)Instantiate(Resources.Load("Prefabs/DraggingItem") as GameObject); // Загружаем и создаем объект для перетаскиваемого предмета
            draggingItem.transform.SetParent(Canvas.transform, true); // Устанавливаем его как дочерний объект канваса
            Inventory temp = panel.AddComponent<Inventory>(); // Добавляем компонент Inventory к панели
            Instantiate(Resources.Load("Prefabs/EventSystem") as GameObject); // Создаем объект EventSystem
            panel.AddComponent<InventoryDesign>(); // Добавляем компонент InventoryDesign к панели
            temp.getPrefabs(); // Вызываем метод для получения префабов
        }
        else // Если канвас уже существует
        {
            GameObject panel = (GameObject)Instantiate(Resources.Load("Prefabs/Panel - Inventory") as GameObject); // Загружаем и создаем панель инвентаря
            panel.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, true); // Устанавливаем панель как дочерний объект существующего канваса
            panel.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0); // Устанавливаем позицию панели в (0, 0, 0)
            Inventory temp = panel.AddComponent<Inventory>(); // Добавляем компонент Inventory к панели
            panel.AddComponent<InventoryDesign>(); // Добавляем компонент InventoryDesign к панели
            DestroyImmediate(GameObject.FindGameObjectWithTag("DraggingItem")); // Уничтожаем существующий объект перетаскиваемого предмета
            GameObject draggingItem = (GameObject)Instantiate(Resources.Load("Prefabs/DraggingItem") as GameObject); // Загружаем и создаем новый объект для перетаскиваемого предмета
            draggingItem.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, true); // Устанавливаем его как дочерний объект канваса
            temp.getPrefabs(); // Вызываем метод для получения префабов
        }
    }
#endif

    public void setImportantVariables() // Метод для установки важных переменных
    {
        PanelRectTransform = GetComponent<RectTransform>(); // Получаем RectTransform панели
        SlotContainer = transform.GetChild(1).gameObject; // Получаем контейнер слотов
        SlotGridLayout = SlotContainer.GetComponent<GridLayoutGroup>(); // Получаем компонент GridLayoutGroup контейнера слотов
        SlotGridRectTransform = SlotContainer.GetComponent<RectTransform>(); // Получаем RectTransform контейнера слотов
    }

    public void getPrefabs() // Метод для получения префабов
    {
        if (prefabCanvasWithPanel == null) // Если префаб канваса с панелью не загружен
            prefabCanvasWithPanel = Resources.Load("Prefabs/Canvas - Inventory") as GameObject; // Загружаем префаб канваса с панелью
        if (prefabSlot == null) // Если префаб слота не загружен
            prefabSlot = Resources.Load("Prefabs/Slot - Inventory") as GameObject; // Загружаем префаб слота
        if (prefabSlotContainer == null) // Если префаб контейнера слотов не загружен
            prefabSlotContainer = Resources.Load("Prefabs/Slots - Inventory") as GameObject; // Загружаем префаб контейнера слотов
        if (prefabItem == null) // Если префаб предмета не загружен
            prefabItem = Resources.Load("Prefabs/Item") as GameObject; // Загружаем префаб предмета
        if (itemDatabase == null) // Если база данных предметов не загружена
            itemDatabase = (ItemDataBaseList)Resources.Load("ItemDatabase"); // Загружаем базу данных предметов
        if (prefabDraggingItemContainer == null) // Если префаб контейнера для перетаскиваемого предмета не загружен
            prefabDraggingItemContainer = Resources.Load("Prefabs/DraggingItem") as GameObject; // Загружаем префаб контейнера для перетаскиваемого предмета
        if (prefabPanel == null) // Если префаб панели инвентаря не загружен
            prefabPanel = Resources.Load("Prefabs/Panel - Inventory") as GameObject; // Загружаем префаб панели инвентаря

        setImportantVariables(); // Вызываем метод для установки важных переменных
        setDefaultSettings(); // Вызываем метод для установки настроек по умолчанию
        adjustInventorySize(); // Вызываем метод для настройки размеров инвентаря
        updateSlotAmount(width, height); // Обновляем количество слотов в зависимости от ширины и высоты
        updateSlotSize(); // Обновляем размеры слотов
        updatePadding(paddingBetweenX, paddingBetweenY); // Обновляем отступы между слотами
    }


    public void updateItemList() // Метод для обновления списка предметов в инвентаре
    {
        ItemsInInventory.Clear(); // Очищаем текущий список предметов
        for (int i = 0; i < SlotContainer.transform.childCount; i++) // Проходим по всем дочерним объектам контейнера слотов
        {
            Transform trans = SlotContainer.transform.GetChild(i); // Получаем дочерний объект
            if (trans.childCount != 0) // Если в слоте есть предмет
            {
                // Добавляем предмет из слота в список предметов инвентаря
                ItemsInInventory.Add(trans.GetChild(0).GetComponent<ItemOnObject>().item);
            }
        }
    }

    public bool characterSystem() // Метод для проверки наличия системы персонажа
    {
        // Если компонент EquipmentSystem присутствует, возвращаем true
        return GetComponent<EquipmentSystem>() != null;
    }

    public void setDefaultSettings() // Метод для установки настроек по умолчанию
    {
        // Если ни один из указанных компонентов не найден
        if (GetComponent<EquipmentSystem>() == null && GetComponent<Hotbar>() == null && GetComponent<CraftSystem>() == null)
        {
            // Устанавливаем параметры для стандартного инвентаря
            height = 5;
            width = 5;
            slotSize = 50;
            iconSize = 45;
            paddingBetweenX = 5;
            paddingBetweenY = 5;
            paddingTop = 35;
            paddingBottom = 10;
            paddingLeft = 10;
            paddingRight = 10;
        }
        else if (GetComponent<Hotbar>() != null) // Если присутствует Hotbar
        {
            // Устанавливаем параметры для горячей панели
            height = 1;
            width = 9;
            slotSize = 50;
            iconSize = 45;
            paddingBetweenX = 5;
            paddingBetweenY = 5;
            paddingTop = 10;
            paddingBottom = 10;
            paddingLeft = 10;
            paddingRight = 10;
        }
        else if (GetComponent<CraftSystem>() != null) // Если присутствует CraftSystem
        {
            // Устанавливаем параметры для системы крафта
            height = 3;
            width = 3;
            slotSize = 55;
            iconSize = 45;
            paddingBetweenX = 5;
            paddingBetweenY = 5;
            paddingTop = 35;
            paddingBottom = 95;
            paddingLeft = 25;
            paddingRight = 25;
        }
        else // Если присутствует другой тип системы
        {
            // Устанавливаем параметры по умолчанию
            height = 4;
            width = 2;
            slotSize = 50;
            iconSize = 45;
            paddingBetweenX = 100;
            paddingBetweenY = 20;
            paddingTop = 35;
            paddingBottom = 10;
            paddingLeft = 10;
            paddingRight = 10;
        }
    }

    public void adjustInventorySize() // Метод для настройки размера инвентаря
    {
        // Вычисляем ширину и высоту панели инвентаря
        int x = (((width * slotSize) + ((width - 1) * paddingBetweenX)) + paddingLeft + paddingRight);
        int y = (((height * slotSize) + ((height - 1) * paddingBetweenY)) + paddingTop + paddingBottom);
        PanelRectTransform.sizeDelta = new Vector2(x, y); // Устанавливаем размер панели
        SlotGridRectTransform.sizeDelta = new Vector2(x, y); // Устанавливаем размер контейнера слотов
    }

    public void updateSlotAmount(int width, int height) // Метод для обновления количества слотов
    {
        // Загружаем префаб слота, если он еще не загружен
        if (prefabSlot == null)
            prefabSlot = Resources.Load("Prefabs/Slot - Inventory") as GameObject;

        // Если контейнер слотов не создан, создаем его
        if (SlotContainer == null)
        {
            SlotContainer = (GameObject)Instantiate(prefabSlotContainer); // Создаем контейнер
            SlotContainer.transform.SetParent(PanelRectTransform.transform); // Устанавливаем его родителем панели
            SlotContainerRectTransform = SlotContainer.GetComponent<RectTransform>(); // Получаем RectTransform контейнера
            SlotGridRectTransform = SlotContainer.GetComponent<RectTransform>(); // Получаем RectTransform сетки
        }

        // Получаем RectTransform контейнера слотов, если он еще не получен
        if (SlotContainerRectTransform == null)
            SlotContainerRectTransform = SlotContainer.GetComponent<RectTransform>();

        SlotContainerRectTransform.localPosition = Vector3.zero; // Устанавливаем позицию контейнера в (0, 0, 0)

        List<Item> itemsToMove = new List<Item>(); // Список предметов для перемещения
        List<GameObject> slotList = new List<GameObject>(); // Список слотов
        foreach (Transform child in SlotContainer.transform) // Проходим по всем дочерним объектам контейнера
        {
            if (child.tag == "Slot") // Если объект является слотом
            {
                slotList.Add(child.gameObject); // Добавляем его в список слотов
            }
        }

        // Удаляем лишние слоты, если их больше, чем нужно
        while (slotList.Count > width * height)
        {
            GameObject go = slotList[slotList.Count - 1]; // Получаем последний слот
            ItemOnObject itemInSlot = go.GetComponentInChildren<ItemOnObject>(); // Получаем предмет в слоте
            if (itemInSlot != null) // Если предмет существует
            {
                itemsToMove.Add(itemInSlot.item); // Добавляем его в список предметов для перемещения
                ItemsInInventory.Remove(itemInSlot.item); // Удаляем предмет из инвентаря
            }
            slotList.Remove(go); // Удаляем слот из списка
            DestroyImmediate(go); // Уничтожаем слот
        }

        // Добавляем новые слоты, если их недостаточно
        if (slotList.Count < width * height)
        {
            for (int i = slotList.Count; i < (width * height); i++)
            {
                GameObject Slot = (GameObject)Instantiate(prefabSlot); // Создаем новый слот
                Slot.name = (slotList.Count + 1).ToString(); // Устанавливаем имя слота
                Slot.transform.SetParent(SlotContainer.transform); // Устанавливаем родителем контейнера слотов
                slotList.Add(Slot); // Добавляем слот в список
            }
        }

        // Перемещаем предметы обратно в инвентарь, если это необходимо
        if (itemsToMove != null && ItemsInInventory.Count < width * height)
        {
            foreach (Item i in itemsToMove) // Проходим по всем предметам для перемещения
            {
                addItemToInventory(i.itemID); // Добавляем предмет в инвентарь
            }
        }

        setImportantVariables(); // Устанавливаем важные переменные
        updateItemList(); // Обновляем список предметов в инвентаре
    }


    public void updateSlotAmount() // Метод для обновления количества слотов в инвентаре
    {
        if (prefabSlot == null)
            prefabSlot = Resources.Load("Prefabs/Slot - Inventory") as GameObject; // Загружаем префаб слота, если он еще не загружен

        if (SlotContainer == null) // Если контейнер слотов не создан
        {
            SlotContainer = (GameObject)Instantiate(prefabSlotContainer); // Создаем контейнер
            SlotContainer.transform.SetParent(PanelRectTransform.transform); // Устанавливаем его родителем панели
            SlotContainerRectTransform = SlotContainer.GetComponent<RectTransform>(); // Получаем RectTransform контейнера
            SlotGridRectTransform = SlotContainer.GetComponent<RectTransform>(); // Получаем RectTransform сетки
            SlotGridLayout = SlotContainer.GetComponent<GridLayoutGroup>(); // Получаем GridLayoutGroup для управления сеткой
        }

        if (SlotContainerRectTransform == null) // Проверяем, получен ли RectTransform контейнера
            SlotContainerRectTransform = SlotContainer.GetComponent<RectTransform>();
        SlotContainerRectTransform.localPosition = Vector3.zero; // Устанавливаем позицию контейнера в (0, 0, 0)

        List<Item> itemsToMove = new List<Item>(); // Список предметов для перемещения
        List<GameObject> slotList = new List<GameObject>(); // Список слотов
        foreach (Transform child in SlotContainer.transform) // Проходим по всем дочерним объектам контейнера
        {
            if (child.tag == "Slot") { slotList.Add(child.gameObject); } // Добавляем слоты в список
        }

        // Удаляем лишние слоты, если их больше, чем нужно
        while (slotList.Count > width * height)
        {
            GameObject go = slotList[slotList.Count - 1]; // Получаем последний слот
            ItemOnObject itemInSlot = go.GetComponentInChildren<ItemOnObject>(); // Получаем предмет в слоте
            if (itemInSlot != null) // Если предмет существует
            {
                itemsToMove.Add(itemInSlot.item); // Добавляем его в список предметов для перемещения
                ItemsInInventory.Remove(itemInSlot.item); // Удаляем предмет из инвентаря
            }
            slotList.Remove(go); // Удаляем слот из списка
            DestroyImmediate(go); // Уничтожаем слот
        }

        // Добавляем новые слоты, если их недостаточно
        if (slotList.Count < width * height)
        {
            for (int i = slotList.Count; i < (width * height); i++)
            {
                GameObject Slot = (GameObject)Instantiate(prefabSlot); // Создаем новый слот
                Slot.name = (slotList.Count + 1).ToString(); // Устанавливаем имя слота
                Slot.transform.SetParent(SlotContainer.transform); // Устанавливаем родителем контейнера слотов
                slotList.Add(Slot); // Добавляем слот в список
            }
        }

        // Перемещаем предметы обратно в инвентарь, если это необходимо
        if (itemsToMove != null && ItemsInInventory.Count < width * height)
        {
            foreach (Item i in itemsToMove) // Проходим по всем предметам для перемещения
            {
                addItemToInventory(i.itemID); // Добавляем предмет в инвентарь
            }
        }

        setImportantVariables(); // Устанавливаем важные переменные
        updateItemList(); // Обновляем список предметов в инвентаре
    }

    public void updateSlotSize(int slotSize) // Метод для обновления размера слота с параметром
    {
        SlotGridLayout.cellSize = new Vector2(slotSize, slotSize); // Устанавливаем размер ячейки сетки

        updateItemSize(); // Обновляем размер предметов в слоте
    }

    public void updateSlotSize() // Метод для обновления размера слота без параметров
    {
        SlotGridLayout.cellSize = new Vector2(slotSize, slotSize); // Устанавливаем размер ячейки сетки

        updateItemSize(); // Обновляем размер предметов в слоте
    }

    void updateItemSize() // Метод для обновления размера предметов в каждом слоте
    {
        for (int i = 0; i < SlotContainer.transform.childCount; i++) // Проходим по каждому слоту
        {
            if (SlotContainer.transform.GetChild(i).childCount > 0) // Если в слоте есть предмет
            {
                // Устанавливаем размер RectTransform предмета в слоте
                SlotContainer.transform.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(slotSize, slotSize);
                SlotContainer.transform.GetChild(i).GetChild(0).GetChild(2).GetComponent<RectTransform>().sizeDelta = new Vector2(slotSize, slotSize);
            }
        }
    }

    public void updatePadding(int spacingBetweenX, int spacingBetweenY) // Метод для обновления отступов с параметрами
    {
        SlotGridLayout.spacing = new Vector2(spacingBetweenX, spacingBetweenY); // Устанавливаем расстояние между ячейками
        SlotGridLayout.padding.bottom = paddingBottom; // Устанавливаем нижний отступ
        SlotGridLayout.padding.right = paddingRight; // Устанавливаем правый отступ
        SlotGridLayout.padding.left = paddingLeft; // Устанавливаем левый отступ
        SlotGridLayout.padding.top = paddingTop; // Устанавливаем верхний отступ
    }

    public void updatePadding() // Метод для обновления отступов без параметров
    {
        SlotGridLayout.spacing = new Vector2(paddingBetweenX, paddingBetweenY); // Устанавливаем расстояние между ячейками
        SlotGridLayout.padding.bottom = paddingBottom; // Устанавливаем нижний отступ
        SlotGridLayout.padding.right = paddingRight; // Устанавливаем правый отступ
        SlotGridLayout.padding.left = paddingLeft; // Устанавливаем левый отступ
        SlotGridLayout.padding.top = paddingTop; // Устанавливаем верхний отступ
    }

    public void addAllItemsToInventory() // Метод для добавления всех предметов в инвентарь
    {
        for (int k = 0; k < ItemsInInventory.Count; k++) // Проходим по всем предметам в инвентаре
        {
            for (int i = 0; i < SlotContainer.transform.childCount; i++) // Проходим по всем слотам
            {
                if (SlotContainer.transform.GetChild(i).childCount == 0) // Если слот пустой
                {
                    GameObject item = (GameObject)Instantiate(prefabItem); // Создаем новый предмет
                    item.GetComponent<ItemOnObject>().item = ItemsInInventory[k]; // Устанавливаем предмет
                    item.transform.SetParent(SlotContainer.transform.GetChild(i)); // Устанавливаем родителем слота
                    item.GetComponent<RectTransform>().localPosition = Vector3.zero; // Устанавливаем позицию предмета
                    item.transform.GetChild(0).GetComponent<Image>().sprite = ItemsInInventory[k].itemIcon; // Устанавливаем иконку предмета
                    updateItemSize(); // Обновляем размер предметов
                    break; // Выходим из цикла, так как предмет добавлен
                }
            }
        }
        stackableSettings(); // Настраиваем отображение предметов в стеке
        updateItemList(); // Обновляем список предметов в инвентаре
    }


    public bool checkIfItemAllreadyExist(int itemID, int itemValue) // Метод для проверки существования предмета в инвентаре
    {
        updateItemList(); // Обновляем список предметов в инвентаре
        int stack; // Переменная для хранения текущего количества предметов
        for (int i = 0; i < ItemsInInventory.Count; i++) // Проходим по всем предметам в инвентаре
        {
            if (ItemsInInventory[i].itemID == itemID) // Если ID предмета совпадает с искомым
            {
                stack = ItemsInInventory[i].itemValue + itemValue; // Рассчитываем новое количество предметов
                if (stack <= ItemsInInventory[i].maxStack) // Проверяем, не превышает ли новое количество максимальное значение
                {
                    ItemsInInventory[i].itemValue = stack; // Обновляем количество предметов в инвентаре
                    GameObject temp = getItemGameObject(ItemsInInventory[i]); // Получаем объект предмета в игре
                    if (temp != null && temp.GetComponent<ConsumeItem>().duplication != null) // Проверяем, существует ли объект и дублирующий элемент
                        temp.GetComponent<ConsumeItem>().duplication.GetComponent<ItemOnObject>().item.itemValue = stack; // Обновляем количество предметов в дублирующем элементе
                    return true; // Возвращаем true, если предмет был найден и обновлен
                }
            }
        }
        return false; // Возвращаем false, если предмет не найден
    }

    public void addItemToInventory(int id) // Метод для добавления предмета в инвентарь по ID
    {
        for (int i = 0; i < SlotContainer.transform.childCount; i++) // Проходим по всем слотам в контейнере
        {
            if (SlotContainer.transform.GetChild(i).childCount == 0) // Если слот пустой
            {
                GameObject item = (GameObject)Instantiate(prefabItem); // Создаем новый объект предмета
                item.GetComponent<ItemOnObject>().item = itemDatabase.getItemByID(id); // Получаем предмет из базы данных по ID
                item.transform.SetParent(SlotContainer.transform.GetChild(i)); // Устанавливаем слот как родитель для предмета
                item.GetComponent<RectTransform>().localPosition = Vector3.zero; // Устанавливаем локальную позицию предмета в (0, 0, 0)
                item.transform.GetChild(0).GetComponent<Image>().sprite = item.GetComponent<ItemOnObject>().item.itemIcon; // Устанавливаем иконку предмета
                item.GetComponent<ItemOnObject>().item.indexItemInList = ItemsInInventory.Count - 1; // Устанавливаем индекс предмета в списке инвентаря
                break; // Выходим из цикла, так как предмет добавлен
            }
        }

        stackableSettings(); // Обновляем настройки для стекуемых предметов
        updateItemList(); // Обновляем список предметов в инвентаре
    }

    public GameObject addItemToInventory(int id, int value) // Метод для добавления предмета в инвентарь с заданным значением
    {
        for (int i = 0; i < SlotContainer.transform.childCount; i++) // Проходим по всем слотам в контейнере
        {
            if (SlotContainer.transform.GetChild(i).childCount == 0) // Если слот пустой
            {
                GameObject item = (GameObject)Instantiate(prefabItem); // Создаем новый объект предмета
                ItemOnObject itemOnObject = item.GetComponent<ItemOnObject>(); // Получаем компонент ItemOnObject
                itemOnObject.item = itemDatabase.getItemByID(id); // Получаем предмет из базы данных по ID
                                                                  // Проверяем, можно ли установить заданное значение предмета
                if (itemOnObject.item.itemValue <= itemOnObject.item.maxStack && value <= itemOnObject.item.maxStack)
                    itemOnObject.item.itemValue = value; // Устанавливаем значение предмета
                else
                    itemOnObject.item.itemValue = 1; // Если значение превышает максимум, устанавливаем 1
                item.transform.SetParent(SlotContainer.transform.GetChild(i)); // Устанавливаем слот как родитель для предмета
                item.GetComponent<RectTransform>().localPosition = Vector3.zero; // Устанавливаем локальную позицию предмета в (0, 0, 0)
                item.transform.GetChild(0).GetComponent<Image>().sprite = itemOnObject.item.itemIcon; // Устанавливаем иконку предмета
                itemOnObject.item.indexItemInList = ItemsInInventory.Count - 1; // Устанавливаем индекс предмета в списке инвентаря
                if (inputManagerDatabase == null) // Проверяем, загружен ли InputManager
                    inputManagerDatabase = (InputManager)Resources.Load("InputManager"); // Загружаем InputManager, если он не загружен
                return item; // Возвращаем созданный объект предмета
            }
        }

        stackableSettings(); // Обновляем настройки для стекуемых предметов
        updateItemList(); // Обновляем список предметов в инвентаре
        return null; // Возвращаем null, если не удалось добавить предмет
    }

    public void addItemToInventoryStorage(int itemID, int value) // Метод для добавления предмета в хранилище
    {
        for (int i = 0; i < SlotContainer.transform.childCount; i++) // Проходим по всем слотам в контейнере
        {
            if (SlotContainer.transform.GetChild(i).childCount == 0) // Если слот пустой
            {
                GameObject item = (GameObject)Instantiate(prefabItem); // Создаем новый объект предмета
                ItemOnObject itemOnObject = item.GetComponent<ItemOnObject>(); // Получаем компонент ItemOnObject
                itemOnObject.item = itemDatabase.getItemByID(itemID); // Получаем предмет из базы данных по ID
                                                                      // Проверяем, можно ли установить заданное значение предмета
                if (itemOnObject.item.itemValue < itemOnObject.item.maxStack && value <= itemOnObject.item.maxStack)
                    itemOnObject.item.itemValue = value; // Устанавливаем значение предмета
                else
                    itemOnObject.item.itemValue = 1; // Если значение превышает максимум, устанавливаем 1
                item.transform.SetParent(SlotContainer.transform.GetChild(i)); // Устанавливаем слот как родитель для предмета
                item.GetComponent<RectTransform>().localPosition = Vector3.zero; // Устанавливаем локальную позицию предмета в (0, 0, 0)
                itemOnObject.item.indexItemInList = 999; // Устанавливаем индекс предмета в списке как 999 (для хранения)
                if (inputManagerDatabase == null) // Проверяем, загружен ли InputManager
                    inputManagerDatabase = (InputManager)Resources.Load("InputManager"); // Загружаем InputManager, если он не загружен
                updateItemSize(); // Обновляем размер предметов в инвентаре
                stackableSettings(); // Обновляем настройки для стекуемых предметов
                break; // Выходим из цикла, так как предмет добавлен
            }
        }
        stackableSettings(); // Обновляем настройки для стекуемых предметов
        updateItemList(); // Обновляем список предметов в инвентаре
    }

    public void updateIconSize(int iconSize) // Метод для обновления размера иконок
    {
        for (int i = 0; i < SlotContainer.transform.childCount; i++) // Проходим по всем слотам в контейнере
        {
            if (SlotContainer.transform.GetChild(i).childCount > 0) // Если в слоте есть предмет
            {
                SlotContainer.transform.GetChild(i).GetChild(0).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(iconSize, iconSize); // Устанавливаем размер иконки
            }
        }
        updateItemSize(); // Обновляем размер предметов в инвентаре
    }

    public void updateIconSize() // Метод для обновления размера иконок без параметров
    {
        for (int i = 0; i < SlotContainer.transform.childCount; i++) // Проходим по всем слотам в контейнере
        {
            if (SlotContainer.transform.GetChild(i).childCount > 0) // Если в слоте есть предмет
            {
                SlotContainer.transform.GetChild(i).GetChild(0).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(iconSize, iconSize); // Устанавливаем размер иконки
            }
        }
        updateItemSize(); // Обновляем размер предметов в инвентаре
    }

    public void stackableSettings(bool stackable, Vector3 posi) // Метод для настройки отображения стекуемых предметов
    {
        for (int i = 0; i < SlotContainer.transform.childCount; i++) // Проходим по всем слотам в контейнере
        {
            if (SlotContainer.transform.GetChild(i).childCount > 0) // Если в слоте есть предмет
            {
                ItemOnObject item = SlotContainer.transform.GetChild(i).GetChild(0).GetComponent<ItemOnObject>(); // Получаем компонент предмета
                if (item.item.maxStack > 1) // Если предмет стекуемый
                {
                    RectTransform textRectTransform = SlotContainer.transform.GetChild(i).GetChild(0).GetChild(1).GetComponent<RectTransform>(); // Получаем RectTransform для текста
                    Text text = SlotContainer.transform.GetChild(i).GetChild(0).GetChild(1).GetComponent<Text>(); // Получаем компонент текста
                    text.text = "" + item.item.itemValue; // Устанавливаем текст с количеством предметов
                    text.enabled = stackable; // Устанавливаем видимость текста в зависимости от параметра stackable
                    textRectTransform.localPosition = posi; // Устанавливаем позицию текста
                }
            }
        }
    }

    public void deleteAllItems() // Метод для удаления всех предметов из инвентаря
    {
        for (int i = 0; i < SlotContainer.transform.childCount; i++) // Проходим по всем слотам в контейнере
        {
            if (SlotContainer.transform.GetChild(i).childCount != 0) // Если слот не пустой
            {
                Destroy(SlotContainer.transform.GetChild(i).GetChild(0).gameObject); // Удаляем предмет из слота
            }
        }
        ItemsInInventory.Clear(); // Очистка списка предметов
        updateItemList(); // Обновляем список предметов в инвентаре
    }

    public List<Item> getItemList() // Метод для получения списка предметов в инвентаре
    {
        List<Item> theList = new List<Item>(); // Создаем новый список предметов
        for (int i = 0; i < SlotContainer.transform.childCount; i++) // Проходим по всем слотам в контейнере
        {
            if (SlotContainer.transform.GetChild(i).childCount != 0) // Если слот не пустой
                theList.Add(SlotContainer.transform.GetChild(i).GetChild(0).GetComponent<ItemOnObject>().item); // Добавляем предмет в список
        }
        return theList; // Возвращаем список предметов

    }

    public void stackableSettings() // Метод для настройки отображения стекуемых предметов
    {
        for (int i = 0; i < SlotContainer.transform.childCount; i++) // Проходим по всем слотам в контейнере
        {
            if (SlotContainer.transform.GetChild(i).childCount > 0) // Если в слоте есть предмет
            {
                ItemOnObject item = SlotContainer.transform.GetChild(i).GetChild(0).GetComponent<ItemOnObject>(); // Получаем компонент предмета
                if (item.item.maxStack > 1) // Если предмет стекуемый
                {
                    RectTransform textRectTransform = SlotContainer.transform.GetChild(i).GetChild(0).GetChild(1).GetComponent<RectTransform>(); // Получаем RectTransform для текста
                    Text text = SlotContainer.transform.GetChild(i).GetChild(0).GetChild(1).GetComponent<Text>(); // Получаем компонент текста
                    text.text = "" + item.item.itemValue; // Устанавливаем текст с количеством предметов
                    text.enabled = stackable; // Устанавливаем видимость текста в зависимости от параметра stackable
                    textRectTransform.localPosition = new Vector3(positionNumberX, positionNumberY, 0); // Устанавливаем позицию текста
                }
                else // Если предмет не стекуемый
                {
                    Text text = SlotContainer.transform.GetChild(i).GetChild(0).GetChild(1).GetComponent<Text>(); // Получаем компонент текста
                    text.enabled = false; // Скрываем текст
                }
            }
        }
    }

    public GameObject getItemGameObjectByName(Item item) // Метод для получения объекта предмета по его имени
    {
        for (int k = 0; k < SlotContainer.transform.childCount; k++) // Проходим по всем слотам в контейнере
        {
            if (SlotContainer.transform.GetChild(k).childCount != 0) // Если слот не пустой
            {
                GameObject itemGameObject = SlotContainer.transform.GetChild(k).GetChild(0).gameObject; // Получаем объект предмета
                Item itemObject = itemGameObject.GetComponent<ItemOnObject>().item; // Получаем компонент ItemOnObject
                if (itemObject.itemName.Equals(item.itemName)) // Сравниваем имена предметов
                {
                    return itemGameObject; // Возвращаем объект, если имена совпадают
                }
            }
        }
        return null; // Возвращаем null, если предмет не найден
    }

    public GameObject getItemGameObject(Item item) // Метод для получения объекта предмета
    {
        for (int k = 0; k < SlotContainer.transform.childCount; k++) // Проходим по всем слотам в контейнере
        {
            if (SlotContainer.transform.GetChild(k).childCount != 0) // Если слот не пустой
            {
                GameObject itemGameObject = SlotContainer.transform.GetChild(k).GetChild(0).gameObject; // Получаем объект предмета
                Item itemObject = itemGameObject.GetComponent<ItemOnObject>().item; // Получаем компонент ItemOnObject
                if (itemObject.Equals(item)) // Сравниваем предметы
                {
                    return itemGameObject; // Возвращаем объект, если они равны
                }
            }
        }
        return null; // Возвращаем null, если предмет не найден
    }

    public void changeInventoryPanelDesign(Image image) // Метод для изменения дизайна панели инвентаря
    {
        Image inventoryDesign = transform.GetChild(0).GetChild(0).GetComponent<Image>(); // Получаем компонент изображения панели
        inventoryDesign.sprite = (Sprite)image.sprite; // Устанавливаем спрайт
        inventoryDesign.color = image.color; // Устанавливаем цвет
        inventoryDesign.material = image.material; // Устанавливаем материал
        inventoryDesign.type = image.type; // Устанавливаем тип
        inventoryDesign.fillCenter = image.fillCenter; // Устанавливаем заполнение центра
    }

    public void deleteItem(Item item) // Метод для удаления предмета из списка инвентаря
    {
        for (int i = 0; i < ItemsInInventory.Count; i++) // Проходим по всем предметам в инвентаре
        {
            if (item.Equals(ItemsInInventory[i])) // Если предмет совпадает с искомым
                ItemsInInventory.RemoveAt(item.indexItemInList); // Удаляем его из списка
        }
        updateItemList(); // Обновляем список предметов в инвентаре
    }

    public void deleteItemFromInventory(Item item) // Метод для удаления предмета из инвентаря
    {
        for (int i = 0; i < ItemsInInventory.Count; i++) // Проходим по всем предметам в инвентаре
        {
            if (item.Equals(ItemsInInventory[i])) // Если предмет совпадает с искомым
                ItemsInInventory.RemoveAt(i); // Удаляем его из списка
        }
        updateItemList(); // Обновляем список предметов в инвентаре
    }

    public void deleteItemFromInventoryWithGameObject(Item item) // Метод для удаления предмета из инвентаря и игрового объекта
    {
        for (int i = 0; i < ItemsInInventory.Count; i++) // Проходим по всем предметам в инвентаре
        {
            if (item.Equals(ItemsInInventory[i])) // Если предмет совпадает с искомым
            {
                ItemsInInventory.RemoveAt(i); // Удаляем его из списка
            }
        }

        for (int k = 0; k < SlotContainer.transform.childCount; k++) // Проходим по всем слотам в контейнере
        {
            if (SlotContainer.transform.GetChild(k).childCount != 0) // Если слот не пустой
            {
                GameObject itemGameObject = SlotContainer.transform.GetChild(k).GetChild(0).gameObject; // Получаем объект предмета
                Item itemObject = itemGameObject.GetComponent<ItemOnObject>().item; // Получаем компонент ItemOnObject
                if (itemObject.Equals(item)) // Если предмет совпадает с искомым
                {
                    Destroy(itemGameObject); // Удаляем объект из игры
                    break; // Выходим из цикла
                }
            }
        }
        updateItemList(); // Обновляем список предметов в инвентаре
    }

    public int getPositionOfItem(Item item) // Метод для получения позиции предмета в инвентаре
    {
        for (int i = 0; i < SlotContainer.transform.childCount; i++) // Проходим по всем слотам в контейнере
        {
            if (SlotContainer.transform.GetChild(i).childCount != 0) // Если слот не пустой
            {
                Item item2 = SlotContainer.transform.GetChild(i).GetChild(0).GetComponent<ItemOnObject>().item; // Получаем предмет из слота
                if (item.Equals(item2)) // Если предмет совпадает с искомым
                    return i; // Возвращаем индекс слота
            }
        }
        return -1; // Возвращаем -1, если предмет не найден
    }

    public void addItemToInventory(int ignoreSlot, int itemID, int itemValue) // Метод для добавления предмета в инвентарь с игнорированием указанного слота
    {
        for (int i = 0; i < SlotContainer.transform.childCount; i++) // Проходим по всем слотам в контейнере
        {
            if (SlotContainer.transform.GetChild(i).childCount == 0 && i != ignoreSlot) // Если слот пустой и не является игнорируемым
            {
                GameObject item = (GameObject)Instantiate(prefabItem); // Создаем новый объект предмета
                ItemOnObject itemOnObject = item.GetComponent<ItemOnObject>(); // Получаем компонент ItemOnObject
                itemOnObject.item = itemDatabase.getItemByID(itemID); // Получаем предмет из базы данных по ID
                if (itemOnObject.item.itemValue < itemOnObject.item.maxStack && itemValue <= itemOnObject.item.maxStack) // Проверяем, можно ли установить заданное значение предмета
                    itemOnObject.item.itemValue = itemValue; // Устанавливаем значение предмета
                else
                    itemOnObject.item.itemValue = 1; // Если значение превышает максимум, устанавливаем 1
                item.transform.SetParent(SlotContainer.transform.GetChild(i)); // Устанавливаем слот как родитель для предмета
                item.GetComponent<RectTransform>().localPosition = Vector3.zero; // Устанавливаем локальную позицию предмета в (0, 0, 0)
                itemOnObject.item.indexItemInList = 999; // Устанавливаем индекс предмета в списке как 999 (для хранения)
                updateItemSize(); // Обновляем размер предметов в инвентаре
                stackableSettings(); // Обновляем настройки для стекуемых предметов
                break; // Выходим из цикла, так как предмет добавлен
            }
        }
        stackableSettings(); // Обновляем настройки для стекуемых предметов
        updateItemList(); // Обновляем список предметов в инвентаре
    }

    public void updateItemIndex() // Метод для обновления индексов предметов в инвентаре
    {
        for (int i = 0; i < ItemsInInventory.Count; i++) // Проходим по всем предметам в инвентаре
        {
            ItemsInInventory[i].indexItemInList = i; // Устанавливаем индекс предмета в списке
        }
    }

    //добавление
    private string inventoryFilePath;

    void Awake()
    {
        // Устанавливаем путь к файлу для сохранения инвентаря
        inventoryFilePath = Path.Combine(Application.persistentDataPath, "inventory.json");
    }

    private string GetInventoryFilePath(string inventoryName)
    {
        return Path.Combine(Application.persistentDataPath, $"{inventoryName}_inventory.json");
    }
    public void SaveInventory(string inventoryName)
    {
        updateItemList();
        InventoryData inventoryData = new InventoryData();
        inventoryData.items = new List<InventoryData.ItemData>();

        foreach (var item in ItemsInInventory)
        {
            InventoryData.ItemData itemData = new InventoryData.ItemData
            {
                itemID = item.itemID,
                itemValue = item.itemValue
            };
            inventoryData.items.Add(itemData);
        }

        string inventoryFilePath = Path.Combine(Application.persistentDataPath, $"{inventoryName}.json");
        string json = JsonUtility.ToJson(inventoryData, true);
        File.WriteAllText(inventoryFilePath, json);
        Debug.Log($"Inventory '{inventoryName}' saved to " + inventoryFilePath);
    }

    public IEnumerator LoadInventoryCoroutine(string inventoryName)
    {
        string inventoryFilePath = Path.Combine(Application.persistentDataPath, $"{inventoryName}.json");

        if (File.Exists(inventoryFilePath))
        {
            deleteAllItems(); // Удаление всех предметов из визуальных слотов
            ItemsInInventory.Clear(); // Очищаем список предметов в инвентаре

            // Задержка для обновления UI
            yield return new WaitForEndOfFrame();

            string json = File.ReadAllText(inventoryFilePath);
            InventoryData inventoryData = JsonUtility.FromJson<InventoryData>(json);

            // Добавляем предметы в инвентарь
            foreach (var itemData in inventoryData.items)
            {
                addItemToInventory(itemData.itemID, itemData.itemValue);
            }

            updateItemList(); // Обновляем визуальные слоты после загрузки
            Debug.Log($"Inventory '{inventoryName}' loaded from " + inventoryFilePath);
        }
        else
        {
            Debug.LogWarning($"No inventory file found at " + inventoryFilePath);
        }
    }



    //public void LoadInventory()
    //{
    //    StartCoroutine(LoadInventoryCoroutine());
    //}
}

    

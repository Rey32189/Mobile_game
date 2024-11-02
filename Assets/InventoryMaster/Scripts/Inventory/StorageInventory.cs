using UnityEngine; // Подключение пространства имен Unity, необходимого для работы с игровыми объектами и компонентами.
using System.Collections; // Подключение пространства имен для работы с коллекциями, такими как списки и массивы.
#if UNITY_EDITOR
using UnityEditor; // Подключение пространства имен для работы с редакторскими функциями Unity (только в редакторе).
#endif
using System.Collections.Generic; // Подключение пространства имен для работы с обобщенными коллекциями, такими как списки.
using UnityEngine.UI; // Подключение пространства имен для работы с пользовательским интерфейсом Unity, включая элементы UI.
using UnityEngine.EventSystems; // Подключение пространства имен для работы с событиями ввода и пользовательскими интерфейсами.
using System.IO;

public class StorageInventory : MonoBehaviour // Определение класса StorageInventory, наследующего от MonoBehaviour.
{
    [SerializeField] // Атрибут, позволяющий редактировать поле в инспекторе Unity.
    public GameObject inventory; // Ссылка на объект инвентаря в сцене.

    [SerializeField]
    public List<Item> storageItems = new List<Item>(); // Список предметов, хранящихся в инвентаре.

    [SerializeField]
    private ItemDataBaseList itemDatabase; // Ссылка на базу данных предметов, содержащую информацию о всех доступных предметах.

    [SerializeField]
    public int distanceToOpenStorage; // Расстояние, на котором игрок может открыть инвентарь.

    public float timeToOpenStorage; // Время, необходимое для открытия инвентаря.

    private InputManager inputManagerDatabase; // Ссылка на менеджер ввода, который управляет клавишами и событиями ввода.

    float startTimer; // Время начала таймера для открытия инвентаря.
    float endTimer; // Время окончания таймера (не используется в данном коде).
    bool showTimer; // Флаг, указывающий, показывать ли таймер.

    public int itemAmount; // Количество предметов, которое может быть сгенерировано в инвентаре.

    Tooltip tooltip; // Ссылка на компонент Tooltip, показывающий подсказки.

    Inventory inv; // Ссылка на компонент Inventory, который управляет инвентарем.

    GameObject player; // Ссылка на объект игрока.

    static Image timerImage; // Статическая ссылка на изображение таймера (для отображения прогресса открытия инвентаря).
    static GameObject timer; // Статическая ссылка на объект таймера.

    bool closeInv; // Флаг, указывающий, нужно ли закрыть инвентарь (не используется в данном коде).

    bool showStorage; // Флаг, указывающий, открыт ли инвентарь.

    // Метод для добавления предмета в инвентарь.
    public void addItemToStorage(int id, int value)
    {
        Item item = itemDatabase.getItemByID(id); // Получение предмета из базы данных по его ID.
        item.itemValue = value; // Установка значения предмета.
        storageItems.Add(item); // Добавление предмета в список хранения.
    }

    // Метод, вызываемый при старте игры.
    void Start()
    {
        if (inputManagerDatabase == null) // Проверка, инициализирован ли менеджер ввода.
            inputManagerDatabase = (InputManager)Resources.Load("InputManager"); // Загрузка менеджера ввода из ресурсов.

        player = GameObject.FindGameObjectWithTag("Player"); // Поиск объекта игрока по тегу "Player".
        inv = inventory.GetComponent<Inventory>(); // Получение компонента Inventory из объекта инвентаря.
        ItemDataBaseList inventoryItemList = (ItemDataBaseList)Resources.Load("ItemDatabase"); // Загрузка базы данных предметов из ресурсов.

        int creatingItemsForChest = 1; // Счетчик для количества создаваемых предметов в сундуке.

        int randomItemAmount = Random.Range(1, itemAmount); // Генерация случайного количества предметов для сундука.

        // Цикл для создания предметов в сундуке.
        while (creatingItemsForChest < randomItemAmount)
        {
            int randomItemNumber = Random.Range(1, inventoryItemList.itemList.Count - 1); // Генерация случайного номера предмета.
            int raffle = Random.Range(1, 100); // Генерация случайного числа для определения редкости.

            // Проверка, попадает ли предмет в редкость.
            if (raffle <= inventoryItemList.itemList[randomItemNumber].rarity)
            {
                int randomValue = Random.Range(1, inventoryItemList.itemList[randomItemNumber].getCopy().maxStack); // Генерация случайного значения предмета.
                Item item = inventoryItemList.itemList[randomItemNumber].getCopy(); // Получение копии предмета из базы данных.
                item.itemValue = randomValue; // Установка значения предмета.
                storageItems.Add(item); // Добавление предмета в список хранения.
                creatingItemsForChest++; // Увеличение счетчика созданных предметов.
            }
        }

        // Проверка, существует ли объект таймера в сцене.
        if (GameObject.FindGameObjectWithTag("Timer") != null)
        {
            timerImage = GameObject.FindGameObjectWithTag("Timer").GetComponent<Image>(); // Получение компонента Image из объекта таймера.
            timer = GameObject.FindGameObjectWithTag("Timer"); // Сохранение ссылки на объект таймера.
            timer.SetActive(false); // Деактивация таймера при старте.
        }
        // Проверка, существует ли объект подсказки в сцене.
        if (GameObject.FindGameObjectWithTag("Tooltip") != null)
            tooltip = GameObject.FindGameObjectWithTag("Tooltip").GetComponent<Tooltip>(); // Получение компонента Tooltip из объекта подсказки.
    }

    // Метод для установки важных переменных.
    public void setImportantVariables()
    {
        if (itemDatabase == null) // Проверка, инициализирована ли база данных предметов.
            itemDatabase = (ItemDataBaseList)Resources.Load("ItemDatabase"); // Загрузка базы данных предметов из ресурсов.
    }

    // Метод, вызываемый каждый кадр.
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.S)) // Например, с комбинацией Shift
        //{
        //    SaveStorageInventory();
        //}

        ////Загрузка инвентаря сундука
        //if (Input.GetKeyDown(KeyCode.L)) // Например, с комбинацией Shift
        //{
        //    LoadStorageInventory();
        //}


        float distance = Vector3.Distance(this.gameObject.transform.position, player.transform.position); // Вычисление расстояния до игрока.

        // Проверка, нужно ли показывать таймер.
        if (showTimer)
        {
            if (timerImage != null) // Проверка, инициализирован ли таймер.
            {
                timer.SetActive(true); // Активация таймера.
                float fillAmount = (Time.time - startTimer) / timeToOpenStorage; // Вычисление заполнения таймера.
                timerImage.fillAmount = fillAmount; // Установка заполнения таймера.
            }
        }

        // Проверка, находится ли игрок в пределах расстояния для открытия инвентаря и нажал ли он клавишу.
        if (distance <= distanceToOpenStorage && Input.GetKeyDown(inputManagerDatabase.StorageKeyCode))
        {
            showStorage = !showStorage; // Переключение состояния показа инвентаря.
            StartCoroutine(OpenInventoryWithTimer()); // Запуск корутины для открытия инвентаря с таймером.
        }

        // Если игрок вышел за пределы расстояния для открытия инвентаря и инвентарь открыт.
        if (distance > distanceToOpenStorage && showStorage)
        {
            showStorage = false; // Скрытие инвентаря.
            if (inventory.activeSelf) // Проверка, активен ли инвентарь.
            {
                storageItems.Clear(); // Очистка списка хранения.
                setListofStorage(); // Установка списка предметов в инвентаре.
                inventory.SetActive(false); // Деактивация инвентаря.
                inv.deleteAllItems(); // Удаление всех предметов из инвентаря.
            }
            tooltip.deactivateTooltip(); // Деактивация подсказки.
            timerImage.fillAmount = 0; // Сброс заполнения таймера.
            timer.SetActive(false); // Деактивация таймера.
            showTimer = false; // Скрытие таймера.
        }
    }

    // Корутину для открытия инвентаря с таймером.
    IEnumerator OpenInventoryWithTimer()
    {
        if (showStorage) // Если инвентарь открыт.
        {
            startTimer = Time.time; // Запоминаем текущее время.
            showTimer = true; // Устанавливаем флаг показа таймера.
            yield return new WaitForSeconds(timeToOpenStorage); // Ждем указанное время открытия инвентаря.
            if (showStorage) // Если инвентарь все еще открыт.
            {
                inv.ItemsInInventory.Clear(); // Очищаем инвентарь.
                inventory.SetActive(true); // Активируем инвентарь.
                addItemsToInventory(); // Добавляем предметы из хранения в инвентарь.
                showTimer = false; // Скрываем таймер.
                if (timer != null) // Проверка, инициализирован ли таймер.
                    timer.SetActive(false); // Деактивация таймера.
            }
        }
        else // Если инвентарь не открыт.
        {
            storageItems.Clear(); // Очистка списка хранения.
            setListofStorage(); // Установка списка предметов в инвентаре.
            inventory.SetActive(false); // Деактивация инвентаря.
            inv.deleteAllItems(); // Удаление всех предметов из инвентаря.
            tooltip.deactivateTooltip(); // Деактивация подсказки.
        }
    }

    // Метод для установки списка предметов из инвентаря.
    void setListofStorage()
    {
        Inventory inv = inventory.GetComponent<Inventory>(); // Получение компонента Inventory из объекта инвентаря.
        storageItems = inv.getItemList(); // Установка списка предметов из инвентаря.
    }

    // Метод для добавления предметов из хранения в инвентарь.
    void addItemsToInventory()
    {
        Inventory iV = inventory.GetComponent<Inventory>(); // Получение компонента Inventory из объекта инвентаря.
        for (int i = 0; i < storageItems.Count; i++) // Цикл по всем предметам в storageItems.
        {
            iV.addItemToInventory(storageItems[i].itemID, storageItems[i].itemValue); // Добавление предмета в инвентарь по его ID и значению.
        }
        iV.stackableSettings(); // Настройка предметов на возможность складывания (если это предусмотрено).
    }

    //private string inventoryFilePath;

    //void Awake()
    //{
    //    // Устанавливаем путь к файлу для сохранения инвентаря
    //    inventoryFilePath = Path.Combine(Application.persistentDataPath, "sunduk.json");
    //}
    //public void SaveStorageInventory()
    //{
    //    CombinedInventoryData combinedData = new CombinedInventoryData();
    //    combinedData.chestInventory = new CombinedInventoryData.InventoryData();
    //    combinedData.chestInventory.items = new List<CombinedInventoryData.InventoryData.ItemData>();

    //    foreach (var item in storageItems)
    //    {
    //        CombinedInventoryData.InventoryData.ItemData itemData = new CombinedInventoryData.InventoryData.ItemData
    //        {
    //            itemID = item.itemID,
    //            itemValue = item.itemValue
    //        };
    //        combinedData.chestInventory.items.Add(itemData);
    //    }

    //    // Сохранение данных в файл
    //    string json = JsonUtility.ToJson(combinedData, true);
    //    File.WriteAllText(inventoryFilePath, json);
    //    Debug.Log("Chest inventory saved to " + inventoryFilePath);
    //}

    //public IEnumerator LoadStorageInventoryCoroutine()
    //{
    //    if (File.Exists(inventoryFilePath))
    //    {
    //        storageItems.Clear(); // Очищаем список предметов в сундуке

    //        // Задержка для обновления UI
    //        yield return new WaitForEndOfFrame();

    //        string json = File.ReadAllText(inventoryFilePath);
    //        CombinedInventoryData combinedData = JsonUtility.FromJson<CombinedInventoryData>(json);

    //        // Добавляем предметы в инвентарь сундука
    //        foreach (var itemData in combinedData.chestInventory.items)
    //        {
    //            addItemToStorage(itemData.itemID, itemData.itemValue);
    //        }

    //        Debug.Log("Chest inventory loaded from " + inventoryFilePath);
    //    }
    //    else
    //    {
    //        Debug.LogWarning("No inventory file found at " + inventoryFilePath);
    //    }
    //}
    //public void LoadStorageInventory()
    //{
    //    StartCoroutine(LoadStorageInventoryCoroutine());
    //}
}

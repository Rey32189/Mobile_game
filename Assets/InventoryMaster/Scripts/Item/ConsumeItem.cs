using UnityEngine; // Подключение пространства имен Unity для работы с игровыми объектами.
using System.Collections; // Подключение пространства имен для работы с коллекциями и перечислениями.
using UnityEngine.UI; // Подключение пространства имен для работы с пользовательским интерфейсом.
using UnityEngine.EventSystems; // Подключение пространства имен для работы с событиями ввода.
using System; // Подключение пространства имен для использования базовых типов и функций.

public class ConsumeItem : MonoBehaviour, IPointerDownHandler // Определение класса ConsumeItem, который наследует MonoBehaviour и реализует интерфейс IPointerDownHandler.
{
    public Item item; // Переменная для хранения информации о предмете.
    private static Tooltip tooltip; // Статическая переменная для хранения подсказки о предмете.
    public ItemType[] itemTypeOfSlot; // Массив типов предметов для слотов инвентаря.
    public static EquipmentSystem eS; // Статическая ссылка на систему экипировки.
    public GameObject duplication; // Ссылка на дубликат предмета.
    public static GameObject mainInventory; // Статическая ссылка на основной инвентарь игрока.

    void Start() // Метод, который вызывается при инициализации объекта.
    {
        item = GetComponent<ItemOnObject>().item; // Получение предмета из компонента ItemOnObject.
        if (GameObject.FindGameObjectWithTag("Tooltip") != null) // Проверка, существует ли объект с тегом "Tooltip".
            tooltip = GameObject.FindGameObjectWithTag("Tooltip").GetComponent<Tooltip>(); // Получение компонента Tooltip.
        if (GameObject.FindGameObjectWithTag("EquipmentSystem") != null) // Проверка, существует ли объект с тегом "EquipmentSystem".
            eS = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>().characterSystem.GetComponent<EquipmentSystem>(); // Получение системы экипировки из персонажа игрока.

        if (GameObject.FindGameObjectWithTag("MainInventory") != null) // Проверка, существует ли объект с тегом "MainInventory".
            mainInventory = GameObject.FindGameObjectWithTag("MainInventory"); // Получение основного инвентаря игрока.
    }

    public void OnPointerDown(PointerEventData data) // Метод, который вызывается при нажатии на объект мышью.
    {
        if (this.gameObject.transform.parent.parent.parent.GetComponent<EquipmentSystem>() == null) // Проверка, не находится ли объект в системе экипировки.
        {
            bool gearable = false; // Переменная для отслеживания возможности экипировки предмета.
            Inventory inventory = transform.parent.parent.parent.GetComponent<Inventory>(); // Получение инвентаря родительского объекта.

            if (eS != null) // Проверка, существует ли система экипировки.
                itemTypeOfSlot = eS.itemTypeOfSlots; // Получение типов предметов для слотов.

            if (data.button == PointerEventData.InputButton.Right) // Проверка, была ли нажата правая кнопка мыши.
            {
                //item from craft system to inventory
                if (transform.parent.GetComponent<CraftResultSlot>() != null) // Проверка, находится ли объект в слоте результата крафта.
                {
                    bool check = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>().inventory.GetComponent<Inventory>().checkIfItemAllreadyExist(item.itemID, item.itemValue); // Проверка, существует ли уже предмет в инвентаре.

                    if (!check) // Если предмет не существует в инвентаре.
                    {
                        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>().inventory.GetComponent<Inventory>().addItemToInventory(item.itemID, item.itemValue); // Добавление предмета в инвентарь.
                        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>().inventory.GetComponent<Inventory>().stackableSettings(); // Настройка стекаемых предметов.
                    }
                    CraftSystem cS = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>().craftSystem.GetComponent<CraftSystem>(); // Получение системы крафта.
                    cS.deleteItems(item); // Удаление предмета из системы крафта.
                    CraftResultSlot result = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>().craftSystem.transform.GetChild(3).GetComponent<CraftResultSlot>(); // Получение слота результата крафта.
                    result.temp = 0; // Сброс временного значения слота.
                    tooltip.deactivateTooltip(); // Деактивация подсказки.
                    gearable = true; // Установка флага экипировки в true.
                    GameObject.FindGameObjectWithTag("MainInventory").GetComponent<Inventory>().updateItemList(); // Обновление списка предметов в основном инвентаре.
                }
                else // Если объект не находится в слоте результата крафта.
                {
                    bool stop = false; // Переменная для отслеживания остановки цикла.
                    if (eS != null) // Проверка, существует ли система экипировки.
                    {
                        for (int i = 0; i < eS.slotsInTotal; i++) // Итерация по всем слотам экипировки.
                        {
                            if (itemTypeOfSlot[i].Equals(item.itemType)) // Проверка, совпадает ли тип предмета с типом слота.
                            {
                                if (eS.transform.GetChild(1).GetChild(i).childCount == 0) // Проверка, пуст ли слот.
                                {
                                    stop = true; // Установка флага остановки.
                                    if (eS.transform.GetChild(1).GetChild(i).parent.parent.GetComponent<EquipmentSystem>() != null && this.gameObject.transform.parent.parent.parent.GetComponent<EquipmentSystem>() != null) { } // Проверка на наличие системы экипировки.
                                    else
                                        inventory.EquiptItem(item); // Экипировка предмета в инвентаре.

                                    this.gameObject.transform.SetParent(eS.transform.GetChild(1).GetChild(i)); // Установка родителя для объекта.
                                    this.gameObject.GetComponent<RectTransform>().localPosition = Vector3.zero; // Установка локальной позиции объекта.
                                    eS.gameObject.GetComponent<Inventory>().updateItemList(); // Обновление списка предметов в системе экипировки.
                                    inventory.updateItemList(); // Обновление списка предметов в инвентаре.
                                    gearable = true; // Установка флага экипировки в true.
                                    if (duplication != null) // Проверка, существует ли дубликат.
                                        Destroy(duplication.gameObject); // Уничтожение дубликата.
                                    break; // Выход из цикла.
                                }
                            }
                        }

                        if (!stop) // Если флаг остановки не установлен.
                        {
                            for (int i = 0; i < eS.slotsInTotal; i++) // Итерация по всем слотам экипировки.
                            {
                                if (itemTypeOfSlot[i].Equals(item.itemType)) // Проверка, совпадает ли тип предмета с типом слота.
                                {
                                    if (eS.transform.GetChild(1).GetChild(i).childCount != 0) // Проверка, занят ли слот.
                                    {
                                        GameObject otherItemFromCharacterSystem = eS.transform.GetChild(1).GetChild(i).GetChild(0).gameObject; // Получение другого предмета из слота.
                                        Item otherSlotItem = otherItemFromCharacterSystem.GetComponent<ItemOnObject>().item; // Получение информации о другом предмете.
                                        if (item.itemType == ItemType.UFPS_Weapon) // Проверка, является ли предмет оружием.
                                        {
                                            inventory.UnEquipItem1(otherItemFromCharacterSystem.GetComponent<ItemOnObject>().item); // Снятие другого предмета с экипировки.
                                            inventory.EquiptItem(item); // Экипировка нового предмета.
                                        }
                                        else // Если предмет не является оружием.
                                        {
                                            inventory.EquiptItem(item); // Экипировка нового предмета.
                                            if (item.itemType != ItemType.Backpack) // Проверка, не является ли предмет рюкзаком.
                                                inventory.UnEquipItem1(otherItemFromCharacterSystem.GetComponent<ItemOnObject>().item); // Снятие другого предмета с экипировки.
                                        }
                                        if (this == null) // Проверка, не является ли текущий объект null.
                                        {
                                            GameObject dropItem = (GameObject)Instantiate(otherSlotItem.itemModel); // Создание нового объекта для другого предмета.
                                            dropItem.AddComponent<PickUpItem>(); // Добавление компонента для взаимодействия с предметом.
                                            dropItem.GetComponent<PickUpItem>().item = otherSlotItem; // Установка информации о предмете.
                                            dropItem.transform.localPosition = GameObject.FindGameObjectWithTag("Player").transform.localPosition; // Установка позиции предмета на позицию игрока.
                                            inventory.OnUpdateItemList(); // Обновление списка предметов в инвентаре.
                                        }
                                        else // Если текущий объект не null.
                                        {
                                            otherItemFromCharacterSystem.transform.SetParent(this.transform.parent); // Установка родителя для другого предмета.
                                            otherItemFromCharacterSystem.GetComponent<RectTransform>().localPosition = Vector3.zero; // Установка локальной позиции другого предмета.
                                            if (this.gameObject.transform.parent.parent.parent.GetComponent<Hotbar>() != null) // Проверка, находится ли объект в хотбаре.
                                                createDuplication(otherItemFromCharacterSystem); // Создание дубликата другого предмета.

                                            this.gameObject.transform.SetParent(eS.transform.GetChild(1).GetChild(i)); // Установка родителя для текущего предмета.
                                            this.gameObject.GetComponent<RectTransform>().localPosition = Vector3.zero; // Установка локальной позиции текущего предмета.
                                        }

                                        gearable = true; // Установка флага экипировки в true.                                        
                                        if (duplication != null) // Проверка, существует ли дубликат.
                                            Destroy(duplication.gameObject); // Уничтожение дубликата.
                                        eS.gameObject.GetComponent<Inventory>().updateItemList(); // Обновление списка предметов в системе экипировки.
                                        inventory.OnUpdateItemList(); // Обновление списка предметов в инвентаре.
                                        break; // Выход из цикла.
                                    }
                                }
                            }
                        }
                    }
                }
                if (!gearable && item.itemType != ItemType.UFPS_Ammo && item.itemType != ItemType.UFPS_Grenade) // Если предмет не может быть экипирован и не является боеприпасом или гранатой.
                {
                    Item itemFromDup = null; // Переменная для хранения информации о предмете из дубликата.
                    if (duplication != null) // Проверка, существует ли дубликат.
                        itemFromDup = duplication.GetComponent<ItemOnObject>().item; // Получение информации о предмете из дубликата.

                    inventory.ConsumeItem(item); // Использование предмета.

                    item.itemValue--; // Уменьшение значения предмета.
                    if (itemFromDup != null) // Проверка, существует ли предмет из дубликата.
                    {
                        duplication.GetComponent<ItemOnObject>().item.itemValue--; // Уменьшение значения предмета из дубликата.
                        if (itemFromDup.itemValue <= 0) // Проверка, достигло ли значение нуля.
                        {
                            if (tooltip != null) // Проверка, существует ли подсказка.
                                tooltip.deactivateTooltip(); // Деактивация подсказки.
                            inventory.deleteItemFromInventory(item); // Удаление предмета из инвентаря.
                            Destroy(duplication.gameObject); // Уничтожение дубликата. 
                        }
                    }
                    if (item.itemValue <= 0) // Проверка, достигло ли значение нуля.
                    {
                        if (tooltip != null) // Проверка, существует ли подсказка.
                            tooltip.deactivateTooltip(); // Деактивация подсказки.
                        inventory.deleteItemFromInventory(item); // Удаление предмета из инвентаря.
                        Destroy(this.gameObject); // Уничтожение текущего объекта.                        
                    }
                }
            }
        }
    }

    public void consumeIt() // Метод для использования предмета.
    {
        Inventory inventory = transform.parent.parent.parent.GetComponent<Inventory>(); // Получение инвентаря родительского объекта.

        bool gearable = false; // Переменная для отслеживания возможности экипировки предмета.

        if (GameObject.FindGameObjectWithTag("EquipmentSystem") != null) // Проверка, существует ли объект с тегом "EquipmentSystem".
            eS = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>().characterSystem.GetComponent<EquipmentSystem>(); // Получение системы экипировки из персонажа игрока.

        if (eS != null) // Проверка, существует ли система экипировки.
            itemTypeOfSlot = eS.itemTypeOfSlots; // Получение типов предметов для слотов.

        Item itemFromDup = null; // Переменная для хранения информации о предмете из дубликата.
        if (duplication != null) // Проверка, существует ли дубликат.
            itemFromDup = duplication.GetComponent<ItemOnObject>().item; // Получение информации о предмете из дубликата.       

        bool stop = false; // Переменная для отслеживания остановки цикла.
        if (eS != null) // Проверка, существует ли система экипировки.
        {
            for (int i = 0; i < eS.slotsInTotal; i++) // Итерация по всем слотам экипировки.
            {
                if (itemTypeOfSlot[i].Equals(item.itemType)) // Проверка, совпадает ли тип предмета с типом слота.
                {
                    if (eS.transform.GetChild(1).GetChild(i).childCount == 0) // Проверка, пуст ли слот.
                    {
                        stop = true; // Установка флага остановки.
                        this.gameObject.transform.SetParent(eS.transform.GetChild(1).GetChild(i)); // Установка родителя для текущего предмета.
                        this.gameObject.GetComponent<RectTransform>().localPosition = Vector3.zero; // Установка локальной позиции текущего предмета.
                        eS.gameObject.GetComponent<Inventory>().updateItemList(); // Обновление списка предметов в системе экипировки.
                        inventory.updateItemList(); // Обновление списка предметов в инвентаре.
                        inventory.EquiptItem(item); // Экипировка предмета в инвентаре.
                        gearable = true; // Установка флага экипировки в true.
                        if (duplication != null) // Проверка, существует ли дубликат.
                            Destroy(duplication.gameObject); // Уничтожение дубликата.
                        break; // Выход из цикла.
                    }
                }
            }

            if (!stop) // Если флаг остановки не установлен.
            {
                for (int i = 0; i < eS.slotsInTotal; i++) // Итерация по всем слотам экипировки.
                {
                    if (itemTypeOfSlot[i].Equals(item.itemType)) // Проверка, совпадает ли тип предмета с типом слота.
                    {
                        if (eS.transform.GetChild(1).GetChild(i).childCount != 0) // Проверка, занят ли слот.
                        {
                            GameObject otherItemFromCharacterSystem = eS.transform.GetChild(1).GetChild(i).GetChild(0).gameObject; // Получение другого предмета из слота.
                            Item otherSlotItem = otherItemFromCharacterSystem.GetComponent<ItemOnObject>().item; // Получение информации о другом предмете.
                            if (item.itemType == ItemType.UFPS_Weapon) // Проверка, является ли предмет оружием.
                            {
                                inventory.UnEquipItem1(otherItemFromCharacterSystem.GetComponent<ItemOnObject>().item); // Снятие другого предмета с экипировки.
                                inventory.EquiptItem(item); // Экипировка нового предмета.
                            }
                            else // Если предмет не является оружием.
                            {
                                inventory.EquiptItem(item); // Экипировка нового предмета.
                                if (item.itemType != ItemType.Backpack) // Проверка, не является ли предмет рюкзаком.
                                    inventory.UnEquipItem1(otherItemFromCharacterSystem.GetComponent<ItemOnObject>().item); // Снятие другого предмета с экипировки.
                            }
                            if (this == null) // Проверка, не является ли текущий объект null.
                            {
                                GameObject dropItem = (GameObject)Instantiate(otherSlotItem.itemModel); // Создание нового объекта для другого предмета.
                                dropItem.AddComponent<PickUpItem>(); // Добавление компонента для взаимодействия с предметом.
                                dropItem.GetComponent<PickUpItem>().item = otherSlotItem; // Установка информации о предмете.
                                dropItem.transform.localPosition = GameObject.FindGameObjectWithTag("Player").transform.localPosition; // Установка позиции предмета на позицию игрока.
                                inventory.OnUpdateItemList(); // Обновление списка предметов в инвентаре.
                            }
                            else // Если текущий объект не null.
                            {
                                otherItemFromCharacterSystem.transform.SetParent(this.transform.parent); // Установка родителя для другого предмета.
                                otherItemFromCharacterSystem.GetComponent<RectTransform>().localPosition = Vector3.zero; // Установка локальной позиции другого предмета.
                                if (this.gameObject.transform.parent.parent.parent.GetComponent<Hotbar>() != null) // Проверка, находится ли объект в хотбаре.
                                    createDuplication(otherItemFromCharacterSystem); // Создание дубликата другого предмета.

                                this.gameObject.transform.SetParent(eS.transform.GetChild(1).GetChild(i)); // Установка родителя для текущего предмета.
                                this.gameObject.GetComponent<RectTransform>().localPosition = Vector3.zero; // Установка локальной позиции текущего предмета.
                            }

                            gearable = true; // Установка флага экипировки в true.
                            if (duplication != null) // Проверка, существует ли дубликат.
                                Destroy(duplication.gameObject); // Уничтожение дубликата.
                            eS.gameObject.GetComponent<Inventory>().updateItemList(); // Обновление списка предметов в системе экипировки.
                            inventory.OnUpdateItemList(); // Обновление списка предметов в инвентаре.
                            break; // Выход из цикла.                           
                        }
                    }
                }
            }
        }
        if (!gearable && item.itemType != ItemType.UFPS_Ammo && item.itemType != ItemType.UFPS_Grenade) // Если предмет не может быть экипирован и не является боеприпасом или гранатой.
        {
            if (duplication != null) // Проверка, существует ли дубликат.
                itemFromDup = duplication.GetComponent<ItemOnObject>().item; // Получение информации о предмете из дубликата.

            inventory.ConsumeItem(item); // Использование предмета.

            item.itemValue--; // Уменьшение значения предмета.
            if (itemFromDup != null) // Проверка, существует ли предмет из дубликата.
            {
                duplication.GetComponent<ItemOnObject>().item.itemValue--; // Уменьшение значения предмета из дубликата.
                if (itemFromDup.itemValue <= 0) // Проверка, достигло ли значение нуля.
                {
                    if (tooltip != null) // Проверка, существует ли подсказка.
                        tooltip.deactivateTooltip(); // Деактивация подсказки.
                    inventory.deleteItemFromInventory(item); // Удаление предмета из инвентаря.
                    Destroy(duplication.gameObject); // Уничтожение дубликата.
                }
            }
            if (item.itemValue <= 0) // Проверка, достигло ли значение нуля.
            {
                if (tooltip != null) // Проверка, существует ли подсказка.
                    tooltip.deactivateTooltip(); // Деактивация подсказки.
                inventory.deleteItemFromInventory(item); // Удаление предмета из инвентаря.
                Destroy(this.gameObject); // Уничтожение текущего объекта. 
            }
        }
    }

    public void createDuplication(GameObject Item) // Метод для создания дубликата предмета.
    {
        Item item = Item.GetComponent<ItemOnObject>().item; // Получение информации о предмете.
        GameObject dup = mainInventory.GetComponent<Inventory>().addItemToInventory(item.itemID, item.itemValue); // Добавление предмета в основной инвентарь и получение дубликата.
        Item.GetComponent<ConsumeItem>().duplication = dup; // Установка дубликата в компоненте ConsumeItem.
        dup.GetComponent<ConsumeItem>().duplication = Item; // Установка текущего предмета в дубликате.
    }
}

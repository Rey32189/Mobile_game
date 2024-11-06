using UnityEngine; // Подключение пространства имен Unity для работы с игровыми объектами и компонентами
using System.Collections; // Подключение пространства имен для работы с коллекциями и IEnumerator
using UnityEngine.UI; // Подключение пространства имен для работы с UI элементами
using UnityEngine.EventSystems; // Подключение пространства имен для работы с событиями ввода

// Класс DragItem реализует интерфейсы для обработки перетаскивания и взаимодействия с мышью
public class DragItem : MonoBehaviour, IDragHandler, IPointerDownHandler, IEndDragHandler
{
    private Vector2 pointerOffset; // Смещение указателя мыши относительно позиции предмета
    private RectTransform rectTransform; // Ссылка на RectTransform текущего объекта (предмета)
    private RectTransform rectTransformSlot; // Ссылка на RectTransform слота, в который перетаскивается предмет
    private CanvasGroup canvasGroup; // Группа канваса для управления взаимодействием с объектом
    private GameObject oldSlot; // Ссылка на предыдущий слот, из которого был перетянут предмет
    private Inventory inventory; // Ссылка на инвентарь, к которому принадлежит предмет
    private Transform draggedItemBox; // Ссылка на контейнер для перетаскиваемого предмета

    // Делегат для обновления списка предметов в инвентаре
    public delegate void ItemDelegate();
    public static event ItemDelegate updateInventoryList; // Статическое событие для обновления инвентаря

    void Start()
    {
        // Получение ссылки на RectTransform текущего объекта
        rectTransform = GetComponent<RectTransform>();
        // Получение ссылки на CanvasGroup для управления взаимодействием
        canvasGroup = GetComponent<CanvasGroup>();
        // Поиск RectTransform объекта, который будет использоваться для перетаскивания
        rectTransformSlot = GameObject.FindGameObjectWithTag("DraggingItem").GetComponent<RectTransform>();
        // Получение ссылки на инвентарь из родительских объектов
        inventory = transform.parent.parent.parent.GetComponent<Inventory>();
        // Получение ссылки на контейнер для перетаскиваемого предмета
        draggedItemBox = GameObject.FindGameObjectWithTag("DraggingItem").transform;
    }

    // Метод, вызываемый при перетаскивании предмета
    public void OnDrag(PointerEventData data)
    {
        // Проверка, существует ли RectTransform
        if (rectTransform == null)
            return; // Если нет, выход из метода

        // Проверка, что нажата левая кнопка мыши и предмет не находится в слоте крафта
        if (data.button == PointerEventData.InputButton.Left && transform.parent.GetComponent<CraftResultSlot>() == null)
        {
            // Перемещение предмета на верхний уровень и установка родителя на контейнер для перетаскиваемого предмета
            rectTransform.SetAsLastSibling();
            transform.SetParent(draggedItemBox);
            Vector2 localPointerPosition; // Переменная для хранения локальной позиции указателя
            canvasGroup.blocksRaycasts = false; // Отключение блокировки взаимодействия с объектом
            // Преобразование экранной точки в локальную точку относительно RectTransform слота
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransformSlot, Input.mousePosition, data.pressEventCamera, out localPointerPosition))
            {
                // Установка локальной позиции RectTransform на основе позиции указателя
                rectTransform.localPosition = localPointerPosition - pointerOffset;
                // Удаление дубликата предмета, если он существует
                if (transform.GetComponent<ConsumeItem>().duplication != null)
                    Destroy(transform.GetComponent<ConsumeItem>().duplication);
            }
        }

        // Обновление списка предметов в инвентаре
        inventory.OnUpdateItemList();
    }

    // Метод, вызываемый при нажатии на предмет
    public void OnPointerDown(PointerEventData data)
    {
        // Проверка, что нажата левая кнопка мыши
        if (data.button == PointerEventData.InputButton.Left)
        {
            // Получение смещения указателя мыши относительно RectTransform предмета
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, data.position, data.pressEventCamera, out pointerOffset);
            // Сохранение ссылки на старый слот
            oldSlot = transform.parent.gameObject;
        }
        // Вызов события обновления списка инвентаря, если есть подписчики
        if (updateInventoryList != null)
            updateInventoryList();
    }

    // Метод для создания дубликата предмета
    public void createDuplication(GameObject Item)
    {
        // Получение компонента ItemOnObject и извлечение объекта Item из него
        Item item = Item.GetComponent<ItemOnObject>().item;
        // Поиск объекта с тегом "MainInventory" и добавление предмета в инвентарь, получая дубликат
        GameObject duplication = GameObject.FindGameObjectWithTag("MainInventory").GetComponent<Inventory>().addItemToInventory(item.itemID, item.itemValue);
        // Настройка параметров стеков для нового дубликата предмета
        duplication.transform.parent.parent.parent.GetComponent<Inventory>().stackableSettings();
        // Сохранение ссылки на дубликат в компоненте ConsumeItem оригинального предмета
        Item.GetComponent<ConsumeItem>().duplication = duplication;
        // Сохранение ссылки на оригинальный предмет в компоненте ConsumeItem дубликата
        duplication.GetComponent<ConsumeItem>().duplication = Item;
    }

    // Метод, вызываемый при завершении перетаскивания предмета
    public void OnEndDrag(PointerEventData data)
    {
        // Проверка, что была нажата левая кнопка мыши
        if (data.button == PointerEventData.InputButton.Left)
        {
            // Включение блокировки взаимодействия с объектом
            canvasGroup.blocksRaycasts = true;
            Transform newSlot = null; // Переменная для хранения нового слота

            // Если указатель мыши находится над объектом, сохраняем ссылку на новый слот
            if (data.pointerEnter != null)
                newSlot = data.pointerEnter.transform;

            // Проверка, что новый слот не равен null
            if (newSlot != null)
            {
                // Получение игровых объектов и RectTransform для предметов
                GameObject firstItemGameObject = this.gameObject; // Игровой объект текущего предмета
                GameObject secondItemGameObject = newSlot.parent.gameObject; // Игровой объект нового слота
                RectTransform firstItemRectTransform = this.gameObject.GetComponent<RectTransform>(); // RectTransform текущего предмета
                RectTransform secondItemRectTransform = newSlot.parent.GetComponent<RectTransform>(); // RectTransform нового слота
                Item firstItem = rectTransform.GetComponent<ItemOnObject>().item; // Получение предмета из текущего RectTransform
                Item secondItem = new Item(); // Создание нового экземпляра Item
                                              // Если новый слот содержит компонент ItemOnObject, получаем его предмет
                if (newSlot.parent.GetComponent<ItemOnObject>() != null)
                    secondItem = newSlot.parent.GetComponent<ItemOnObject>().item;

                // Получение информации о двух предметах
                bool sameItem = firstItem.itemName == secondItem.itemName; // Проверка, одинаковые ли имена предметов
                bool sameItemRerferenced = firstItem.Equals(secondItem); // Проверка, ссылаются ли оба предмета на один и тот же объект
                bool secondItemStack = false; // Флаг для проверки, можно ли стекать второй предмет
                bool firstItemStack = false; // Флаг для проверки, можно ли стекать первый предмет
                                             // Если имена предметов одинаковые, проверяем, можно ли их стекать
                if (sameItem)
                {
                    firstItemStack = firstItem.itemValue < firstItem.maxStack; // Проверка, можно ли стекать первый предмет
                    secondItemStack = secondItem.itemValue < secondItem.maxStack; // Проверка, можно ли стекать второй предмет
                }

                // Получение объекта инвентаря из RectTransform второго предмета
                GameObject Inventory = secondItemRectTransform.parent.gameObject;
                // Если инвентарь имеет тег "Slot", получаем родительский объект
                if (Inventory.tag == "Slot")
                    Inventory = secondItemRectTransform.parent.parent.parent.gameObject;

                // Если инвентарь имеет тег "Slot", получаем родительский объект еще раз
                if (Inventory.tag.Equals("Slot"))
                    Inventory = Inventory.transform.parent.parent.gameObject;

                // Проверка, что объект Inventory не является горячей панелью, системой экипировки или системой крафта
                if (Inventory.GetComponent<Hotbar>() == null && Inventory.GetComponent<EquipmentSystem>() == null && Inventory.GetComponent<CraftSystem>() == null)
                {
                    // Проверка, что новый слот или его родительский слот имеют тег "ResultSlot", чтобы предотвратить прикрепление предметов к слоту результата системы крафта
                    if (newSlot.transform.parent.tag == "ResultSlot" || newSlot.transform.tag == "ResultSlot" || newSlot.transform.parent.parent.tag == "ResultSlot")
                    {
                        // Возврат предмета в старый слот, если он пытается прикрепиться к слоту результата
                        firstItemGameObject.transform.SetParent(oldSlot.transform);
                        // Установка локальной позиции предмета в ноль, чтобы он находился в центре старого слота
                        firstItemRectTransform.localPosition = Vector3.zero;
                    }
                    else
                    {
                        // Получение количества дочерних объектов в родительском слоте нового слота
                        int newSlotChildCount = newSlot.transform.parent.childCount;
                        // Проверка, есть ли в новом слоте предмет (дочерний объект с тегом "ItemIcon")
                        bool isOnSlot = newSlot.transform.parent.GetChild(0).tag == "ItemIcon";
                        // Проверка, перетаскивается ли предмет на слот, где уже есть предмет
                        if (newSlotChildCount != 0 && isOnSlot)
                        {
                            // Переменная для проверки, помещается ли предмет в другой предмет
                            bool fitsIntoStack = false;
                            // Если предметы одинаковые, проверяем, можно ли их объединить в стек
                            if (sameItem)
                                fitsIntoStack = (firstItem.itemValue + secondItem.itemValue) <= firstItem.maxStack;

                            // Если инвентарь поддерживает стекание и оба предмета одинаковые, проверяем, что стеки не полные
                            if (inventory.stackable && sameItem && firstItemStack && secondItemStack)
                            {
                                // Если предмет помещается в другой предмет и они не ссылаются на один и тот же объект
                                if (fitsIntoStack && !sameItemRerferenced)
                                {
                                    // Обновляем количество предметов во втором предмете
                                    secondItem.itemValue = firstItem.itemValue + secondItem.itemValue;
                                    // Перемещаем игровой объект второго предмета в родительский слот нового слота
                                    secondItemGameObject.transform.SetParent(newSlot.parent.parent);
                                    // Уничтожаем первый предмет, так как он был объединен
                                    Destroy(firstItemGameObject);
                                    // Устанавливаем локальную позицию второго предмета в ноль, чтобы он находился в центре нового слота
                                    secondItemRectTransform.localPosition = Vector3.zero;

                                    // Если у второго предмета есть дубликат, обновляем его значение
                                    if (secondItemGameObject.GetComponent<ConsumeItem>().duplication != null)
                                    {
                                        // Получаем ссылку на дубликат
                                        GameObject dup = secondItemGameObject.GetComponent<ConsumeItem>().duplication;
                                        // Обновляем значение предмета дубликата
                                        dup.GetComponent<ItemOnObject>().item.itemValue = secondItem.itemValue;
                                        // Применяем настройки стеков к дубликату
                                        dup.GetComponent<SplitItem>().inv.stackableSettings();
                                        // Обновляем список предметов в инвентаре
                                        dup.transform.parent.parent.parent.GetComponent<Inventory>().updateItemList();
                                    }
                                }

                                else
                                {
                                    // Создаем остаток от объединения предметов, который не помещается в стек
                                    int rest = (firstItem.itemValue + secondItem.itemValue) % firstItem.maxStack;

                                    // Проверяем, помещается ли предмет в стек, и есть ли остаток
                                    if (!fitsIntoStack && rest > 0)
                                    {
                                        // Заполняем первый предмет до максимума
                                        firstItem.itemValue = firstItem.maxStack;
                                        // Устанавливаем значение второго предмета равным остатку
                                        secondItem.itemValue = rest;

                                        // Перемещаем первый предмет в родительский слот второго предмета
                                        firstItemGameObject.transform.SetParent(secondItemGameObject.transform.parent);
                                        // Перемещаем второй предмет в старый слот
                                        secondItemGameObject.transform.SetParent(oldSlot.transform);

                                        // Устанавливаем локальные позиции обоих предметов в ноль, чтобы они находились в центре своих слотов
                                        firstItemRectTransform.localPosition = Vector3.zero;
                                        secondItemRectTransform.localPosition = Vector3.zero;
                                    }
                                }
                            }
                            // Если предметы не помещаются в стек
                            else
                            {
                                // Создаем остаток от объединения предметов, если они одинаковые
                                int rest = 0;
                                if (sameItem)
                                    rest = (firstItem.itemValue + secondItem.itemValue) % firstItem.maxStack;

                                // Проверяем, помещается ли предмет в стек, и есть ли остаток
                                if (!fitsIntoStack && rest > 0)
                                {
                                    // Заполняем второй предмет до максимума
                                    secondItem.itemValue = firstItem.maxStack;
                                    // Устанавливаем значение первого предмета равным остатку
                                    firstItem.itemValue = rest;

                                    // Перемещаем первый предмет в родительский слот второго предмета
                                    firstItemGameObject.transform.SetParent(secondItemGameObject.transform.parent);
                                    // Перемещаем второй предмет в старый слот
                                    secondItemGameObject.transform.SetParent(oldSlot.transform);

                                    // Устанавливаем локальные позиции обоих предметов в ноль, чтобы они находились в центре своих слотов
                                    firstItemRectTransform.localPosition = Vector3.zero;
                                    secondItemRectTransform.localPosition = Vector3.zero;
                                }
                                // Если предметы разные или стек полон, они меняются местами
                                else if (!fitsIntoStack && rest == 0)
                                {
                                    // Проверяем, перетаскивается ли предмет из системы экипировки в инвентарь и пытаемся ли мы поменять его на предмет того же типа
                                    if (oldSlot.transform.parent.parent.GetComponent<EquipmentSystem>() != null && firstItem.itemType == secondItem.itemType)
                                    {
                                        // Снимаем предмет с экипировки
                                        newSlot.transform.parent.parent.parent.parent.GetComponent<Inventory>().UnEquipItem1(firstItem);
                                        // Экипируем второй предмет
                                        oldSlot.transform.parent.parent.GetComponent<Inventory>().EquiptItem(secondItem);

                                        // Перемещаем первый предмет в родительский слот второго предмета
                                        firstItemGameObject.transform.SetParent(secondItemGameObject.transform.parent);
                                        // Перемещаем второй предмет в старый слот
                                        secondItemGameObject.transform.SetParent(oldSlot.transform);
                                        // Устанавливаем локальную позицию второго предмета в ноль
                                        secondItemRectTransform.localPosition = Vector3.zero;
                                        // Устанавливаем локальную позицию первого предмета в ноль
                                        firstItemRectTransform.localPosition = Vector3.zero;

                                        // Если у второго предмета есть дубликат, уничтожаем его
                                        if (secondItemGameObject.GetComponent<ConsumeItem>().duplication != null)
                                            Destroy(secondItemGameObject.GetComponent<ConsumeItem>().duplication);
                                    }
                                }
                            }

                        }

                        // Если новый слот пустой
                        else
                        {
                            // Проверяем, что новый слот не является слотом или иконкой предмета
                            if (newSlot.tag != "Slot" && newSlot.tag != "ItemIcon")
                            {
                                // Если новый слот не подходит, возвращаем первый предмет в старый слот
                                firstItemGameObject.transform.SetParent(oldSlot.transform);
                                // Устанавливаем локальную позицию первого предмета в ноль, чтобы он находился в центре старого слота
                                firstItemRectTransform.localPosition = Vector3.zero;
                            }
                            else
                            {
                                // Если новый слот подходит, перемещаем первый предмет в новый слот
                                firstItemGameObject.transform.SetParent(newSlot.transform);
                                // Устанавливаем локальную позицию первого предмета в ноль, чтобы он находился в центре нового слота
                                firstItemRectTransform.localPosition = Vector3.zero;

                                // Проверяем, что новый слот не принадлежит системе экипировки, а старый слот принадлежит
                                if (newSlot.transform.parent.parent.GetComponent<EquipmentSystem>() == null && oldSlot.transform.parent.parent.GetComponent<EquipmentSystem>() != null)
                                    // Если это так, снимаем предмет с экипировки
                                    oldSlot.transform.parent.parent.GetComponent<Inventory>().UnEquipItem1(firstItem);
                            }
                        }

                    }
                }



                // Проверяем, перетаскивается ли предмет в панель быстрого доступа (Hotbar)
                if (Inventory.GetComponent<Hotbar>() != null)
                {
                    // Получаем количество дочерних элементов у нового слота
                    int newSlotChildCount = newSlot.transform.parent.childCount;
                    // Проверяем, есть ли в новом слоте уже предмет (иконка предмета)
                    bool isOnSlot = newSlot.transform.parent.GetChild(0).tag == "ItemIcon";

                    // Проверяем, перетаскивается ли предмет на слот, где уже есть предмет
                    if (newSlotChildCount != 0 && isOnSlot)
                    {
                        // Переменная для проверки, помещается ли предмет в другой предмет
                        bool fitsIntoStack = false;

                        // Если предметы одинаковые, проверяем, помещаются ли они в стек
                        if (sameItem)
                            fitsIntoStack = (firstItem.itemValue + secondItem.itemValue) <= firstItem.maxStack;

                        // Если предметы могут быть сложены (stackable) и оба предмета являются одинаковыми
                        if (inventory.stackable && sameItem && firstItemStack && secondItemStack)
                        {
                            // Если предмет не помещается в другой предмет
                            if (fitsIntoStack && !sameItemRerferenced)
                            {
                                // Обновляем значение второго предмета, добавляя значение первого
                                secondItem.itemValue = firstItem.itemValue + secondItem.itemValue;

                                // Перемещаем второй предмет в родительский слот нового слота
                                secondItemGameObject.transform.SetParent(newSlot.parent.parent);

                                // Уничтожаем объект первого предмета, так как он больше не нужен
                                Destroy(firstItemGameObject);

                                // Устанавливаем локальную позицию второго предмета в ноль, чтобы он находился в центре своего слота
                                secondItemRectTransform.localPosition = Vector3.zero;

                                // Если у второго предмета есть дубликат, обновляем его значение
                                if (secondItemGameObject.GetComponent<ConsumeItem>().duplication != null)
                                {
                                    GameObject dup = secondItemGameObject.GetComponent<ConsumeItem>().duplication;
                                    // Обновляем значение дубликата
                                    dup.GetComponent<ItemOnObject>().item.itemValue = secondItem.itemValue;

                                    // Вызываем настройки для стеков в инвентаре
                                    Inventory.GetComponent<Inventory>().stackableSettings();

                                    // Обновляем список предметов в инвентаре
                                    dup.transform.parent.parent.parent.GetComponent<Inventory>().updateItemList();
                                }
                            }

                            else
                            {
                                // Вычисляем остаток от сложения значений первого и второго предметов, чтобы определить, сколько осталось после заполнения стека
                                int rest = (firstItem.itemValue + secondItem.itemValue) % firstItem.maxStack;

                                // Проверяем, помещается ли предмет в стек, и есть ли остаток, который нужно распределить
                                if (!fitsIntoStack && rest > 0)
                                {
                                    // Устанавливаем значение первого предмета в максимальное значение стека
                                    firstItem.itemValue = firstItem.maxStack;
                                    // Устанавливаем значение второго предмета на остаток
                                    secondItem.itemValue = rest;

                                    // Перемещаем первый предмет в родительский объект второго предмета
                                    firstItemGameObject.transform.SetParent(secondItemGameObject.transform.parent);
                                    // Перемещаем второй предмет обратно в старый слот
                                    secondItemGameObject.transform.SetParent(oldSlot.transform);

                                    // Устанавливаем локальную позицию первого и второго предмета в ноль, чтобы они находились в центре своих слотов
                                    firstItemRectTransform.localPosition = Vector3.zero;
                                    secondItemRectTransform.localPosition = Vector3.zero;

                                    // Создаем дубликат предмета
                                    createDuplication(this.gameObject);
                                    // Обновляем дубликат второго предмета
                                    secondItemGameObject.GetComponent<ConsumeItem>().duplication.GetComponent<ItemOnObject>().item = secondItem;

                                    // Обновляем настройки стека в инвентаре
                                    secondItemGameObject.GetComponent<SplitItem>().inv.stackableSettings();
                                }
                            }
                        }
                        // Если предмет не помещается в стек
                        else
                        {
                            // Создаем переменную для остатка
                            int rest = 0;
                            // Если предметы одинаковые, вычисляем остаток от сложения значений
                            if (sameItem)
                                rest = (firstItem.itemValue + secondItem.itemValue) % firstItem.maxStack;

                            // Проверяем, является ли старый слот частью системы экипировки
                            bool fromEquip = oldSlot.transform.parent.parent.GetComponent<EquipmentSystem>() != null;

                            // Проверяем, помещается ли предмет в стек и есть ли остаток
                            if (!fitsIntoStack && rest > 0)
                            {
                                // Устанавливаем значение второго предмета на максимальное значение стека
                                secondItem.itemValue = firstItem.maxStack;
                                // Устанавливаем значение первого предмета на остаток
                                firstItem.itemValue = rest;

                                // Создаем дубликат предмета
                                createDuplication(this.gameObject);

                                // Перемещаем первый предмет в родительский объект второго предмета
                                firstItemGameObject.transform.SetParent(secondItemGameObject.transform.parent);
                                // Перемещаем второй предмет обратно в старый слот
                                secondItemGameObject.transform.SetParent(oldSlot.transform);

                                // Устанавливаем локальную позицию первого и второго предмета в ноль, чтобы они находились в центре своих слотов
                                firstItemRectTransform.localPosition = Vector3.zero;
                                secondItemRectTransform.localPosition = Vector3.zero;
                            }
                            // Если предметы разные или стек полон, они меняются местами
                            else if (!fitsIntoStack && rest == 0)
                            {
                                // Если предметы не из системы экипировки
                                if (!fromEquip)
                                {
                                    // Перемещаем первый предмет в родительский объект второго предмета
                                    firstItemGameObject.transform.SetParent(secondItemGameObject.transform.parent);
                                    // Перемещаем второй предмет обратно в старый слот
                                    secondItemGameObject.transform.SetParent(oldSlot.transform);
                                    // Устанавливаем локальную позицию второго и первого предмета в ноль
                                    secondItemRectTransform.localPosition = Vector3.zero;
                                    firstItemRectTransform.localPosition = Vector3.zero;

                                    // Если старый слот принадлежит основному инвентарю
                                    if (oldSlot.transform.parent.parent.gameObject.Equals(GameObject.FindGameObjectWithTag("MainInventory")))
                                    {
                                        // Уничтожаем дубликат второго предмета
                                        Destroy(secondItemGameObject.GetComponent<ConsumeItem>().duplication);
                                        // Создаем дубликат первого предмета
                                        createDuplication(firstItemGameObject);
                                    }
                                    else
                                    {
                                        // Создаем дубликат первого предмета
                                        createDuplication(firstItemGameObject);
                                    }
                                }
                                else
                                {
                                    // Если предметы из системы экипировки, перемещаем первый предмет обратно в старый слот
                                    firstItemGameObject.transform.SetParent(oldSlot.transform);
                                    // Устанавливаем локальную позицию первого предмета в ноль
                                    firstItemRectTransform.localPosition = Vector3.zero;
                                }
                            }
                        }
                    }
                    // Если слот пустой
                    else
                    {
                        // Проверяем, является ли новый слот не слотом и не иконкой предмета
                        if (newSlot.tag != "Slot" && newSlot.tag != "ItemIcon")
                        {
                            // Перемещаем первый предмет обратно в старый слот
                            firstItemGameObject.transform.SetParent(oldSlot.transform);
                            // Устанавливаем локальную позицию первого предмета в ноль, чтобы он находился в центре слота
                            firstItemRectTransform.localPosition = Vector3.zero;
                        }
                        else
                        {
                            // Перемещаем первый предмет в новый слот
                            firstItemGameObject.transform.SetParent(newSlot.transform);
                            // Устанавливаем локальную позицию первого предмета в ноль
                            firstItemRectTransform.localPosition = Vector3.zero;

                            // Проверяем, если новый слот не из системы экипировки, а старый слот из системы экипировки
                            if (newSlot.transform.parent.parent.GetComponent<EquipmentSystem>() == null && oldSlot.transform.parent.parent.GetComponent<EquipmentSystem>() != null)
                                // Разэкипируем предмет из старого слота
                                oldSlot.transform.parent.parent.GetComponent<Inventory>().UnEquipItem1(firstItem);

                            // Создаем дубликат первого предмета
                            createDuplication(firstItemGameObject);
                        }
                    }


                }


                // Проверяем, находится ли инвентарь в системе экипировки/системе персонажа
                if (Inventory.GetComponent<EquipmentSystem>() != null)
                {
                    // Получаем типы предметов, которые могут находиться в слотах системы экипировки
                    ItemType[] itemTypeOfSlots = GameObject.FindGameObjectWithTag("EquipmentSystem").GetComponent<EquipmentSystem>().itemTypeOfSlots;
                    // Получаем количество дочерних объектов нового слота
                    int newSlotChildCount = newSlot.transform.parent.childCount;
                    // Проверяем, находится ли в новом слоте предмет (иконка)
                    bool isOnSlot = newSlot.transform.parent.GetChild(0).tag == "ItemIcon";
                    // Проверяем, совпадают ли типы предметов
                    bool sameItemType = firstItem.itemType == secondItem.itemType;
                    // Проверяем, был ли старый слот частью хотбара
                    bool fromHot = oldSlot.transform.parent.parent.GetComponent<Hotbar>() != null;

                    // Если в новом слоте уже есть предмет
                    if (newSlotChildCount != 0 && isOnSlot)
                    {
                        // Если типы предметов совпадают и они не ссылаются на один и тот же объект
                        if (sameItemType && !sameItemRerferenced)
                        {
                            // Сохраняем родительские объекты для временного использования
                            Transform temp1 = secondItemGameObject.transform.parent.parent.parent;
                            Transform temp2 = oldSlot.transform.parent.parent;

                            // Меняем местами предметы
                            firstItemGameObject.transform.SetParent(secondItemGameObject.transform.parent);
                            secondItemGameObject.transform.SetParent(oldSlot.transform);
                            // Устанавливаем локальную позицию обоих предметов в ноль
                            secondItemRectTransform.localPosition = Vector3.zero;
                            firstItemRectTransform.localPosition = Vector3.zero;

                            // Проверяем, находятся ли предметы в разных родительских объектах
                            if (!temp1.Equals(temp2))
                            {
                                // Если первый предмет - оружие, разэкипируем второй предмет и экипируем первый
                                if (firstItem.itemType == ItemType.UFPS_Weapon)
                                {
                                    Inventory.GetComponent<Inventory>().UnEquipItem1(secondItem);
                                    Inventory.GetComponent<Inventory>().EquiptItem(firstItem);
                                }
                                else
                                {
                                    // Экипируем первый предмет
                                    Inventory.GetComponent<Inventory>().EquiptItem(firstItem);
                                    // Если второй предмет не рюкзак, разэкипируем его
                                    if (secondItem.itemType != ItemType.Backpack)
                                        Inventory.GetComponent<Inventory>().UnEquipItem1(secondItem);
                                }
                            }

                            // Если предмет был из хотбара, создаем дубликат второго предмета
                            if (fromHot)
                                createDuplication(secondItemGameObject);
                        }
                        // Если типы предметов не совпадают, возвращаем перетаскиваемый предмет обратно
                        else
                        {
                            firstItemGameObject.transform.SetParent(oldSlot.transform);
                            firstItemRectTransform.localPosition = Vector3.zero;

                            // Если предмет был из хотбара, создаем дубликат первого предмета
                            if (fromHot)
                                createDuplication(firstItemGameObject);
                        }
                    }


                    // Если слот пустой
                    else
                    {
                        // Проходим по всем дочерним элементам родителя нового слота
                        for (int i = 0; i < newSlot.parent.childCount; i++)
                        {
                            // Проверяем, совпадает ли новый слот с текущим дочерним элементом
                            if (newSlot.Equals(newSlot.parent.GetChild(i)))
                            {
                                // Проверяем, является ли тип слота подходящим для предмета
                                if (itemTypeOfSlots[i] == transform.GetComponent<ItemOnObject>().item.itemType)
                                {
                                    // Устанавливаем родителем предмета новый слот
                                    transform.SetParent(newSlot);
                                    // Устанавливаем локальную позицию предмета в ноль, чтобы он находился в центре слота
                                    rectTransform.localPosition = Vector3.zero;

                                    // Если старый слот и новый слот не находятся в одном родительском объекте, экипируем предмет
                                    if (!oldSlot.transform.parent.parent.Equals(newSlot.transform.parent.parent))
                                        Inventory.GetComponent<Inventory>().EquiptItem(firstItem);
                                }
                                // Если тип слота не подходит, возвращаем предмет обратно в старый слот
                                else
                                {
                                    transform.SetParent(oldSlot.transform);
                                    rectTransform.localPosition = Vector3.zero;
                                    // Если предмет был из хотбара, создаем дубликат первого предмета
                                    if (fromHot)
                                        createDuplication(firstItemGameObject);
                                }
                            }
                        }
                    }

                }

                // Проверяем, существует ли система крафта в инвентаре
                if (Inventory.GetComponent<CraftSystem>() != null)
                {
                    // Получаем компонент CraftSystem
                    CraftSystem cS = Inventory.GetComponent<CraftSystem>();
                    // Получаем количество дочерних элементов нового слота
                    int newSlotChildCount = newSlot.transform.parent.childCount;

                    // Проверяем, есть ли предмет в новом слоте
                    bool isOnSlot = newSlot.transform.parent.GetChild(0).tag == "ItemIcon";

                    // Если в новом слоте уже есть предмет
                    if (newSlotChildCount != 0 && isOnSlot)
                    {
                        // Переменная для проверки, помещается ли предмет в другой предмет
                        bool fitsIntoStack = false;

                        // Проверяем, совпадают ли предметы
                        if (sameItem)
                            fitsIntoStack = (firstItem.itemValue + secondItem.itemValue) <= firstItem.maxStack;

                        // Если предметы могут складываться, и у обоих предметов есть место для увеличения
                        if (inventory.stackable && sameItem && firstItemStack && secondItemStack)
                        {
                            // Если предмет помещается в другой предмет и они не ссылаются на один и тот же объект
                            if (fitsIntoStack && !sameItemRerferenced)
                            {
                                // Обновляем значение второго предмета, складывая его с первым
                                secondItem.itemValue = firstItem.itemValue + secondItem.itemValue;
                                // Устанавливаем родителем второго предмета новый слот
                                secondItemGameObject.transform.SetParent(newSlot.parent.parent);
                                // Удаляем первый предмет из игры
                                Destroy(firstItemGameObject);
                                // Устанавливаем локальную позицию второго предмета в ноль
                                secondItemRectTransform.localPosition = Vector3.zero;

                                // Если у второго предмета есть дубликат
                                if (secondItemGameObject.GetComponent<ConsumeItem>().duplication != null)
                                {
                                    // Получаем дубликат и обновляем его значение
                                    GameObject dup = secondItemGameObject.GetComponent<ConsumeItem>().duplication;
                                    dup.GetComponent<ItemOnObject>().item.itemValue = secondItem.itemValue;
                                    dup.GetComponent<SplitItem>().inv.stackableSettings();
                                    // Обновляем список предметов в инвентаре
                                    dup.transform.parent.parent.parent.GetComponent<Inventory>().updateItemList();
                                }
                                // Обновляем список предметов в системе крафта
                                cS.ListWithItem();
                            }

                            else
                            {
                                // Создаем остаток от сложения значений предметов
                                int rest = (firstItem.itemValue + secondItem.itemValue) % firstItem.maxStack;

                                // Если предметы не помещаются в один стек и есть остаток
                                if (!fitsIntoStack && rest > 0)
                                {
                                    // Устанавливаем значение первого предмета на максимум
                                    firstItem.itemValue = firstItem.maxStack;
                                    // Устанавливаем остаток во втором предмете
                                    secondItem.itemValue = rest;

                                    // Устанавливаем родителем первого предмета родитель второго предмета
                                    firstItemGameObject.transform.SetParent(secondItemGameObject.transform.parent);
                                    // Устанавливаем родителем второго предмета старый слот
                                    secondItemGameObject.transform.SetParent(oldSlot.transform);

                                    // Устанавливаем локальную позицию обоих предметов в ноль
                                    firstItemRectTransform.localPosition = Vector3.zero;
                                    secondItemRectTransform.localPosition = Vector3.zero;

                                    // Обновляем список предметов в системе крафта
                                    cS.ListWithItem();
                                }
                            }


                        }
                        // Если предметы не помещаются в один стек
                        else
                        {
                            // Создаем переменную для остатка
                            int rest = 0;

                            // Если предметы одинаковые, вычисляем остаток от сложения их значений
                            if (sameItem)
                                rest = (firstItem.itemValue + secondItem.itemValue) % firstItem.maxStack;

                            // Если предметы не помещаются в один стек и есть остаток
                            if (!fitsIntoStack && rest > 0)
                            {
                                // Устанавливаем значение второго предмета на максимум
                                secondItem.itemValue = firstItem.maxStack;
                                // Устанавливаем остаток в первом предмете
                                firstItem.itemValue = rest;

                                // Устанавливаем родителем первого предмета родитель второго предмета
                                firstItemGameObject.transform.SetParent(secondItemGameObject.transform.parent);
                                // Устанавливаем родителем второго предмета старый слот
                                secondItemGameObject.transform.SetParent(oldSlot.transform);

                                // Устанавливаем локальную позицию обоих предметов в ноль
                                firstItemRectTransform.localPosition = Vector3.zero;
                                secondItemRectTransform.localPosition = Vector3.zero;

                                // Обновляем список предметов в системе крафта
                                cS.ListWithItem();
                            }
                            // Если предметы разные или стек полон, они меняются местами
                            else if (!fitsIntoStack && rest == 0)
                            {
                                // Проверка: перетаскивается ли предмет из системы экипировки в инвентарь и попытка обменять его на предмет того же типа
                                if (oldSlot.transform.parent.parent.GetComponent<EquipmentSystem>() != null && firstItem.itemType == secondItem.itemType)
                                {
                                    // Меняем местами предметы в иерархии объектов
                                    firstItemGameObject.transform.SetParent(secondItemGameObject.transform.parent);
                                    secondItemGameObject.transform.SetParent(oldSlot.transform);

                                    // Сбрасываем локальные позиции предметов
                                    secondItemRectTransform.localPosition = Vector3.zero;
                                    firstItemRectTransform.localPosition = Vector3.zero;

                                    // Экипируем второй предмет
                                    oldSlot.transform.parent.parent.GetComponent<Inventory>().EquiptItem(secondItem);
                                    // Снимаем экипировку с первого предмета
                                    newSlot.transform.parent.parent.parent.parent.GetComponent<Inventory>().UnEquipItem1(firstItem);
                                }
                                // Если перетаскивается предмет из системы экипировки в инвентарь и они не одного типа, обмен не происходит
                                else if (oldSlot.transform.parent.parent.GetComponent<EquipmentSystem>() != null && firstItem.itemType != secondItem.itemType)
                                {
                                    // Возвращаем первый предмет в старый слот
                                    firstItemGameObject.transform.SetParent(oldSlot.transform);
                                    firstItemRectTransform.localPosition = Vector3.zero;
                                }
                                // Обмен происходит для остальных слотов инвентаря
                                else if (oldSlot.transform.parent.parent.GetComponent<EquipmentSystem>() == null)
                                {
                                    // Меняем местами предметы в инвентаре
                                    firstItemGameObject.transform.SetParent(secondItemGameObject.transform.parent);
                                    secondItemGameObject.transform.SetParent(oldSlot.transform);

                                    // Сбрасываем локальные позиции предметов
                                    secondItemRectTransform.localPosition = Vector3.zero;
                                    firstItemRectTransform.localPosition = Vector3.zero;
                                }
                            }


                        }
                    }
                    else
                    {
                        // Проверка: если новый слот не является слотом инвентаря и не иконкой предмета
                        if (newSlot.tag != "Slot" && newSlot.tag != "ItemIcon")
                        {
                            // Возвращаем первый предмет в старый слот, если новый слот недопустим
                            firstItemGameObject.transform.SetParent(oldSlot.transform);
                            firstItemRectTransform.localPosition = Vector3.zero;
                        }
                        else
                        {
                            // Перемещаем первый предмет в новый слот
                            firstItemGameObject.transform.SetParent(newSlot.transform);
                            firstItemRectTransform.localPosition = Vector3.zero;

                            // Проверка: если новый слот не принадлежит системе экипировки, а старый принадлежит
                            if (newSlot.transform.parent.parent.GetComponent<EquipmentSystem>() == null && oldSlot.transform.parent.parent.GetComponent<EquipmentSystem>() != null)
                                // Снимаем экипировку с первого предмета
                                oldSlot.transform.parent.parent.GetComponent<Inventory>().UnEquipItem1(firstItem);
                        }
                    }

                }


            }

            else
            {
                // Создание нового объекта предмета, который будет сброшен на землю
                GameObject dropItem = (GameObject)Instantiate(GetComponent<ItemOnObject>().item.itemModel);

                // Добавление компонента PickUpItem к новому объекту
                dropItem.AddComponent<PickUpItem>();

                // Установка ссылки на объект предмета в компоненте PickUpItem
                dropItem.GetComponent<PickUpItem>().item = this.gameObject.GetComponent<ItemOnObject>().item;

                // Установка позиции нового предмета на позицию игрока
                dropItem.transform.localPosition = GameObject.FindGameObjectWithTag("Player").transform.localPosition;

                // Обновление списка предметов в инвентаре
                inventory.OnUpdateItemList();

                // Если старый слот принадлежит системе экипировки, снимаем предмет с экипировки
                if (oldSlot.transform.parent.parent.GetComponent<EquipmentSystem>() != null)
                    inventory.GetComponent<Inventory>().UnEquipItem1(dropItem.GetComponent<PickUpItem>().item);
                // Удаляем текущий объект (предмет), который был перемещен
                Destroy(this.gameObject);

            }
        }
        // Обновление списка предметов в инвентаре после всех изменений
        inventory.OnUpdateItemList();
    }

}

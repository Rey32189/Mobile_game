using UnityEngine; // Подключение пространства имен UnityEngine, которое содержит основные классы и функции для работы с Unity
using System.Collections; // Подключение пространства имен для работы с коллекциями и коллекциями данных
using UnityEngine.UI; // Подключение пространства имен для работы с элементами пользовательского интерфейса (UI) в Unity
using UnityEngine.EventSystems; // Подключение пространства имен для работы с событиями ввода (например, клики мыши, касания)
using static UnityEditor.Progress;

public class DragItem : MonoBehaviour, IDragHandler, IPointerDownHandler, IEndDragHandler // Определение класса DragItem, который наследует MonoBehaviour и реализует интерфейсы для обработки событий перетаскивания
{
    private Vector2 pointerOffset; // Хранит смещение указателя от центра объекта при перетаскивании
    private RectTransform rectTransform; // Ссылка на RectTransform текущего объекта для управления его положением
    private RectTransform rectTransformSlot; // Ссылка на RectTransform слота, в который будет помещен перетаскиваемый объект
    private CanvasGroup canvasGroup; // Ссылка на CanvasGroup для управления видимостью и взаимодействием с объектом
    private GameObject oldSlot; // Ссылка на предыдущий слот, в котором находился объект
    private Inventory inventory; // Ссылка на компонент Inventory, который управляет инвентарем
    private Transform draggedItemBox; // Ссылка на объект, в который будет помещен перетаскиваемый элемент

    

    public PlayerInventory playerInventory; // Ссылка на PlayerInventory
    public delegate void ItemEventHandler(Item item); // Делегат для события
    public static event ItemEventHandler ItemPickedUp; // Событие, которое будет вызвано, когда предмет будет взят
    private Orugie orugie; // ссылка на код с оружием

    public delegate void ItemDelegate(); // Определение делегата для обновления списка инвентаря
    public static event ItemDelegate updateInventoryList; // Объявление статического события, которое будет вызываться для обновления списка инвентаря


    bool IsAmmoInventorySlot(Transform slot) // это метод для того, что бы сразу определять тег инвентаря боеприпасов при перетаскивании предметов
    {
        Transform parent = slot.parent;
        while (parent != null)
        {
            if (parent.CompareTag("AmmoInventory"))
            {
                return true;
            }
            parent = parent.parent;
        }
        return false;
    }


    void Start() // Метод, который вызывается при инициализации объекта
    {
        
        rectTransform = GetComponent<RectTransform>(); // Получение компонента RectTransform текущего объекта
        canvasGroup = GetComponent<CanvasGroup>(); // Получение компонента CanvasGroup текущего объекта
        rectTransformSlot = GameObject.FindGameObjectWithTag("DraggingItem").GetComponent<RectTransform>(); // Поиск объекта с тегом "DraggingItem" и получение его RectTransform
        inventory = transform.parent.parent.parent.GetComponent<Inventory>(); // Получение компонента Inventory из родительского объекта
        draggedItemBox = GameObject.FindGameObjectWithTag("DraggingItem").transform; // Получение трансформа объекта с тегом "DraggingItem"


        // Убедитесь, что playerInventory ссылается на правильный объект
        if (playerInventory == null)
        {
            playerInventory = FindObjectOfType<PlayerInventory>();
        }
        if (orugie == null)
        {
            orugie = FindObjectOfType<Orugie>(); // Ищем объект Orugie в сцене
        }
        
    }

    public void OnDrag(PointerEventData data) // Метод, который вызывается при перетаскивании объекта
    {
        if (rectTransform == null) // Проверка, инициализирован ли RectTransform
            return; // Если нет, выход из метода

        if (data.button == PointerEventData.InputButton.Left && transform.parent.GetComponent<CraftResultSlot>() == null) // Проверка, что нажата левая кнопка мыши и объект не находится в слоте для результата крафта
        {
            rectTransform.SetAsLastSibling(); // Перемещение RectTransform в конец списка дочерних объектов
            transform.SetParent(draggedItemBox); // Установка родителем для объекта - draggedItemBox
            Vector2 localPointerPosition; // Переменная для хранения локальной позиции указателя
            canvasGroup.blocksRaycasts = false; // Отключение блокировки лучей для взаимодействия с другими элементами UI
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransformSlot, Input.mousePosition, data.pressEventCamera, out localPointerPosition)) // Преобразование координат экрана в локальные координаты RectTransform слота
            {
                rectTransform.localPosition = localPointerPosition - pointerOffset; // Установка локальной позиции RectTransform с учетом смещения указателя
                if (oldSlot.transform.parent.parent.CompareTag("AmmoInventory")) // проверяем, был ли взят предмет из аммоинвентаря
                {
                    if (orugie.currentAmmo != 0 )
                    {
                        //Debug.Log("взяли обойму в руки");
                        Item item = GetComponent<ItemOnObject>().item;
                        ItemPickedUp?.Invoke(item);
                       
                    }
                   

                }
                if (transform.GetComponent<ConsumeItem>().duplication != null) // Проверка на наличие дубликата у ConsumeItem
                    Destroy(transform.GetComponent<ConsumeItem>().duplication); // Уничтожение дубликата, если он существует
            }
        }

        inventory.OnUpdateItemList(); // Вызов метода обновления списка предметов в инвентаре
    }



    public void OnPointerDown(PointerEventData data) // Метод, который вызывается при нажатии указателя на объект
    {
        if (data.button == PointerEventData.InputButton.Left) // Проверка, что нажата левая кнопка мыши
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, data.position, data.pressEventCamera, out pointerOffset); // Преобразование координат экрана в локальные координаты прямоугольника
            oldSlot = transform.parent.gameObject; // Сохранение ссылки на родительский объект (слот инвентаря)

        
        }
        if (updateInventoryList != null) // Проверка, что событие обновления инвентаря не равно null
            updateInventoryList(); // Вызов события обновления инвентаря
    }

    public void createDuplication(GameObject Item) // Метод для создания дубликата предмета
    {
        Item item = Item.GetComponent<ItemOnObject>().item; // Получение компонента ItemOnObject и извлечение информации о предмете
        GameObject duplication = GameObject.FindGameObjectWithTag("MainInventory").GetComponent<Inventory>().addItemToInventory(item.itemID, item.itemValue); // Добавление предмета в инвентарь и сохранение ссылки на дубликат
        duplication.transform.parent.parent.parent.GetComponent<Inventory>().stackableSettings(); // Настройка свойств стекуемых предметов в инвентаре
        Item.GetComponent<ConsumeItem>().duplication = duplication; // Установка ссылки на дубликат в оригинальном предмете
        duplication.GetComponent<ConsumeItem>().duplication = Item; // Установка ссылки на оригинал в дубликате
    }


    public void OnEndDrag(PointerEventData data) // Метод, который вызывается, когда пользователь завершает перетаскивание объекта.
    {
        
        if (data.button == PointerEventData.InputButton.Left) // Проверка, что была нажата левая кнопка мыши.
        {
            canvasGroup.blocksRaycasts = true; // Включение блокировки взаимодействия с другими элементами интерфейса.
            Transform newSlot = null; // Объявление переменной для хранения нового слота, в который будет перемещен предмет.
            if (data.pointerEnter != null) // Проверка, что указатель на что-то указывает.
                newSlot = data.pointerEnter.transform; // Присваивание нового слота на основе объекта, на который указывает указатель.

            if (newSlot != null) // Проверка, что новый слот был найден.
            {
                
                // Получение предметов из слотов, GameObjects и RectTransform
                GameObject firstItemGameObject = this.gameObject; // Ссылка на текущий объект (предмет, который перетаскивается).
                GameObject secondItemGameObject = newSlot.parent.gameObject; // Ссылка на объект второго слота (куда будет перемещен предмет).
                RectTransform firstItemRectTransform = this.gameObject.GetComponent<RectTransform>(); // Получение RectTransform текущего предмета.
                RectTransform secondItemRectTransform = newSlot.parent.GetComponent<RectTransform>(); // Получение RectTransform второго предмета.
                Item firstItem = rectTransform.GetComponent<ItemOnObject>().item; // Получение информации о первом предмете.
                Item secondItem = new Item(); // Создание нового экземпляра второго предмета.


                if (newSlot.parent.GetComponent<ItemOnObject>() != null) // Проверка, есть ли компонент ItemOnObject у второго слота.
                    secondItem = newSlot.parent.GetComponent<ItemOnObject>().item; // Получение информации о втором предмете, если он существует.

                // Получить информацию о двух предметах
                bool sameItem = firstItem.itemName == secondItem.itemName; // Проверка, имеют ли предметы одинаковое имя.
                bool sameItemRerferenced = firstItem.Equals(secondItem); // Проверка, ссылаются ли оба предмета на один и тот же объект.
                bool secondItemStack = false; // Переменная для отслеживания, может ли второй предмет быть сложен.
                bool firstItemStack = false; // Переменная для отслеживания, может ли первый предмет быть сложен.

                // Проверка, является ли новый слот инвентарем боеприпасов
                bool isAmmoInventory = IsAmmoInventorySlot(newSlot.transform);
                //if (ShouldCreateAmmoDuplication()) // это если вытаскиваем обойму из аммоинвенторя если там еще остались патроны
                //{
                //    CreateAmmoDuplication(this.gameObject); // Создаем дубликат обоймы с нулевым значением
                //}
                if (sameItem) // Если предметы одинаковые.
                    {
                        firstItemStack = firstItem.itemValue < firstItem.maxStack; // Проверка, можно ли сложить первый предмет.
                        secondItemStack = secondItem.itemValue < secondItem.maxStack; // Проверка, можно ли сложить второй предмет.
                    }

                    GameObject Inventory = secondItemRectTransform.parent.gameObject; // Получение родительского объекта второго предмета как инвентаря.
                    if (Inventory.tag == "Slot") // Проверка, является ли родительский объект слотом.
                        Inventory = secondItemRectTransform.parent.parent.parent.gameObject; // Переход к родительскому инвентарю, если это слот.

                    if (Inventory.tag.Equals("Slot")) // Проверка, является ли инвентарь слотом.
                        Inventory = Inventory.transform.parent.parent.gameObject; // Переход к родительскому инвентарю, если это слот.

               
                // перетаскивание в инвентаре".    
                if (Inventory.GetComponent<Hotbar>() == null && Inventory.GetComponent<EquipmentSystem>() == null && Inventory.GetComponent<CraftSystem>() == null ) // Проверка, что инвентарь не является горячей панелью, системой оборудования или системой крафта.
                    {
                        //вы не можете прикреплять предметы к результирующему слоту системы крафта
                        if (newSlot.transform.parent.tag == "ResultSlot" || newSlot.transform.tag == "ResultSlot" || newSlot.transform.parent.parent.tag == "ResultSlot") // Проверка, является ли новый слот слотом результата крафта.
                        {
                            firstItemGameObject.transform.SetParent(oldSlot.transform); // Возвращение предмета обратно в старый слот, если он был перемещен в слот результата.
                            firstItemRectTransform.localPosition = Vector3.zero; // Сброс позиции предмета к началу относительно старого слота.
                        }
                    

                        else // Начало блока else, который выполняется, если условие выше не выполнено
                        {
                            int newSlotChildCount = newSlot.transform.parent.childCount; // Получаем количество дочерних объектов у родителя нового слота
                            bool isOnSlot = newSlot.transform.parent.GetChild(0).tag == "ItemIcon"; // Проверяем, есть ли уже предмет в новом слоте
                                                                                                    // перетаскивание на слот, где уже есть предмет

                        if (newSlotChildCount != 0 && isOnSlot) // Если в новом слоте есть предмет
                            {
                            // Проверка на инвентарь боеприпасов
                            bool isFirstItemAmmo = firstItem.itemType == ItemType.Ammo; // Проверка, является ли первый предмет боеприпасом.
                            bool isSecondItemAmmo = secondItem.itemType == ItemType.Ammo; // Проверка, является ли второй предмет боеприпасом.
                            Debug.Log("isFirstItemAmmo: " + isFirstItemAmmo);
                            Debug.Log("isSecondItemAmmo: " + isSecondItemAmmo);
                            // Если хотя бы один из предметов не является боеприпасом, отменяем замену
                            if (isAmmoInventory)
                            {
                                if (!isFirstItemAmmo || !isSecondItemAmmo)
                                {
                                    firstItemGameObject.transform.SetParent(oldSlot.transform); // Возвращаем первый предмет в старый слот.
                                    firstItemRectTransform.localPosition = Vector3.zero; // Сбрасываем позицию первого предмета.
                                    if (secondItemGameObject != null)
                                    {
                                        secondItemGameObject.transform.SetParent(newSlot.transform); // Возвращаем второй предмет в новый слот.
                                        secondItemRectTransform.localPosition = Vector3.zero; // Сбрасываем позицию второго предмета.
                                    }
                                    return; // Прерываем выполнение метода.

                                }
                            }
                            // проверяем, помещается ли один предмет в другой
                            bool fitsIntoStack = false; // Переменная для проверки, помещается ли один предмет в другой
                                if (sameItem) // Если предметы одинаковые
                                    fitsIntoStack = (firstItem.itemValue + secondItem.itemValue) <= firstItem.maxStack; // Проверяем, помещаются ли они в один стек
                            // если предмет стекуемый, проверяем, что стеки первого и второго предметов не полные и что они одинаковые
                            if (inventory.stackable && sameItem && firstItemStack && secondItemStack) // Если инвентарь позволяет стекать предметы и оба предмета одинаковы
                                {
                                    // если предмет не помещается в другой предмет
                                    if (fitsIntoStack && !sameItemRerferenced) // Если предметы помещаются в один стек и ссылки на них разные
                                    {
                                        secondItem.itemValue = firstItem.itemValue + secondItem.itemValue; // Обновляем количество предметов во втором предмете
                                        secondItemGameObject.transform.SetParent(newSlot.parent.parent); // Устанавливаем второй предмет в родительский объект нового слота
                                        Destroy(firstItemGameObject); // Удаляем первый предмет
                                        secondItemRectTransform.localPosition = Vector3.zero; // Устанавливаем позицию второго предмета в ноль
                                        if (secondItemGameObject.GetComponent<ConsumeItem>().duplication != null) // Проверяем, есть ли дубликат у второго предмета
                                        {
                                            GameObject dup = secondItemGameObject.GetComponent<ConsumeItem>().duplication; // Получаем объект дубликата
                                            dup.GetComponent<ItemOnObject>().item.itemValue = secondItem.itemValue; // Обновляем значение предмета в дубликате
                                            dup.GetComponent<SplitItem>().inv.stackableSettings(); // Применяем настройки стекания к инвентарю дубликата
                                            dup.transform.parent.parent.parent.GetComponent<Inventory>().updateItemList(); // Обновляем список предметов в инвентаре дубликата
                                        }
                                    }
                                    else // Если предмет не помещается в другой предмет
                                    {
                                        // создаем остаток предмета
                                        int rest = (firstItem.itemValue + secondItem.itemValue) % firstItem.maxStack; // Вычисляем остаток предметов

                                        // заполняем другой стек и добавляем остаток в другой стек 
                                        if (!fitsIntoStack && rest > 0) // Если предметы не помещаются в один стек и остаток больше нуля
                                        {
                                            firstItem.itemValue = firstItem.maxStack; // Устанавливаем максимальное значение для первого предмета
                                            secondItem.itemValue = rest; // Устанавливаем остаток для второго предмета

                                            firstItemGameObject.transform.SetParent(secondItemGameObject.transform.parent); // Перемещаем первый предмет в родительский объект второго
                                            secondItemGameObject.transform.SetParent(oldSlot.transform); // Возвращаем второй предмет в старый слот

                                            firstItemRectTransform.localPosition = Vector3.zero; // Устанавливаем позицию первого предмета в ноль
                                            secondItemRectTransform.localPosition = Vector3.zero; // Устанавливаем позицию второго предмета в ноль
                                        }
                                    }
                                }
                                // если не помещается // Конец блока, обрабатывающего случай, когда предметы не помещаются в один стек
                                else
                                {
                                    // создает остаток предмета
                                    int rest = 0; // инициализация переменной остатка
                                    if (sameItem) // если предметы одинаковые
                                        rest = (firstItem.itemValue + secondItem.itemValue) % firstItem.maxStack; // вычисляем остаток от сложения значений предметов

                                    //fill up the other stack and adds the rest to the other stack // заполняет другой стек и добавляет остаток в другой стек
                                    if (!fitsIntoStack && rest > 0) // если не помещается в стек и есть остаток
                                    {
                                        secondItem.itemValue = firstItem.maxStack; // устанавливаем максимальное значение для второго предмета
                                        firstItem.itemValue = rest; // обновляем значение первого предмета на остаток

                                        firstItemGameObject.transform.SetParent(secondItemGameObject.transform.parent); // перемещаем первый предмет под родитель второго предмета
                                        secondItemGameObject.transform.SetParent(oldSlot.transform); // перемещаем второй предмет обратно в старый слот

                                        firstItemRectTransform.localPosition = Vector3.zero; // устанавливаем локальную позицию первого предмета
                                        secondItemRectTransform.localPosition = Vector3.zero; // устанавливаем локальную позицию второго предмета
                                    }
                                    // если предметы разные или стек полон, они меняются местами
                                    else if (!fitsIntoStack && rest == 0) // если не помещается в стек и остатка нет
                                    {
                                        // если вы перетаскиваете предмет из системы экипировки в инвентарь и пытаетесь поменять его с предметом того же типа
                                        if (oldSlot.transform.parent.parent.GetComponent<EquipmentSystem>() != null && firstItem.itemType == secondItem.itemType) // если старый слот принадлежит системе экипировки и типы предметов совпадают
                                        {
                                            newSlot.transform.parent.parent.parent.parent.GetComponent<Inventory>().UnEquipItem1(firstItem); // снимаем первый предмет с экипировки
                                            oldSlot.transform.parent.parent.GetComponent<Inventory>().EquiptItem(secondItem); // одеваем второй предмет на экипировку

                                            firstItemGameObject.transform.SetParent(secondItemGameObject.transform.parent); // перемещаем первый предмет под родитель второго предмета
                                            secondItemGameObject.transform.SetParent(oldSlot.transform); // перемещаем второй предмет обратно в старый слот
                                            secondItemRectTransform.localPosition = Vector3.zero; // устанавливаем локальную позицию второго предмета
                                            firstItemRectTransform.localPosition = Vector3.zero; // устанавливаем локальную позицию первого предмета

                                            if (secondItemGameObject.GetComponent<ConsumeItem>().duplication != null) // если у второго предмета есть дубликат
                                                Destroy(secondItemGameObject.GetComponent<ConsumeItem>().duplication); // уничтожаем дубликат второго предмета
                                        }
                                        // если вы перетаскиваете предмет из системы экипировки в инвентарь и они не одного типа, они не меняются местами.
                                        else if (oldSlot.transform.parent.parent.GetComponent<EquipmentSystem>() != null && firstItem.itemType != secondItem.itemType) // если старый слот принадлежит системе экипировки и типы предметов не совпадают
                                        {
                                            firstItemGameObject.transform.SetParent(oldSlot.transform); // перемещаем первый предмет обратно в старый слот
                                            firstItemRectTransform.localPosition = Vector3.zero; // устанавливаем локальную позицию первого предмета
                                        }
                                        // swapping for the rest of the inventorys // обмен для остальных предметов в инвентаре
                                        else if (oldSlot.transform.parent.parent.GetComponent<EquipmentSystem>() == null) // если старый слот не принадлежит системе экипировки
                                        {

                                        if (isAmmoInventory)
                                        {

                                            if (orugie.currentAmmo != 0)
                                            {
                                                // Получаем старую обойму (ту, которая была в слоте до замены)
                                                // Вызываем метод OnItemPickedUp для старой обоймы
                                                ItemPickedUp?.Invoke(secondItem);
                                            }
                                        }
                                            firstItemGameObject.transform.SetParent(secondItemGameObject.transform.parent); // перемещаем первый предмет под родитель второго предмета
                                            secondItemGameObject.transform.SetParent(oldSlot.transform); // перемещаем второй предмет обратно в старый слот
                                            secondItemRectTransform.localPosition = Vector3.zero; // устанавливаем локальную позицию второго предмета
                                            firstItemRectTransform.localPosition = Vector3.zero; // устанавливаем локальную позицию первого предмета
                                        }
                                    }
                                }
                            }

                            //empty slot // Пустой слот
                            else // Иначе
                            {
                            if (isAmmoInventory)
                            {
                                if(firstItem.itemType != ItemType.Ammo)
                                {
                                    // Если это инвентарь боеприпасов, но предмет не является боеприпасом, возвращаем
                                    firstItemGameObject.transform.SetParent(oldSlot.transform);
                                    firstItemRectTransform.localPosition = Vector3.zero;
                                    return;
                                }
                               
                            }
                            if (newSlot.tag != "Slot" && newSlot.tag != "ItemIcon") // Если новый слот не является слотом и не является иконкой предмета
                                {
                                    firstItemGameObject.transform.SetParent(oldSlot.transform); // Устанавливаем родителем первого предмета старый слот
                                    firstItemRectTransform.localPosition = Vector3.zero; // Устанавливаем локальную позицию первого предмета в ноль
                                }
                                else // Иначе
                                {
                                    firstItemGameObject.transform.SetParent(newSlot.transform); // Устанавливаем родителем первого предмета новый слот
                                    firstItemRectTransform.localPosition = Vector3.zero; // Устанавливаем локальную позицию первого предмета в ноль

                                    if (newSlot.transform.parent.parent.GetComponent<EquipmentSystem>() == null && oldSlot.transform.parent.parent.GetComponent<EquipmentSystem>() != null) // Если новый слот не находится в системе экипировки, а старый находится
                                        oldSlot.transform.parent.parent.GetComponent<Inventory>().UnEquipItem1(firstItem); // Снимаем экипировку с первого предмета в старом слоте
                                }
                            }
                        }
                    }

                // перетаскивание в горячую панель
                if (Inventory.GetComponent<Hotbar>() != null) // проверка, есть ли компонент Hotbar в инвентаре
                    {
                        int newSlotChildCount = newSlot.transform.parent.childCount; // получаем количество дочерних элементов нового слота
                        bool isOnSlot = newSlot.transform.parent.GetChild(0).tag == "ItemIcon"; // проверяем, есть ли предмет в новом слоте
                                                                                                // перетаскивание на слот, где уже есть предмет
                        if (newSlotChildCount != 0 && isOnSlot) // если в слоте есть предмет
                        {
                            // проверяем, помещается ли предмет в другой предмет
                            bool fitsIntoStack = false; // инициализируем переменную для проверки возможности стекания
                            if (sameItem) // если предметы одинаковые
                                fitsIntoStack = (firstItem.itemValue + secondItem.itemValue) <= firstItem.maxStack; // проверяем, помещаются ли предметы в стек по максимальному количеству

                            // if the item is stackable checking if the first item stack and second item stack is not full and check if they are the same items
                            if (inventory.stackable && sameItem && firstItemStack && secondItemStack) // если предметы стекуемые и оба стека не полные
                            {
                                // если предмет не помещается в другой предмет
                                if (fitsIntoStack && !sameItemRerferenced) // если помещается в стек и это не тот же предмет
                                {
                                    secondItem.itemValue = firstItem.itemValue + secondItem.itemValue; // обновляем количество предметов во втором предмете
                                    secondItemGameObject.transform.SetParent(newSlot.parent.parent); // устанавливаем родителя для второго предмета
                                    Destroy(firstItemGameObject); // уничтожаем первый предмет
                                    secondItemRectTransform.localPosition = Vector3.zero; // сбрасываем позицию второго предмета
                                    if (secondItemGameObject.GetComponent<ConsumeItem>().duplication != null) // проверяем, есть ли дубликат у второго предмета
                                    {
                                        GameObject dup = secondItemGameObject.GetComponent<ConsumeItem>().duplication; // получаем дубликат
                                        dup.GetComponent<ItemOnObject>().item.itemValue = secondItem.itemValue; // обновляем значение предмета дубликата
                                        Inventory.GetComponent<Inventory>().stackableSettings(); // обновляем настройки стекуемых предметов в инвентаре
                                        dup.transform.parent.parent.parent.GetComponent<Inventory>().updateItemList(); // обновляем список предметов в инвентаре
                                    }
                                }

                                else
                                {
                                    // создаёт остаток предмета
                                    int rest = (firstItem.itemValue + secondItem.itemValue) % firstItem.maxStack;

                                    // заполняет другой стек и добавляет остаток в другой стек
                                    if (!fitsIntoStack && rest > 0)
                                    {
                                        firstItem.itemValue = firstItem.maxStack; // устанавливает значение первого предмета в максимальное количество
                                        secondItem.itemValue = rest; // устанавливает остаток для второго предмета

                                        firstItemGameObject.transform.SetParent(secondItemGameObject.transform.parent); // перемещает объект первого предмета в родительский объект второго предмета
                                        secondItemGameObject.transform.SetParent(oldSlot.transform); // перемещает объект второго предмета в старый слот

                                        firstItemRectTransform.localPosition = Vector3.zero; // устанавливает локальную позицию первого предмета в ноль
                                        secondItemRectTransform.localPosition = Vector3.zero; // устанавливает локальную позицию второго предмета в ноль

                                        createDuplication(this.gameObject); // создаёт дубликат текущего объекта
                                        secondItemGameObject.GetComponent<ConsumeItem>().duplication.GetComponent<ItemOnObject>().item = secondItem; // присваивает второй предмет дубликату
                                        secondItemGameObject.GetComponent<SplitItem>().inv.stackableSettings(); // обновляет настройки стекуемости инвентаря
                                    }
                                }

                            }
                            // если не помещается
                            else
                            {
                                // создаёт остаток предмета
                                int rest = 0; // инициализирует остаток как 0
                                if (sameItem) // если предметы одинаковые
                                    rest = (firstItem.itemValue + secondItem.itemValue) % firstItem.maxStack; // вычисляет остаток

                                bool fromEquip = oldSlot.transform.parent.parent.GetComponent<EquipmentSystem>() != null; // проверяет, является ли родитель старого слота системой экипировки

                                // заполняет другой стек и добавляет остаток в другой стек
                                if (!fitsIntoStack && rest > 0)
                                {
                                    secondItem.itemValue = firstItem.maxStack; // устанавливает значение второго предмета в максимальное количество
                                    firstItem.itemValue = rest; // устанавливает остаток для первого предмета

                                    createDuplication(this.gameObject); // создаёт дубликат текущего объекта

                                    firstItemGameObject.transform.SetParent(secondItemGameObject.transform.parent); // перемещает объект первого предмета в родительский объект второго предмета
                                    secondItemGameObject.transform.SetParent(oldSlot.transform); // перемещает объект второго предмета в старый слот

                                    firstItemRectTransform.localPosition = Vector3.zero; // устанавливает локальную позицию первого предмета в ноль
                                    secondItemRectTransform.localPosition = Vector3.zero; // устанавливает локальную позицию второго предмета в ноль
                                }

                                else if (!fitsIntoStack && rest == 0) // если предметы не помещаются в стек и остатка нет
                                {
                                    if (!fromEquip) // если предмет не из экипировки
                                    {
                                        firstItemGameObject.transform.SetParent(secondItemGameObject.transform.parent); // перемещаем первый предмет в родитель второго предмета
                                        secondItemGameObject.transform.SetParent(oldSlot.transform); // перемещаем второй предмет в старый слот
                                        secondItemRectTransform.localPosition = Vector3.zero; // сбрасываем локальную позицию второго предмета
                                        firstItemRectTransform.localPosition = Vector3.zero; // сбрасываем локальную позицию первого предмета

                                        if (oldSlot.transform.parent.parent.gameObject.Equals(GameObject.FindGameObjectWithTag("MainInventory"))) // если старый слот в главном инвентаре
                                        {
                                            Destroy(secondItemGameObject.GetComponent<ConsumeItem>().duplication); // уничтожаем дубликат второго предмета
                                            createDuplication(firstItemGameObject); // создаем дубликат первого предмета
                                        }
                                        else // если это не главный инвентарь
                                        {
                                            createDuplication(firstItemGameObject); // создаем дубликат первого предмета
                                        }
                                    }
                                    else // если предмет из экипировки
                                    {
                                        firstItemGameObject.transform.SetParent(oldSlot.transform); // перемещаем первый предмет в старый слот
                                        firstItemRectTransform.localPosition = Vector3.zero; // сбрасываем локальную позицию первого предмета
                                    }
                                }

                            }
                        }

                        else // если слот пустой
                        {
                            if (newSlot.tag != "Slot" && newSlot.tag != "ItemIcon") // если новый слот не является слотом или иконкой предмета
                            {
                                firstItemGameObject.transform.SetParent(oldSlot.transform); // перемещаем первый предмет в старый слот
                                firstItemRectTransform.localPosition = Vector3.zero; // сбрасываем локальную позицию первого предмета
                            }
                            else // если новый слот является слотом или иконкой предмета
                            {
                                firstItemGameObject.transform.SetParent(newSlot.transform); // перемещаем первый предмет в новый слот
                                firstItemRectTransform.localPosition = Vector3.zero; // сбрасываем локальную позицию первого предмета

                                if (newSlot.transform.parent.parent.GetComponent<EquipmentSystem>() == null && oldSlot.transform.parent.parent.GetComponent<EquipmentSystem>() != null) // если новый слот не в системе экипировки, а старый в системе экипировки
                                    oldSlot.transform.parent.parent.GetComponent<Inventory>().UnEquipItem1(firstItem); // снимаем предмет с экипировки
                                createDuplication(firstItemGameObject); // создаем дубликат первого предмета
                            }
                        }
                    }


                    // перетаскивание в систему экипировки/систему персонажа
                    if (Inventory.GetComponent<EquipmentSystem>() != null) // проверка, есть ли компонент EquipmentSystem в инвентаре
                    {
                        ItemType[] itemTypeOfSlots = GameObject.FindGameObjectWithTag("EquipmentSystem").GetComponent<EquipmentSystem>().itemTypeOfSlots; // получение типов предметов в слотах системы экипировки
                        int newSlotChildCount = newSlot.transform.parent.childCount; // подсчет количества дочерних объектов в новом слоте
                        bool isOnSlot = newSlot.transform.parent.GetChild(0).tag == "ItemIcon"; // проверка, находится ли предмет в слоте (с тегом "ItemIcon")
                        bool sameItemType = firstItem.itemType == secondItem.itemType; // проверка, совпадают ли типы предметов
                        bool fromHot = oldSlot.transform.parent.parent.GetComponent<Hotbar>() != null; // проверка, является ли старый слот частью горячей панели

                        // перетаскивание на слот, где уже есть предмет
                        if (newSlotChildCount != 0 && isOnSlot) // если в новом слоте есть предмет и он является слотом
                        {
                            // предметы меняются местами, если они одного типа
                            if (sameItemType && !sameItemRerferenced) // если типы предметов совпадают и они не ссылаются на один и тот же объект
                            {
                                Transform temp1 = secondItemGameObject.transform.parent.parent.parent; // временное сохранение родителя второго предмета
                                Transform temp2 = oldSlot.transform.parent.parent; // временное сохранение родителя старого слота

                                firstItemGameObject.transform.SetParent(secondItemGameObject.transform.parent); // установка родителем первого предмета родителя второго предмета
                                secondItemGameObject.transform.SetParent(oldSlot.transform); // установка родителем второго предмета старый слот
                                secondItemRectTransform.localPosition = Vector3.zero; // сброс локальной позиции второго предмета
                                firstItemRectTransform.localPosition = Vector3.zero; // сброс локальной позиции первого предмета

                                if (!temp1.Equals(temp2)) // если временные родительские объекты не равны
                                {
                                    if (firstItem.itemType == ItemType.Weapon) // если тип первого предмета - оружие
                                    {
                                        Inventory.GetComponent<Inventory>().UnEquipItem1(secondItem); // снятие второго предмета с экипировки
                                        Inventory.GetComponent<Inventory>().EquiptItem(firstItem); // экипировка первого предмета
                                    }
                                    else // если тип первого предмета не оружие
                                    {
                                        Inventory.GetComponent<Inventory>().EquiptItem(firstItem); // экипировка первого предмета
                                        if (secondItem.itemType != ItemType.Backpack) // если второй предмет не рюкзак
                                            Inventory.GetComponent<Inventory>().UnEquipItem1(secondItem); // снятие второго предмета с экипировки
                                    }
                                }

                                if (fromHot)
                                    createDuplication(secondItemGameObject);

                            }
                            //if they are not from the same Itemtype the dragged one getting placed back
                            else // иначе
                            {
                                firstItemGameObject.transform.SetParent(oldSlot.transform); // устанавливаем родителем объекта старый слот
                                firstItemRectTransform.localPosition = Vector3.zero; // сбрасываем позицию объекта в слоте на (0, 0, 0)

                                if (fromHot) // если предмет был из горячей панели
                                    createDuplication(firstItemGameObject); // создаем дубликат предмета
                            }
                        }

                        //if the slot is empty
                        else // иначе
                        {
                            for (int i = 0; i < newSlot.parent.childCount; i++) // для каждого слота в родительском объекте нового слота
                            {
                                if (newSlot.Equals(newSlot.parent.GetChild(i))) // если новый слот совпадает с текущим слотом
                                {
                                    //checking if it is the right slot for the item
                                    if (itemTypeOfSlots[i] == transform.GetComponent<ItemOnObject>().item.itemType) // проверяем, соответствует ли тип предмета типу слота
                                    {
                                        transform.SetParent(newSlot); // устанавливаем новый слот как родителя для предмета
                                        rectTransform.localPosition = Vector3.zero; // сбрасываем позицию предмета в новом слоте на (0, 0, 0)

                                        if (!oldSlot.transform.parent.parent.Equals(newSlot.transform.parent.parent)) // если старый слот не в том же родительском объекте
                                            Inventory.GetComponent<Inventory>().EquiptItem(firstItem); // экипируем предмет в инвентаре
                                    }
                                    //else it get back to the old slot
                                    else // иначе
                                    {
                                        transform.SetParent(oldSlot.transform); // возвращаем предмет в старый слот
                                        rectTransform.localPosition = Vector3.zero; // сбрасываем позицию предмета в старом слоте на (0, 0, 0)
                                        if (fromHot) // если предмет был из горячей панели
                                            createDuplication(firstItemGameObject); // создаем дубликат предмета
                                    }
                                }
                            }
                        }
                    }

                    if (Inventory.GetComponent<CraftSystem>() != null) // Проверка, есть ли компонент CraftSystem в инвентаре
                    {
                        CraftSystem cS = Inventory.GetComponent<CraftSystem>(); // Получение компонента CraftSystem
                        int newSlotChildCount = newSlot.transform.parent.childCount; // Получение количества дочерних объектов у нового слота

                        bool isOnSlot = newSlot.transform.parent.GetChild(0).tag == "ItemIcon"; // Проверка, находится ли предмет на слоте с иконкой предмета
                                                                                                // dragging on a slot where allready is an item on
                        if (newSlotChildCount != 0 && isOnSlot) // Если в новом слоте есть предмет и он является иконкой
                        {
                            // check if the items fits into the other item
                            bool fitsIntoStack = false; // Переменная для проверки, помещается ли предмет в стек
                            if (sameItem) // Если предметы одинаковые
                                fitsIntoStack = (firstItem.itemValue + secondItem.itemValue) <= firstItem.maxStack; // Проверка, помещаются ли предметы в один стек

                            // if the item is stackable checking if the firstitemstack and seconditemstack is not full and check if they are the same items
                            if (inventory.stackable && sameItem && firstItemStack && secondItemStack) // Если предметы можно складывать и оба стека не полные
                            {
                                // if the item does not fit into the other item
                                if (fitsIntoStack && !sameItemRerferenced) // Если предмет помещается в стек и ссылки на предметы не совпадают
                                {
                                    secondItem.itemValue = firstItem.itemValue + secondItem.itemValue; // Обновление значения предмета второго стека
                                    secondItemGameObject.transform.SetParent(newSlot.parent.parent); // Установка второго предмета как дочернего нового слота
                                    Destroy(firstItemGameObject); // Удаление первого предмета
                                    secondItemRectTransform.localPosition = Vector3.zero; // Сброс позиции второго предмета

                                    if (secondItemGameObject.GetComponent<ConsumeItem>().duplication != null) // Проверка, есть ли дубликат у второго предмета
                                    {
                                        GameObject dup = secondItemGameObject.GetComponent<ConsumeItem>().duplication; // Получение дубликата
                                        dup.GetComponent<ItemOnObject>().item.itemValue = secondItem.itemValue; // Обновление значения предмета дубликата
                                        dup.GetComponent<SplitItem>().inv.stackableSettings(); // Обновление настроек стека для дубликата
                                        dup.transform.parent.parent.parent.GetComponent<Inventory>().updateItemList(); // Обновление списка предметов в инвентаре
                                    }
                                    cS.ListWithItem(); // Обновление списка предметов в CraftSystem
                                }

                                else // Если предметы не помещаются в один стек
                                {
                                    // creates the rest of the item
                                    int rest = (firstItem.itemValue + secondItem.itemValue) % firstItem.maxStack; // Вычисление остатка предмета

                                    // fill up the other stack and adds the rest to the other stack 
                                    if (!fitsIntoStack && rest > 0) // Если предметы не помещаются в один стек и есть остаток
                                    {
                                        firstItem.itemValue = firstItem.maxStack; // Установка значения первого предмета на максимум
                                        secondItem.itemValue = rest; // Установка остатка для второго предмета

                                        firstItemGameObject.transform.SetParent(secondItemGameObject.transform.parent); // Перемещение первого предмета в родитель второго предмета
                                        secondItemGameObject.transform.SetParent(oldSlot.transform); // Перемещение второго предмета обратно в старый слот

                                        firstItemRectTransform.localPosition = Vector3.zero; // Сброс позиции первого предмета
                                        secondItemRectTransform.localPosition = Vector3.zero; // Сброс позиции второго предмета
                                        cS.ListWithItem(); // Обновление списка предметов в CraftSystem
                                    }
                                }
                            }

                            //если не помещается
                            else
                            {
                                //создает остаток предмета
                                int rest = 0;
                                if (sameItem) //если предметы одинаковые
                                    rest = (firstItem.itemValue + secondItem.itemValue) % firstItem.maxStack; //вычисляет остаток от сложения значений предметов

                                //заполняет другой стек и добавляет остаток в другой стек
                                if (!fitsIntoStack && rest > 0) //если не помещается в стек и есть остаток
                                {
                                    secondItem.itemValue = firstItem.maxStack; //максимальное значение второго предмета
                                    firstItem.itemValue = rest; //остаток для первого предмета

                                    firstItemGameObject.transform.SetParent(secondItemGameObject.transform.parent); //устанавливает родителя для первого предмета
                                    secondItemGameObject.transform.SetParent(oldSlot.transform); //устанавливает родителя для второго предмета

                                    firstItemRectTransform.localPosition = Vector3.zero; //устанавливает локальную позицию первого предмета
                                    secondItemRectTransform.localPosition = Vector3.zero; //устанавливает локальную позицию второго предмета
                                    cS.ListWithItem(); //обновляет список предметов

                                }
                                //если это разные предметы или стек полный, они меняются местами
                                else if (!fitsIntoStack && rest == 0) //если не помещается в стек и остатка нет
                                {
                                    //если вы перетаскиваете предмет из системы экипировки в инвентарь и пытаетесь поменять его на предмет того же типа
                                    if (oldSlot.transform.parent.parent.GetComponent<EquipmentSystem>() != null && firstItem.itemType == secondItem.itemType) //проверка на систему экипировки и совпадение типов
                                    {

                                        firstItemGameObject.transform.SetParent(secondItemGameObject.transform.parent); //устанавливает родителя для первого предмета
                                        secondItemGameObject.transform.SetParent(oldSlot.transform); //устанавливает родителя для второго предмета
                                        secondItemRectTransform.localPosition = Vector3.zero; //устанавливает локальную позицию второго предмета
                                        firstItemRectTransform.localPosition = Vector3.zero; //устанавливает локальную позицию первого предмета

                                        oldSlot.transform.parent.parent.GetComponent<Inventory>().EquiptItem(secondItem); //экипирует второй предмет
                                        newSlot.transform.parent.parent.parent.parent.GetComponent<Inventory>().UnEquipItem1(firstItem); //снимает экипировку с первого предмета
                                    }
                                    //если вы перетаскиваете предмет из системы экипировки в инвентарь и они не одного типа, они не меняются местами.
                                    else if (oldSlot.transform.parent.parent.GetComponent<EquipmentSystem>() != null && firstItem.itemType != secondItem.itemType) //проверка на систему экипировки и несовпадение типов
                                    {
                                        firstItemGameObject.transform.SetParent(oldSlot.transform); //устанавливает родителя для первого предмета
                                        firstItemRectTransform.localPosition = Vector3.zero; //устанавливает локальную позицию первого предмета
                                    }
                                    //обмен для остальной части инвентаря
                                    else if (oldSlot.transform.parent.parent.GetComponent<EquipmentSystem>() == null) //если не система экипировки
                                    {
                                        firstItemGameObject.transform.SetParent(secondItemGameObject.transform.parent); //устанавливает родителя для первого предмета
                                        secondItemGameObject.transform.SetParent(oldSlot.transform); //устанавливает родителя для второго предмета
                                        secondItemRectTransform.localPosition = Vector3.zero; //устанавливает локальную позицию второго предмета
                                        firstItemRectTransform.localPosition = Vector3.zero; //устанавливает локальную позицию первого предмета
                                    }
                                }

                            }
                        }

                        else // В противном случае
                        {
                            if (newSlot.tag != "Slot" && newSlot.tag != "ItemIcon") // Если новый слот не является слотом или иконкой предмета
                            {
                                firstItemGameObject.transform.SetParent(oldSlot.transform); // Устанавливаем родителя для предмета на старый слот
                                firstItemRectTransform.localPosition = Vector3.zero; // Сбрасываем локальную позицию предмета в старом слоте
                            }
                            else // Иначе
                            {
                                firstItemGameObject.transform.SetParent(newSlot.transform); // Устанавливаем родителя для предмета на новый слот
                                firstItemRectTransform.localPosition = Vector3.zero; // Сбрасываем локальную позицию предмета в новом слоте

                                if (newSlot.transform.parent.parent.GetComponent<EquipmentSystem>() == null && oldSlot.transform.parent.parent.GetComponent<EquipmentSystem>() != null) // Если в новом слоте нет системы экипировки, а в старом есть
                                    oldSlot.transform.parent.parent.GetComponent<Inventory>().UnEquipItem1(firstItem); // Снимаем экипировку с предмета из старого слота
                            }
                        }
                    }
                }
            

            else // иначе
            {
                GameObject dropItem = (GameObject)Instantiate(GetComponent<ItemOnObject>().item.itemModel); // создаем новый объект dropItem на основе модели предмета
                dropItem.AddComponent<PickUpItem>(); // добавляем компонент PickUpItem к новому объекту
                dropItem.GetComponent<PickUpItem>().item = this.gameObject.GetComponent<ItemOnObject>().item; // присваиваем предмет из текущего объекта новому объекту
                dropItem.transform.localPosition = GameObject.FindGameObjectWithTag("Player").transform.localPosition; // устанавливаем позицию нового объекта на позицию игрока
                inventory.OnUpdateItemList(); // обновляем список предметов в инвентаре
                if (oldSlot.transform.parent.parent.GetComponent<EquipmentSystem>() != null) // если родительский слот имеет компонент EquipmentSystem
                    inventory.GetComponent<Inventory>().UnEquipItem1(dropItem.GetComponent<PickUpItem>().item); // снимаем предмет с экипировки
                Destroy(this.gameObject); // уничтожаем текущий объект
            }

            }
            inventory.OnUpdateItemList(); // обновляем список предметов в инвентаре
        }   

}

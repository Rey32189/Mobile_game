using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.UI;
using System.Collections.Generic;

public class CraftSystem : MonoBehaviour
{

    [SerializeField]
    public int finalSlotPositionX;
    [SerializeField]
    public int finalSlotPositionY;
    [SerializeField]
    public int leftArrowPositionX;
    [SerializeField]
    public int leftArrowPositionY;
    [SerializeField]
    public int rightArrowPositionX;
    [SerializeField]
    public int rightArrowPositionY;
    [SerializeField]
    public int leftArrowRotation;
    [SerializeField]
    public int rightArrowRotation;

    public Image finalSlotImage;
    public Image arrowImage;

    //List<CraftSlot> slots = new List<CraftSlot>();
    public List<Item> itemInCraftSystem = new List<Item>();
    public List<GameObject> itemInCraftSystemGameObject = new List<GameObject>();
    BlueprintDatabase blueprintDatabase;
    public List<Item> possibleItems = new List<Item>();
    public List<bool> possibletoCreate = new List<bool>();


    //PlayerScript PlayerstatsScript;

    // Use this for initialization
    void Start()
    {
        blueprintDatabase = (BlueprintDatabase)Resources.Load("BlueprintDatabase"); // Загружаем ресурс с именем "BlueprintDatabase" из папки Resources и приводим его к типу BlueprintDatabase.
        //playerStatsScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
    }

#if UNITY_EDITOR
    [MenuItem("Master System/Create/Craft System")] // Создает пункт меню в редакторе Unity для вызова метода menuItemCreateInventory.
    public static void menuItemCreateInventory() // Определяет статический метод, который будет вызываться при выборе пункта меню.
    {
        GameObject Canvas = null; // Инициализирует переменную для хранения ссылки на объект Canvas.
        if (GameObject.FindGameObjectWithTag("Canvas") == null) // Проверяет, существует ли объект с тегом "Canvas" в сцене.
        {
            GameObject inventory = new GameObject(); // Создает новый объект GameObject для хранения инвентаря.
            inventory.name = "Inventories"; // Устанавливает имя для нового объекта инвентаря.
            Canvas = (GameObject)Instantiate(Resources.Load("Prefabs/Canvas - Inventory") as GameObject); // Загружает префаб "Canvas - Inventory" из ресурсов и создает его экземпляр.
            Canvas.transform.SetParent(inventory.transform, true); // Устанавливает родителя для Canvas как созданный объект inventory.
            GameObject panel = (GameObject)Instantiate(Resources.Load("Prefabs/Panel - CraftSytem") as GameObject); // Загружает префаб "Panel - CraftSystem" и создает его экземпляр.
            panel.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0); // Устанавливает локальную позицию панели в (0, 0, 0).
            panel.transform.SetParent(Canvas.transform, true); // Устанавливает родителем панели созданный Canvas.
            GameObject draggingItem = (GameObject)Instantiate(Resources.Load("Prefabs/DraggingItem") as GameObject); // Загружает префаб "DraggingItem" и создает его экземпляр.
            Instantiate(Resources.Load("Prefabs/EventSystem") as GameObject); // Загружает и создает экземпляр префаба "EventSystem".
            draggingItem.transform.SetParent(Canvas.transform, true); // Устанавливает родителем draggingItem созданный Canvas.
            panel.AddComponent<CraftSystem>(); // Добавляет компонент CraftSystem к панели.
        }
        else // Если Canvas уже существует в сцене
        {
            GameObject panel = (GameObject)Instantiate(Resources.Load("Prefabs/Panel - CraftSystem") as GameObject); // Загружает префаб "Panel - CraftSystem" и создает его экземпляр.
            panel.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, true); // Устанавливает родителем панели существующий Canvas.
            panel.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0); // Устанавливает локальную позицию панели в (0, 0, 0).
            panel.AddComponent<CraftSystem>(); // Добавляет компонент CraftSystem к панели.
            DestroyImmediate(GameObject.FindGameObjectWithTag("DraggingItem")); // Удаляет объект с тегом "DraggingItem" из сцены.
            GameObject draggingItem = (GameObject)Instantiate(Resources.Load("Prefabs/DraggingItem") as GameObject); // Загружает префаб "DraggingItem" и создает его экземпляр.
            draggingItem.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, true); // Устанавливает родителем draggingItem существующий Canvas.
        }
    }

#endif

    void Update()
    {
        ListWithItem();
    }


    public void setImages()
    {
        finalSlotImage = transform.GetChild(3).GetComponent<Image>();
        arrowImage = transform.GetChild(4).GetComponent<Image>();

        Image image = transform.GetChild(5).GetComponent<Image>();
        image.sprite = arrowImage.sprite;
        image.color = arrowImage.color;
        image.material = arrowImage.material;
        image.type = arrowImage.type;
        image.fillCenter = arrowImage.fillCenter;
    }


    public void setArrowSettings()
    {
        RectTransform leftRect = transform.GetChild(4).GetComponent<RectTransform>();
        RectTransform rightRect = transform.GetChild(5).GetComponent<RectTransform>();

        leftRect.localPosition = new Vector3(leftArrowPositionX, leftArrowPositionY, 0);
        rightRect.localPosition = new Vector3(rightArrowPositionX, rightArrowPositionY, 0);

        leftRect.eulerAngles = new Vector3(0, 0, leftArrowRotation);
        rightRect.eulerAngles = new Vector3(0, 0, rightArrowRotation);
    }

    public void setPositionFinalSlot()
    {
        RectTransform rect = transform.GetChild(3).GetComponent<RectTransform>();
        rect.localPosition = new Vector3(finalSlotPositionX, finalSlotPositionY, 0);
    }

    public int getSizeX()
    {
        return (int)GetComponent<RectTransform>().sizeDelta.x;
    }

    public int getSizeY()
    {
        return (int)GetComponent<RectTransform>().sizeDelta.y;
    }

    public void backToInventory()
    {
        int length = itemInCraftSystem.Count;
        for (int i = 0; i < length; i++)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>().inventory.GetComponent<Inventory>().addItemToInventory(itemInCraftSystem[i].itemID, itemInCraftSystem[i].itemValue);
            Destroy(itemInCraftSystemGameObject[i]);
        }

        itemInCraftSystem.Clear();
        itemInCraftSystemGameObject.Clear();
    }



    public void ListWithItem()
    {
        // Открытие метода ListWithItem, который не возвращает значение и может быть вызван из других классов.
        itemInCraftSystem.Clear();
        // Очистка списка itemInCraftSystem, чтобы удалить все предыдущие элементы.
        possibleItems.Clear();
        // Очистка списка possibleItems, чтобы удалить все предыдущие возможные предметы.
        possibletoCreate.Clear();
        // Очистка списка possibletoCreate, чтобы удалить все предыдущие данные о возможных созданиях.
        itemInCraftSystemGameObject.Clear();
        // Очистка списка itemInCraftSystemGameObject, чтобы удалить все предыдущие игровые объекты.
        for (int i = 0; i < transform.GetChild(1).childCount; i++)
        {
            // Цикл по всем дочерним элементам второго дочернего объекта текущего объекта (index 1).
            Transform trans = transform.GetChild(1).GetChild(i);
            // Получение текущего дочернего объекта по индексу i.
            if (trans.childCount != 0)
            {
                // Проверка, есть ли у текущего дочернего объекта дочерние элементы.
                itemInCraftSystem.Add(trans.GetChild(0).GetComponent<ItemOnObject>().item);
                // Добавление предмета из первого дочернего элемента текущего объекта в список itemInCraftSystem.
                itemInCraftSystemGameObject.Add(trans.GetChild(0).gameObject);
                // Добавление игрового объекта из первого дочернего элемента текущего объекта в список itemInCraftSystemGameObject.
            }
        }

        for (int k = 0; k < blueprintDatabase.blueprints.Count; k++)
        {
            int amountOfTrue = 0;
            // Инициализация счетчика amountOfTrue для подсчета совпадений ингредиентов.
            for (int z = 0; z < blueprintDatabase.blueprints[k].ingredients.Count; z++)
            {
                for (int d = 0; d < itemInCraftSystem.Count; d++)
                {
                    if (blueprintDatabase.blueprints[k].ingredients[z] == itemInCraftSystem[d].itemID && blueprintDatabase.blueprints[k].amount[z] <= itemInCraftSystem[d].itemValue)
                    {
                        // Проверка, совпадает ли текущий ингредиент с предметом в системе крафта и достаточно ли у него значений
                        amountOfTrue++;
                        // Увеличение счетчика amountOfTrue на 1, если ингредиент найден.
                        break;
                    }
                }
                if (amountOfTrue == blueprintDatabase.blueprints[k].ingredients.Count)
                {
                    // Если количество совпадений равно количеству ингредиентов в чертеже...
                    possibleItems.Add(blueprintDatabase.blueprints[k].finalItem);
                    // Добавление финального предмета из чертежа в список возможных предметов.
                    possibleItems[possibleItems.Count - 1].itemValue = blueprintDatabase.blueprints[k].amountOfFinalItem;
                    // Установка значения финального предмета равным количеству, указанному в чертеже.
                    possibletoCreate.Add(true);
                    // Добавление значения true в список возможных созданий, указывая, что этот предмет можно создать.
                }
            }
        }
    }


    public void deleteItems(Item item)
    {
        for (int i = 0; i < blueprintDatabase.blueprints.Count; i++)
        {
            if (blueprintDatabase.blueprints[i].finalItem.Equals(item))
            {
                for (int k = 0; k < blueprintDatabase.blueprints[i].ingredients.Count; k++)
                {
                    for (int z = 0; z < itemInCraftSystem.Count; z++)
                    {
                        if (itemInCraftSystem[z].itemID == blueprintDatabase.blueprints[i].ingredients[k])
                        {
                            if (itemInCraftSystem[z].itemValue == blueprintDatabase.blueprints[i].amount[k])
                            {
                                itemInCraftSystem.RemoveAt(z);
                                Destroy(itemInCraftSystemGameObject[z]);
                                itemInCraftSystemGameObject.RemoveAt(z);
                                ListWithItem();
                                break;
                            }
                            else if (itemInCraftSystem[z].itemValue >= blueprintDatabase.blueprints[i].amount[k])
                            {
                                itemInCraftSystem[z].itemValue = itemInCraftSystem[z].itemValue - blueprintDatabase.blueprints[i].amount[k];
                                ListWithItem();
                                break;
                            }
                        }
                    }
                }
            }
        }
    }



}

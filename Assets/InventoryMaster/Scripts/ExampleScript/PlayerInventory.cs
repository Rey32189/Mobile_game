using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerInventory : MonoBehaviour
{




    public GameObject inventory; // Переменная для хранения ссылки на объект инвентаря
    public GameObject characterSystem; // Переменная для хранения ссылки на систему персонажа
    public GameObject craftSystem; // Переменная для хранения ссылки на систему крафта
    private Inventory craftSystemInventory; // Переменная для хранения инвентаря системы крафта
    private CraftSystem cS; // Переменная для хранения ссылки на систему крафта
    private Inventory mainInventory; // Переменная для хранения основного инвентаря
    private Inventory characterSystemInventory; // Переменная для хранения инвентаря системы персонажа
    private Tooltip toolTip; // Переменная для хранения ссылки на подсказку

    private InputManager inputManagerDatabase; // Переменная для хранения ссылки на менеджер ввода

    public GameObject HPMANACanvas; // Переменная для хранения ссылки на канвас для отображения HP и маны

    Text hpText; // Переменная для хранения текста HP
    Text manaText; // Переменная для хранения текста маны
    Image hpImage; // Переменная для хранения изображения HP
    Image manaImage; // Переменная для хранения изображения маны

    float maxHealth = 100; // Максимальное здоровье игрока
    float maxMana = 100; // Максимальная мана игрока
    float maxDamage = 0; // Максимальный урон игрока
    float maxArmor = 0; // Максимальная броня игрока

    public float currentHealth = 60; // Текущее здоровье игрока
    float currentMana = 100; // Текущая мана игрока
    float currentDamage = 0; // Текущий урон игрока
    float currentArmor = 0; // Текущая броня игрока

    int normalSize = 3; // Нормальный размер инвентаря

    public void OnEnable() // Метод, вызываемый при активации объекта
    {
        // Подписываемся на события инвентаря для обработки экипировки и разэкипировки предметов
        Inventory.ItemEquip += OnBackpack;
        Inventory.UnEquipItem += UnEquipBackpack;

        Inventory.ItemEquip += OnGearItem;
        Inventory.ItemConsumed += OnConsumeItem;
        Inventory.UnEquipItem += OnUnEquipItem;

        Inventory.ItemEquip += EquipWeapon;
        Inventory.UnEquipItem += UnEquipWeapon;
    }

    public void OnDisable() // Метод, вызываемый при деактивации объекта
    {
        // Отписываемся от событий инвентаря
        Inventory.ItemEquip -= OnBackpack;
        Inventory.UnEquipItem -= UnEquipBackpack;

        Inventory.ItemEquip -= OnGearItem;
        Inventory.ItemConsumed -= OnConsumeItem;
        Inventory.UnEquipItem -= OnUnEquipItem;

        Inventory.UnEquipItem -= UnEquipWeapon;
        Inventory.ItemEquip -= EquipWeapon;
    }

    void EquipWeapon(Item item) // Метод для экипировки оружия
    {
        if (item.itemType == ItemType.Weapon) // Проверяем, является ли предмет оружием
        {
            // Здесь будет добавление оружия, если оно было разэкипировано
        }
    }

    void UnEquipWeapon(Item item) // Метод для разэкипировки оружия
    {
        if (item.itemType == ItemType.Weapon) // Проверяем, является ли предмет оружием
        {
            // Здесь будет удаление оружия, если оно было разэкипировано
        }
    }

    void OnBackpack(Item item) // Метод, вызываемый при экипировке рюкзака
    {
        if (item.itemType == ItemType.Backpack) // Проверяем, является ли предмет рюкзаком
        {
            for (int i = 0; i < item.itemAttributes.Count; i++) // Проходим по всем атрибутам предмета
            {
                if (mainInventory == null) // Если основной инвентарь еще не инициализирован
                    mainInventory = inventory.GetComponent<Inventory>(); // Получаем компонент инвентаря

                mainInventory.sortItems(); // Сортируем предметы в инвентаре

                if (item.itemAttributes[i].attributeName == "Slots") // Если атрибут - это количество слотов
                    changeInventorySize(item.itemAttributes[i].attributeValue); // Меняем размер инвентаря
            }
        }
    }


    void UnEquipBackpack(Item item) // Метод, вызываемый при разэкипировке рюкзака
    {
        if (item.itemType == ItemType.Backpack) // Проверяем, является ли предмет рюкзаком
            changeInventorySize(normalSize); // Возвращаем инвентарь к нормальному размеру
    }

    void changeInventorySize(int size) // Метод для изменения размера инвентаря
    {
        dropTheRestItems(size); // Удаляем лишние предметы, если размер меньше текущего

        if (mainInventory == null) // Если основной инвентарь еще не инициализирован
            mainInventory = inventory.GetComponent<Inventory>(); // Получаем компонент инвентаря

        // Устанавливаем размеры инвентаря в зависимости от нового размера
        if (size == 3)
        {
            mainInventory.width = 3; // Ширина инвентаря
            mainInventory.height = 1; // Высота инвентаря
            mainInventory.updateSlotAmount(); // Обновляем количество слотов
            mainInventory.adjustInventorySize(); // Корректируем размер инвентаря
        }
        if (size == 6)
        {
            mainInventory.width = 3;
            mainInventory.height = 2;
            mainInventory.updateSlotAmount();
            mainInventory.adjustInventorySize();
        }
        else if (size == 12)
        {
            mainInventory.width = 4;
            mainInventory.height = 3;
            mainInventory.updateSlotAmount();
            mainInventory.adjustInventorySize();
        }
        else if (size == 16)
        {
            mainInventory.width = 4;
            mainInventory.height = 4;
            mainInventory.updateSlotAmount();
            mainInventory.adjustInventorySize();
        }
        else if (size == 24)
        {
            mainInventory.width = 6;
            mainInventory.height = 4;
            mainInventory.updateSlotAmount();
            mainInventory.adjustInventorySize();
        }
    }

    void dropTheRestItems(int size) // Метод для удаления лишних предметов из инвентаря
    {
        if (size < mainInventory.ItemsInInventory.Count) // Если новый размер меньше текущего количества предметов
        {
            for (int i = size; i < mainInventory.ItemsInInventory.Count; i++) // Проходим по лишним предметам
            {
                GameObject dropItem = (GameObject)Instantiate(mainInventory.ItemsInInventory[i].itemModel); // Создаем экземпляр предмета
                dropItem.AddComponent<PickUpItem>(); // Добавляем компонент для подбора предмета
                dropItem.GetComponent<PickUpItem>().item = mainInventory.ItemsInInventory[i]; // Устанавливаем предмет в компонент
                dropItem.transform.localPosition = GameObject.FindGameObjectWithTag("Player").transform.localPosition; // Устанавливаем позицию предмета
            }
        }
    }

    void Start() // Метод, вызываемый при старте игры
    {
      

        // Код для инициализации текстов и изображений HP и маны (закомментирован)
        //if (HPMANACanvas != null)
        //{
        //    hpText = HPMANACanvas.transform.GetChild(1).GetChild(0).GetComponent<Text>(); // Получаем текст HP
        //    manaText = HPMANACanvas.transform.GetChild(2).GetChild(0).GetComponent<Text>(); // Получаем текст маны
        //    hpImage = HPMANACanvas.transform.GetChild(1).GetComponent<Image>(); // Получаем изображение HP
        //    manaImage = HPMANACanvas.transform.GetChild(1).GetComponent<Image>(); // Получаем изображение маны
        //    UpdateHPBar(); // Обновляем полосу HP
        //    UpdateManaBar(); // Обновляем полосу маны
        //}

        if (inputManagerDatabase == null) // Если менеджер ввода еще не инициализирован
            inputManagerDatabase = (InputManager)Resources.Load("InputManager"); // Загружаем менеджер ввода из ресурсов

        if (craftSystem != null) // Если система крафта задана
            cS = craftSystem.GetComponent<CraftSystem>(); // Получаем компонент системы крафта

        if (GameObject.FindGameObjectWithTag("Tooltip") != null) // Если существует объект с тегом "Tooltip"
            toolTip = GameObject.FindGameObjectWithTag("Tooltip").GetComponent<Tooltip>(); // Получаем компонент подсказки

        if (inventory != null) // Если инвентарь задан
            mainInventory = inventory.GetComponent<Inventory>(); // Получаем компонент основного инвентаря

        if (characterSystem != null) // Если система персонажа задана
            characterSystemInventory = characterSystem.GetComponent<Inventory>(); // Получаем компонент инвентаря системы персонажа

        if (craftSystem != null) // Если система крафта задана
            craftSystemInventory = craftSystem.GetComponent<Inventory>(); // Получаем компонент инвентаря системы крафта
    }

    //void UpdateHPBar() // Метод для обновления полосы здоровья (закомментирован)
    //{
    //    hpText.text = (currentHealth + "/" + maxHealth); // Обновляем текст HP
    //    float fillAmount = currentHealth / maxHealth; // Рассчитываем заполненность полосы HP
    //    hpImage.fillAmount = fillAmount; // Устанавливаем заполненность полосы HP
    //}

    //void UpdateManaBar() // Метод для обновления полосы маны (закомментирован)
    //{
    //    manaText.text = (currentMana + "/" + maxMana); // Обновляем текст маны
    //    float fillAmount = currentMana / maxMana; // Рассчитываем заполненность полосы маны
    //    manaImage.fillAmount = fillAmount; // Устанавливаем заполненность полосы маны
    //}


    public void OnConsumeItem(Item item) // Метод, вызываемый при потреблении предмета
    {
        for (int i = 0; i < item.itemAttributes.Count; i++) // Проходим по всем атрибутам предмета
        {
            if (item.itemAttributes[i].attributeName == "Health") // Если атрибут - здоровье
            {
                if ((currentHealth + item.itemAttributes[i].attributeValue) > maxHealth) // Если новое здоровье превышает максимум
                    currentHealth = maxHealth; // Устанавливаем здоровье на максимум
                else
                    currentHealth += item.itemAttributes[i].attributeValue; // Увеличиваем здоровье
            }
            if (item.itemAttributes[i].attributeName == "Mana") // Если атрибут - мана
            {
                if ((currentMana + item.itemAttributes[i].attributeValue) > maxMana) // Если новая мана превышает максимум
                    currentMana = maxMana; // Устанавливаем ману на максимум
                else
                    currentMana += item.itemAttributes[i].attributeValue; // Увеличиваем ману
            }
            if (item.itemAttributes[i].attributeName == "Armor") // Если атрибут - броня
            {
                if ((currentArmor + item.itemAttributes[i].attributeValue) > maxArmor) // Если новая броня превышает максимум
                    currentArmor = maxArmor; // Устанавливаем броню на максимум
                else
                    currentArmor += item.itemAttributes[i].attributeValue; // Увеличиваем броню
            }
            if (item.itemAttributes[i].attributeName == "Damage") // Если атрибут - урон
            {
                if ((currentDamage + item.itemAttributes[i].attributeValue) > maxDamage) // Если новый урон превышает максимум
                    currentDamage = maxDamage; // Устанавливаем урон на максимум
                else
                    currentDamage += item.itemAttributes[i].attributeValue; // Увеличиваем урон
            }
        }
        //if (HPMANACanvas != null) // Код для обновления полосы HP и маны (закомментирован)
        //{
        //    UpdateManaBar(); // Обновляем полосу маны
        //    UpdateHPBar(); // Обновляем полосу HP
        //}
    }

    public void OnGearItem(Item item) // Метод, вызываемый при экипировке предмета
    {
        for (int i = 0; i < item.itemAttributes.Count; i++) // Проходим по всем атрибутам предмета
        {
            if (item.itemAttributes[i].attributeName == "Health") // Если атрибут - здоровье
                maxHealth += item.itemAttributes[i].attributeValue; // Увеличиваем максимальное здоровье
            if (item.itemAttributes[i].attributeName == "Mana") // Если атрибут - мана
                maxMana += item.itemAttributes[i].attributeValue; // Увеличиваем максимальную ману
            if (item.itemAttributes[i].attributeName == "Armor") // Если атрибут - броня
                maxArmor += item.itemAttributes[i].attributeValue; // Увеличиваем максимальную броню
            if (item.itemAttributes[i].attributeName == "Damage") // Если атрибут - урон
                maxDamage += item.itemAttributes[i].attributeValue; // Увеличиваем максимальный урон
        }
        //if (HPMANACanvas != null) // Код для обновления полосы HP и маны (закомментирован)
        //{
        //    UpdateManaBar(); // Обновляем полосу маны
        //    UpdateHPBar(); // Обновляем полосу HP
        //}
    }

    public void OnUnEquipItem(Item item) // Метод, вызываемый при разэкипировке предмета
    {
        for (int i = 0; i < item.itemAttributes.Count; i++) // Проходим по всем атрибутам предмета
        {
            if (item.itemAttributes[i].attributeName == "Health") // Если атрибут - здоровье
                maxHealth -= item.itemAttributes[i].attributeValue; // Уменьшаем максимальное здоровье
            if (item.itemAttributes[i].attributeName == "Mana") // Если атрибут - мана
                maxMana -= item.itemAttributes[i].attributeValue; // Уменьшаем максимальную ману
            if (item.itemAttributes[i].attributeName == "Armor") // Если атрибут - броня
                maxArmor -= item.itemAttributes[i].attributeValue; // Уменьшаем максимальную броню
            if (item.itemAttributes[i].attributeName == "Damage") // Если атрибут - урон
                maxDamage -= item.itemAttributes[i].attributeValue; // Уменьшаем максимальный урон
        }
        //if (HPMANACanvas != null) // Код для обновления полосы HP и маны (закомментирован)
        //{
        //    UpdateManaBar(); // Обновляем полосу маны
        //    UpdateHPBar(); // Обновляем полосу HP
        //}
    }

    // Update is called once per frame

    void Update() // Метод, вызываемый каждый кадр
    {



                  // Сохранение инвентаря при нажатии клавиши "S"
            //    if (Input.GetKeyDown(KeyCode.S))
            //     {
           
            //     // mainInventory.SaveInventory();

            //     }
        

            //// Загрузка инвентаря при нажатии клавиши "L"
            //if (Input.GetKeyDown(KeyCode.L))
            //{
            //    mainInventory.LoadInventory();
            
            //}

        if (Input.GetKeyDown(KeyCode.V))
        {
            mainInventory.ItemsInInventory.Clear();
            mainInventory.updateItemList();
        }
        // ... ваш существующий код ...


        // Проверяем, нажата ли клавиша для открытия системы персонажа
        if (Input.GetKeyDown(inputManagerDatabase.CharacterSystemKeyCode))
        {
            if (!characterSystem.activeSelf) // Если система персонажа не активна
            {
                characterSystemInventory.openInventory(); // Открываем инвентарь системы персонажа
            }
            else // Если система персонажа активна
            {
                if (toolTip != null) // Если подсказка существует
                    toolTip.deactivateTooltip(); // Деактивируем подсказку
                characterSystemInventory.closeInventory(); // Закрываем инвентарь системы персонажа
            }
        }

        // Проверяем, нажата ли клавиша для открытия инвентаря
        if (Input.GetKeyDown(inputManagerDatabase.InventoryKeyCode))
        {
            if (!inventory.activeSelf) // Если инвентарь не активен
            {
                mainInventory.openInventory(); // Открываем основной инвентарь
            }
            else // Если инвентарь активен
            {
                if (toolTip != null) // Если подсказка существует
                    toolTip.deactivateTooltip(); // Деактивируем подсказку
                mainInventory.closeInventory(); // Закрываем основной инвентарь
            }
        }

        // Проверяем, нажата ли клавиша для открытия системы крафта
        if (Input.GetKeyDown(inputManagerDatabase.CraftSystemKeyCode))
        {
            if (!craftSystem.activeSelf) // Если система крафта не активна
                craftSystemInventory.openInventory(); // Открываем инвентарь системы крафта
            else // Если система крафта активна
            {
                if (cS != null) // Если система крафта существует
                    cS.backToInventory(); // Возвращаемся к инвентарю
                if (toolTip != null) // Если подсказка существует
                    toolTip.deactivateTooltip(); // Деактивируем подсказку
                craftSystemInventory.closeInventory(); // Закрываем инвентарь системы крафта
            }
        }
    }

    
}

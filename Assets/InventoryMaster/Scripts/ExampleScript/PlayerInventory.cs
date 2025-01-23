using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;
using static WeaponSwitcher;
using static UnityEditor.Progress;
using Unity.VisualScripting;

public class PlayerInventory : MonoBehaviour
{

    public List<Item> ammoItems = new List<Item>(); // Список предметов, находящихся в инвентаре боеприпасов
    public List<GameObject> itemAmmo = new List<GameObject>();

    [SerializeField]
    private ItemDataBaseList itemDatabase; // Ссылка на базу данных предметов


    public GameObject inventory; // Переменная для хранения ссылки на объект инвентаря
    public GameObject ammoinvetory; // Переменная для хранения ссылки на объект инвентаря
    public GameObject characterSystem; // Переменная для хранения ссылки на систему персонажа
    public GameObject craftSystem; // Переменная для хранения ссылки на систему крафта
    private Inventory craftSystemInventory; // Переменная для хранения инвентаря системы крафта
    private CraftSystem cS; // Переменная для хранения ссылки на систему крафта
    private Inventory mainInventory; // Переменная для хранения основного инвентаря
    public Inventory ammoInventory; // Переменная для хранения инвентаря боеприпасов
    private Inventory characterSystemInventory; // Переменная для хранения инвентаря системы персонажа
    private Tooltip toolTip; // Переменная для хранения ссылки на подсказку
            
    public int itemIDWeapon; //переменная для хранения id оружия


    private InputManager inputManagerDatabase; // Переменная для хранения ссылки на менеджер ввода

    public GameObject HPMANACanvas; // Переменная для хранения ссылки на канвас для отображения HP и маны

    Text hpText; // Переменная для хранения текста HP
    Text manaText; // Переменная для хранения текста маны
    Image hpImage; // Переменная для хранения изображения HP
    Image manaImage; // Переменная для хранения изображения маны

    float maxHealth = 100; // Максимальное здоровье игрока
    float maxMana = 100; // Максимальная мана игрока
    public float maxDamage = 0; // Максимальный урон игрока
    public float maxArmor = 0; // Максимальная броня игрока

    public float currentHealth = 60; // Текущее здоровье игрока
    public float currentMana = 100; // Текущая мана игрока
    public float currentDamage = 0; // Текущий урон игрока
    public float currentArmor = 0; // Текущая броня игрока

    int normalSize = 3; // Нормальный размер инвентаря

    public void OnEnable() // Метод, вызываемый при активации объекта
    {
        // Подписываемся на события инвентаря для обработки экипировки и разэкипировки предметов
        Inventory.ItemEquip += OnBackpack; // Подписка на событие экипировки предмета, вызывая метод OnBackpack при его срабатывании
        Inventory.UnEquipItem += UnEquipBackpack; // Подписка на событие разэкипировки предмета, вызывая метод UnEquipBackpack при его срабатывании

        Inventory.ItemEquip += OnGearItem; // Подписка на событие экипировки предмета, вызывая метод OnGearItem при его срабатывании
        Inventory.ItemConsumed += OnConsumeItem; // Подписка на событие потребления предмета, вызывая метод OnConsumeItem при его срабатывании
        Inventory.UnEquipItem += OnUnEquipItem; // Подписка на событие разэкипировки предмета, вызывая метод OnUnEquipItem при его срабатывании

        Inventory.ItemEquip += EquipWeapon; // Подписка на событие экипировки предмета, вызывая метод EquipWeapon при его срабатывании
        Inventory.UnEquipItem += UnEquipWeapon; // Подписка на событие разэкипировки предмета, вызывая метод UnEquipWeapon при его срабатывании

        Inventory.ItemEquip += EquipAmmo; // Подписка на событие экипировки предмета, вызывая метод EquipWeapon при его срабатывании
        Inventory.UnEquipItem += UnEquipAmmo; // Подписка на событие разэкипировки предмета, вызывая метод UnEquipWeapon при его срабатывании
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

        Inventory.UnEquipItem -= UnEquipAmmo;
        Inventory.ItemEquip -= EquipAmmo;
   
    }
    public int itemIDAmmo = -1;//переменная для хранения id боеприпасов
    public int itemValueAmmo = 0;//переменная для хранения количества боеприпасов


    // Вложенный статический класс для обработки ItemID
    public int GetItemIDWeapon() //для возврата значения idоружия и дальнейшей его передачи
    {
        return itemIDWeapon; // Возвращаем текущее значение itemID оружия
    }
    public int GetItemIDAmmo() //для возврата значения id патронов и дальнейшей его передачи
    {
        return itemIDAmmo; // Возвращаем текущее значение itemID патронов
    }
    public int GetItemValueAmmo() //для возврата значения колличества и дальнейшей его передачи
    {
        return itemValueAmmo; // Возвращаем текущее значение колличества патронов
    }


    public void EquipWeapon(Item item) // Метод для экипировки оружия
    {
        //Debug.Log("EquipWeapon вызван с Item: " + item);

        if (item.itemType == ItemType.Weapon) // Проверяем, является ли предмет оружием
        {

            if (item.itemID != 0) // Проверяем, что ItemID не равен 0 
            {
                itemIDWeapon = item.itemID;
                //Debug.Log("ItemID установлен: " + item.itemID);
            }

        }
    }


    void UnEquipWeapon(Item item) // Метод для разэкипировки оружия
    {
        if (item.itemType == ItemType.Weapon) // Проверяем, является ли предмет оружием
        {
            for (int i = 0; i < item.itemAttributes.Count; i++) // Проходим по всем атрибутам предмета
            {

                if (item.itemID != 0) // Проверяем, что ItemID не равен 0 
                {
                    itemIDWeapon = 0; // Получаем ID предмета                                                  
                    //Debug.Log("Найден ItemID: " + itemIDWeapon);
                }
            }
        }
    }
    public void EquipAmmo(Item item) // Метод для экипировки боеприпасов
    {
        Debug.Log("EquipAmmo вызван с Item: " + item);

        if (item.itemType == ItemType.Ammo) // Проверяем, является ли предмет боеприпасов
        {
            Debug.Log("Предмет является боеприпасов.");

            // Устанавливаем itemID в ItemIDHandler только один раз
            if (item.itemID != 0) // Проверяем, что ItemID не равен 0 
            {
                itemIDAmmo = item.itemID;
                Debug.Log("ItemID установлен: " + item.itemID);
            }
            if (item.itemValue != 0) // Проверяем, что ItemID не равен 0 
            {
                itemValueAmmo = item.itemValue;
                Debug.Log("itemValueAmmo установлен: " + item.itemValue);
            }
            else
            {
                Debug.LogWarning("itemValue равен 0, не устанавливаем оружие.");
            }
        }
        else
        {
            Debug.LogWarning("Предмет не является боеприпасом.");
        }
    }


    void UnEquipAmmo(Item item) // Метод для разэкипировки боеприпасов
    {
        if (item.itemType == ItemType.Ammo) // Проверяем, является ли предмет боеприпасом
        {
            for (int i = 0; i < item.itemAttributes.Count; i++) // Проходим по всем атрибутам предмета
            {

                if (item.itemID != 0) // Проверяем, что ItemID не равен 0 
                {
                    itemIDAmmo = 0; // Получаем ID предмета                                                  
                    Debug.Log("Найден ItemID: " + itemIDAmmo);
                }
                if (item.itemValue != 0) // Проверяем, что ItemID не равен 0 
                {
                    itemValueAmmo = item.itemValue;
                    Debug.Log("itemValueAmmo установлен: " + item.itemValue);
                }
            }
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
        if (HPMANACanvas != null)
        {
            hpText = HPMANACanvas.transform.GetChild(1).GetChild(0).GetComponent<Text>(); // Получаем текст HP
            manaText = HPMANACanvas.transform.GetChild(2).GetChild(0).GetComponent<Text>(); // Получаем текст маны
            hpImage = HPMANACanvas.transform.GetChild(1).GetComponent<Image>(); // Получаем изображение HP
            manaImage = HPMANACanvas.transform.GetChild(2).GetComponent<Image>(); // Получаем изображение маны
            UpdateHPBar(); // Обновляем полосу HP
            UpdateManaBar(); // Обновляем полосу маны
        }

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



        // Проверьте, что все инвентарные компоненты инициализированы
        if (ammoinvetory != null)
        {
            ammoInventory = ammoinvetory.GetComponent<Inventory>(); // Получаем компонент инвентаря для боеприпасов
            Debug.Log("Инвентарь боеприпасов инициализирован.");
        }
        else
        {
            Debug.LogError("Инвентарь боеприпасов не установлен.");
        }
    }

    void UpdateHPBar() // Метод для обновления полосы здоровья (закомментирован)
    {
        hpText.text = (currentHealth + "/" + maxHealth); // Обновляем текст HP
        float fillAmount = currentHealth / maxHealth; // Рассчитываем заполненность полосы HP
        hpImage.fillAmount = fillAmount; // Устанавливаем заполненность полосы HP
    }

    void UpdateManaBar() // Метод для обновления полосы маны (закомментирован)
    {
        manaText.text = (currentMana + "/" + maxMana); // Обновляем текст маны
        float fillAmount = currentMana / maxMana; // Рассчитываем заполненность полосы маны
        manaImage.fillAmount = fillAmount; // Устанавливаем заполненность полосы маны
    }


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
        if (HPMANACanvas != null) // Код для обновления полосы HP и маны (закомментирован)
        {
            UpdateManaBar(); // Обновляем полосу маны
            UpdateHPBar(); // Обновляем полосу HP
        }
    }

    public void OnGearItem(Item item) // Метод, вызываемый при экипировке предмета
    {
        for (int i = 0; i < item.itemAttributes.Count; i++) // Проходим по всем атрибутам предмета
        {
            if (item.itemAttributes[i].attributeName == "Health") // Если атрибут - здоровье
            {
                maxHealth += item.itemAttributes[i].attributeValue; // Увеличиваем максимальное здоровье
                currentHealth += item.itemAttributes[i].attributeValue;
            }
            if (item.itemAttributes[i].attributeName == "Mana") // Если атрибут - мана
                maxMana += item.itemAttributes[i].attributeValue; // Увеличиваем максимальную ману
            if (item.itemAttributes[i].attributeName == "Armor") // Если атрибут - броня
                maxArmor += item.itemAttributes[i].attributeValue; // Увеличиваем максимальную броню
            if (item.itemAttributes[i].attributeName == "Damage") // Если атрибут - урон
            {
                maxDamage += item.itemAttributes[i].attributeValue; // Увеличиваем максимальный урон
                currentDamage = maxDamage; // Обновляем текущий урон до максимального
            }
        }
        if (HPMANACanvas != null) // Код для обновления полосы HP и маны (закомментирован)
        {
            UpdateManaBar(); // Обновляем полосу маны
            UpdateHPBar(); // Обновляем полосу HP
        }
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
            {
                maxDamage -= item.itemAttributes[i].attributeValue; // Уменьшаем максимальный урон
                currentDamage = maxDamage; // Обновляем текущий урон до максимального
            }
        }
        if (HPMANACanvas != null) // Код для обновления полосы HP и маны (закомментирован)
        {
            UpdateManaBar(); // Обновляем полосу маны
            UpdateHPBar(); // Обновляем полосу HP
        }
    }

    // Update is called once per frame



    public void DeleteAmmo() //метод для удаления предмета из инфентаря боеприпасов
    {
        Debug.Log($"Делит аммо запустился");
        for (int i = 0; i < ammoInventory.transform.childCount; i++) // Проходим по всем слотам в контейнере

        {
            Debug.Log($"супустился цикл фор в делит аммо");
            if (ammoInventory.transform.GetChild(i).childCount != 0) // Если слот не пустой
            {
                Debug.Log($"должен запуститься дистрой");
                Destroy(ammoInventory.transform.GetChild(i).GetChild(0).GetChild(0).gameObject); // Удаляем предмет из слота
                break;
            }
        }
        Debug.LogWarning("Патроны не найдены в инвентаре.");
    }


    // метод для уменьшения патрон после перезарядки
    public void DecreaseAmmo(int amount)
    {
        if (ammoInventory.ItemsInInventory.Count > 0)
        {
            Item ammoItem = ammoInventory.ItemsInInventory[0]; // Получаем текущий предмет
            Debug.Log($"Получен предмет после перезарядки {ammoItem.itemValue}");
            if (ammoItem.itemValue >= amount)
            {
                ammoItem.itemValue -= amount; // Уменьшаем количество патронов
                Debug.Log($"уменьшено колличество патрон {ammoItem.itemValue}");
                if (ammoItem.itemValue == 0 )
                {
                    Debug.Log($"предмет равен 0 {ammoItem.itemValue}");
                    if (ammoItem.itemModel != null)
                    {
                        DeleteAmmo();
                        Debug.Log("запущен делит аммо");
                    }
                   // ammoInventory.ItemsInInventory.Remove(ammoItem); // Удаляем его из инвентаря
                    Debug.Log("тут  должен был удалить предмет из инвентаря");
                    
                }
            }
            else
            {
                Debug.LogWarning("удаление патрон не вышло");
            }
        }
    }



    void Update() // Метод, вызываемый каждый кадр
    {

        if (ammoInventory != null && ammoInventory.ItemsInInventory.Count > 0)
        {
            Item ammoItem = ammoInventory.ItemsInInventory[0]; // Получаем текущий предмет
            //int newAmmoValue = ammoItem.itemValue; // Получаем текущее количество патронов

            // Проверяем, изменился ли ID или количество патронов
            if (ammoItem.itemID != itemIDAmmo || ammoItem.itemValue != itemValueAmmo)
            {
                // Если значения изменились, обновляем переменные
                itemIDAmmo = ammoItem.itemID;
                itemValueAmmo = ammoItem.itemValue;

                // Вызываем метод обновления
                EquipAmmo(ammoItem);
                Debug.Log($"Ammo Changed: ID = {itemIDAmmo}, Value = {itemValueAmmo}");
            }
        }
        else
        {
            // Если инвентарь пуст, сбросьте значения
            if (itemIDAmmo != -1 || itemValueAmmo != 0)
            {
                itemIDAmmo = -1;
                itemValueAmmo = 0;
                Debug.Log("Ammo Inventory is empty, reset values.");
            }
        }

       


        //CheckAmmoInventory();
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
    public void TakeDamage_player(int damage_player)
    {
        currentHealth -= damage_player; // Уменьшаем здоровье на полученный урон
        UpdateHPBar(); // Обновляем полосу HP
        //spriteRend.material = matBlink; // когда попали, жизни вычитаются

        //if (health <= 0) // когда жизни опускается до 0 объект разрушается
        //{
        //    Die();
        //}
        //else
        //{
        //    Invoke("ResetMaterial", 0.5f); //если не убили, сработает функия через 0.2 секунды
        //}
        if (currentHealth < 0) // Проверяем, чтобы здоровье не стало отрицательным
        {
            currentHealth = 0; // Устанавливаем здоровье в 0, если оно меньше 0

        }
    }


}

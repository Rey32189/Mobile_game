using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using static UnityEditor.Progress;

public class Orugie : MonoBehaviour // скрипт на стрельбу
{
    public Transform firePoint; // Ссылка на болванку откуда вылетает снаряд
    public GameObject bullet; // сюда вставляется пакет снаряда
    public PlayerInventory playerInventory; // Ссылка на PlayerInventory
   // public Inventory inventory;

    private float timeShot; // для задержки времени стрельбы
    public float startTime; // для задержки времени стрельбы

    public int currentAmmo; // текущее количество боеприпасов в стволе
    public int allAmmo; // все боеприпасы в наличии
    public int fullAmmo; //максимальное колличество пуль для переноса
    public int damagAmmo; // дамаг боеприпасов


    public int chet; // переменная для метода вычета пуль из предмета


    //private int lastItemID = 0; // Начальное значение, которое гарантированно не совпадет с валидным itemID
    private int lastItemValue = 0;
    [SerializeField]
    private TextMeshProUGUI ammoCount; // для счетчика

    private bool attack = false;

    void Start()
    {
        // Убедитесь, что playerInventory ссылается на правильный объект
        if (playerInventory == null)
        {
            playerInventory = FindObjectOfType<PlayerInventory>();
        }
    }




    //public void EquipAmmo(Item item)
    //{

    //    if (playerInventory.ammoInventory.ItemsInInventory != null &&
    //        playerInventory.ammoInventory.ItemsInInventory.Count > 0) // Добавляем проверку на наличие элементов
    //    {
    //        Debug.Log("прошли проверку на соответствие");

    //        var damageAttribute = item.itemAttributes.Find(attr => attr.attributeName == "Damage");

    //        if (damageAttribute != null) // Проверяем, найден ли атрибут "Damage"
    //        {
    //            Debug.Log("найден атрибут дамаг");
    //            if (damagAmmo != damageAttribute.attributeValue)
    //            {
    //                Debug.Log("дамаг не равен атрибуту и должен установиться");
    //                damagAmmo = damageAttribute.attributeValue; // Уменьшаем максимальный урон
    //            }

    //        }
    //    }
    //    else
    //    {
    //        Debug.LogWarning("Инвентарь боеприпасов пуст.");
    //    }

    //}



    void Update()
    {
        //int currentItemID = playerInventory.GetItemIDAmmo();

        //if (currentItemID != lastItemID) // Проверяем на изменение
        //{
        //    lastItemID = currentItemID; // Обновляем последнее значение
        //                                // Проверяем, есть ли элементы в инвентаре


        //    playerInventory.ammoInventory.updateItemList(); // Обновляем список предметов в инвентаре

        //    if (lastItemID > 0)
        //    {
        //        Debug.Log("ID боеприпасов не равно 0 так что пытаемся передать данные в метод для дамага");
        //        Item item = playerInventory.ammoInventory.ItemsInInventory[0];
        //        EquipAmmo(item);
        //    }
        //    else
        //    {
        //        Debug.Log("ID равен 0 так что дамаг нужно поставить на 1");
        //        damagAmmo = 0;
        //    }
        //}

        int currentItemValue = playerInventory.GetItemValueAmmo();


        if (currentItemValue != lastItemValue) // Проверяем на изменение
        {
            allAmmo = currentItemValue; // Обновляем последнее значение
            playerInventory.ammoInventory.updateItemList(); // Обновляем список предметов в инвентаре
        }
        if (currentItemValue == 0) // проверяем, что бы предмет находился в инвентаре и если нет, то ставим боеприпасы на 0
        {
            allAmmo = 0;
            playerInventory.ammoInventory.updateItemList(); // Обновляем список предметов в инвентаре
           
        }
        
        if (timeShot <= 0)
        {
            if (Input.GetButtonDown("Fire1") && currentAmmo > 0) // для атаки с мышки
            {
                Shoot();
                timeShot = startTime;
                currentAmmo -= 1;
            }
            if (attack && currentAmmo > 0) //для атаки с кнопок
            {
                Shoot();
                timeShot = startTime;
                currentAmmo -= 1;
            }
        }
        else
        {
            timeShot -= Time.deltaTime; // время повторного выстрела
        }
        ammoCount.text = currentAmmo + " / " + allAmmo; //это для панели счетчика пуль
        
        if (Input.GetKeyDown(KeyCode.R) && allAmmo > 0) // клавиша для перезорядки
        {
            Invoke("Reload", 0.5f);
        }
        if (currentAmmo == 0)
        {
            Invoke("Reload", 0.5f);
        }
        if (allAmmo == 0 && currentAmmo == 0)
        {
            if (playerInventory.ammoInventory.ItemsInInventory.Count > 0)
            {
                Item ammoItem = playerInventory.ammoInventory.ItemsInInventory[0];
                if (ammoItem.itemValue == 0)
                {
                    playerInventory.DeleteAmmo();
                }

            }

        }
    }

    public void Reload() // метод для расчета патрон
    {
      
        // Проверяем, есть ли патроны для перезарядки
        if (allAmmo <= 0)
        {
            return; // Если патронов нет, выходим из метода
            //playerInventory.DecreaseAmmo(0); // Уменьшаем количество патронов в инвентаре
        }

        // Проверяем, заполнен ли магазин
        if (currentAmmo >= fullAmmo)
        {
            return; // Если магазин полон, выходим из метода
        }
        if (allAmmo > 0)
        {
            // Определяем, сколько патронов нужно для полной обоймы
            int neededAmmo = fullAmmo - currentAmmo;

            // Если у нас достаточно патронов для полной перезарядки
            if (allAmmo >= neededAmmo)
            {
                currentAmmo = fullAmmo; // Заполняем обойму полностью
                allAmmo -= neededAmmo; // Уменьшаем количество доступных патронов
            }
            else // Если не хватает патронов для полной перезарядки
            {
                currentAmmo += allAmmo; // Добавляем все доступные патроны
                allAmmo = 0; // Обнуляем доступные патроны
            }

            playerInventory.DecreaseAmmo(currentAmmo - (fullAmmo - neededAmmo)); // Уменьшаем количество патронов в инвентаре
        }
        if (allAmmo == 0 && currentAmmo == 0)
        {
            playerInventory.DeleteAmmo();
        }

    }
    void Shoot()
    {
        Instantiate(bullet, firePoint.position, firePoint.rotation); // создает выстрел
    }
    public void AttackDown() // для привязки к кнопкам
    {
        attack = true;
    }
    public void AttackUp() // для привязки к кнопкам
    {
        attack = false;
    }

}

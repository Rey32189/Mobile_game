using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class Orugie : MonoBehaviour // скрипт на стрельбу
{
    public Transform firePoint; // Ссылка на болванку откуда вылетает снаряд
    public GameObject bullet; // сюда вставляется пакет снаряда
    public PlayerInventory playerInventory; // Ссылка на PlayerInventory

    private float timeShot; // для задержки времени стрельбы
    public float startTime; // для задержки времени стрельбы

    public int currentAmmo; // текущее количество боеприпасов в обойме
    public int allAmmo; // все боеприпасы в наличии
    public int fullAmmo; //максимальное колличество пуль для переноса


    public int chet; // переменная для метода вычета пуль из предмета


    private int lastItemID = 0; // Начальное значение, которое гарантированно не совпадет с валидным itemID
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
    void Update()
    {
        int currentItemID = playerInventory.GetItemIDAmmo();

        if (currentItemID != lastItemID) // Проверяем на изменение
        {
            lastItemID = currentItemID; // Обновляем последнее значение
                       //SetWeaponByItemID(currentItemID); // Вызываем метод только при изменении
        }

        int currentItemValue = playerInventory.GetItemValueAmmo();

        if (currentItemValue != lastItemValue) // Проверяем на изменение
        {
            allAmmo = currentItemValue; // Обновляем последнее значение
                                        //SetWeaponByItemID(currentItemID); // Вызываем метод только при изменении
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
            Invoke("Reload", 1f);
        }
        if (currentAmmo == 0)
        {
            Invoke("Reload", 1f);
        }
    }

    public void Reload()
    {
        int reason = 15 - currentAmmo;
        if (allAmmo >= reason)
        {
            allAmmo = allAmmo - reason;
            currentAmmo = 15;
            playerInventory.DecreaseAmmo(reason); // Уменьшаем количество патронов в инвентаре
        }
        else
        {
            currentAmmo = currentAmmo + allAmmo;
            allAmmo = 0;
            playerInventory.DecreaseAmmo(reason);
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

    //private void OnTriggerEnter(Collider collision)
    //{
    //    pistolClic pistolClicScript = collision.GetComponent<pistolClic>();

    //    //if (collision.GetComponent<pistolClic>() && currentAmmo < fullAmmo && allAmmo < fullAmmo)
    //    if (pistolClicScript != null && allAmmo < fullAmmo)
    //    {
    //        if (pistolClicScript.randomAmmo + allAmmo < fullAmmo)
    //        {
    //            allAmmo += pistolClicScript.randomAmmo;
    //        }
    //        else
    //        {
    //            allAmmo = fullAmmo;
    //        }

    //        Destroy(collision.gameObject);
    //    }

    //}

}

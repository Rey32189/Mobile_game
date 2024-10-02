using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Orugie : MonoBehaviour // скрипт на стрельбу
{
    public Transform firePoint; // Ссылка на болванку откуда вылетает снаряд
    public GameObject bullet; // сюда вставляется пакет снаряда


    private float timeShot; // для задержки времени стрельбы
    public float startTime; // для задержки времени стрельбы

    public int currentAmmo; // текущее количество боеприпасов в обойме
    public int allAmmo; // все боеприпасы в наличии
    public int fullAmmo; //максимальное колличество пуль для переноса

    [SerializeField]
    private TextMeshProUGUI ammoCount; // для счетчика

    private bool attack = false;
    void Update()
    {
        
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
        ammoCount.text = currentAmmo + " / " + allAmmo;
        
        if (Input.GetKeyDown(KeyCode.R) && allAmmo > 0) // клавиша для перезорядки
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
        }
        else
        {
            currentAmmo = currentAmmo + allAmmo;
            allAmmo = 0;
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

    private void OnTriggerEnter(Collider collision)
    {
        pistolClic pistolClicScript = collision.GetComponent<pistolClic>();
        
        //if (collision.GetComponent<pistolClic>() && currentAmmo < fullAmmo && allAmmo < fullAmmo)
        if (pistolClicScript != null && allAmmo < fullAmmo)
        {
            if (pistolClicScript.randomAmmo + allAmmo < fullAmmo)
            {
                allAmmo += pistolClicScript.randomAmmo;
            }
            else
            {
                allAmmo = fullAmmo;
            }
            
            Destroy(collision.gameObject);
        }
        
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : MonoBehaviour // замена оружия
{

    public int weaponSwitch = 0; // цифры индификатор для оружия

    // Start is called before the first frame update
    void Start()
    {
        SelectWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        int curentWeapon = weaponSwitch;
        if (Input.GetAxis("Mouse ScrollWheel") > 0F) //если колесико мышки вверх, то применяется скрипт
        {
            if (weaponSwitch >= transform.childCount - 1)
            {
                weaponSwitch = 0;
            }
            else
            {
                weaponSwitch++;
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0F) //если колесико мышки вниз, то применяется скрипт
        {
            if (weaponSwitch <= 0)
            {
                weaponSwitch = transform.childCount - 1;
            }
            else
            {
                weaponSwitch--;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            weaponSwitch = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && transform.childCount >=2)
        {
            weaponSwitch = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && transform.childCount >= 3)
        {
            weaponSwitch = 2;
        }

        if (curentWeapon != weaponSwitch)
        {
            SelectWeapon();
        }

    }
    void SelectWeapon() // метод для указания оружия
    {
        int i = 0;
        foreach (Transform weapon in transform) // с помощью трансформа меняем один объект на другой
        {
            if (i == weaponSwitch)
            {
                weapon.gameObject.SetActive(true); // гейим обджик долден стать активным
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
            i++;
        }
    }
}

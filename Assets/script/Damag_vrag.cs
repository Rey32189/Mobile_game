using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damag_vrag : MonoBehaviour
{
    public int damage_player; //количество урона
    //нанесение урона игроку
    private void OnTriggerEnter(Collider currentHealth) // ищем объект с компонентом currentHealth
    {

        if (currentHealth.CompareTag("Player"))
        {
            PlayerInventory healthBar = currentHealth.GetComponent<PlayerInventory>(); // берет кусок кода из врага для взаимодействия
            if (healthBar != null)
            {
                healthBar.TakeDamage_player(damage_player);
            }
            //Destroy(gameObject); // после взаимодействия разрушает стрелу
        }
        else if (currentHealth.CompareTag("Object"))
        {
            // Предполагаем, что у объекта есть компонент Health
            Destroy_objekt objectHealth = currentHealth.GetComponent<Destroy_objekt>();
            if (objectHealth != null)
            {
                objectHealth.TakeDamage_Object(damage_player); // Уменьшаем здоровье объекта
            }
        }
        else // отвечает за то, что бы выстрелы не разрушались при встрече с игроком и друг другом
        {
            return;
        }

    }
   
}

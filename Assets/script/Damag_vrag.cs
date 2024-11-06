using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damag_vrag : MonoBehaviour
{
    public int damage_player; //количество урона
    //нанесение урона игроку
    private void OnTriggerEnter(Collider currentHealth) // ищем объект с компонентом хитинфо
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
        else // отвечает за то, что бы выстрелы не разрушались при встрече с игроком и друг другом
        {
            return;
        }

    }
}

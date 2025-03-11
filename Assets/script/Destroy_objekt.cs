using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy_objekt : MonoBehaviour
{
    public float hp_Obekt;
  
   
    public void TakeDamage_Object(int damage_player)
    {
        hp_Obekt -= damage_player; // Уменьшаем здоровье на полученный урон

        if (hp_Obekt < 0) // Проверяем, чтобы здоровье не стало отрицательным
        {
            hp_Obekt = 0; // Устанавливаем здоровье в 0, если оно меньше 0
            Destroy(gameObject); // разрушаем объект
        }
    }
}

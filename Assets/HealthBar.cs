using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class HealthBar : MonoBehaviour
{
    public Image bar;
    public float fill;
    public int health; // фактическое здоровье
    public TextMeshProUGUI playerHealth; // численное определение жизней
    public int max_health = 15; // максимальное колличество жизней

    // Start is called before the first frame update
    void Start()
    {
        fill = 1f; // Ўкала здоровь€ заполнена на 100%
    }

    // Update is called once per frame
    void Update()
    {
        // ќбновл€ем заполнение шкалы здоровь€
        fill = (float)health / max_health; // ¬ычисл€ем процент заполнени€ шкалы
        bar.fillAmount = fill; // ”станавливаем значение заполнени€ в компонент Image
        playerHealth.text = health.ToString(); // ќбновл€ем текст с количеством жизней
    }

    public void TakeDamage_player(int damage_player)
    {
        health -= damage_player; // ”меньшаем здоровье на полученный урон
        if (health < 0) // ѕровер€ем, чтобы здоровье не стало отрицательным
        {
          health = 0; // ”станавливаем здоровье в 0, если оно меньше 0
        }
    }
    
   // public void TakeDamage_player(int damage_player)
    //{
    //    health -= damage_player;
        //spriteRend.material = matBlink; // когда попали, жизни вычитаютс€

        //if (health <= 0) // когда жизни опускаетс€ до 0 объект разрушаетс€
        //{
        //    Die();
        //}
        //else
        //{
        //    Invoke("ResetMaterial", 0.5f); //если не убили, сработает функи€ через 0.2 секунды
        //}
    //}

}

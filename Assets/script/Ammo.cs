using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour // появление патронов
{
    public float speed; // скорость снаряда
    public float destroyTime; // время до разрушения снаряда
    public int damage; //количество урона

    private PlayerInventory playerInventory; // Ссылка на PlayerInventory
    public Rigidbody rb;

    void Start()
    {
        // Найдите объект с PlayerInventory. 
        playerInventory = FindObjectOfType<PlayerInventory>();
        // Устанавливаем скорость снаряда
        rb.velocity = transform.forward * speed;
        // Уничтожаем снаряд через определенное время
        Invoke("DestroyAmmo", destroyTime);
    }
    private void OnTriggerEnter(Collider hitInfo) // ищем объект с компонентом hitInfo
    {
        if (hitInfo.CompareTag("Vrag"))
        {
            Enemy enemy = hitInfo.GetComponent<Enemy>(); // берет кусок кода из врага для взаимодействия
            if (enemy != null)
            {
                // Получаем актуальное значение currentDamage перед нанесением урона
                int damage = playerInventory != null ? (int)playerInventory.currentDamage : 0;
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject); // после взаимодействия разрушает стрелу
        }
        else // отвечает за то, что бы выстрелы не разрушались при встрече с игроком и друг другом
        {
            return;
        }

    }
void DestroyAmmo()
    {
        Destroy(gameObject);
    }
}

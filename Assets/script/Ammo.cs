using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour // появление патронов
{
    public float speed; // скорость снаряда
    public float destroyTime; // время до разрушения снаряда
    public int damage; //количество урона
    

    public Rigidbody rb;

    void Start()
    {
        Invoke("DestroyAmmo", destroyTime);
        rb.velocity = transform.forward * speed;
    }
    private void OnTriggerEnter(Collider hitInfo) // ищем объект с компонентом хитинфо
    {
       
        if (hitInfo.CompareTag("Vrag"))
        {
            Enemy enemy = hitInfo.GetComponent<Enemy>(); // берет кусок кода из врага для взаимодействия
            if (enemy != null)
            {
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

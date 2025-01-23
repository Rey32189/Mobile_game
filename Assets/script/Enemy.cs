using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour // мигание противника и его смерть
{
    
    public Icheznovenie_sten disappearingWall; // Ссылка на стену для уничтожения
    public int nomer_stens; // Значение врага для связи со стеной

    public int health; // колличество жизней

    private Material matBlink; // материал мигания
    private Material matDefault; // оригинальный материал

    private MeshRenderer spriteRend; //для работы со спрайт рендером

    private UnityEngine.Object vragRef;

    private UnityEngine.Object explosion; // позволяет использовать объект частицы

    [SerializeField] float timeDestroy; // время восстановления противника

    [SerializeField] Vector3 spavnPos; // позиция появления противника

    private LootDropper lootDropper; // для выпадения ресурсов


    private void Start()
    {
        lootDropper = GetComponent<LootDropper>(); //инициализируем 
        // spavnPos = transform.position; // положение противника

        explosion = Resources.Load("Explosion");

        // в зависимости от номера стены выбираем разные префабы врагов
        // с разными врагами предутся развивать это место кода
        if (nomer_stens == 1 )
        {
            vragRef = Resources.Load("Vrag");
        }
        if (nomer_stens == 2)
        {
            vragRef = Resources.Load("Vrag_1");
        }
        if (nomer_stens == 3)
        {
            vragRef = Resources.Load("Vrag_2");
        }

        spriteRend = GetComponent<MeshRenderer>(); // ркализуется компонент спрайт рендер

        matBlink = Resources.Load("EnemyBlink", typeof(Material)) as Material; //Resources - путь папки с материалом load подгурузить "EnemyBlink" искать что то с этим названием
        //  typeof(Material) из какого вида надо найти что то as Material испольовать его как материал
        matDefault = spriteRend.material; // базовый материал, который есть сейчас
        if (disappearingWall == null) // Если стена не назначена, попробуем найти ее
        {
            disappearingWall = FindWallByValue(nomer_stens);
        }
    }
    //кусок кода для связывания со стеной
    private Icheznovenie_sten FindWallByValue(int value)
    {
        Icheznovenie_sten[] walls = FindObjectsOfType<Icheznovenie_sten>();
        foreach (Icheznovenie_sten wall in walls)
        {
            if (wall.nomer_stens == value) // Сравниваем значения
            {
                return wall;
            }
        }
        return null; // Если не нашли стену с таким значением
    }

    void ResetMaterial()
    {
        spriteRend.material = matDefault; // возвращается базовый материал
    }
    void KillEnemy()
    {
        Destroy(gameObject); // отвечает за разрушение объекта
    }
    public void TakeDamage(int damage)
    {
        health -= damage;
        spriteRend.material = matBlink; // когда попали, жизни вычитаются

        if (health <= 0) // когда жизни опускается до 0 объект разрушается
        {
            Die();
        }
        else
        {
            Invoke("ResetMaterial", 0.5f); //если не убили, сработает функия через 0.2 секунды
        }
    }
    
    
    void Die()
    {
        GameObject explosionRef = (GameObject)Instantiate(explosion);// происходит инициация частиц
        explosionRef.transform.position = new Vector3 (transform.position.x, transform.position.y, transform.position.z); // где появляются частицы

        lootDropper.DropLoot(); // Запускаем выпадение предмета


        gameObject.SetActive(false);
        Invoke("Respawn", timeDestroy); // Invoke позволяет вызвать событие и настроить время срабатывания
        if (disappearingWall != null) //если стена существует, то срабатывает счетчик
        {
            disappearingWall.EnemyDefeated(); // Увеличиваем счетчик для стены
        }
        Destroy(explosionRef, 2f);
    }

    void Respawn()
    {
        // Получаем текущее положение врага перед его уничтожением
        Vector3 respawnPosition = transform.position;

        // Добавляем небольшую случайную вариацию к позиции
        respawnPosition += new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f));

        // Создаем нового врага
        GameObject vragCopi = (GameObject)Instantiate(vragRef);

        // Устанавливаем позицию нового врага на место уничтоженного врага
        vragCopi.transform.position = respawnPosition;

        // Удаляем текущий объект врага
        Destroy(gameObject);
    }
    //для обнуления ссылки на стену, когда уничтожена, дабы уменьшить колличество ошибок
    public void DisappearingWallDestroyed()
    {
        disappearingWall = null; // Обнуляем ссылку на стену
    }
}

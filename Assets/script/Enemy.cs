using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour // мигание противника и его смерть
{
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

        vragRef = Resources.Load("Vrag");

        spriteRend = GetComponent<MeshRenderer>(); // ркализуется компонент спрайт рендер

        matBlink = Resources.Load("EnemyBlink", typeof(Material)) as Material; //Resources - путь папки с материалом load подгурузить "EnemyBlink" искать что то с этим названием
        //  typeof(Material) из какого вида надо найти что то as Material испольовать его как материал
        matDefault = spriteRend.material; // базовый материал, который есть сейчас
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
        //Destroy(gameObject); // отвечает за разрушение объекта, но не подходит для респауна
        GameObject explosionRef = (GameObject)Instantiate(explosion);// происходит инициация частиц
        explosionRef.transform.position = new Vector3 (transform.position.x, transform.position.y, transform.position.z); // где появляются частицы

        lootDropper.DropLoot(); // --------------------


        gameObject.SetActive(false);
        Invoke("Respawn", timeDestroy); // Invoke позволяет вызвать событие и настроить время срабатывания
        Destroy(explosionRef, 2f);
    }

    void Respawn()
    {
        GameObject vragCopi = (GameObject)Instantiate(vragRef); // появился ресурс, который будет создавать противников на базе врагреф
        
        vragCopi.transform.position = new Vector3(UnityEngine.Random.Range(spavnPos.x -3, spavnPos.x +3), spavnPos.y, spavnPos.z);
        Destroy(gameObject); // происходит разрушение копии
    }
}

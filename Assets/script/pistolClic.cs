using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pistolClic : MonoBehaviour
{
    // пустой скрипт, для обращения к нему другого скрипта

    // Добавляем переменную для хранения рандомного количества пуль в объекте
    public int randomAmmo = 0;
    // Start is called before the first frame update
    void Start()
    {
        randomAmmo = Random.Range(3, 16); // от 3 до 15 включительно
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

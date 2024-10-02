using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrul : MonoBehaviour
{
    public float speed; // скорость движения врага

    public int positionOfPatrol; //дистанция патрулирования противника
    //public Transform point; //точка начала патрулирования, точка возврата противника
    [SerializeField] Vector3 point2; //точка начала патрулирования, точка возврата противника
    bool movingRight; // переменная для поворота противника
    bool movingForward; // переменная для движения вперед-назад

    Transform player; //считывание где находится игрок
    public float stoppingDistance; //расстояние от противника до главного героя

    bool chill = false;
    bool angry = false;
    bool goBack = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // для поиска объекта с тегом игрок 
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, point2) < positionOfPatrol && angry == false) // это состояния покоя в котором он просто патрулирует
        {
            chill = true;
        }

        if (Vector3.Distance(transform.position, player.position) < stoppingDistance) // состояние если расстояние между патрульным и игроком меньше указанного
        {
            angry = true;
            chill = false;
            goBack = false;
        }
        if (Vector3.Distance(transform.position, player.position) > stoppingDistance) // состояние если расстояние между патрульным и игроком больше указанного
        {
            goBack = true; 
            angry = false;
        }

        if (chill == true)
        {
            Chill();
        }
        else if (angry == true)
        {
            Angry();
        }
        else if (goBack == true)
        {
            GoBack();
        }
    }

    void Chill() //состояние патрулирования
    {
        if (transform.position.x > point2.x + positionOfPatrol)  // когда доходит до края точки, разворачивается
        {
            movingRight = false;
        }
        else if (transform.position.x < point2.x - positionOfPatrol)
        {
            movingRight = true;
        }
        if (transform.position.z > point2.z + positionOfPatrol)
        {
            movingForward = false;
        }
        else if (transform.position.z < point2.z - positionOfPatrol)
        {
            movingForward = true;
        }

        if (movingRight)
        {
            transform.position = new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y, transform.position.z); // направление движения противника
        }
        else
        {
            transform.position = new Vector3(transform.position.x - speed * Time.deltaTime, transform.position.y, transform.position.z);
        }
        // Изменено: добавлено движение по оси z
        float moveX = movingRight ? speed * Time.deltaTime : -speed * Time.deltaTime;
        float moveZ = movingForward ? speed * Time.deltaTime : -speed * Time.deltaTime;

        transform.position = new Vector3(transform.position.x + moveX, transform.position.y, transform.position.z + moveZ);
    }

    void Angry() // состояние агра и погони за персом
    {
        //MoveTowards - значит противник должен следовать к объекту
        transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime); 
    }

    void GoBack() // состояния возврата к точке патрулирования
    {
        transform.position = Vector3.MoveTowards(transform.position, point2, speed * Time.deltaTime);
    }
}

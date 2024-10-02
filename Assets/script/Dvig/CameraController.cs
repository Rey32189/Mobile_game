using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float dumping = 3f; // для плавной остановки камеры
    public Vector3 offset = new Vector3(1f, 1f, -3f); // камера относительно персонажа
    public bool isLeft; // определяет взгляд персонажа, на случай если смотрит влево
    private Transform player; // определяет положение персонажа
    private int lastX; // определяет, в какую сторону смотрел персонаж
    public float distansCamera;

    public Camera mainCamera; // добавлено для доступа к параметрам камеры
    // Start is called before the first frame update
    void Start()
    {
        offset = new Vector3(Mathf.Abs(offset.x), offset.y, offset.z); //матматическое вычисление положения камеры
        FindPlayer(isLeft);


        // Увеличиваем размер ортографической камеры
        if (mainCamera != null && mainCamera.orthographic)
        {
            mainCamera.orthographicSize = distansCamera; // увеличено значение size
        }

    }
    
    public void FindPlayer(bool playerIsLeft)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; //что бы персонажа можно было найти по тэгу
        lastX = Mathf.RoundToInt(player.position.x); //строка позволяет работать по оси х
        if (playerIsLeft )
        {
            transform.position = new Vector3(player.position.x -  offset.x, player.position.y + offset.y, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(player.position.x + offset.x, player.position.y + offset.y, transform.position.z);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (player)
        {
            int currentX = Mathf.RoundToInt(player.position.x);
            if (currentX > lastX) isLeft = false; else if (currentX < lastX) isLeft = true;
            lastX = Mathf.RoundToInt(player.position.x);

            Vector3 target;
            if (isLeft)
            {
                target = new Vector3(player.position.x - offset.x, player.position.y + offset.y, transform.position.z);
            }
            else
            {
                target = new Vector3(player.position.x + offset.x, player.position.y + offset.y, transform.position.z); //  player.position.z + offset.z) если захочу что бы двигалась по z
            }

            Vector3 currentPosition = Vector3.Lerp(transform.position, target, dumping * Time.deltaTime);
            transform .position = currentPosition;
        }
    }
}

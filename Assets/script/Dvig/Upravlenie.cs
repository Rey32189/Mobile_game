using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Upravlenie : MonoBehaviour
{
    public float speed = 5f; // для ввода с клавиатуры
    // Start is called before the first frame update
    [SerializeField] float Speed; //скорость движения
    [SerializeField] float jumpForce = 50000.0f; // сила прыжка
    private bool isMoving = false;
    private bool isMovingL = false;
    private bool isMovingVerh = false;
    private bool isMovingNiz = false;
    private bool Jimpup = false;


    public Transform groundCheck; // Точка проверки соприкосновения с землей
    public float groundCheckRadius = 0.2f; // Радиус проверки соприкосновения
    public LayerMask groundLayer; // Слой, на котором персонаж считается стоящим

    private Rigidbody rb; // Компонент Rigidbody персонажа
    private bool isGrounded; // Флаг, указывающий, стоит ли персонаж на земле

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Получение компонента Rigidbody
    }

   

    void Update()
    {
        // Получаем ввод пользователя
        float moveX = Input.GetAxis("Horizontal"); //для клавы
        float moveZ = Input.GetAxis("Vertical");

        // Создаем вектор движения
        Vector3 movement = new Vector3(moveX, 0f, moveZ); //для клавы

        // Применяем движение к позиции
        transform.position += movement * speed * Time.deltaTime; // для клавы

        if (movement != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(movement);
        }
        Vector3 buttonMovement = Vector3.zero;
        if (isMoving)
        {
            buttonMovement += Vector3.forward;
        }
        if (isMovingL)
        {
            buttonMovement += Vector3.back;
        }
        if (isMovingVerh)
        {
            buttonMovement += Vector3.left;
        }
        if (isMovingNiz)
        {
            buttonMovement += Vector3.right;
        }
        if (buttonMovement != Vector3.zero)
        {
            transform.position += buttonMovement.normalized * Speed * Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(buttonMovement);
        }

        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

     
        if (isGrounded && Jimpup)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        }

    }
    public void OnButtonDownJimp()
    {
        Jimpup = true;
    }
    public void OnButtonUpJimp()
    {  
        Jimpup = false; 
    }
    public void OnButtonDown()
    {
        isMoving = true;
    }

    public void OnButtonUp()
    {
        isMoving = false;
    }
    public void OnButtonDownL()
    {
        isMovingL = true;
    }

    public void OnButtonUpL()
    {
        isMovingL = false;
    }
    public void OnButtonDownVerh()
    {
        isMovingVerh = true;
    }
    public void OnButtonUpVerh()

    { isMovingVerh = false; }
    public void OnButtonDownNiz()
    { isMovingNiz = true; }

    public void OnButtonUpNiz()
    { isMovingNiz = false; }
   
}

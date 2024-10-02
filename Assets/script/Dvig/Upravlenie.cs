using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Upravlenie : MonoBehaviour
{
    public float speed = 5f; // ��� ����� � ����������
    // Start is called before the first frame update
    [SerializeField] float Speed; //�������� ��������
    [SerializeField] float jumpForce = 50000.0f; // ���� ������
    private bool isMoving = false;
    private bool isMovingL = false;
    private bool isMovingVerh = false;
    private bool isMovingNiz = false;
    private bool Jimpup = false;


    public Transform groundCheck; // ����� �������� ��������������� � ������
    public float groundCheckRadius = 0.2f; // ������ �������� ���������������
    public LayerMask groundLayer; // ����, �� ������� �������� ��������� �������

    private Rigidbody rb; // ��������� Rigidbody ���������
    private bool isGrounded; // ����, �����������, ����� �� �������� �� �����

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // ��������� ���������� Rigidbody
    }

   

    void Update()
    {
        // �������� ���� ������������
        float moveX = Input.GetAxis("Horizontal"); //��� �����
        float moveZ = Input.GetAxis("Vertical");

        // ������� ������ ��������
        Vector3 movement = new Vector3(moveX, 0f, moveZ); //��� �����

        // ��������� �������� � �������
        transform.position += movement * speed * Time.deltaTime; // ��� �����

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

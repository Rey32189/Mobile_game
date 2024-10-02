using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrul : MonoBehaviour
{
    public float speed; // �������� �������� �����

    public int positionOfPatrol; //��������� �������������� ����������
    //public Transform point; //����� ������ ��������������, ����� �������� ����������
    [SerializeField] Vector3 point2; //����� ������ ��������������, ����� �������� ����������
    bool movingRight; // ���������� ��� �������� ����������
    bool movingForward; // ���������� ��� �������� ������-�����

    Transform player; //���������� ��� ��������� �����
    public float stoppingDistance; //���������� �� ���������� �� �������� �����

    bool chill = false;
    bool angry = false;
    bool goBack = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // ��� ������ ������� � ����� ����� 
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, point2) < positionOfPatrol && angry == false) // ��� ��������� ����� � ������� �� ������ �����������
        {
            chill = true;
        }

        if (Vector3.Distance(transform.position, player.position) < stoppingDistance) // ��������� ���� ���������� ����� ���������� � ������� ������ ����������
        {
            angry = true;
            chill = false;
            goBack = false;
        }
        if (Vector3.Distance(transform.position, player.position) > stoppingDistance) // ��������� ���� ���������� ����� ���������� � ������� ������ ����������
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

    void Chill() //��������� ��������������
    {
        if (transform.position.x > point2.x + positionOfPatrol)  // ����� ������� �� ���� �����, ���������������
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
            transform.position = new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y, transform.position.z); // ����������� �������� ����������
        }
        else
        {
            transform.position = new Vector3(transform.position.x - speed * Time.deltaTime, transform.position.y, transform.position.z);
        }
        // ��������: ��������� �������� �� ��� z
        float moveX = movingRight ? speed * Time.deltaTime : -speed * Time.deltaTime;
        float moveZ = movingForward ? speed * Time.deltaTime : -speed * Time.deltaTime;

        transform.position = new Vector3(transform.position.x + moveX, transform.position.y, transform.position.z + moveZ);
    }

    void Angry() // ��������� ���� � ������ �� ������
    {
        //MoveTowards - ������ ��������� ������ ��������� � �������
        transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime); 
    }

    void GoBack() // ��������� �������� � ����� ��������������
    {
        transform.position = Vector3.MoveTowards(transform.position, point2, speed * Time.deltaTime);
    }
}

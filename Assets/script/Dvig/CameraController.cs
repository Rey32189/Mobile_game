using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float dumping = 3f; // ��� ������� ��������� ������
    public Vector3 offset = new Vector3(1f, 1f, -3f); // ������ ������������ ���������
    public bool isLeft; // ���������� ������ ���������, �� ������ ���� ������� �����
    private Transform player; // ���������� ��������� ���������
    private int lastX; // ����������, � ����� ������� ������� ��������
    public float distansCamera;

    public Camera mainCamera; // ��������� ��� ������� � ���������� ������
    // Start is called before the first frame update
    void Start()
    {
        offset = new Vector3(Mathf.Abs(offset.x), offset.y, offset.z); //������������� ���������� ��������� ������
        FindPlayer(isLeft);


        // ����������� ������ ��������������� ������
        if (mainCamera != null && mainCamera.orthographic)
        {
            mainCamera.orthographicSize = distansCamera; // ��������� �������� size
        }

    }
    
    public void FindPlayer(bool playerIsLeft)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; //��� �� ��������� ����� ���� ����� �� ����
        lastX = Mathf.RoundToInt(player.position.x); //������ ��������� �������� �� ��� �
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
                target = new Vector3(player.position.x + offset.x, player.position.y + offset.y, transform.position.z); //  player.position.z + offset.z) ���� ������ ��� �� ��������� �� z
            }

            Vector3 currentPosition = Vector3.Lerp(transform.position, target, dumping * Time.deltaTime);
            transform .position = currentPosition;
        }
    }
}

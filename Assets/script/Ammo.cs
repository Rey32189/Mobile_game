using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour // ��������� ��������
{
    public float speed; // �������� �������
    public float destroyTime; // ����� �� ���������� �������
    public int damage; //���������� �����
    

    public Rigidbody rb;

    void Start()
    {
        Invoke("DestroyAmmo", destroyTime);
        rb.velocity = transform.forward * speed;
    }
    private void OnTriggerEnter(Collider hitInfo) // ���� ������ � ����������� �������
    {
       
        if (hitInfo.CompareTag("Vrag"))
        {
            Enemy enemy = hitInfo.GetComponent<Enemy>(); // ����� ����� ���� �� ����� ��� ��������������
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject); // ����� �������������� ��������� ������
        }
        else // �������� �� ��, ��� �� �������� �� ����������� ��� ������� � ������� � ���� ������
        {
            return;
        }

    }
void DestroyAmmo()
    {
        Destroy(gameObject);
    }
}

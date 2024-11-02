using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damag_vrag : MonoBehaviour
{
    public int damage_player; //���������� �����
    //��������� ����� ������
    private void OnTriggerEnter(Collider hitInfo) // ���� ������ � ����������� �������
    {

        if (hitInfo.CompareTag("Player"))
        {
            HealthBar healthBar = hitInfo.GetComponent<HealthBar>(); // ����� ����� ���� �� ����� ��� ��������������
            if (healthBar != null)
            {
                healthBar.TakeDamage_player(damage_player);
            }
            //Destroy(gameObject); // ����� �������������� ��������� ������
        }
        else // �������� �� ��, ��� �� �������� �� ����������� ��� ������� � ������� � ���� ������
        {
            return;
        }

    }
}

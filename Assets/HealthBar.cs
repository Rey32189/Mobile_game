using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class HealthBar : MonoBehaviour
{
    public Image bar;
    public float fill;
    public int health; // ����������� ��������
    public TextMeshProUGUI playerHealth; // ��������� ����������� ������
    public int max_health = 15; // ������������ ����������� ������

    // Start is called before the first frame update
    void Start()
    {
        fill = 1f; // ����� �������� ��������� �� 100%
    }

    // Update is called once per frame
    void Update()
    {
        // ��������� ���������� ����� ��������
        fill = (float)health / max_health; // ��������� ������� ���������� �����
        bar.fillAmount = fill; // ������������� �������� ���������� � ��������� Image
        playerHealth.text = health.ToString(); // ��������� ����� � ����������� ������
    }

    public void TakeDamage_player(int damage_player)
    {
        health -= damage_player; // ��������� �������� �� ���������� ����
        if (health < 0) // ���������, ����� �������� �� ����� �������������
        {
          health = 0; // ������������� �������� � 0, ���� ��� ������ 0
        }
    }
    
   // public void TakeDamage_player(int damage_player)
    //{
    //    health -= damage_player;
        //spriteRend.material = matBlink; // ����� ������, ����� ����������

        //if (health <= 0) // ����� ����� ���������� �� 0 ������ �����������
        //{
        //    Die();
        //}
        //else
        //{
        //    Invoke("ResetMaterial", 0.5f); //���� �� �����, ��������� ������ ����� 0.2 �������
        //}
    //}

}

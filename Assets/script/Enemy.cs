using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour // ������� ���������� � ��� ������
{
    public int health; // ����������� ������

    private Material matBlink; // �������� �������
    private Material matDefault; // ������������ ��������

    private MeshRenderer spriteRend; //��� ������ �� ������ ��������

    private UnityEngine.Object vragRef;

    private UnityEngine.Object explosion; // ��������� ������������ ������ �������

    [SerializeField] float timeDestroy; // ����� �������������� ����������

    [SerializeField] Vector3 spavnPos; // ������� ��������� ����������

    private LootDropper lootDropper; // ��� ��������� ��������


    private void Start()
    {
        lootDropper = GetComponent<LootDropper>(); //�������������� 
        // spavnPos = transform.position; // ��������� ����������

        explosion = Resources.Load("Explosion");

        vragRef = Resources.Load("Vrag");

        spriteRend = GetComponent<MeshRenderer>(); // ����������� ��������� ������ ������

        matBlink = Resources.Load("EnemyBlink", typeof(Material)) as Material; //Resources - ���� ����� � ���������� load ����������� "EnemyBlink" ������ ��� �� � ���� ���������
        //  typeof(Material) �� ������ ���� ���� ����� ��� �� as Material ����������� ��� ��� ��������
        matDefault = spriteRend.material; // ������� ��������, ������� ���� ������
    }

    void ResetMaterial()
    {
        spriteRend.material = matDefault; // ������������ ������� ��������
    }
    void KillEnemy()
    {
        Destroy(gameObject); // �������� �� ���������� �������
    }
    public void TakeDamage(int damage)
    {
        health -= damage;
        spriteRend.material = matBlink; // ����� ������, ����� ����������

        if (health <= 0) // ����� ����� ���������� �� 0 ������ �����������
        {
            Die();
        }
        else
        {
            Invoke("ResetMaterial", 0.5f); //���� �� �����, ��������� ������ ����� 0.2 �������
        }
    }
    
    
    void Die()
    {
        //Destroy(gameObject); // �������� �� ���������� �������, �� �� �������� ��� ��������
        GameObject explosionRef = (GameObject)Instantiate(explosion);// ���������� ��������� ������
        explosionRef.transform.position = new Vector3 (transform.position.x, transform.position.y, transform.position.z); // ��� ���������� �������

        lootDropper.DropLoot(); // --------------------


        gameObject.SetActive(false);
        Invoke("Respawn", timeDestroy); // Invoke ��������� ������� ������� � ��������� ����� ������������
        Destroy(explosionRef, 2f);
    }

    void Respawn()
    {
        GameObject vragCopi = (GameObject)Instantiate(vragRef); // �������� ������, ������� ����� ��������� ����������� �� ���� �������
        
        vragCopi.transform.position = new Vector3(UnityEngine.Random.Range(spavnPos.x -3, spavnPos.x +3), spavnPos.y, spavnPos.z);
        Destroy(gameObject); // ���������� ���������� �����
    }
}

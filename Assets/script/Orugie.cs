using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Orugie : MonoBehaviour // ������ �� ��������
{
    public Transform firePoint; // ������ �� �������� ������ �������� ������
    public GameObject bullet; // ���� ����������� ����� �������


    private float timeShot; // ��� �������� ������� ��������
    public float startTime; // ��� �������� ������� ��������

    public int currentAmmo; // ������� ���������� ����������� � ������
    public int allAmmo; // ��� ���������� � �������
    public int fullAmmo; //������������ ����������� ���� ��� ��������

    [SerializeField]
    private TextMeshProUGUI ammoCount; // ��� ��������

    private bool attack = false;
    void Update()
    {
        
        if (timeShot <= 0)
        {
            if (Input.GetButtonDown("Fire1") && currentAmmo > 0) // ��� ����� � �����
            {
                Shoot();
                timeShot = startTime;
                currentAmmo -= 1;
            }
            if (attack && currentAmmo > 0) //��� ����� � ������
            {
                Shoot();
                timeShot = startTime;
                currentAmmo -= 1;
            }
        }
        else
        {
            timeShot -= Time.deltaTime; // ����� ���������� ��������
        }
        ammoCount.text = currentAmmo + " / " + allAmmo;
        
        if (Input.GetKeyDown(KeyCode.R) && allAmmo > 0) // ������� ��� �����������
        {
            Invoke("Reload", 1f);
        }
    }

    public void Reload()
    {
        int reason = 15 - currentAmmo;
        if (allAmmo >= reason)
        {
            allAmmo = allAmmo - reason;
            currentAmmo = 15;
        }
        else
        {
            currentAmmo = currentAmmo + allAmmo;
            allAmmo = 0;
        }
    }
    void Shoot()
    {
        Instantiate(bullet, firePoint.position, firePoint.rotation); // ������� �������
    }
    public void AttackDown() // ��� �������� � �������
    {
        attack = true;
    }
    public void AttackUp() // ��� �������� � �������
    {
        attack = false;
    }

    private void OnTriggerEnter(Collider collision)
    {
        pistolClic pistolClicScript = collision.GetComponent<pistolClic>();
        
        //if (collision.GetComponent<pistolClic>() && currentAmmo < fullAmmo && allAmmo < fullAmmo)
        if (pistolClicScript != null && allAmmo < fullAmmo)
        {
            if (pistolClicScript.randomAmmo + allAmmo < fullAmmo)
            {
                allAmmo += pistolClicScript.randomAmmo;
            }
            else
            {
                allAmmo = fullAmmo;
            }
            
            Destroy(collision.gameObject);
        }
        
    }

}

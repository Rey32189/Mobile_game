using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pistolClic : MonoBehaviour
{
    // ������ ������, ��� ��������� � ���� ������� �������

    // ��������� ���������� ��� �������� ���������� ���������� ���� � �������
    public int randomAmmo = 0;
    // Start is called before the first frame update
    void Start()
    {
        randomAmmo = Random.Range(3, 16); // �� 3 �� 15 ������������
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ����� ��� ��������� �������������� � ���������
public class ItemPickup : MonoBehaviour
{
    public string itemName; // ��� ��������
    public Sprite itemIcon; // ������ ��������
    public int quantity = 1; // ���������� ��������� (�� ��������� 1)

    private InventoryManager inventoryManager; // ������ �� ������ InventoryManager

    void Start()
    {
        // ������� InventoryManager � �����
        inventoryManager = FindObjectOfType<InventoryManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        // ���������, ��� ����� ������������� � ���������
        if (other.CompareTag("Player"))
        {
            // ������� ������� � �������� ������, ������� � �����������
            StackableItem item = new StackableItem(itemName, itemIcon, quantity);

            // ��������� ������� � ���������
            if (inventoryManager != null)
            {
                // ���������, ������� �� �������� ������� � ���������
                bool added = inventoryManager.AddItem(item);

                // ������� ������� �� ����� ������ ���� �� ��� ������� �������� � ���������
                if (added)
                {
                    Destroy(gameObject);
                }
                else
                {
                    Debug.Log("��������� �����, ������� �� ��� ��������: " + itemName);
                }
            }
        }
    }
}

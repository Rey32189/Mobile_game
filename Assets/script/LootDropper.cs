using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootDropper : MonoBehaviour
{
    // ������ ��� ��������� ���������
    [System.Serializable]
    public class LootEntry
    {
        public GameObject itemPrefab; // ������ ��������
        public float dropChance;     // ����������� ��������� (�� 0 �� 1)
        public Vector3 forceRange;   // �������� ���� ������ (x, y, z)
    }

    public LootEntry[] lootTable; // ������� ������������
    public int maxDrops = 3;       // ������������ ���������� ���������� ���������

    // ����� ��� ��������� ���������
    public void DropLoot()
    {
        List<LootEntry> possibleDrops = new List<LootEntry>();

        // ������� ������ ��������� ���������, ������� ����� �������
        foreach (var loot in lootTable)
        {
            if (Random.value < loot.dropChance)
            {
                possibleDrops.Add(loot);
            }
        }

        // ������������ ���������� ���������� ���������
        int numberOfDrops = Mathf.Min(possibleDrops.Count, maxDrops);

        for (int i = 0; i < numberOfDrops; i++)
        {
            // �������� ��������� ������� �� ������ ���������
            LootEntry selectedLoot = possibleDrops[Random.Range(0, possibleDrops.Count)];

            // ������� ������� � ����
            GameObject item = Instantiate(selectedLoot.itemPrefab, transform.position, Quaternion.identity);

            // �������� ��������� Rigidbody ��������
            Rigidbody rb = item.GetComponent<Rigidbody>();

            // ���� Rigidbody ����������, ��������� ����
            if (rb != null)
            {
                // ���������� ��������� ���� � �������� ���������� ���������
                Vector3 force = new Vector3(
                    Random.Range(-selectedLoot.forceRange.x, selectedLoot.forceRange.x),
                    Random.Range(-selectedLoot.forceRange.y, selectedLoot.forceRange.y),
                    Random.Range(-selectedLoot.forceRange.z, selectedLoot.forceRange.z)
                );

                // ��������� ���� � Rigidbody
                rb.AddForce(force, ForceMode.VelocityChange);
            }
        }
    }
}




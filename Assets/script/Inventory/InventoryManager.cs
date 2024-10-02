using System;
using System.Collections;
using System.Collections.Generic; // ��� ������ �� ��������
using Unity.VisualScripting;
using UnityEngine; // �������� ������������ ��� Unity
using UnityEngine.UI; // ��� ������ � UI ����������
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public GameObject itemPrefab; // ������ ��� ����������� ��������� � ���������
    public Transform inventoryPanel; // ������ ��������� ��� ���������� ���������
    public Transform playerTransform; // ������� ������ ��� ���������
    public List<ItemPrefabEntry> itemPrefabsList;  // ������� ��� �������� �������� ���������

    private Dictionary<string, GameObject> itemWorldPrefabs = new Dictionary<string, GameObject>(); // ������� ��� �������� �������� ��� ������ ����� ���������
    private List<StackableItem> items = new List<StackableItem>(); // ������ ��� �������� ���������
    void Start()
    {
        // ��������������� ������� �������� �� ������
        itemWorldPrefabs.Clear();
        foreach (var entry in itemPrefabsList)
        {
            itemWorldPrefabs[entry.itemName] = entry.prefab;
        }

        DisplayItems(); // ���������� ��������� (���������� ������)
    }

    // ����� ��� ���������� �������� � ���������
    public void AddItem(StackableItem newItem)
    {
        // ����� �������� � ������
        List<StackableItem> existingStacks = items.FindAll(item => item.itemName == newItem.itemName);

        // ��������� �������� � ������������ ������, ���� �� ���������� �������� ��� ������
        foreach (var stack in existingStacks)
        {
            if (newItem.quantity == 0) break; // ���� �������� �����������, ������� �� �����

            int quantityToAdd = Mathf.Min(newItem.quantity, 3 - stack.quantity); // ������� ��������� ����� �������� � ��� ������
            stack.quantity += quantityToAdd;
            newItem.quantity -= quantityToAdd;
        }

        // ���� �������� ��������, ������� ����� ������
        while (newItem.quantity > 0)
        {
            int quantityToAdd = Mathf.Min(newItem.quantity, 3); // ������� ��������� ����� �������� � ����� ������
            items.Add(new StackableItem(newItem.itemName, newItem.itemIcon, quantityToAdd));
            newItem.quantity -= quantityToAdd;
        }

        DisplayItems(); // ��������� ����������� ���������
    }

    // ����� ��� �������� �������� �� ���������
    public void RemoveItem(StackableItem itemToRemove, bool destroyImmediately = false)
    {
        if (itemToRemove != null)
        {
            itemToRemove.quantity -= 1;  // ���������� ���������� ������������� ��������

            if (itemToRemove.quantity <= 0)
            {
                items.Remove(itemToRemove);// �������� ��������, ���� ��� ���������� ����� ���� ��� ������
            }

            if (destroyImmediately)
            {
                DestroyItemInWorld(itemToRemove); // �������� �������� �� ����
            }
        }

        DisplayItems(); // ��������� ����������� ���������
    }

    //����� ��� �������� �������� � ����
    private void DestroyItemInWorld(StackableItem item)
    {
        if (playerTransform != null)
        {
            // ��������� ��������� �������� �� ���������� �� 1 �� 2 ������
            Vector3 offset = new Vector3(UnityEngine.Random.Range(1f, 2f), 0f, UnityEngine.Random.Range(1f, 2f));

            // �������� ������ ��� �������� �������� � ���� � ����������� �� ����� ��������
            if (itemWorldPrefabs.TryGetValue(item.itemName, out GameObject itemWorldPrefab))
            {
                // ������� ������� � ���� �� ��������� �������
                GameObject itemWorldObject = Instantiate(itemWorldPrefab, playerTransform.position + offset, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("No prefab assigned for item: " + item.itemName);
            }
        }
        else
        {
            Debug.LogWarning("Player Transform is not assigned!");
        }
    }

    // ����� ��� ����������� ��������� � UI
    void DisplayItems()
    {
        // ������� ������ �������� �� ������
        foreach (Transform child in inventoryPanel)
        {
            Destroy(child.gameObject); // ������� ������ ������� �� ������
        }

        // ������� ����� �������� ��� ������� ��������
        foreach (var item in items)
        {
            // ������� ����� ������� UI ��� ��������
            GameObject itemObject = Instantiate(itemPrefab, inventoryPanel);

            // ����������� ����������� ������ ��������
            Image itemImage = itemObject.GetComponentInChildren<Image>();
            if (itemImage != null)
            {
                itemImage.sprite = item.itemIcon; // ������������� ������ ��������
            }

            // ����������� ����������� ���������� ���������
            Text itemText = itemObject.GetComponentInChildren<Text>();
            if (itemText != null)
            {
                itemText.text = $"{item.itemName} ({item.quantity})"; // ������������� ����� ��� ��� �������� � ����������
            }

            // ��������� ������ ��� ������������ �������� � ���

            Button throwButton = itemObject.transform.Find("Delit_Predmet")?.GetComponentInChildren<Button>();
            if (throwButton != null)
            {
                throwButton.onClick.AddListener(() => ThrowItem(item));
            }
        }
    }

    // ����� ����� ��� ������������ �������� � ���
    void ThrowItem(StackableItem item)
    {
        RemoveItem(item, true);
    }
}

[System.Serializable]
public class ItemPrefabEntry
{
    public string itemName;
    public GameObject prefab;
}
// ����� ��� �������� ���������, ������� ����� ���������� � ������
[System.Serializable]
public class StackableItem
{
    public string itemName; // ��� ��������
    public Sprite itemIcon; // ������ ��������
    public int quantity; // ���������� ��������� � ������

    // ����������� ��� �������� ������ ���������
    public StackableItem(string name, Sprite icon, int quantity)
    {
        this.itemName = name; // ������������� ��� ��������
        this.itemIcon = icon; // ������������� ������ ��������
        this.quantity = quantity; // ������������� ���������� ���������
    }
}
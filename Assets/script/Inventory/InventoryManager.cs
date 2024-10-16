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
    public int maxSlots = 5; // ������������ ���������� ����� � ���������

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
    public bool AddItem(StackableItem newItem)
    {
        // ���������� ��� ������������ ���������� ����������� ���������
        int totalAdded = 0;

        // ����� �������� � ������
        List<StackableItem> existingStacks = items.FindAll(item => item.itemName == newItem.itemName);

        // ��������� �������� � ������������ ������, ���� �� ���������� �������� ��� ������
        foreach (var stack in existingStacks)
        {
            if (newItem.quantity == 0) break; // ���� �������� �����������, ������� �� �����

            // ������� ��������� ����� �������� � ��� ������
            int quantityToAdd = Mathf.Min(newItem.quantity, 3 - stack.quantity);

            // ��������, ����� �� ��������� ������������ ���������� � ������
            if (quantityToAdd > 0)
            {
                stack.quantity += quantityToAdd;
                newItem.quantity -= quantityToAdd;
                totalAdded += quantityToAdd; // ����������� ����� ���������� ����������� ���������
            }
        }

        // ���� �������� ��������, ������� ����� ������
        while (newItem.quantity > 0 && items.Count < maxSlots)
        {
            int quantityToAdd = Mathf.Min(newItem.quantity, 3); // �������� 3 �������� � ����� ������

            // ������� ����� ������ ������ ���� ���� ����������� �������� ��������
            if (quantityToAdd > 0)
            {
                items.Add(new StackableItem(newItem.itemName, newItem.itemIcon, quantityToAdd));
                newItem.quantity -= quantityToAdd;
                totalAdded += quantityToAdd; // ����������� ����� ���������� ����������� ���������
            }
        }

        if (totalAdded == 0)
        {
            return false; // �� ������� �������� �� ������ ��������
        }
        DisplayItems(); // ��������� ����������� ���������
        return true; // �������� ���� ������� ���������
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

            Dropdown dropdownMenu = itemObject.GetComponentInChildren<Dropdown>(true);
            if (dropdownMenu != null)
            {
                dropdownMenu.onValueChanged.AddListener(delegate
                {
                    DropdownValueChanged(dropdownMenu, item);
                });
            }
        }
    }

    // ����� ����� ��� ������ ���������� ���������
    void DropdownValueChanged(Dropdown change, StackableItem item)
    {
        switch (change.value)
        {
            //case 0:
            //    // ������ ��� ����������� ���������� � ��������
            //    break;
            case 0:
                // ������ ��� ����������� ���������� � ��������
                break;
            case 1:
                // ������ ��� ���������� ��������
                break;
            case 2:
                // ������ ��� ������������ �������� � ���
                RemoveItem(item, true);
                break;
            default:
                break;
        }
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

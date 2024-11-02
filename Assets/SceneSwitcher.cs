using UnityEngine;
using UnityEngine.SceneManagement; // ����������� ������������ ���� ��� ������ �� �������

public class SceneSwitcher : MonoBehaviour
{
    // ����� ��� �������� � ����� �� �����
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // ������ ������ ��� �������� � �������� ����
    public void LoadMainMenu()
    {
        LoadScene("SampleScene"); // �������� "MainMenu" �� ��� ����� �����
    }

    // ������ ������ ��� �������� � ������� �����
    public void LoadGameScene()
    {
    LoadScene("Baza_start"); // �������� "GameScene" �� ��� ����� �����
       
    }
   
    void Start()
    {
        
    }
    //public void LoadInventor()
    //{
    //    Inventory inventoryManager = FindObjectOfType<Inventory>();
    //    if (inventoryManager != null)
    //    {
    //        inventoryManager.LoadInventory();
    //    }
    //    else
    //    {
    //        Debug.LogWarning("InventoryManager not found!");
    //    }
    //}    
}




using UnityEngine;
using UnityEngine.SceneManagement; // Импортируем пространство имен для работы со сценами

public class SceneSwitcher : MonoBehaviour
{
    // Метод для перехода к сцене по имени
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Пример метода для перехода к главному меню
    public void LoadMainMenu()
    {
        LoadScene("SampleScene"); // Замените "MainMenu" на имя вашей сцены
    }

    // Пример метода для перехода к игровой сцене
    public void LoadGameScene()
    {
    LoadScene("Baza_start"); // Замените "GameScene" на имя вашей сцены
       
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




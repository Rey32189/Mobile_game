using Unity.VisualScripting;
using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    public PlayerInventory playerInventory; // Ссылка на PlayerInventory
    public SpriteRenderer spriteRenderer; // Ссылка на компонент SpriteRenderer
    public int weaponNumber; // Переменная для хранения текущего номера оружия
    private int lastItemID = -1; // Начальное значение, которое гарантированно не совпадет с валидным itemID
    void Start()
    {
        // Убедитесь, что playerInventory ссылается на правильный объект
        if (playerInventory == null)
        {
            playerInventory = FindObjectOfType<PlayerInventory>();
        }
    }

    void Update()
    {
        int currentItemID = playerInventory.GetItemIDWeapon();

        if (currentItemID != lastItemID) // Проверяем на изменение
        {
            lastItemID = currentItemID; // Обновляем последнее значение
            SetWeaponByItemID(currentItemID); // Вызываем метод только при изменении
        }
    }
    
    
    private void LoadSprite(string spriteName)
    {
        Sprite loadedSprite = Resources.Load<Sprite>(spriteName);
        if (loadedSprite != null)
        {
            spriteRenderer.sprite = loadedSprite; // Устанавливаем загруженный спрайт
            SetSpriteSize(new Vector3(0.2f, 0.2f, 0.2f)); // Устанавливаем фиксированный размер
        }
        else
        {
            Debug.LogWarning($"Спрайт с именем {spriteName} не найден в папке Resources.");
        }
    }

    private void SetSpriteSize(Vector3 size)
    {
        transform.localScale = size; // Устанавливаем размер спрайта
    }

    public void SetWeaponByItemID(int itemID)
    {
        Debug.Log("В метод SetWeaponByItemID отправлен " + itemID);
        if (itemID == 23) // ID 23 соответствует автомату
        {
            Debug.Log("Установлен номер 1 ");
            weaponNumber = 1;
        }
        else if (itemID == 9) // ID 2 соответствует пистолету
        {
            Debug.Log("Установлен номер 2 ");
            weaponNumber = 2;
        }
        else
        {
            weaponNumber = 0; // Неизвестное оружие
        }
        Debug.Log("установили спрайт ");
        // Обновляем спрайт сразу после изменения weaponNumber
        UpdateWeaponSprite();
    }

    private void UpdateWeaponSprite()
    {
        if (weaponNumber == 1)
        {
            LoadSprite("Orugie/AutomaticWeapon");
        }
        else if (weaponNumber == 2)
        {
            LoadSprite("Orugie/Pistol");
        }
        else
        {
            spriteRenderer.sprite = null; // Удаление спрайта
        }
    }
}

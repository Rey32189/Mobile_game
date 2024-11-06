using UnityEngine; // Подключаем пространство имен Unity для доступа к игровым объектам и компонентам
using System.Collections; // Подключаем пространство имен для работы с коллекциями и корутинами

public class PickUpItem : MonoBehaviour // Определяем класс PickUpItem, который наследует от MonoBehaviour
{
    public Item item; // Публичная переменная для хранения информации о предмете, который можно подобрать
    private Inventory _inventory; // Закрытая переменная для хранения ссылки на инвентарь игрока
    private GameObject _player; // Закрытая переменная для хранения ссылки на объект игрока

    // Метод, вызываемый при инициализации
    void Start()
    {
        // Находим объект игрока по тегу "Player"
        _player = GameObject.FindGameObjectWithTag("Player");
        // Если объект игрока найден, получаем его инвентарь
        if (_player != null)
            _inventory = _player.GetComponent<PlayerInventory>().inventory.GetComponent<Inventory>();
    }
    void Update()
    {
        // Проверяем, есть ли инвентарь и нажата ли клавиша E
        if (_inventory != null && Input.GetKeyDown(KeyCode.E))
        {
            // Вычисляем расстояние между предметом и игроком
            float distance = Vector3.Distance(this.gameObject.transform.position, _player.transform.position);

            // Проверяем, находится ли игрок в пределах 3 единиц расстояния от предмета
            if (distance <= 3)
            {
                // Проверяем, существует ли предмет уже в инвентаре
                bool check = _inventory.checkIfItemAllreadyExist(item.itemID, item.itemValue);
                if (check) // Если предмет уже есть
                    Destroy(this.gameObject); // Удаляем предмет из мира
                else if (_inventory.ItemsInInventory.Count < (_inventory.width * _inventory.height)) // Если инвентарь не полон
                {
                    // Добавляем предмет в инвентарь
                    _inventory.addItemToInventory(item.itemID, item.itemValue);
                    _inventory.updateItemList(); // Обновляем список предметов в инвентаре
                    _inventory.stackableSettings(); // Настраиваем параметры для стекуемых предметов
                    Destroy(this.gameObject); // Удаляем предмет из мира
                }

            }
        }
    }
}
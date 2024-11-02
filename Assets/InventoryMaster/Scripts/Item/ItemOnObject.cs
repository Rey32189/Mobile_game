using UnityEngine;                                        // Подключаем пространство имен для работы с Unity
using System.Collections;                                 // Подключаем пространство имен для работы с коллекциями
using UnityEngine.UI;                                    // Подключаем пространство имен для работы с UI элементами
using UnityEngine.EventSystems;                          // Подключаем пространство имен для работы с событиями интерфейса

public class ItemOnObject : MonoBehaviour               // Определяем класс ItemOnObject, наследующий от MonoBehaviour
{
    public Item item;                                    // Объявляем публичное поле для хранения информации об объекте Item
    private Text text;                                   // Объявляем приватное поле для хранения ссылки на компонент Text
    private Image image;                                 // Объявляем приватное поле для хранения ссылки на компонент Image

    void Update()                                        // Метод, который вызывается каждый кадр
    {
        text.text = "" + item.itemValue;                // Обновляем текстовое поле, устанавливая значение itemValue из объекта Item
        image.sprite = item.itemIcon;                   // Устанавливаем спрайт изображения в соответствии с иконкой предмета
        GetComponent<ConsumeItem>().item = item;       // Получаем компонент ConsumeItem и присваиваем ему текущий предмет
    }

    void Start()                                         // Метод, который вызывается при инициализации объекта
    {
        image = transform.GetChild(0).GetComponent<Image>(); // Получаем компонент Image из первого дочернего объекта
        transform.GetChild(0).GetComponent<Image>().sprite = item.itemIcon; // Устанавливаем спрайт для изображения предмета
        text = transform.GetChild(1).GetComponent<Text>(); // Получаем компонент Text из второго дочернего объекта
    }
}


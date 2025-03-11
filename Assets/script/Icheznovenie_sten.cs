using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Icheznovenie_sten : MonoBehaviour
{

    public int requiredEnemiesToDestroy; // Количество врагов для исчезновения
    public int enemiesDefeated = 0; //стартовое значение
    public int nomer_stens; // Значение стены для связи с врагами

    // Метод для увеличения счетчика убитых врагов
    public void EnemyDefeated()
    {
        enemiesDefeated++;
        CheckForDisappearance();
    }

    // Проверка, нужно ли исчезнуть стене
    private void CheckForDisappearance()
    {
        if (enemiesDefeated >= requiredEnemiesToDestroy)
        {
            Destroy(gameObject); // Уничтожаем стену
            NotifyEnemies(); // Уведомляем врагов о том, что стена исчезла
        }
    }
    private void NotifyEnemies()
    {
        // Найдите всех врагов в сцене и обнулите ссылку на стену
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            enemy.DisappearingWallDestroyed(); // Уведомляем врага
        }
    }
}

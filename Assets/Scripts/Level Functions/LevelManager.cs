using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    // the event that gets all enemies
    [SerializeField] private GameObjectEvent _getEnemiesEvent;

    // make a list to store the enemies in the level
    [SerializeField] private List<GameObject> _enemiesInLevel;

    private void Start()
    {
        _getEnemiesEvent.Invoke(this.gameObject);
    }

    public void AddEnemyToLevel(GameObject enemy)
    {
        _enemiesInLevel.Add(enemy);
    }

    public void RemoveEnemyFromLevel(GameObject enemy)
    {
        _enemiesInLevel.Remove(enemy);
    }
}

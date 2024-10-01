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

    [SerializeField] private BoolEvent _openExit;

    private void Start()
    {
        // ping all enemies to populate the enemies in level list
        _getEnemiesEvent.Invoke(this.gameObject);
    }

    // method to trigger for each enemy that pings back to level manager
    public void AddEnemyToLevel(GameObject enemy)
    {
        _enemiesInLevel.Add(enemy);
    }

    // when an enemy death event triggers, remove the enemy from the list of enemies
    public void RemoveEnemyFromLevel(GameObject enemy)
    {
        _enemiesInLevel.Remove(enemy);

        // if there are no more enemies in the list, enable the level exit
        if(_enemiesInLevel.Count <= 0)
        {
            EnableLevelExit();
        }
    }

    
    private void EnableLevelExit()
    {
        _openExit.Invoke(this.gameObject);
    }
}

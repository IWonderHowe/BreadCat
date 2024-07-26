using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelExit : MonoBehaviour
{
    private Collider _exitCollider;
    private bool _exitActive = false;
    private GameObject _gameManager;

    private void Awake()
    {
        _exitCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(_exitActive == true && other.gameObject.GetComponent<PlayerCombat>() != null)
        {
            Debug.Log("You win this level!");
        }
    }

    public void SetExitActive()
    {
        _exitActive = true;
    }
}

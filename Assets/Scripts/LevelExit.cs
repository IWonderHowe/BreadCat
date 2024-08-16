using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelExit : MonoBehaviour
{
    private Collider _exitCollider;
    private bool _exitActive = false;
    private GameObject _gameManager;

    [SerializeField] private BoolEvent _exitTriggered;

    private void Awake()
    {
        _exitCollider = GetComponent<Collider>();
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(_exitActive == true && other.gameObject.GetComponent<PlayerCombat>() != null)
        {
            _exitTriggered.Invoke(true);
        }
    }

    public void SetExitActive()
    {
        _exitActive = true;
    }
}

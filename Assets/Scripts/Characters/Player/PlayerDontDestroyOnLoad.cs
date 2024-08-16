using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDontDestroyOnLoad : MonoBehaviour
{

    private GameObject _player;
    [SerializeField] private GameObjectEvent _playerPing;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //_playerPing.Invoke(_player);
        //_player.GoToEntrance();
    }

    private IEnumerator DelayedPlayerPing(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _playerPing.Invoke(_player);
    }

    // Start is called before the first frame update
    void Start()
    {
        // make sure there is only ever one upgrade manager
        GameObject[] playerContainers = GameObject.FindGameObjectsWithTag("UpgradeManager");

        if (playerContainers.Length > 1)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);

        _player = GetComponentInChildren<PlayerController>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

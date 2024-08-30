using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDontDestroyOnLoad : MonoBehaviour
{

    //[SerializeField] private GameObjectEvent _playerPing;

    // Start is called before the first frame update
    void Start()
    {
        // make sure there is only ever one upgrade manager, as it is the only object tagged upgrade manager under the player
        GameObject[] playerContainers = GameObject.FindGameObjectsWithTag("UpgradeManager");

        // destroy this object if theres more than one player object in the scene
        if (playerContainers.Length > 1)
        {
            Destroy(this.gameObject);
        }
        
        // add this playerobject to not be destroyed on scene loads
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

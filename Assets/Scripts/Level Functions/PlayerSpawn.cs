using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    private GameObject _player; 

    // upon being pinged by the player object, set the player to the positon of this object
    public void SetPlayerPosition(GameObject player)
    {
        player.transform.position = this.gameObject.transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), 1f);
    }
}

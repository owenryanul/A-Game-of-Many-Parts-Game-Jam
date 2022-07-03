using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oryan_FollowPlayerScript : MonoBehaviour
{
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("base_player");
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.position = player.transform.position;
    }
}

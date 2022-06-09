using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleStatusEffectMonitorLogic : MonoBehaviour, OnDeathListener
{
    private GameObject player;
    private GameObject waterParticleEmitter;
    public GameObject waterParticleEmitterPrefab;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("base_player");
        player.GetComponent<PlayerLogic>().addOnDeathListener(this);
        waterParticleEmitter = Instantiate(waterParticleEmitterPrefab, player.transform.position, player.transform.rotation);
        waterParticleEmitter.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        waterParticleEmitter.transform.position = player.transform.position;
        string exampleStatus = player.GetComponent<PlayerLogic>().getStatusData("Wet");
        if (exampleStatus != null)
        {
            float durationLeft = float.Parse(exampleStatus);
            durationLeft -= Time.deltaTime;
            if(durationLeft <= 0)
            {
                player.GetComponent<PlayerLogic>().removeStatus("Wet");
                waterParticleEmitter.SetActive(false);
            }
            else
            {
                player.GetComponent<PlayerLogic>().modifyStatus("Wet", "" + durationLeft);
                waterParticleEmitter.SetActive(true);
            }
        }
    }

    public void OnPlayerDies(PlayerLogic playerLogic)
    {
        player.GetComponent<PlayerLogic>().removeStatus("Wet");
        waterParticleEmitter.SetActive(false);
    }
}

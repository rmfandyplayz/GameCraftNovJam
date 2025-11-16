using System;
using System.Linq;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private AudioSource hordeTrack;
    private AudioSource nearDeathTrack;

    [SerializeField] private float musicFadeTime;

    [SerializeField] private int hordeSize;
    [SerializeField] private float ratDist;

    [SerializeField] [Range(0,1)] private float healthPercentTrigger;

    private Player player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindAnyObjectByType<Player>();

        hordeTrack = transform.GetChild(1).GetComponent<AudioSource>();
        nearDeathTrack = transform.GetChild(2).GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        int ratsNear = FindObjectsByType<RatBase>(FindObjectsSortMode.None).Count(rat => (rat.transform.position - player.transform.position).magnitude < ratDist);
        float hordeMod = -1;

        if (ratsNear >= hordeSize)
        {
            hordeMod = 1;
        }

        hordeTrack.volume = Math.Clamp(hordeTrack.volume + hordeMod * Time.deltaTime * musicFadeTime, 0, 1);

        float healthMod = -1;
        if (player.lightLeft / player.maxLight < healthPercentTrigger)
        {
            healthMod = 1;
        }
        nearDeathTrack.volume = Math.Clamp(nearDeathTrack.volume + healthMod * Time.deltaTime * musicFadeTime, 0, 1);
    }
}

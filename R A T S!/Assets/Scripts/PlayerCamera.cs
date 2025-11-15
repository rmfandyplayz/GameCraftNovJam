using System;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private Player player;

    [SerializeField] private float speed;

    private static Vector3 offset = new( 0, 0, -10);

    private void Start()
    {
        player = FindAnyObjectByType<Player>();
    }


    private void FixedUpdate()
    {
        transform.position =
            MysticUtil.DampVector(transform.position, player.transform.position + offset, speed, Time.deltaTime);
    }
}

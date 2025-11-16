using System;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private Player player;

    [SerializeField] private float speed;

    private static Vector3 offset = new(0, 0, -10);

    private float shakeDuration = 0f;
    private float shakeMagnitude = 0.05f;   
    private float shakeSpeed = 40f;         
    private Vector3 originalPos;

    private void Start()
    {
        player = FindAnyObjectByType<Player>();
        originalPos = transform.localPosition;
    }

    private void FixedUpdate()
    {
        // Follow player
        Vector3 targetPos = MysticUtil.DampVector(
            transform.position,
            player.transform.position + offset,
            speed,
            Time.deltaTime
        );

        transform.position = targetPos + GetShakeOffset();
    }

    public void ShakeCamera(float duration, float magnitude = 0.05f)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
    }

    private Vector3 GetShakeOffset()
    {
        if (shakeDuration <= 0f)
            return Vector3.zero;

        shakeDuration -= Time.deltaTime;

        // quick oscillating shake
        float x = Mathf.Sin(Time.time * shakeSpeed) * shakeMagnitude;
        float y = Mathf.Cos(Time.time * shakeSpeed) * shakeMagnitude;

        return new Vector3(x, y, 0f);
    }
}

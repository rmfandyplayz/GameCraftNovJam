using UnityEngine;

public class PickupHint : MonoBehaviour
{
    private Quaternion originalRotation;

    void Start()
    {
        originalRotation = transform.rotation;
    }

    void LateUpdate()
    {
        transform.rotation = originalRotation;
    }
}

using System;
using UnityEngine;

public class ServeTable : MonoBehaviour
{
    [SerializeField] private Vector3 placedOffset;
    public void PlaceItem(FoodItem item)
    {
        item.transform.position = transform.position + placedOffset;
        FadeoutTransition.SceneTransition("WinScreen");
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position + placedOffset, new Vector3(.1f,.1f,.1f));
    }
}

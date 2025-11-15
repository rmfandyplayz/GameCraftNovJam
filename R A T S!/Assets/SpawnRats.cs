using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using NUnit.Framework;
using UnityEngine;

public class SpawnRats : MonoBehaviour
{
    private List<Transform> spawnPoints;
    public List<GameObject> ratPrefabs;
    private float timer;
    public int spawnThreshold;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnPoints = new List<Transform>();
        GameObject[] objectArr = GameObject.FindGameObjectsWithTag("Spawn Point");
        foreach (GameObject point in objectArr)
        {
            spawnPoints.Add(point.transform);
        }
        Debug.Log(spawnPoints.Count);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > spawnThreshold)
        {
            spawnRat();
            timer = 0;
        }
    }

    void spawnRat()
    {
        int spawnIndex = Random.Range(0, spawnPoints.Count);
        int ratIndex = Random.Range(0, ratPrefabs.Count);
        GameObject rat = Instantiate(ratPrefabs[ratIndex], spawnPoints[spawnIndex].position, Quaternion.identity);
        rat.name = "rat";

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        foreach(GameObject item in GameObject.FindGameObjectsWithTag("Spawn Point"))
        {
            Gizmos.DrawSphere(item.transform.position, .5f);
        }
    }
}

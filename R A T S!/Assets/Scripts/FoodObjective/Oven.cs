using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class Oven : MonoBehaviour
{
    private List<FoodItem> foodRequired;
    private List<FoodItem> acquiredItems = new();

    private SpriteRenderer brightSprite;
    private Light2D light;
    private float brightnessTimer;
    private float lightBright;

    private float cookinTimer;
    [SerializeField] private float cookTime = 10;


    private void Start()
    {
        foodRequired = FindObjectsByType<FoodItem>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID).ToList();

        brightSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        light = brightSprite.transform.GetChild(0).GetComponent<Light2D>();
        lightBright = light.intensity;
    }

    private const float brightFlicker = 0.05f;


    private void Update()
    {
        brightnessTimer = Mathf.Max(brightnessTimer - Time.deltaTime * .2f, -brightFlicker);
        float randAmount = Random.Range(-brightFlicker, brightFlicker);
        brightSprite.color = new Color(1, 1, 1,
            brightnessTimer + randAmount);
        light.intensity = Mathf.Lerp(0, lightBright, brightnessTimer + randAmount);
        
        if (cookinTimer > 0)
        {
            cookinTimer += Time.deltaTime;
            brightnessTimer = 1;
            if (cookinTimer > cookTime)
            {
                transform.GetChild(1).gameObject.SetActive(true);
                GetComponent<Animator>().SetTrigger("Spit");
                cookinTimer = 0;
            }
        }
    }

    public void KillAnimator()
    {
        GetComponent<Animator>().enabled = false;
    }

    public void StoreItem(FoodItem item)
    {
        foodRequired.Remove(item);
        Destroy(item.gameObject);
        brightnessTimer = 1;

        if (foodRequired.Count == 1)
        {
            cookinTimer += Time.deltaTime;
        }
    }
}

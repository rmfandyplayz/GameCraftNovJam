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
    private AudioSource fireSound;

    private GameObject ingredientTemplate;
    
    
    private float cookinTimer;
    [SerializeField] private float cookTime = 10;

    [SerializeField] private int ColumnNum;
    [SerializeField] private Vector2 ingredientOffset;

    private Dictionary<FoodItem, GameObject> ingredients = new();

    private void Start()
    {
        foodRequired = FindObjectsByType<FoodItem>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID).ToList();

        brightSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        light = brightSprite.transform.GetChild(0).GetComponent<Light2D>();
        lightBright = light.intensity;

        fireSound = transform.Find("Sound").GetComponent<AudioSource>();

        ingredientTemplate = transform.Find("ThoughtBubble").GetChild(0).gameObject;
        int j = 0;
        foreach (FoodItem item in foodRequired.Where(item => !item.finalMeal))
        {
            GameObject newListing = Instantiate(ingredientTemplate);
            int columnPos = j % ColumnNum;
            int rowPos = j / ColumnNum;
            newListing.transform.position = ingredientTemplate.transform.position + new Vector3(
                ingredientOffset.x * columnPos,
                ingredientOffset.y * rowPos,
                0
            );
            newListing.GetComponent<SpriteRenderer>().sprite = item.GetComponent<SpriteRenderer>().sprite;
            ingredients.Add(item, newListing);
            j++;
        }
        Destroy(ingredientTemplate);
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
                GetComponent<Animator>().SetTrigger("Progress");
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
        Destroy(ingredients[item]);
        Destroy(item.gameObject);
        brightnessTimer = 1;
        fireSound.Play();

        if (foodRequired.Count == 1)
        {
            cookinTimer += Time.deltaTime;
            GetComponent<Animator>().SetTrigger("Progress");
            foreach (RatBase rat in FindObjectsByType<RatBase>(FindObjectsSortMode.None))
            {
                rat.CallTo(transform.position);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        ingredientTemplate = transform.Find("ThoughtBubble").GetChild(0).gameObject;
        Gizmos.color = Color.black;
        for (int i = 0; i < 6; i++)
        {
            int columnPos = i % ColumnNum;
            int rowPos = i / ColumnNum;
            Gizmos.DrawWireCube(ingredientTemplate.transform.position + new Vector3(
                ingredientOffset.x * columnPos,
                ingredientOffset.y * rowPos,
                0
            ), new Vector3(0.5f,.5f,.5f));
        }
    }
}

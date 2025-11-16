using UnityEngine;

public class Posess : MonoBehaviour
{
    private Animator playerAnim;
    private Transform player;
    private float posessTimer;
    public float posessTime;
    private Player playerScript;
    private bool posessionHappening = false;
    private RatBase ratBase;
    public int biteDistance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerAnim = FindAnyObjectByType<Player>().GetComponent<Animator>();
        player = FindAnyObjectByType<Player>().transform;
        playerScript = FindAnyObjectByType<Player>();
        ratBase = GetComponent<RatBase>();
    }

    // Update is called once per frame
    void Update()
    {
        if (posessionHappening)
        {
        posessTimer += Time.deltaTime;
        }
        if (posessTimer >= posessTime)
        {
            playerAnim.SetBool("Possessed", false);
            ratBase.TakeDamage();
            posessionHappening = false;
        }
        if ((player.transform.position - transform.position).magnitude <= biteDistance && !posessionHappening) 
        {
          posessTimer = 0;
          Posession();  
        }
    }
    
    void Posession()
    {
        posessionHappening = true;
        GetComponent<SpriteRenderer>().enabled = false;
        playerAnim.SetBool("Possessed", true);
    }
}

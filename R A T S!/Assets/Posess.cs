using UnityEngine;

public class Posess : MonoBehaviour
{
    private Animator playerAnim;
    private Transform player;
    private float posessTimer;
    public float posessTime;
    private Player playerScript;
    private bool possessionOnCooldown = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerAnim = FindAnyObjectByType<Player>().GetComponent<Animator>();
        player = FindAnyObjectByType<Player>().transform;
        playerScript = FindAnyObjectByType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        posessTimer += Time.deltaTime;
        if (posessTimer >= posessTime)
        {
            playerAnim.SetBool("Possessed", false);
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.layer);
        if (collision.gameObject.layer.Equals("Player") && !possessionOnCooldown)
        {
            possessionOnCooldown = true;
            Posession();
        }
    }

    void Posession()
    {
        playerAnim.SetBool("Possessed", true);
    }
}

using System.Collections;
using UnityEngine;

public class Shriek : MonoBehaviour
{
    
    private Transform rat;
    private Animator anim;
    private RatBase ratBase;
    private PathfindingManager pathFinder;
    private Transform player;
    private RatBase[] rats;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      rat = GetComponent<Transform>();
      anim = GetComponent<Animator>();
      ratBase = GetComponent<RatBase>();
      pathFinder = FindAnyObjectByType<PathfindingManager>(); 
      player = FindAnyObjectByType<Player>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(ratBase.CanSeePlayer());
        if (ratBase.CanSeePlayer())
        {
            anim.SetBool("inShriekingDistance", true);
        }
        else 
        {
            anim.SetBool("inShriekingDistance", false);
        }
    }

    public void LockAnimation()
    {
        anim.SetBool("CanSwitch", false);
        ratBase.movingEnemy = false;
    }

    public void UnlockAnimation()
    {
        anim.SetBool("CanSwitch", true);
        ratBase.movingEnemy = true;
    }
    public void Summon()
    {
       GameObject[] ratObjects = GameObject.FindGameObjectsWithTag("Enemy");
       rats = new RatBase[ratObjects.Length];
       for (int i = 0; i < ratObjects.Length; i++)
        {
            rats[i] = ratObjects[i].GetComponent<RatBase>();
            rats[i].currentRoute = pathFinder.PathfindTo(rats[i].transform.position, player.position);
        }
    }
    
    
}

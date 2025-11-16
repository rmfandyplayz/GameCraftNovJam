using UnityEngine;

public class Shriek : MonoBehaviour
{
    
    private Transform rat;
    private Animator anim;
    public RatBase ratBase;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      rat = GetComponent<Transform>();
      anim = GetComponent<Animator>(); 
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
       GameObject[] rats = GameObject.FindGameObjectsWithTag("Enemy");
       foreach(GameObject rat in rats)
        {
            
        }
    }
    
    
}

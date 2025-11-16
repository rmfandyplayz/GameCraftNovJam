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
            ratBase.movingEnemy = false;
            Summon();
        }
        else 
        {
            anim.SetBool("inShriekingDistance", false);
            ratBase.movingEnemy = true;
        }
    }

    public void LockAnimation()
    {
        anim.SetBool("CanSwitch", false);
    }

    public void UnlockAnimation()
    {
        anim.SetBool("CanSwitch", true);
    }
    void Summon()
    {
       GameObject[] rats = GameObject.FindGameObjectsWithTag("Enemy");
       foreach(GameObject rat in rats)
        {
            
        }
    }
    
    
}

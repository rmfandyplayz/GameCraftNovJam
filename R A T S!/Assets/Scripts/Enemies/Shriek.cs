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

    private ParticleSystem shriekParticleA;
    private ParticleSystem shriekParticleB;
    private AudioSource shriekSound;
    private float particleXOffset;

    private Transform eyeTransform;
    private float eyeXOffset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      rat = GetComponent<Transform>();
      anim = GetComponent<Animator>();
      ratBase = GetComponent<RatBase>();
      pathFinder = FindAnyObjectByType<PathfindingManager>(); 
      player = FindAnyObjectByType<Player>().transform;

      shriekParticleA = transform.Find("Scream Particle").GetComponent<ParticleSystem>();
      shriekParticleB = shriekParticleA.transform.GetChild(0).GetComponent<ParticleSystem>();
      particleXOffset = shriekParticleA.transform.localPosition.x;
      shriekSound = shriekParticleA.GetComponent<AudioSource>();

      eyeTransform = transform.Find("Eye");
      eyeXOffset = eyeTransform.localPosition.x;
    }

    // Update is called once per frame
    void Update()
    {
        shriekParticleA.transform.localPosition = new Vector3(
            particleXOffset * (ratBase.spriteRenderer.flipX ? -1 : 1),
            shriekParticleA.transform.localPosition.y,
            0
        );
        eyeTransform.localPosition = new Vector3(
            eyeXOffset * (ratBase.spriteRenderer.flipX ? -1 : 1),
            eyeTransform.localPosition.y,
            0
        );
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
        shriekParticleA.Play();
        shriekParticleB.Play();
        shriekSound.Play();
       GameObject[] ratObjects = GameObject.FindGameObjectsWithTag("Enemy");
       rats = new RatBase[ratObjects.Length];
       for (int i = 0; i < ratObjects.Length; i++)
        {
            rats[i] = ratObjects[i].GetComponent<RatBase>();
            rats[i].currentRoute = pathFinder.PathfindTo(rats[i].transform.position, player.position);
        }
    }
    
    
}

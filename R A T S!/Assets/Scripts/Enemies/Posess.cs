using UnityEngine;

public class Posess : RatBase
{
    [SerializeField] private float possesTime = 5;
    [SerializeField] private GameObject possessParticle;
    public override void Attack()
    {
        player.Possess(possesTime);
        GameObject particle = Instantiate(possessParticle);
        particle.transform.position = player.transform.position;
        Destroy(gameObject);
    }
}

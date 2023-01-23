using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Transform objective;
    public int damage;
    public float projectileSpeed;
    [SerializeField] private bool isExplosive;
    [SerializeField] private GameObject explosion;
    public float arcHeight;
    private AudioSource audioSource;
    [SerializeField] private AudioClip explosionAudio;
    private bool damageDone;

   
    private Vector3 startPosition;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
       
    }
    

    // Update is called once per frame
    void FixedUpdate()
    {
        if (objective.CompareTag("Unit"))
        {
            if (!objective.GetComponent<UnitMain>().isDead)
            {
                transform.position = Vector3.MoveTowards(transform.position, objective.position, projectileSpeed * Time.deltaTime);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        if (Vector3.Distance(transform.position, objective.position) <= 0.01f)
        {
            DoDamage();
        }
        
        if (objective.CompareTag("Player"))
        {
            if (!objective.GetComponent<PlayerMain>().isDead)
            {
               
                transform.position =  Vector3.MoveTowards(transform.position, objective.position, projectileSpeed * Time.deltaTime);
            }
           
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
    private void DoDamage()
    {
        if (!damageDone)
        {
            if (objective.CompareTag("Unit"))
            {
                if (!objective.GetComponent<UnitMain>().isDead)
                {
                    objective.GetComponent<UnitMain>().TakeDamage(damage);
                    Invoke("Deactivate", 0.5f);
                }
            }
            if (objective.CompareTag("Player"))
            {
                if (!objective.GetComponent<PlayerMain>().isDead)
                {
                    objective.GetComponent<PlayerMain>().TakeDamage(damage);
                    Invoke("Deactivate", 0.5f);
                }
            }
            if (isExplosive)
            {
                explosion.SetActive(true);
                audioSource.PlayOneShot(explosionAudio);
            }
            damageDone = true;
        }
    }
    private void Deactivate()
    {
        damageDone = false;
        if (isExplosive) explosion.SetActive(false);
        gameObject.SetActive(false);
    }
}

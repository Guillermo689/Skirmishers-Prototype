using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private Collider[] unitsNear;
    private Collider[] unitsAtGate;
    [SerializeField] private float doorRadius;
    [SerializeField] private float enterRadius;
    [SerializeField] private List<GameObject> guardUnits;
    [SerializeField] private List<GameObject> enteringUnits;
    [SerializeField] private bool doorUnlocked;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openSound;
    private Animator animator;
    private bool audioPlayed;

    private int isOpenID;
    private bool alreadyOpen;
   
    void Start()
    {
        animator = GetComponent<Animator>();
        unitsNear = Physics.OverlapSphere(transform.position, doorRadius);
        isOpenID = Animator.StringToHash("IsOpen");
        audioSource = GetComponent<AudioSource>();
     
        foreach (Collider unit in unitsNear)
        {
            if (unit.CompareTag("Unit"))
            {
                if (!guardUnits.Contains(unit.gameObject) && unit.GetComponent<UnitMain>().isAI)
                {
                    guardUnits.Add(unit.gameObject);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < guardUnits.Count; i++)
        {
            UnitMain unitMain = guardUnits[i].GetComponent<UnitMain>();
            if (unitMain.isDead) guardUnits.Remove(guardUnits[i]);
           // else if (guardUnits[i] == null) guardUnits.Remove(guardUnits[i]);
        }
        if (guardUnits.Count <= 0) doorUnlocked = true;
        if (doorUnlocked)
        {
            CheckUnitsAtDoor();
            UnlockDoor();
        }
       
    }
    
    IEnumerator RestartAudio()
    {
        yield return new WaitForSeconds(1f);
        audioPlayed = false;
    }

    private void CheckUnitsAtDoor()
    {
        unitsAtGate = Physics.OverlapSphere(transform.position, doorRadius);
        foreach (Collider unit in unitsAtGate)
        {
            if (unit.CompareTag("Unit"))
            {
                if (!enteringUnits.Contains(unit.gameObject))
                {
                    if (!unit.GetComponent<UnitMain>().isDead)
                    {
                        enteringUnits.Add(unit.gameObject);
                    }
                   
                }

            }
            if (unit.CompareTag("Player"))
            {
                if (!enteringUnits.Contains(unit.gameObject))
                {
                    if (!unit.GetComponent<PlayerMain>().isDead)
                    {
                        enteringUnits.Add(unit.gameObject);
                    }
                }
            }

        }

        if (enteringUnits.Count > 0)
        {
            for (int i = 0; i < enteringUnits.Count; i++)
            {
                if (enteringUnits[i] == null)
                {
                    enteringUnits.Remove(enteringUnits[i]);
                    return;
                }
                if (Vector3.Distance(transform.position, enteringUnits[i].transform.position) > enterRadius)
                {
                    if (enteringUnits[i].CompareTag("Unit"))
                    {
                        if (!enteringUnits[i].GetComponent<UnitMain>().isDead)
                        {
                            enteringUnits.Remove(enteringUnits[i]);
                            return;
                        }
                    }
                    if (enteringUnits[i].CompareTag("Player"))
                    {
                        if (!enteringUnits[i].GetComponent<PlayerMain>().isDead)
                        {
                            enteringUnits.Remove(enteringUnits[i]);
                            return;
                        }
                    }
                }
                if (enteringUnits[i].CompareTag("Unit"))
                {
                    if (enteringUnits[i].GetComponent<UnitMain>().isDead)
                    {
                        enteringUnits.Remove(enteringUnits[i]);
                        return;
                    }
                }
                if (enteringUnits[i].CompareTag("Player"))
                {
                    if (enteringUnits[i].GetComponent<PlayerMain>().isDead)
                    {
                        enteringUnits.Remove(enteringUnits[i]);
                    }
                }
            }
        }




    }
   private void UnlockDoor()
    {
        if (enteringUnits.Count <= 0)
        {
            if (doorUnlocked && alreadyOpen)
            {
               
                if (!audioPlayed)
                {
                    animator.SetBool(isOpenID, false);
                    audioSource.PlayOneShot(openSound, 0.3f);
                    audioPlayed = true;
                    StartCoroutine(RestartAudio());
                }
                alreadyOpen = false;
            }
        }
        if (enteringUnits.Count > 0)
        {
            if (doorUnlocked && !alreadyOpen)
            {

               
                if (!audioPlayed)
                {
                    animator.SetBool("IsOpen", true);
                    audioSource.PlayOneShot(openSound, 0.3f);
                    audioPlayed = true;
                    StartCoroutine(RestartAudio());
                }


                alreadyOpen = true;

            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, doorRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enterRadius);
    }
}

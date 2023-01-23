using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMain : MonoBehaviour
{
    public int playerNumber;
    public int playerGroup;
    internal PlayerMovement movementScript;
    internal PlayerCommands commandScript;
    internal bool isSelecting;
    public int currentHealth;
    public bool isDead;
    [SerializeField] private int maxHealth;
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject blood;
    public GameObject healthBar;
    public GameObject shiny;
    private bool deathTriggered = false;
    private bool inCombat;
    private float combatTimer;
    private float combatStartTimer = 3;
    public float regenerationRate = 1;

    private Animator animator;
    private int attackID;
    private int velocityID;
    private int commandID;
    private int dieID;

   
    //=================== Sound Variables ========================
    [Header("Sound Effects")]
    [SerializeField] private AudioClip rallyAudio;
    [SerializeField] private AudioClip commandAttackAudio;
    [SerializeField] private AudioClip  deathAudio;
    [SerializeField] private AudioClip takeDamageAudio;
    [SerializeField] private AudioClip soloAttackAudio;

    [SerializeField] private AudioClip[] weaponClash;
    AudioSource audioSource;
    private bool regenerationStarted;
    private bool takeDamageAudioPlayed;

    // Start is called before the first frame update
    void Start()
    {
        movementScript = GetComponent<PlayerMovement>();
        commandScript = GetComponent<PlayerCommands>();
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
        currentHealth = maxHealth;

        attackID = Animator.StringToHash("Attack");
        velocityID = Animator.StringToHash("Velocity");
        commandID = Animator.StringToHash("Command");
        dieID = Animator.StringToHash("Die");
        slider.maxValue = maxHealth;
        slider.value = currentHealth;
        HideHealthBar();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= 0) Die();
        VelocityAnimation();
        Regenerate();
        HealthBar();
    }
    public void ShowHealthBar()
    {
        healthBar.SetActive(true);
        shiny.SetActive(true);
    }
    public void HideHealthBar()
    {
        healthBar.SetActive(false);
        shiny.SetActive(false);
    }
    private void HealthBar()
    {
        // slider.value = CalculateHealth();
        slider.value = currentHealth;
    }
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        blood.SetActive(true);
        TakeDamageAudio();
        StartCombatTimer();
    }
    internal void StartCombatTimer()
    {
        combatTimer = combatStartTimer;
    }
    private void Regenerate()
    {
        combatTimer -= Time.deltaTime;
        if (combatTimer <= 0) combatTimer = 0;
        if (currentHealth >= maxHealth) currentHealth = maxHealth;
        if (combatTimer > 0) inCombat = true;
        if (!inCombat)
        {
            if (!regenerationStarted)
            {
                StartCoroutine(RegenerationCycle());
                regenerationStarted = true;
            }
        }
        else
        {
            StopCoroutine(RegenerationCycle());
            if (combatTimer <= 0)
            {
                combatTimer = 0;
                inCombat = false;
            }
        }
    }
    IEnumerator RegenerationCycle()
    {
        currentHealth+=5;
        yield return new WaitForSeconds(regenerationRate);
        regenerationStarted = false;
    }
    public void Die()
    {
        isDead = true;
        if (!deathTriggered)
        {
            animator.SetTrigger(dieID);
            DeathAudio();
            deathTriggered = true;
        }
        Debug.Log("Game Over");
    }
    #region Animation Functions
    internal void AttackAnimation()
    {
        animator.SetTrigger(attackID);
    }
    internal void VelocityAnimation()
    {
        animator.SetFloat(velocityID, movementScript.agent.velocity.magnitude);
    }
    internal void CommandAnimation()
    {
        animator.SetTrigger(commandID);
    }

    #endregion
    #region Audio Functions
    public void RallyAudio()
    {
        audioSource.PlayOneShot(rallyAudio);
    }
    public void CommandAttackAudio()
    {
        audioSource.PlayOneShot(commandAttackAudio);
    }
    public void SoloAttackAudio()
    {
        audioSource.PlayOneShot(soloAttackAudio);
    }
    public void DeathAudio()
    {
        audioSource.PlayOneShot(deathAudio);
    }
    public void TakeDamageAudio()
    {
        if (!takeDamageAudioPlayed) StartCoroutine(TakeDamageClip());
    }
    IEnumerator TakeDamageClip()
    {
       audioSource.PlayOneShot(takeDamageAudio);
       takeDamageAudioPlayed = true;
        yield return new WaitForSeconds(1f);
        takeDamageAudioPlayed = false;
    }

    public void WeaponAudio()
    {
        int weapon = Random.Range(0, weaponClash.Length);
        audioSource.PlayOneShot(weaponClash[weapon]);
    }
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class UnitMain : MonoBehaviour
{
    public int playerNumber;
    public int playerGroup;
    public bool isRanged;
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public bool isDead = false;
    private GameObject player;
    public int currentHealth;
    public int maxHealth;
    [SerializeField] private Slider slider;
    public GameObject healthBar;
    private bool regenerationStarted;
    public float regenerationRate = 1;
    public bool isRescuable;

    //=============== AI Parameters ==========================
    [Header("AI Paremeters")]
    public bool isAI;
    [SerializeField] private bool patrollStance;
    private Vector3 moveSpot;
    private float waitTime = 3;
    public bool aggressiveStance = false;
    public bool isCommanded = false;
    [SerializeField] private bool hasPatrolPoints;
    [SerializeField] private Transform[] patrolPoints;

    //================ Melee Attack Variables =====================
    [Header("Melee Attack Variables")]
    public bool isAggressive = false;
    public bool canAttack = false;
    public GameObject attackObjective;
    public float attackSpeed;
    public float attackRange;
    public int damage;

    private bool inCombat;
    private float combatTimer;
   float combatStartTimer = 3;
    //=============== Ranged Attack Variables ===================
    [Header("Ranged Attack Variables")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private GameObject projectile;
    [SerializeField] private float projectileSpeed;
    private bool alreadyAttacked = false;
    [SerializeField] private float arcHeight;

    //=============== Search for enemies Variables ================
    [Header("Search for enemies Variables")]
    [SerializeField] private List<GameObject> nearbyEnemies = new List<GameObject>();
    private bool isAttacking = false;
    [SerializeField] private float searchRadius;
    [SerializeField] Collider[] unitsNear;

    //================== Radius Variables =================
    [Header("FormationVariables")]
    public int _amount;
    public float _radius;
    public float _radiusGrowthMultiplier;
    public float _rotations;
    public int _rings;
    public float _ringOffset;
    public float _nthOffset;
    [Range(0, 1)] public float noise;
    public float Spread;
    //================== Animation Variables ===================
    private Animator animator;
    private bool diePlayed = false;
    private int attackID;
    private int velocityID;
    private int dieID;
    private int rallyID;
    private int isDeadID;

    //================== Audio Variables =========================
    [Header("Sound Effects")]
    [SerializeField] private AudioClip deathAudio;
    [SerializeField] private AudioClip takeDamageAudio;
    [SerializeField] private AudioClip[] weaponClash;
    [SerializeField] private AudioClip throwAudio;
    [SerializeField] private AudioClip attackAudio;
    bool throwAudioPlayed = false;
    AudioSource audioSource;
    private bool takeDamageAudioPlayed = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        attackID = Animator.StringToHash("Attack");
        velocityID = Animator.StringToHash("Velocity");
        dieID = Animator.StringToHash("Die");
        rallyID = Animator.StringToHash("Rally");
        isDeadID = Animator.StringToHash("IsDead");
        audioSource = GetComponent<AudioSource>();
        currentHealth = maxHealth;
        slider.maxValue = maxHealth;
        slider.value = currentHealth;
        healthBar.SetActive(false);
        agent.enabled = true;
        if (playerNumber == 0 && playerGroup == 0) isAI = true;
        if (playerNumber == 0 && playerGroup != 0) isRescuable = true;
        if (isRanged)
        {
            //================= If is ranged, create the projectile to use and turn it off ===================================
            projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity, transform);
            projectile.SetActive(false);
        }
        animator.SetBool(isDeadID, false);
       SearchUnitsNearby();
        
        if (isAI )
        {
            isAggressive = true;
            aggressiveStance = true;
            SearchEnemies();
            if (patrollStance)
            {
                StartPatrolling();
            }
        }
        if (isAggressive) 
        {
            SearchForEnemies();
        }
        StartCoroutine(RescueCheck());

    }
    void Update()
    {
       
        if (!isDead)
        {
            UpdateEnemies();
            //======================= if not dead, check units nearby =====================================
            SearchUnitsNearby();
            if (isAggressive)
            {
                UpdateEnemies();
                if (nearbyEnemies.Count > 0)
                {
                    GoToAttack();
                }
                if (attackObjective != null) agent.SetDestination(attackObjective.transform.position);
            }
         
            if (canAttack && attackObjective != null && isAggressive)
            {
                if (Vector3.Distance(transform.position, attackObjective.transform.position) <= attackRange)
                {
                    //======================= if is in range, has an objective, can attack is aggressive, stop and look at target
                    agent.SetDestination(transform.position);
                   
                    if(!alreadyAttacked && isAttacking) Attack();
                }
                else
                {
                    //======================= if target is out of range, chase target =========================
                    GoToAttack();
                   
                }
            }
            animator.SetFloat(velocityID, agent.velocity.magnitude);
            if (isCommanded)
            {
                if (agent.destination == transform.position)
                {
                    isCommanded = false;
                    if (aggressiveStance) isAggressive = true;
                }

            }
            if (currentHealth <= 0) Die();
            if (attackObjective == null)
            {
                StopCoroutine(AttackCycle());
                StopCoroutine(RangedAttackCycle());
            }
            if (isAI)
            {
                isAggressive = true;
                aggressiveStance = true;
            }
            Regenerate();
            HealthBar();
        }
       
    }
    IEnumerator RescueCheck()
    {
        if (playerNumber == 0 && playerGroup != 0)
        {
            isRescuable = true;
            yield return new WaitForSeconds(1);
            StartCoroutine(RescueCheck());
        }
        else
        {
            yield return null;
        }
    }
    private void HealthBar()
    {
        slider.value =currentHealth;
    }
   
    public void ShowHealthBar()
    {
        if (!isDead)
        {
            healthBar.SetActive(true);
        }
    }
    public void HideHealthBar()
    {
        healthBar.SetActive(false);
    }
    public void Die()
    {
        isDead = true;
        agent.isStopped = true;

       
        StartCoroutine(Disable());
    }
    IEnumerator Disable()
    {
        yield return new WaitForEndOfFrame();
        StopCoroutinesSelected();
        if (!diePlayed)
        {
            animator.SetBool(isDeadID, true);
            animator.Play(dieID);
            DeathAudio();
            diePlayed = true;
        }
        yield return new WaitForSeconds(3f);
        agent.enabled = false;
        nearbyEnemies.Clear();
        attackObjective = null;
        Destroy(gameObject, 120f);
    }
    private void StopCoroutinesSelected()
    {
        StopCoroutine(AttackCycle());
        StopCoroutine(RangedAttackCycle());
        StopCoroutine(RescueCheck());
        StopCoroutine(RallyRoutine());
        StopCoroutine(SearchEnemies());
        StopCoroutine(RegenerationCycle());
        StopCoroutine(DistanceCheck());
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        TakeDamageAudio();
        StartCombatTimer();
        StopCoroutine(RegenerationCycle());
        regenerationStarted = false;
       

    }
    // Update is called once per frame
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
        currentHealth+= 5;
        
        yield return new WaitForSeconds(regenerationRate);
        regenerationStarted = false;
    }
    private void StartCombatTimer()
    {
        combatTimer = combatStartTimer;
    }
    private void Attack()
    {
        //======================= Check if ranged and use ranged attack, otherise use melee attack =====================
        
        if (isRanged) StartCoroutine(RangedAttackCycle());
        else StartCoroutine(AttackCycle());
    }
    #region BehaviourFunctions
    public void RallyAnimation()
    {
        StartCoroutine(RallyRoutine());
    }
    IEnumerator RallyRoutine()
    {
        float reactionTime = Random.Range(0.2f, 1f);
        yield return new WaitForSeconds(reactionTime);
        animator.SetTrigger(rallyID);
    }
    public void GoToAttack()
    {
        //======================= Stop coroutine from check to point and set destination to target ======================
        StopCoroutine(DistanceCheck());
        if (attackObjective != null) agent.SetDestination(attackObjective.transform.position);
        
        canAttack = true;
        isAttacking = true;
    }
    public void StopAttack()
    {
        isAttacking = false;
        canAttack = false;
        isAggressive = false;
        if(!isDead) agent.isStopped = false;
        if (isRanged)
        {
            projectile.SetActive(false);
            StopCoroutine(RangedAttackCycle());
        }
        else
        {
            StopCoroutine(AttackCycle());
        }
        TargetNull();
    }
    private void TargetNull()
    {
        attackObjective = null;
    }
    private void SearchUnitsNearby()
    {
        unitsNear = Physics.OverlapSphere(transform.position, searchRadius);
    }
    IEnumerator RangedAttackCycle()
    {
        StartCombatTimer();
        if (attackObjective != null && isAttacking)
        {
            if (!attackObjective.CompareTag("Player"))
            {
                UnitMain unitMain = attackObjective.GetComponent<UnitMain>();
                if (unitMain.playerNumber != playerNumber && unitMain.playerGroup != playerGroup)
                {
                    projectile.transform.position = shootPoint.position;
                    if (!isDead) animator.SetTrigger(attackID);

                    yield return new WaitForSeconds(0.03f);
                    agent.isStopped = true;
                    if (!unitMain.isDead)
                    {
                        projectile.SetActive(true);
                        transform.LookAt(attackObjective.transform.position);
                        Projectile projectileScript = projectile.GetComponent<Projectile>();
                        projectileScript.arcHeight = arcHeight;
                        projectileScript.objective = attackObjective.transform;
                        projectileScript.damage = damage;
                        projectileScript.projectileSpeed = projectileSpeed;
                        alreadyAttacked = true;
                        ThrowAudio();
                        yield return new WaitForSeconds(attackSpeed);
                        agent.isStopped = false;
                        alreadyAttacked = false;
                    }
                    else
                    {
                        if (isAggressive)
                        {
                            SearchForEnemies();
                        }
                        agent.isStopped = false;
                    }
                }
                

            }
            else
            {
                PlayerMain unitMain = attackObjective.GetComponent<PlayerMain>();
                if (unitMain.playerNumber != playerNumber && unitMain.playerGroup != playerGroup)
                {
                    agent.isStopped = true;
                    projectile.transform.position = shootPoint.position;
                    animator.SetTrigger(attackID);
                    transform.LookAt(attackObjective.transform.position);
                    yield return new WaitForSeconds(0.03f);
                    if (!unitMain.isDead)
                    {
                        projectile.SetActive(true);
                        Projectile projectileScript = projectile.GetComponent<Projectile>();
                        projectileScript.arcHeight = arcHeight;
                        projectileScript.objective = attackObjective.transform;
                        projectileScript.damage = damage;
                        projectileScript.projectileSpeed = projectileSpeed;
                        alreadyAttacked = true;
                        ThrowAudio();
                        yield return new WaitForSeconds(attackSpeed);
                        agent.isStopped = false;
                        alreadyAttacked = false;
                    }
                    else
                    {
                        if (isAggressive)
                        {
                            
                            SearchForEnemies();
                        }
                        agent.isStopped = false;
                        alreadyAttacked = false;
                    }
                }
                   
            }

        }
        else
        {
            SearchForEnemies();
        }
    }
    IEnumerator AttackCycle()
    {
        StartCombatTimer();
        if (attackObjective != null && isAttacking)
        {
            if (!attackObjective.CompareTag("Player"))
            {
                //======================= if is not player, get the player to take damage =============================
               
                UnitMain unitMain = attackObjective.GetComponent<UnitMain>();
                if (unitMain.playerNumber != playerNumber && unitMain.playerGroup != playerGroup)
                {

                    alreadyAttacked = true;
                    agent.isStopped = true;
                    if (!unitMain.isDead)
                    {
                        animator.SetTrigger(attackID);
                        unitMain.TakeDamage(damage);
                        transform.LookAt(attackObjective.transform.position);
                        alreadyAttacked = true;
                        WeaponAudio();
                        yield return new WaitForSeconds(attackSpeed);
                        agent.isStopped = false;
                        alreadyAttacked = false;
                    }
                    else
                    {
                        if (isAggressive)
                        {
                            
                            SearchForEnemies();
                        }
                        agent.isStopped = false;
                        alreadyAttacked = false;
                    }
                }
            }
            else
            {
                //======================= if is player, get the player to take damage =============================
                PlayerMain unitMain = attackObjective.GetComponent<PlayerMain>();
                if (unitMain.playerNumber != playerNumber && unitMain.playerGroup != playerGroup)
                {
                    if (!unitMain.isDead)
                    {
                        agent.isStopped = true;
                        animator.SetTrigger(attackID);
                        transform.LookAt(attackObjective.transform.position);
                        unitMain.TakeDamage(damage);
                        alreadyAttacked = true;
                        WeaponAudio();
                        yield return new WaitForSeconds(attackSpeed);
                        agent.isStopped = false;
                        alreadyAttacked = false;
                    }
                    else
                   
                    {
                        if (isAggressive)
                        {
                            SearchForEnemies();
                        }
                    }
                            agent.isStopped = false;
                            alreadyAttacked = false;
                   
                }
                    
            }
        }
        else
        {
            //======================= if is aggressiveand has no target, look for a new target ====================
            if (isAggressive) SearchForEnemies();
        }
    }
    public void SearchForEnemies()
    {
        StartCoroutine(SearchEnemies());
    }
    private void UpdateEnemies()
    {
        if (nearbyEnemies.Count > 0)
        {
            for (int i = 0; i < nearbyEnemies.Count; i++)
            {
                if (nearbyEnemies[i] == null)
                {
                    //=================== if enemy is null, remove it =============================
                    nearbyEnemies.Remove(nearbyEnemies[i]);
                }
                else
                {
                    if (nearbyEnemies[i].CompareTag("Unit"))
                    {
                        //===================== if enemy is dead, remove it from the list ========================
                        if (nearbyEnemies[i].GetComponent<UnitMain>().isDead)
                        {
                            nearbyEnemies.Remove(nearbyEnemies[i]);
                            return;
                        }


                        if (nearbyEnemies[i].GetComponent<UnitMain>().playerGroup == playerGroup)
                        {
                            nearbyEnemies.Remove(nearbyEnemies[i]);
                            return;
                        }
                        if (nearbyEnemies[i].GetComponent<UnitMain>().isRescuable) nearbyEnemies.Remove(nearbyEnemies[i]);
                    }
                   if (nearbyEnemies[i].CompareTag("Player"))
                    {
                        //===================== if player is dead, remove it from the list ========================
                        if (nearbyEnemies[i].GetComponent<PlayerMain>().isDead) nearbyEnemies.Remove(nearbyEnemies[i]);
                    }
                }
            }
        }
        
        
        //================== if objective is null, stop attacking ===========================================
        if(attackObjective == null) StopAttack();
       

    }
    IEnumerator SearchEnemies()
    {
        //==================== if is enemy, add it to the list of enemies =====================
        FillListOfEnemies();
        if (nearbyEnemies.Count > 0)
        {
            if (attackObjective == null )
            {
                attackObjective = SearchNearestEnemy();
            }        
            else if (attackObjective != null && attackObjective.CompareTag("Unit"))
            {
                if (attackObjective.GetComponent<UnitMain>().isDead) attackObjective = SearchNearestEnemy();
            }
            else if (attackObjective != null && attackObjective.CompareTag("Player"))
            {
                if (attackObjective.GetComponent<PlayerMain>().isDead) attackObjective = SearchNearestEnemy();
            }
            //===================== if objective is null, get the first enemy nearby, check if is alive and set it as objective ========================
        }
        else
        {
            StopAttack();
            if (isAI && patrollStance) StartPatrolling();
            //================= if no nearby enemies, set objective to null and start patrolling
        }
        isAttacking = false;
        yield return new WaitForSeconds(1f);
        
        SearchForEnemies();
    }
    private void FillListOfEnemies()
    {
        for (int i = 0; i < unitsNear.Length; i++)
        {
            if (unitsNear[i] != null && unitsNear[i].CompareTag("Unit"))
            {
                if (!nearbyEnemies.Contains(unitsNear[i].gameObject))
                {
                    UnitMain enemy = unitsNear[i].GetComponent<UnitMain>();
                    if (!enemy.isAI && !enemy.isRescuable)
                    {
                        if (!enemy.isDead && enemy.playerNumber != playerNumber && enemy.playerGroup != playerGroup)
                        {
                            if (!nearbyEnemies.Contains(unitsNear[i].gameObject))
                            {
                                nearbyEnemies.Add(unitsNear[i].gameObject);

                            }
                        }
                    }
                    else
                    {
                        if (!enemy.isDead && enemy.playerNumber != playerNumber && !enemy.isRescuable)
                        {
                            if (!nearbyEnemies.Contains(unitsNear[i].gameObject))
                            {
                                nearbyEnemies.Add(unitsNear[i].gameObject);
                            }
                        }
                    }
                }
            }
            if (unitsNear[i] != null && unitsNear[i].CompareTag("Player"))
            {
                if (!nearbyEnemies.Contains(unitsNear[i].gameObject))
                {
                    PlayerMain player = unitsNear[i].GetComponent<PlayerMain>();
                    if (!player.isDead && player.playerGroup != playerGroup)
                    {
                        if (!nearbyEnemies.Contains(unitsNear[i].gameObject))
                        {
                            nearbyEnemies.Add(unitsNear[i].gameObject);

                        }
                    }
                }
            }
        }
    }
   
    private GameObject SearchNearestEnemy()
    {
        float closestDistance = searchRadius;
        GameObject trans = null;
        if (nearbyEnemies.Count > 1)
        {
            for (int i = 0; i < nearbyEnemies.Count; i++)
            {
                float currentDistance = Vector3.Distance(transform.position, nearbyEnemies[i].transform.position);
                if (currentDistance < closestDistance)
                {
                    closestDistance = currentDistance;
                    trans = nearbyEnemies[i];
                }
            }
            return trans;
        }
        else
        {
            return nearbyEnemies[0];
        }
        

        
        
    }
    public void StopAction()
    {
        StopAttack();
       agent.SetDestination(transform.position);
    }
    #endregion
    #region FormationFunctions
    public IEnumerable<Vector3> EvaluateRadialPoints()
    {
        if (!isDead)
        {
            var amountPerRing = _amount / _rings;
            var ringOffset = 0f;
            for (var i = 0; i < _rings; i++)
            {
                for (var j = 0; j < amountPerRing; j++)
                {
                    var angle = j * Mathf.PI * (2 * _rotations) / amountPerRing + (i % 2 != 0 ? _nthOffset : 0);
                    var radius = _radius + ringOffset + j * _radiusGrowthMultiplier;
                    var x = Mathf.Cos(angle) * radius;
                    var z = Mathf.Sin(angle) * radius;
                    var pos = new Vector3(x, 0, z);
                    pos += GetNoise(pos);
                    pos *= Spread;
                    yield return pos;
                }
                ringOffset += _ringOffset;
            }
        }
        
    }
   
    public Vector3 GetNoise(Vector3 pos)
    {
        var noise = Mathf.PerlinNoise(pos.x * this.noise, pos.z * this.noise);
        return new Vector3(noise, 0, noise);
    }
    #endregion

    #region AiFunctions

    private void StartPatrolling()
    {
        SetWayPoint();
        AIRoam();
        StartCoroutine(DistanceCheck());
        aggressiveStance = true;
        isAggressive = true;
    }
    private void AIRoam()
    {
        if (SetDestination(moveSpot))
        {
            agent.SetDestination(moveSpot);           
        }
        else
        {
            SetWayPoint();           
        }
       
    }
    IEnumerator DistanceCheck()
    {
        
        if (Vector3.Distance(transform.position, moveSpot) <= 1)
        {
            //======================= if is close to the movespot, start wait time and look for a new point ============== 
            yield return new WaitForSeconds(waitTime);
            SetWayPoint();
            AIRoam();
            StartCoroutine(DistanceCheck());
        }
        else
        {
            //======================= if is not close to the moveSpot, wait a second and check again ====================
            yield return new WaitForSeconds(1f);
            StartCoroutine(DistanceCheck());
        }
        
    }
    private void SetWayPoint()
    {
        if (hasPatrolPoints)
        {
            int patrolPoint = Random.Range(0, patrolPoints.Length);
            moveSpot = new Vector3(patrolPoints[patrolPoint].position.x + Random.Range(-5, 5), transform.position.y, patrolPoints[patrolPoint].position.z + Random.Range(-5, 5));

        }
        else
        {
            moveSpot = new Vector3(transform.position.x + Random.Range(-15, 15), transform.position.y, transform.position.z + Random.Range(-15, 15));
            
        }
        //============== set a random waypoint near the GameObject ===================================
    }
    private bool SetDestination(Vector3 targetDestination)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetDestination, out hit, 15f, agent.areaMask))
        {
            return true;
        }
        return false;
    }

    
    #endregion
    #region AudioFunctions

    public void AttackAudio()
    {
        StartCoroutine(PlayAttackAudio());
    }
    IEnumerator PlayAttackAudio()
    {
        float audioDelay = Random.Range(1f, 1.5f);
        yield return new WaitForSeconds(audioDelay);
        audioSource.PlayOneShot(attackAudio);
    }
   
    public void DeathAudio()
    {
        audioSource.PlayOneShot(deathAudio);
    }
    public void TakeDamageAudio()
    {
        if (!takeDamageAudioPlayed)
        {
            StartCoroutine(TakeDamageClip());
        }
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
    public void ThrowAudio()
    {
        StartCoroutine(ThrowAudioClip());
    }
    IEnumerator ThrowAudioClip()
    {
        
        if (!throwAudioPlayed)
        {
            audioSource.PlayOneShot(throwAudio);
            throwAudioPlayed = true;
        }
        yield return new WaitForSeconds(0.7f);
        throwAudioPlayed = false;
       
    }
    #endregion
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, searchRadius);
    }
}

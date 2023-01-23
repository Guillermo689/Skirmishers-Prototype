using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerCommands : MonoBehaviour
{
    private PlayerMain mainScript;
    private PlayerInput playerInput;
    private Camera mainCamera;
    private GameEvents gameEvents;

    private InputAction rallyAll;
    private InputAction rallyMelee;
    private InputAction rallyRanged;
    private InputAction attackAll;
    private InputAction attackMelee;
    private InputAction attackRanged;
    private InputAction soloMode;
    private InputAction aggresiveOn;
    private InputAction aggressiveOff;
    private InputAction actionMenu;
    private InputAction stop;
    private InputAction showHealthBar;


    //==================================== Rally Variables ================================

    [SerializeField] private float rallyRadius;
    [SerializeField] internal List<GameObject> meleeUnits = new List<GameObject>();
    [SerializeField] internal List<GameObject> rangedUnits = new List<GameObject>();
    [SerializeField] internal List<GameObject> allUnits = new List<GameObject>();
    private Collider[] unitsNear;
    [SerializeField] private GameObject particle;

    //==================================== Attack Variables ================================
    internal GameObject attackObjective;
    internal bool selectAttack = false;
    private bool alreadyAttacked;
    private bool isAttacking;
    [SerializeField] private int damage;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float attackRange;

    //=================================== Action Menu Variables ============================
    [SerializeField] private bool actionMenuOn;
   
    [SerializeField] private Animator actionMenuAnimator;
    [SerializeField] private Animator actionMenuAnimatorESP;
    private int actionMenuID;

    [SerializeField] private Texture2D cursorHand;
    [SerializeField] private Texture2D cursorSword;
    [SerializeField] internal GameObject movePoint;
    [SerializeField] internal GameObject attackPoint;
    void Start()
    {
        CursorHand();
        mainScript = GetComponent<PlayerMain>();
        playerInput = GetComponent<PlayerInput>();
       
        mainCamera = Camera.main;
        gameEvents = FindObjectOfType<GameEvents>();

        rallyAll = playerInput.actions["RallyAll"];
        rallyMelee = playerInput.actions["RallyMelee"];
        rallyRanged = playerInput.actions["RallyRanged"];
        attackAll = playerInput.actions["AttackAll"];
        soloMode = playerInput.actions["SoloMode"];
        aggresiveOn = playerInput.actions["AggressiveOn"];
        aggressiveOff = playerInput.actions["AggressiveOff"];
        actionMenu = playerInput.actions["ActionMenu"];
        stop = playerInput.actions["Stop"];
        showHealthBar = playerInput.actions["ShowHealthBar"];
        actionMenuID = Animator.StringToHash("MenuOn");
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUnits();
        if (!mainScript.isDead)
        {
            if (rallyAll.WasPressedThisFrame()) CommandRallyAll();
            if (rallyMelee.WasPressedThisFrame()) CommandRallyMelee();
            if (rallyRanged.WasPressedThisFrame()) CommandRallyRanged();
            if (soloMode.WasPressedThisFrame()) CommandSoloMode();
            if (attackAll.WasPressedThisFrame()) CommandSelectAttackAll();
            if (aggresiveOn.WasPressedThisFrame()) CommandAggressiveOn();
            if (aggressiveOff.WasPressedThisFrame()) CommandAggressiveOff();
            if (stop.WasPressedThisFrame()) CommandStop();
           ShowActionMenu();
           
            if (showHealthBar.IsPressed())
            {
                GameObject[] allUnits = GameObject.FindGameObjectsWithTag("Unit");
                foreach (GameObject unit in allUnits)
                {
                    unit.GetComponent<UnitMain>().ShowHealthBar();
                    mainScript.ShowHealthBar();
                }
            }
            else
            {
                GameObject[] allUnits = GameObject.FindGameObjectsWithTag("Unit");
                foreach (GameObject unit in allUnits)
                {
                    unit.GetComponent<UnitMain>().HideHealthBar();
                    mainScript.HideHealthBar();
                }
            }
            if (selectAttack)
            {

                AttackAllFunction();
            }
           
            DistanceCheck();
        }
       
        

    }
    private void CursorHand()
    {
        Cursor.SetCursor(cursorHand, Vector2.zero, CursorMode.Auto);
    }
    private void CursorSword()
    {
        Cursor.SetCursor(cursorSword, Vector2.zero, CursorMode.Auto);
    }

    #region AttackFunctions
    private void UpdateUnits()
    {
        for (int i = 0; i < meleeUnits.Count; i++)
        {
            if (meleeUnits[i] == null || meleeUnits[i].GetComponent<UnitMain>().isDead)
            {
                meleeUnits.Remove(meleeUnits[i]);
            }
        }
        for (int i = 0; i < rangedUnits.Count; i++)
        {
            if (rangedUnits[i] == null || rangedUnits[i].GetComponent<UnitMain>().isDead)
            {
                rangedUnits.Remove(rangedUnits[i]);
            }
        }
        for (int i = 0; i < allUnits.Count; i++)
        {
            if (allUnits[i] == null || allUnits[i].GetComponent<UnitMain>().isDead)
            {
                allUnits.Remove(allUnits[i]);
            }
        }
        //=================== if unit is dead, remove it from the list =====================
    }
    private void DistanceCheck()
    {
        if (attackObjective != null)
        {
            if (Vector3.Distance(transform.position, attackObjective.transform.position) <= attackRange)
            {
                //======================= if is in range, has an objective, can attack is aggressive, stop and look at target
                
                    mainScript.movementScript.agent.SetDestination(transform.position);
                    transform.LookAt(attackObjective.transform.position);
                    if (!alreadyAttacked && isAttacking) Attack();
               
               
            }
            else
            {
                GoToAttack();
            }
        }
        
    }
    public void AttackAllFunction()
    {        
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            CursorHand();
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray: ray, hitInfo: out RaycastHit hit) && hit.collider)
            {
                if (hit.collider.CompareTag("Unit"))
                {
                    UnitMain unitMain = hit.collider.GetComponent<UnitMain>();
                    if (unitMain.playerNumber != mainScript.playerNumber && unitMain.playerGroup != mainScript.playerGroup)
                    {
                        mainScript.CommandAnimation();
                        mainScript.CommandAttackAudio();
                        attackObjective = hit.collider.gameObject;
                        attackPoint.transform.position = attackObjective.transform.position + new Vector3(0, 1, 0);
                        attackPoint.SetActive(true);
                        AttackAll();
                        mainScript.isSelecting = false;
                        selectAttack = false;
                        GoToAttack();
                        //==================== select the objective, then remove the select function ===================
                        
                    }
                }
                else
                {
                    //================ if no objective selected, just deselect the function ===========================================
                    mainScript.isSelecting = false;
                    selectAttack = false;
                   
                }
            }
        }
        if (Mouse.current.middleButton.wasReleasedThisFrame)
        {
            CursorHand();
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray: ray, hitInfo: out RaycastHit hit) && hit.collider)
            {
                if (hit.collider.CompareTag("Unit"))
                {
                    UnitMain unitMain = hit.collider.GetComponent<UnitMain>();
                    if (unitMain.playerNumber != mainScript.playerNumber && unitMain.playerGroup != mainScript.playerGroup)
                    {
                        mainScript.SoloAttackAudio();
                        attackObjective = hit.collider.gameObject;
                        
                        mainScript.isSelecting = false;
                        selectAttack = false;
                        GoToAttack();
                        //==================== select the objective, then remove the select function ===================
                       
                    }
                }
                else
                {
                    //================ if no objective selected, just deselect the function ===========================================
                    mainScript.isSelecting = false;
                    selectAttack = false;
                   
                }
            }
        }
    }
    public void StopAttack()
    {
        isAttacking = false;
        mainScript.movementScript.agent.isStopped = false;
        TargetNull();
    }
    private void TargetNull()
    {
        attackObjective = null;
    }
    public void GoToAttack()
    {
        //======================= Stop coroutine from check to point and set destination to target ======================
       mainScript.movementScript.agent.SetDestination(attackObjective.transform.position);
       
        isAttacking = true;
    }
    
    public void AttackAll()
    {
        foreach (GameObject unit in meleeUnits)
        {
            UnitMain unitMain = unit.GetComponent<UnitMain>();
            unitMain.attackObjective = attackObjective;
            unitMain.isAggressive = true;
            unitMain.isCommanded = true;
            unitMain.GoToAttack();
            unitMain.AttackAudio();
            
        }
        foreach (GameObject unit in rangedUnits)
        {
            UnitMain unitMain = unit.GetComponent<UnitMain>();
            unitMain.attackObjective = attackObjective;
            unitMain.isAggressive = true;
            unitMain.isCommanded = true;
            unitMain.GoToAttack();
            unitMain.AttackAudio();
        }
        //======================= resume movement, set objective, turn them aggressive and enale the attack function ==================
        
    }
    private void Attack()
    {
        //======================= Use melee attack =====================
      StartCoroutine(AttackCycle());
       
    }
    IEnumerator AttackCycle()
    {
        mainScript.StartCombatTimer();
        if (attackObjective != null && isAttacking)
        {
            if (!attackObjective.CompareTag("Player"))
            {
                //======================= if is not player, get the player to take damage =============================
                alreadyAttacked = true;
                mainScript.movementScript.agent.SetDestination(transform.position);
                UnitMain unitMain = attackObjective.GetComponent<UnitMain>();
               if (!unitMain.isDead)
                {
                    unitMain.TakeDamage(damage);
                    mainScript.AttackAnimation();
                    alreadyAttacked = true;
                    mainScript.WeaponAudio();
                    yield return new WaitForSeconds(attackSpeed);
                    // mainScript.movementScript.agent.isStopped = false;
                    alreadyAttacked = false;
                }
               else
                {
                    StopAttack();
                    alreadyAttacked = false;
                }
                
                
            }
            else
            {
                //======================= if is player, get the player to take damage =============================
                PlayerMain unitMain = attackObjective.GetComponent<PlayerMain>();
                mainScript.movementScript.agent.isStopped = true;
                if (!unitMain.isDead)
                {
                    mainScript.AttackAnimation();
                    unitMain.TakeDamage(damage);
                    alreadyAttacked = true;
                    mainScript.WeaponAudio();
                    yield return new WaitForSeconds(attackSpeed);
                    mainScript.movementScript.agent.isStopped = false;
                    alreadyAttacked = false;
                }
                
                
            }
        }
    }
    #endregion

    //========================= Command Functions ======================================
    #region CommandFunctions
    private void ShowActionMenu()
    {
        if (actionMenu.IsPressed())
        {
            if (gameEvents.data.spanish)
            {
                actionMenuAnimatorESP.SetBool(actionMenuID, true);
            }
            else
            {
                actionMenuAnimator.SetBool(actionMenuID, true);

            }



        }
        else
        {
            if (gameEvents.data.spanish)
            {
                actionMenuAnimatorESP.SetBool(actionMenuID, false);
            }
            else
            {
                actionMenuAnimator.SetBool(actionMenuID, false);
            }
           

        }



    }
    public void CommandAggressiveOn()
    {
        foreach (GameObject unit in allUnits)
        {
            UnitMain unitMain = unit.GetComponent<UnitMain>();
            unitMain.isAggressive = true;
            unitMain.aggressiveStance = true;
            unitMain.SearchForEnemies();
            unitMain.GoToAttack();
        }
    }
    public void CommandAggressiveOff()
    {
        foreach (GameObject unit in allUnits)
        {
            UnitMain unitMain = unit.GetComponent<UnitMain>();
            unitMain.isAggressive = false;
            unitMain.aggressiveStance = false;
        }
    }
    public void CommandSelectAttackAll()
    {
        mainScript.isSelecting = true;
        selectAttack = true;
        CursorSword();
       
        //======================= set attackstate to choose a target ===============================
    }
    public void CommandSoloMode()
    {
        meleeUnits.Clear();
        rangedUnits.Clear();
        allUnits.Clear();
    }
    public void CommandRallyAll()
    {
        mainScript.CommandAnimation();
        meleeUnits.Clear();
        rangedUnits.Clear();
        allUnits.Clear();
        unitsNear = Physics.OverlapSphere(transform.position, rallyRadius);
        particle.SetActive(true);
        mainScript.RallyAudio();
        foreach (Collider unit in unitsNear)
        {
            if (unit != null && unit.gameObject.GetComponent<UnitMain>())
            {
                UnitMain unitMain = unit.gameObject.GetComponent<UnitMain>();
                if (unitMain.playerNumber == mainScript.playerNumber && !unitMain.isRanged)
                {
                    if (!meleeUnits.Contains(unit.gameObject))
                    {
                        meleeUnits.Add(unit.gameObject);
                        unitMain.RallyAnimation();
                    }
                }
                if (unit != null && unitMain.playerNumber == mainScript.playerNumber && unitMain.isRanged)
                {
                    if (!meleeUnits.Contains(unit.gameObject))
                    {
                        rangedUnits.Add(unit.gameObject);
                        unitMain.RallyAnimation();
                    }
                }
                if (unitMain.playerNumber == mainScript.playerNumber)
                {
                    if (!allUnits.Contains(unit.gameObject))
                    {
                        allUnits.Add(unit.gameObject);
                    }
                }
            }
        }
        //=================== get all units in the area ===================================
    }
    public void CommandRallyMelee()
    {
        mainScript.CommandAnimation();
        meleeUnits.Clear();
        rangedUnits.Clear();
        allUnits.Clear();
        unitsNear = Physics.OverlapSphere(transform.position, rallyRadius);
        particle.SetActive(true);
        mainScript.RallyAudio();
        foreach (Collider unit in unitsNear)
        {
            if (unit != null && unit.gameObject.GetComponent<UnitMain>())
            {
                UnitMain unitMain = unit.gameObject.GetComponent<UnitMain>();
                if (unitMain.playerNumber == mainScript.playerNumber && !unitMain.isRanged)
                {
                    if (!meleeUnits.Contains(unit.gameObject))
                    {
                        meleeUnits.Add(unit.gameObject);
                        
                        unitMain.RallyAnimation();
                    }
                    if (!allUnits.Contains(unit.gameObject))
                    {
                        allUnits.Add(unit.gameObject);
                    }
                }
               
            }
        }
        //===================== get all melee units in the area ==============================
    }
    public void CommandStop()
    {
        StopAttack();
        mainScript.movementScript.agent.SetDestination(transform.position);
        foreach (GameObject unit in allUnits)
        {
            UnitMain unitMain = unit.GetComponent<UnitMain>();
            unitMain.StopAttack();
        }

    }

    
    public void CommandRallyRanged()
    {
        mainScript.CommandAnimation();
        meleeUnits.Clear();
        rangedUnits.Clear();
        allUnits.Clear(); 
        unitsNear = Physics.OverlapSphere(transform.position, rallyRadius);
        particle.SetActive(true);
        mainScript.RallyAudio();
        foreach (Collider unit in unitsNear)
        {
            if (unit != null && unit.gameObject.GetComponent<UnitMain>())
            {
                UnitMain unitMain = unit.gameObject.GetComponent<UnitMain>();
               
                if (unit != null && unitMain.playerNumber == mainScript.playerNumber && unitMain.isRanged)
                {
                    if (!rangedUnits.Contains(unit.gameObject))
                    {
                        rangedUnits.Add(unit.gameObject);
                        unitMain.RallyAnimation();
                    }
                    if (!allUnits.Contains(unit.gameObject))
                    {
                        allUnits.Add(unit.gameObject);
                    }
                }
            }
        }
    }
    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, rallyRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

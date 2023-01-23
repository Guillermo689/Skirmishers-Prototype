using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    internal PlayerMain mainScript;
    internal NavMeshAgent agent;
    private Camera mainCamera;

    private List<Vector3> _points = new List<Vector3>();


   
    //==================================== Radial formation Variables ===========================
    [Header("Radial Formation")]
    [SerializeField] private int _amount = 10;
    [SerializeField] private float _radius = 1;
    [SerializeField] private float _radiusGrowthMultiplier = 0;
    [SerializeField] private float _rotations = 1;
    [SerializeField] private int _rings = 1;
    [SerializeField] private float _ringOffset = 1;
    [SerializeField] private float _nthOffset = 0;
    [SerializeField][Range(0, 1)] protected float noise = 0;
    [SerializeField] protected float Spread = 1;
    [SerializeField] private float rotationRounds = 10;

    // Start is called before the first frame update
    void Start()
    {
        mainScript = GetComponent<PlayerMain>();
        agent = GetComponent<NavMeshAgent>();
        mainCamera = Camera.main;
        
    }

    // Update is called once per frame
    void Update()
    {
       
        if (Mouse.current.leftButton.wasPressedThisFrame && !EventSystem.current.IsPointerOverGameObject()) Movement();
        if (Mouse.current.middleButton.wasPressedThisFrame) SoloMovement();
        if (mainScript.commandScript.selectAttack && mainScript.commandScript.attackObjective != null && Vector3.Distance(transform.position, mainScript.commandScript.attackObjective.transform.position) <= 2f)
        {
            agent.SetDestination(transform.position);
            //transform.LookAt(mainScript.commandScript.attackObjective.transform.position);
        }
       
    }

    private void Formations()
    {

        _amount = mainScript.commandScript.allUnits.Count;
        _rotations = mainScript.commandScript.allUnits.Count / 10;
        if (_rotations <= 1) _rotations = 1;
        PlayerCommands commands = mainScript.commandScript;
        foreach (GameObject unit in commands.allUnits)
        {
            UnitMain unitMain = unit.GetComponent<UnitMain>();
            unitMain._amount = _amount;
            unitMain._radius = _radius;
            unitMain._radiusGrowthMultiplier = _radiusGrowthMultiplier;
            unitMain._rotations = _rotations;
            unitMain._rings = _rings;
            unitMain._ringOffset = _ringOffset;
            unitMain._nthOffset = _nthOffset;
            unitMain.noise = noise;
            unitMain.Spread = Spread;
        }
    }
    private void Movement()
    {
        Formations();
        if (!mainScript.isSelecting)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray: ray, hitInfo: out RaycastHit hit) && hit.collider)
            {
                if (hit.collider.CompareTag("Ground"))
                {
                    mainScript.commandScript.movePoint.transform.position = hit.point + new Vector3(0,1,0);
                    mainScript.commandScript.movePoint.SetActive(true);
                    mainScript.commandScript.StopAttack();
                    agent.SetDestination(hit.point);
                    _points.Clear();
                   
                    PlayerCommands commands = mainScript.commandScript;
                    if (commands.allUnits.Count > 0)
                    {
                        
                        for (int i = 0; i < commands.allUnits.Count; i++)
                        {
                            UnitMain unitMain = commands.allUnits[i].GetComponent<UnitMain>();
                            unitMain.StopAttack();
                            unitMain.isCommanded = true;
                            _points = unitMain.EvaluateRadialPoints().ToList();
                            unitMain.agent.SetDestination(hit.point + _points[i]);

                        }
                       
                    }
                    

                }
               
            }
        }
      
    }
    private void SoloMovement()
    {
        if (!mainScript.isSelecting)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray: ray, hitInfo: out RaycastHit hit) && hit.collider)
            {
                if (hit.collider.CompareTag("Ground"))
                {
                    mainScript.commandScript.movePoint.transform.position = hit.point + new Vector3(0, 1, 0);
                    mainScript.commandScript.movePoint.SetActive(true);
                    mainScript.commandScript.StopAttack();
                    agent.SetDestination(hit.point);
                }
                
            }
        }
    }



}

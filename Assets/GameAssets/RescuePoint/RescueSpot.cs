using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RescueSpot : MonoBehaviour
{
    private Collider[] unitsNear;
    [SerializeField] private float rescueRadius;
    [SerializeField] private float playerRadius;
    [SerializeField] private List<GameObject> capturedUnits;
    // Start is called before the first frame update
    void Start()
    {
        unitsNear = Physics.OverlapSphere(transform.position, rescueRadius);
        foreach (Collider unit in unitsNear)
        {
            if (unit.CompareTag("Unit"))
            {
                if (!capturedUnits.Contains(unit.gameObject))
                {
                    capturedUnits.Add(unit.gameObject);
                }
            }
        }
        foreach (GameObject unit in capturedUnits)
        {
            if (unit != null && unit.gameObject.GetComponent<UnitMain>())
            {
                UnitMain unitMain = unit.gameObject.GetComponent<UnitMain>();
                if (unitMain.playerNumber != unitMain.playerGroup)
                {
                    unitMain.playerNumber = 0;
                    unitMain.playerGroup = 7;
                    unitMain.isAggressive = false;
                    unitMain.isRescuable = false;
                }
               
               
            }
        }
    }

    private void Update()
    {
        unitsNear = Physics.OverlapSphere(transform.position, playerRadius);
        foreach (Collider other in unitsNear)
        {
            if (other.CompareTag("Player"))
            {
                PlayerMain playerMain = other.GetComponent<PlayerMain>();
                foreach (GameObject unit in capturedUnits)
                {
                    if (unit != null && unit.gameObject.GetComponent<UnitMain>())
                    {
                        UnitMain unitMain = unit.gameObject.GetComponent<UnitMain>();
                        if (unitMain.isRescuable)
                        {
                            unitMain.playerNumber = playerMain.playerNumber;
                            unitMain.playerGroup = playerMain.playerGroup;
                            unitMain.isRescuable = false;
                            unit.GetComponent<UnitColor>().ChangeColor();
                        }

                    }
                }
            }
        }
            
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, rescueRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, playerRadius);
    }
}

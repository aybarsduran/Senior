using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Targeter : MonoBehaviour
{
    [SerializeField] private CinemachineTargetGroup cineTargetGroup;
    public List<Target> targets = new List<Target>();

    private Camera mainCamera; 

    public Target CurrentTarget { get; private set; }
    private void Start()
    {
        mainCamera = Camera.main; //mainCamera variableýna main cameramýzý atadýk.
    }

    private void OnTriggerEnter(Collider other)
    {
        Target target = other.GetComponent<Target>();
        if(target == null) { return; }
      
        targets.Add(target);
        target.OnDestroyed += RemoveTarget;
    }
    private void OnTriggerExit(Collider other)
    {
        Target target = other.GetComponent<Target>();
        if (target == null)
        {
            return;
        }
        RemoveTarget(target);
    }

    public bool SelectTarget()
    {
        if(targets.Count == 0) { return false; }

        Target closestTarget = null;
        float closestTargetDistance = Mathf.Infinity; // ilk buldugumuz target her zaman bundan kücük olacagi icin infinity yaptik

        foreach (Target target in targets)
        {
            Vector2 viewPos = mainCamera.WorldToViewportPoint(target.transform.position); 
            //targetin gorus acimizda nerde oldugunu gosterir  ve normalde Vector3 doner ama biz vec2 aldik.

            if(viewPos.x < 0 ||  viewPos.x > 1 || viewPos.y < 0 || viewPos.y > 1) // 0 ve 1 arasýnda olursa ekranimizdadir.
            {
                continue; //ignore this demek
            }

            Vector2 toCenter = viewPos - new Vector2(0.5f, 0.5f);  //0.5 0.5 ekranin ortasi. Viewpos ne kadar dusukse o kadar yakin
            if(toCenter.sqrMagnitude < closestTargetDistance ) //sqrmagnitude digerine gore daha performansli.
            {
                closestTarget = target;
                closestTargetDistance = toCenter.sqrMagnitude;
            }

        }

        if (closestTarget == null) { return false; } // closest yoksa false don.

        CurrentTarget = closestTarget; // burasi calisiyorsa vardir onu closest a ata
        cineTargetGroup.AddMember(CurrentTarget.transform, 1, 2);
        return true;

    }
    public void Cancel()
    {
        if(CurrentTarget == null) { return; }
        cineTargetGroup.RemoveMember(CurrentTarget.transform);
        CurrentTarget = null;
        
    }
    
    private void RemoveTarget(Target target)
    {
        if(CurrentTarget == target)
        {
            cineTargetGroup.RemoveMember(CurrentTarget.transform);
            CurrentTarget= null;
        }

        target.OnDestroyed -= RemoveTarget;
        targets.Remove(target);
    }

}

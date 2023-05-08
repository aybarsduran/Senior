using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Targeter : MonoBehaviour
{
    [SerializeField] private CinemachineTargetGroup cineTargetGroup;
    public List<Target> targets = new List<Target>();

    public Target CurrentTarget { get; private set; }

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


        CurrentTarget = targets[0];
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

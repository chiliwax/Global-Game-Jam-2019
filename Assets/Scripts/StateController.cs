using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateController : MonoBehaviour
{

    public State currentState;
    public Transform eyes;
    public State remainState;
    public Player player;
    [HideInInspector] public int destPoint = 0;
    public List<Transform> wayPointList;
    [HideInInspector] public Transform chaseTarget;
    [HideInInspector] public float stateTimeElapsed;

    public bool aiActive;
    [Range(0, 2)]
    public float speed;

    internal bool CheckIfCountDownElapsed(object searchDuration)
    {
        throw new NotImplementedException();
    }

    void Update()
    {
        if (!aiActive)
            return;
        currentState.UpdateState(this);
    }

    public void GotoNextPoint()
    {
        if (wayPointList.Count == 0)
            return ;

        if (destPoint >= wayPointList.Count)
            destPoint = 0;
        else
            destPoint += 1;
    }

    void OnDrawGizmos()
    {
        if (currentState != null && eyes != null)
        {
            Gizmos.color = currentState.sceneGizmoColor;
        }
    }

    public void TransitionToState(State nextState)
    {
        if (nextState != remainState)
        {
            currentState = nextState;
            OnExitState();
        }
    }

    public bool CheckIfCountDownElapsed(float duration)
    {
        stateTimeElapsed += Time.deltaTime;
        return (stateTimeElapsed >= duration);
    }

    private void OnExitState()
    {
        stateTimeElapsed = 0;
    }
}
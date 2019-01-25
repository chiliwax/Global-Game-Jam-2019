using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Patrol")]
public class PatrolAction : Action
{
    public override void Act(StateController controller)
    {
        Patrol(controller);
    }

    private void Patrol(StateController controller)
    {
        controller.transform.position -= (controller.transform.position - controller.wayPointList[controller.destPoint].position).normalized * controller.speed * Time.deltaTime;

        if (Vector2.Distance(controller.transform.position, controller.wayPointList[controller.destPoint].position) <= 0.1)
        {
            controller.GotoNextPoint();
        }
    }
}
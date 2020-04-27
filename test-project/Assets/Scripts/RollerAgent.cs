using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class RollerAgent : Agent
{
    public Transform target = null;
    public float speed = 10; 

    private Vector3 initialPos;
    private Rigidbody rBody;

    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        initialPos = transform.position;
    }

    private void SetTarget()
    {
        // Move the target to a new spot
        target.position = new Vector3(initialPos.x + Random.Range(-6, 6),
                                      initialPos.y,
                                      initialPos.z + Random.Range(-6, 6));
    }

    public override void AgentReset()
    {
        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = Vector3.zero;
        this.transform.position = initialPos;

        SetTarget();
    }

    public override void CollectObservations()
    {
        // Target and Agent positions
        AddVectorObs(target.position);
        AddVectorObs(this.transform.position);

        // Agent velocity
        AddVectorObs(rBody.velocity.x);
        AddVectorObs(rBody.velocity.z);
    }

    public override void AgentAction(float[] vectorAction)
    {
        // Actions
        // The decision of the Brain comes in the form of an action array passed
        // to the AgentAction() function. The number of elements in this
        // array is determined by the Vector Action Space Type and Space Size
        // settings of the agent's Brain. 
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];
        rBody.AddForce(controlSignal * speed);

        // Rewards
        // Reinforcement learning requires rewards. Assign rewards in the
        // AgentAction() function. The learning algorithm uses the rewards
        // assigned to the Agent during the simulation and learning process to
        // determine whether it is giving the Agent the optimal actions.
        float distanceToTarget = Vector3.Distance(this.transform.position,
                                                  target.position);

        // Reached target
        if (distanceToTarget < 2.0f)
        {
            SetReward(1.0f);
            SetTarget();
        }

        // Fell off platform
        if (GetCumulativeReward() == 3)
        {
            Done();
        }
    }

    // In order for the Agent to use the Heuristic, you will need to set the
    // Behavior Type to Heuristic Only in the Behavior Parameters of the
    // RollerAgent.
    public override float[] Heuristic()
    {
        // What this code means is that the heuristic will generate an action 
        // corresponding to the values of the "Horizontal" and "Vertical" input
        // axis (which correspond to the keyboard arrow keys).
        var action = new float[2];
        action[0] = Input.GetAxis("Horizontal");
        action[1] = Input.GetAxis("Vertical");
        return action;
    }
}

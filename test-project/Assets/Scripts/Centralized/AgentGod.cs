using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class AgentGod : Agent
{
    // Agents
    public int maxAgents = 2;
    public List<AgentCentralized> agents;

    // Common
    public Transform target = null;
    public Transform obstacle = null;
    private Vector3 targetInitialPos;

    void Start()
    {
        targetInitialPos = target.position;
    }

    public override void AgentReset()
    {
        for (int i = 0; i < agents.Count; i++)
        {
            agents[i].Reset();
        }

        target.position = new Vector3(targetInitialPos.x + Random.Range(-7, 7),
                                      targetInitialPos.y,
                                      targetInitialPos.z + Random.Range(-3, 3));
    }

    public override void CollectObservations()
    {
        // Common
        AddVectorObs(target.position);
        AddVectorObs(obstacle.position);

        // Agents
        for (int i = 0; i < maxAgents; i++)
        {
            AddVectorObs(agents[i].GetRigidBody().velocity);
            AddVectorObs(agents[i].transform.position);
        }
    }

    public override void AgentAction(float[] vectorAction)
    {
        bool targetReached = false;

        // Actions
        for (int i = 0; i < maxAgents; i++)
        {
            var x = vectorAction[0 + (i * 3)];
            var y = vectorAction[1 + (i * 3)];
            var z = vectorAction[2 + (i * 3)];

            if (y != 0 && agents[i].isGrounded)
            {
                agents[i].GetRigidBody().AddForce(new Vector3(x * agents[i].speed, y * agents[i].jumpForce, z * agents[i].speed));
                agents[i].isGrounded = false;
            }
            else
            {
                agents[i].GetRigidBody().AddForce(new Vector3(x * agents[i].speed, 0, z * agents[i].speed));
            }

            targetReached = TargetReached(agents[i].transform.position);
        }

        // Apply reward to god
        if (targetReached)
        {
            float reward = (1.0f - ((float)GetStepCount() / (float)maxStep)) * 100;
            SetReward(reward);
            Done();
        }
    }

    public override float[] Heuristic()
    {
        var action = new float[6];

        action[0] = Input.GetAxis("Horizontal");
        action[1] = Input.GetKey(KeyCode.Space) ? 1.0f : 0.0f;
        action[2] = Input.GetAxis("Vertical");
        action[3] = Input.GetAxis("Horizontal");
        action[4] = Input.GetKey(KeyCode.Space) ? 1.0f : 0.0f;
        action[5] = Input.GetAxis("Vertical");

        return action;
    }

    public bool TargetReached(Vector3 position)
    {
        return Vector3.Distance(position, target.position) < 3.0f ? true : false;
    }
}

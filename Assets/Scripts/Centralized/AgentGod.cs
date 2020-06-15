using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class AgentGod : Agent
{
    // Agents
    public int maxAgents = 2;
    public List<AgentCentralized> agents;

    // Common
    public Transform target = null;
    public Transform obstacle = null;
    private Vector3 targetInitialPos;

    public override void Initialize()
    {
        targetInitialPos = target.position;
    }

    public override void OnEpisodeBegin()
    {
        for (int i = 0; i < agents.Count; i++)
        {
            agents[i].Reset();
        }

        target.position = new Vector3(targetInitialPos.x + Random.Range(-7, 7),
                                      targetInitialPos.y,
                                      targetInitialPos.z + Random.Range(-3, 3));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Common
        sensor.AddObservation(target.position);
        sensor.AddObservation(obstacle.position);

        // Agents
        for (int i = 0; i < maxAgents; i++)
        {
            sensor.AddObservation(agents[i].GetRigidBody().velocity);
            sensor.AddObservation(agents[i].transform.position);
        }
    }

    public override void OnActionReceived(float[] vectorAction)
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
            float reward = (1.0f - ((float)StepCount / (float)MaxStep)) * 100;
            SetReward(reward);
            EndEpisode();
        }
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxis("Horizontal");
        actionsOut[1] = Input.GetKey(KeyCode.Space) ? 1.0f : 0.0f;
        actionsOut[2] = Input.GetAxis("Vertical");

        actionsOut[3] = Input.GetAxis("Horizontal");
        actionsOut[4] = Input.GetKey(KeyCode.Space) ? 1.0f : 0.0f;
        actionsOut[5] = Input.GetAxis("Vertical");
    }

    public bool TargetReached(Vector3 position)
    {
        return Vector3.Distance(position, target.position) < 3.0f ? true : false;
    }
}

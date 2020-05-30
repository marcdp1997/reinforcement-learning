using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public AgentDistributed agentA;
    public AgentDistributed agentB;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("agent"))
        {
            float reward = (1.0f - ((float)agentA.GetStepCount() / (float)agentA.maxStep)) * 100;

            agentA.SetReward(reward);
            agentB.SetReward(reward);

            agentA.Done();
            agentB.Done();
        }
    }
}

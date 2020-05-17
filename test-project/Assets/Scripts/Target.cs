using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public RollerAgent agentA;
    public RollerAgent agentB;

    // Shared reward
    // Episode ends for both
    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("agent"))
        {
            float reward = (1.0f - ((float)agentA.GetStepCount() / (float)agentA.maxStep)) * 100;

            agentA.SetReward(reward);
            //agentB.SetReward(reward);

            agentA.Done();
            //agentB.Done();
        }
    }
}

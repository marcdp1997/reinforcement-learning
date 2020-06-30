using UnityEngine;

public class Target : MonoBehaviour
{
    public AgentDistributed agentA;
    public AgentDistributed agentB;

    public TextMesh textAgentA;
    public TextMesh textAgentB;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("agent"))
        {
            float reward = (1.0f - ((float)agentA.StepCount / (float)agentA.MaxStep)) * 100;

            agentA.SetReward(reward);
            agentB.SetReward(reward);

            textAgentA.text = "Last reward: " + agentA.GetCumulativeReward();
            textAgentB.text = "Last reward: " + agentB.GetCumulativeReward();

            agentA.EndEpisode();
            agentB.EndEpisode();
        }
    }
}

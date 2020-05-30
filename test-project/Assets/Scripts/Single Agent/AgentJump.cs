using UnityEngine;
using MLAgents;

public class AgentJump : Agent
{
    public TextMesh debugText;

    public Transform target = null;
    public Transform obstacle = null;
    public float speed = 10;
    public float jumpForce = 200;

    private Vector3 initialPos;
    private Vector3 targetInitialPos;

    private Rigidbody rBody;
    private bool isGrounded = true;

    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        initialPos = transform.position;
        targetInitialPos = target.position;
    }

    public override void AgentReset()
    {
        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = Vector3.zero;
        this.transform.position = initialPos;

        target.position = new Vector3(targetInitialPos.x + Random.Range(-7, 7),
                                      targetInitialPos.y,
                                      targetInitialPos.z + Random.Range(-3, 3));
    }

    public override void CollectObservations()
    {
        // Observations
        AddVectorObs(target.position);
        AddVectorObs(this.transform.position);
        AddVectorObs(obstacle.position);
        AddVectorObs(rBody.velocity);
    }

    public override void AgentAction(float[] vectorAction)
    {
        // Actions
        var x = vectorAction[0];
        var y = vectorAction[1];
        var z = vectorAction[2];

        if (y != 0 && isGrounded)
        {
            rBody.AddForce(new Vector3(x * speed, y * jumpForce, z * speed));
            isGrounded = false;
        }
        else
        {
            rBody.AddForce(new Vector3(x * speed, 0, z * speed));
        }

        float distanceToTarget = Vector3.Distance(this.transform.position,
                                          target.position);

        if (distanceToTarget < 3.0f)
        {
            float reward = (1.0f - ((float)GetStepCount() / (float)maxStep)) * 100;
            SetReward(reward);
            debugText.text = "Last episode reward: " + GetCumulativeReward();
            Done();
        }
    }

    // Heuristic behaviour
    public override float[] Heuristic()
    {
        var action = new float[3];

        action[0] = Input.GetAxis("Horizontal");
        action[1] = Input.GetKey(KeyCode.Space) ? 1.0f : 0.0f;
        action[2] = Input.GetAxis("Vertical");
        return action;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("ground"))
            isGrounded = true;
    }
}

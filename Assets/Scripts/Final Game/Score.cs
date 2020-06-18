using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    private int redScore;
    private int blueScore;

    public Text redScoreText;
    public Text blueScoreText;

    public bool RedScoreChanged { set; get; }
    public bool BlueScoreChanged { set; get; }

    void Start()
    {
        ResetScore();
    }

    public void ResetScore()
    {
        redScore = 0;
        blueScore = 0;
    }

    public void AddBlueScore()
    {
        blueScore++;
        blueScoreText.text = blueScore.ToString();
        BlueScoreChanged = true;
    }

    public void AddRedScore()
    {
        redScore++;
        redScoreText.text = redScore.ToString();
        RedScoreChanged = true;
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Score : MonoBehaviour
{
    private int redScore;
    private int blueScore;

    public List<Player> players;
    public Text redScoreText;
    public Text blueScoreText;

    public void ResetScore()
    {
        redScore = 0;
        blueScore = 0;
        blueScoreText.text = blueScore.ToString();
        redScoreText.text = redScore.ToString();
    }

    public void AddBlueScore()
    {
        blueScore++;
        blueScoreText.text = blueScore.ToString();

        if (MatchIsOver())
        {
            AddReward(50.0f, Team.Blue);
            EndGame();
        }
        else AddReward(10.0f, Team.Blue);
    }

    public void AddRedScore()
    {
        redScore++;
        redScoreText.text = redScore.ToString();

        if (MatchIsOver())
        {
            AddReward(50.0f, Team.Red);
            EndGame();
        }
        else AddReward(10.0f, Team.Red);
    }

    private void AddReward(float reward, Team team)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].team == team) players[i].AddReward(reward);
        }
    }

    private void EndGame()
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].EndEpisode();
        }
    }

    public bool MatchIsOver()
    {
        return redScore >= 5 || blueScore >= 5 ? true : false;
    }
}

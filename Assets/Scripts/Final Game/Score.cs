﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Score : MonoBehaviour
{
    private int redScore;
    private int blueScore;

    public int maxScore;
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
            AddReward(1.0f, Team.Blue);
            AddReward(-1.0f, Team.Red);
            EndGame();
        }
        else
        {
            AddReward(1.0f / maxScore, Team.Blue);
            AddReward(-1.0f / maxScore, Team.Red);
        }
    }

    public void AddRedScore()
    {
        redScore++;
        redScoreText.text = redScore.ToString();

        if (MatchIsOver())
        {
            //AddReward(1.0f, Team.Red);
            //AddReward(-1.0f, Team.Blue);
            EndGame();
        }
        else
        {
            //AddReward(1.0f / maxScore, Team.Red);
            //AddReward(-1.0f / maxScore, Team.Blue);
        }
    }

    private void AddReward(float reward, Team team)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].team == team) 
                players[i].AddReward(reward);

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
        return redScore >= maxScore || blueScore >= maxScore ? true : false;
    }
}

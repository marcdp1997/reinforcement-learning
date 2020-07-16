Official documentation: https://github.com/Unity-Technologies/ml-agents/blob/release_3_docs/docs/Readme.md

This project is using the following softwares:
   Unity 2019.4.0f1 (MLAgents 1.1 package installed)
   Anaconda3 2020.02 (Python 3.7.6)
   MLAgents1.1 release from https://github.com/Unity-Technologies/ml-agents/releases

-------------------------------------------------------------------------------------------
SETUP ENVIRONMENT
-------------------------------------------------------------------------------------------

1. Open anaconda prompt
2. conda create -n mlagents1.1 python=3.7
3. conda activate mlagents1.1
4. pip install mlagents

-------------------------------------------------------------------------------------------
TRAIN
-------------------------------------------------------------------------------------------

1. Open anaconda prompt
2. conda activate mlagents1.1
3. cd (MLAgents release repository path)
4. mlagents-learn config/ppo/Default.yaml --run-id=final_game_55 --train
-------------------------------------------------------------------------------------------
TENSORFLOW
-------------------------------------------------------------------------------------------

1. Open anaconda prompt
2. cd (MLAgents release repository path)
3. conda activate mlagents1.1
4. tensorboard --logdir=summaries --port=6006
5. Open a browser window and navigate to localhost:6006.

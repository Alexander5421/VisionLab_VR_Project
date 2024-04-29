using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState { Start, Running, End }
    
    [SerializeField] private UIManager uiManager;
    [SerializeField] private TrialRunner runner;
    [SerializeField] private int practiceCount;
    [SerializeField] private int experimentCount;
    private GameState currState;

    // Start is called before the first frame update
    void Start()
    {
        // start menu
        // waiting the button pressed;
        currState = GameState.Start;
        uiManager.ShowStartMenu();
        
    }
    public void GameStart()
    {
        currState = GameState.Running;
        uiManager.HideStartMenu();
        runner.InitializeTrials(practiceCount,"practice");
        runner.finish += PracticeFinish;
    }
    
    private void PracticeFinish()
    {
        runner.finish -= PracticeFinish;
        uiManager.ShowTransitionMenu();
    }
    public void ExperimentStart()
    {
        uiManager.HideTransitionMenu();
        runner.InitializeTrials(experimentCount,"experiment");
        runner.finish += GameFinish;
    }
    private void GameFinish()
    {
        currState  = GameState.End;
        uiManager.ShowEndMenu(runner.score);
        print($"GameFinish+ {runner.score}/{experimentCount}");
    }

    // Update is called once per frame


    
}
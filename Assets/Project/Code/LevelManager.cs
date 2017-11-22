using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
     

public class LevelManager : MonoBehaviour {

    public static LevelManager Instance { get; private set; }
    public Player Player { get; private set; }
    private List<Checkpoint> _checkpoints;
    private int _currentCheckPointIndex;
    private DateTime _started;
    private int _savedPoints;
    public TimeSpan RunningTime { get {
            return DateTime.UtcNow - _started;
                } }
    public int BonusCutoffSeconds = 10;
    public int BonusSecondMultiplier = 3;
    public int CurrentTimeBonus { get {
            var secondDifference = (int) (BonusCutoffSeconds-RunningTime.TotalSeconds);
            return Mathf.Max(0, secondDifference)*BonusSecondMultiplier;
        } }
    
    public void Awake()
    {
        _savedPoints = GameManager.Instance.points;
        Instance = this;

    }

    public void Start()
    {
        _started = DateTime.UtcNow;
       _checkpoints = FindObjectsOfType <Checkpoint>().OrderBy(t => t.transform.position.x).ToList();
       _currentCheckPointIndex = _checkpoints.Count > 0 ? 0 : -1;
       Player = FindObjectOfType<Player>();

        var listeners = FindObjectsOfType <MonoBehaviour>().OfType<IPlayerRespawnListener>();
        foreach(var listener in listeners)
        {
            for(var i=_checkpoints.Count-1; i>=0; i--)
            {
                var distance = ((MonoBehaviour)listener).transform.position.x - _checkpoints[i].transform.position.x;
                if (distance < 0)
                    continue;
                _checkpoints[i].AssignObjectToCheckPoint(listener);
                break;
            }
        }

    }
    

    public void Update()
    {
        var isAtLastCheckpoint = _currentCheckPointIndex + 1
            >= _checkpoints.Count;

        if (isAtLastCheckpoint)
            return;

        var distanceToNextChechPoint = _checkpoints[_currentCheckPointIndex + 1].transform.position.x
            - Player.transform.position.x;

        if (distanceToNextChechPoint >= 0)
            return;

        _currentCheckPointIndex++;
        _checkpoints[_currentCheckPointIndex].PlayerHitCheckPoint();

        GameManager.Instance.AddPoints(CurrentTimeBonus);
        _savedPoints = GameManager.Instance.points;
        _started = DateTime.UtcNow;
    }
    public void GotoNextLevel(string LevelName)
    {
        StartCoroutine(GoToNextLevelCo(LevelName));
    }

    private IEnumerator GoToNextLevelCo(string LevelName)
    {
        Player.FinishLevel();
        GameManager.Instance.AddPoints(CurrentTimeBonus);
        FloatingText.Show("Level Complete!", "CheckpointText", new CenteredTextPositioner(.2f));
        yield return new WaitForSeconds(1);
        FloatingText.Show(string.Format(" {0} points!", GameManager.Instance.points), "CheckpointText", new CenteredTextPositioner(.10f));
        yield return new WaitForSeconds(5f);
        if (string.IsNullOrEmpty(LevelName))
            Application.LoadLevel("_StartScreen");
        else
            Application.LoadLevel(LevelName);
    } 
    
    public void KillPlayer()
    {
        StartCoroutine(KillPlayerCo());
    }
    private IEnumerator KillPlayerCo()
        
    {
        Player.Kill();
        yield return new WaitForSeconds(1f);
        if(_currentCheckPointIndex!=-1)
        _checkpoints[_currentCheckPointIndex].SpawnPlayer(Player);
        _started = DateTime.UtcNow;
        GameManager.Instance.ResetPoints(_savedPoints);


        
    }
}

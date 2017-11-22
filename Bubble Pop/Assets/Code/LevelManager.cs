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
        Instance = this;
    }

    public void Start()
    {
        _started = DateTime.UtcNow;
       _checkpoints = FindObjectsOfType <Checkpoint>().OrderBy(t => t.transform.position.x).ToList();
       _currentCheckPointIndex = _checkpoints.Count > 0 ? 0 : -1;
       Player = FindObjectOfType<Player>();

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
        GameManager.Instance.AddPoints(CurrentTimeBonus);
        _savedPoints = GameManager.Instance.points;
        _started = DateTime.UtcNow;
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

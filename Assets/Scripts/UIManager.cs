using System;
using Shared;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text rallyDisplay;
    public Text rightPlayerScore;
    public Text leftPlayerScore;
    public Text startHint;

    private void Awake()
    {
        Assert.IsNotNull(rallyDisplay);
        Assert.IsNotNull(rightPlayerScore);
        Assert.IsNotNull(leftPlayerScore);
        Assert.IsNotNull(startHint);
        
        GameManager.Instance.RallyUpdated += (_, rally) => UpdateRallyText(rally);
        GameManager.Instance.PlayerScoreUpdated += (_, tuple) =>
        {
            switch (tuple.side)
            {
                case PlayerSide.Left:
                    UpdateLeftPlayerScoreText(tuple.score);
                    break;
                case PlayerSide.Right:
                    UpdateRightPlayerScoreText(tuple.score);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(tuple.side));
            }
        };
        GameManager.Instance.GameStarted += (_, __) => HideStartHint();
        GameManager.Instance.GameStopped += (_, __) => ShowStartHint();
    }

    private void HideStartHint()
    {
        startHint.enabled = false;
    }

    private void ShowStartHint()
    {
        startHint.enabled = true;
    }

    private void UpdateRallyText(int newRally)
    {
        rallyDisplay.text = newRally.ToString();
    }

    private void UpdateLeftPlayerScoreText(int newScore)
    {
        leftPlayerScore.text = newScore.ToString();
    }

    private void UpdateRightPlayerScoreText(int newScore)
    {
        rightPlayerScore.text = newScore.ToString();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

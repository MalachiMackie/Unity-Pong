using System;
using System.Collections.Generic;
using Shared;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance
    {
        get { return _instance ??= FindObjectOfType<GameManager>(); }
    }

    private readonly Dictionary<PlayerSide, int> _scores = new Dictionary<PlayerSide, int>
    {
        {PlayerSide.Left, 0},
        {PlayerSide.Right, 0}
    };

    public event EventHandler<int> RallyUpdated;
    public event EventHandler<(PlayerSide side, int score)> PlayerScoreUpdated;
    public event EventHandler GameStarted;
    public event EventHandler GameStopped;

    private int _rallyCount;
    private float _initialPlayerSpeed;
    private bool _waitingToStart;
    private PlayerSide _kickTowardsPlayer;
    
    [Range(0, 5)]
    public float playerPositionOffset = 1f;

    [SerializeField] private GameObject ball;
    private Ball _ballScript;

    private void Start()
    {
        Assert.IsNotNull(Camera.main);

        if (!Camera.main!.orthographic)
        {
            Camera.main.orthographic = true;
        }

        var halfBoardHeight = Camera.main.orthographicSize;
        var halfBoardWidth = halfBoardHeight * Screen.width / Screen.height;

        SetupBall(halfBoardWidth, halfBoardHeight);
        SetupScoreZones(halfBoardWidth * 2, halfBoardHeight * 2);
        SetupPlayers(halfBoardHeight, halfBoardWidth);
    }

    public void Update()
    {
        if (_waitingToStart && Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }

        if (Application.isEditor)
        { 
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
            if (Application.isEditor)
            {
                // EditorApplication.isPlaying = false;
            }
        }
    }

    private void StartGame()
    {
        _ballScript.KickStart(_kickTowardsPlayer);
        GameStarted?.Invoke(this, EventArgs.Empty);
    }

    public void PlayerScored(PlayerSide side)
    {
        Assert.IsTrue(Enum.IsDefined(typeof(PlayerSide), side), $"{side} is not a valid value of {nameof(PlayerSide)}");
        Assert.AreNotEqual(side, PlayerSide.None);

        _ballScript.Crashed();

        _scores[side] += 1;
        PlayerScoreUpdated?.Invoke(this, (side, _scores[side]));
        _kickTowardsPlayer = PlayerSideHelpers.OtherSide(side);
        Reset();
    }

    public void Bounce()
    {
        _rallyCount++;
        RallyUpdated?.Invoke(this, _rallyCount);
        var newPlayerMovementSpeed = _initialPlayerSpeed;
        for (var i = 0; i < _rallyCount; i++)
        {
            newPlayerMovementSpeed *= 1.1f;
        }
        foreach (var player in FindObjectsOfType<Player>())
        {
            player.movementSpeed = Math.Min(newPlayerMovementSpeed, player.maxSpeed);
        }

        _ballScript.GetComponent<Rigidbody2D>().velocity *= 1.1f;
    }

    private void Reset()
    {
        _ballScript.Reset();
        foreach (var player in FindObjectsOfType<Player>())
        {
            player.movementSpeed = _initialPlayerSpeed;
        }
        _waitingToStart = true;
        _rallyCount = 0;
        RallyUpdated?.Invoke(this, _rallyCount);
        GameStopped?.Invoke(this, EventArgs.Empty);
    }
    
    private void SetupBall(float halfBoardWidth, float halfBoardHeight)
    {
        Assert.IsNotNull(ball);
        _ballScript = ball.GetComponent<Ball>();
        _ballScript.SetBounds(-halfBoardWidth, halfBoardWidth, -halfBoardHeight, halfBoardHeight);
        _waitingToStart = true;
        _kickTowardsPlayer = Random.value > 0.5 ? PlayerSide.Left : PlayerSide.Right;
    }

    private static void SetupScoreZones(float boardWidth, float boardHeight)
    {
        var scoreZones = FindObjectsOfType<ScoreZone>();
        var halfBoardWidth = boardWidth / 2;
        foreach (var scoreZone in scoreZones)
        {
            var scoreZoneTransform = scoreZone.transform;
            var scoreZoneScale = scoreZoneTransform.localScale;
            var halfXScale = scoreZoneScale.x / 2;
            var position = scoreZoneTransform.position;
            position.y = 0;
            position.x = scoreZone.side switch
            {
                PlayerSide.Left => -halfBoardWidth + halfXScale,
                PlayerSide.Right => halfBoardWidth - halfXScale,
                _ => throw new ArgumentOutOfRangeException(nameof(scoreZone.side))
            };
            scoreZoneTransform.SetPositionAndRotation(position, scoreZoneTransform.rotation);
            scoreZoneScale = new Vector3(scoreZoneScale.x, boardHeight);
            scoreZoneTransform.localScale = scoreZoneScale;
        }
    }

    private void SetupPlayers(float halfBoardHeight, float halfBoardWidth)
    {
        var players = FindObjectsOfType<Player>();
        foreach (var player in players)
        {
            _initialPlayerSpeed = player.movementSpeed;
            player.SetBounds(-halfBoardHeight, halfBoardHeight);
            var playerTransform = player.transform;
            var playerPosition = playerTransform.position;
            var playerScale = playerTransform.localScale;

            var newX = halfBoardWidth - playerPositionOffset - playerScale.x / 2f;
            if (player.side is PlayerSide.Left)
            {
                newX *= -1;
            }

            playerPosition.x = newX;
            player.transform.position = playerPosition;
        }
    }
}
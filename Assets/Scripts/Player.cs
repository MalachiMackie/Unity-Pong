using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Shared;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    public PlayerSide side;
    public float movementSpeed = 1;
    public float maxSpeed = 50;

    private KeyCode[] _upKeys;
    private KeyCode[] _downKeys;
    
    private float _minY; 
    private float _maxY;
    
    // Start is called before the first frame update
    private void Awake()
    {
        Assert.IsTrue(Enum.IsDefined(typeof(PlayerSide), side), $"{side} is not a valid value of {nameof(PlayerSide)}");
        Assert.AreNotEqual(side, PlayerSide.None);

        AssignInputs();
    }

    private void AssignInputs()
    {
        switch (side)
        {
            case PlayerSide.Left:
            {
                _upKeys = new [] {KeyCode.Q, KeyCode.Quote};
                _downKeys = new [] {KeyCode.A};
                break;
            }
            case PlayerSide.Right:
            {
                _upKeys = new [] {KeyCode.P, KeyCode.L};
                _downKeys = new [] {KeyCode.S, KeyCode.Semicolon};
                break;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(side));
        }
    }

    /// <summary>
    /// Do some custom shit to make the ball bounce off in a different direction depending on which end of the player it hits
    /// Tried to use an ellipse collider, but was broken as shit and wouldn't work
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(Tags.Ball))
        {
            return;
        }

        var otherRigidbody = other.GetComponent<Rigidbody2D>();
        Assert.IsNotNull(otherRigidbody);

        GameManager.Instance.Bounce();

        var otherVelocity = otherRigidbody.velocity;
        otherVelocity = Vector3.Normalize(other.transform.position - transform.position) * otherVelocity.magnitude;
        otherRigidbody.velocity = otherVelocity;
        other.GetComponent<Ball>().Bounced();
    }

    private void FixedUpdate()
    {
        DoMovement();
    }

    public void SetBounds(float minY, float maxY)
    {
        Assert.AreNotApproximatelyEqual(minY, maxY);
        
        _minY = minY;
        _maxY = maxY;
    }

    private Vector2 BoundsCheck(Vector2 newPosition)
    {
        var halfYScale = transform.localScale.y / 2;
        var outOfBounds = newPosition.y + halfYScale > _maxY || newPosition.y - halfYScale < _minY;

        if (!outOfBounds)
        {
            return newPosition;
        }

        if (newPosition.y > 0)
        {
            newPosition.y = _maxY - halfYScale;
        }
        else
        {
            newPosition.y = _minY + halfYScale;
        }

        return newPosition;
    }

    private void DoMovement()
    {
        var isUpPressed = _upKeys.Any(Input.GetKey);
        var isDownPressed = _downKeys.Any(Input.GetKey);

        if (isUpPressed == isDownPressed)
        {
            return;
        }

        float multiplier = isUpPressed ? 1 : -1;
        var position = transform.position;
        position.y += movementSpeed * (1f/50f) * multiplier;
        position = BoundsCheck(position);
        transform.SetPositionAndRotation(position, transform.rotation);
    }
}

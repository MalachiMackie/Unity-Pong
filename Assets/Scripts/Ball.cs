using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Shared;
using UnityEngine;
using UnityEngine.Assertions;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public class Ball : MonoBehaviour
{
    private Transform _transform;
    private Rigidbody2D _rigidbody;
    private AudioSource _audioSource;

    private float _minX; 
    private float _maxX; 
    private float _minY; 
    private float _maxY;

    private float _halfScale;

    public float kickStartForce;
    public AudioClip bounceSound;
    public AudioClip crashSound;
    public AudioClip startSound;

    // Start is called before the first frame update
    private void Awake()
    {
        Assert.IsNotNull(bounceSound);
        Assert.IsNotNull(crashSound);
        Assert.IsNotNull(startSound);
        bounceSound.LoadAudioData();
        crashSound.LoadAudioData();
        startSound.LoadAudioData();
        _transform = GetComponent<Transform>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _audioSource = GetComponent<AudioSource>();
        _halfScale = _transform.localScale.x / 2;
    }

    private void FixedUpdate()
    {
        DoBoundsCheck();
    }

    private void DoBoundsCheck()
    {
        var velocity = _rigidbody.velocity;
        var isOutOfBounds = false;
        if (_transform.position.x < _minX - _halfScale || _transform.position.x > _maxX + _halfScale)
        {
            velocity = new Vector2(-velocity.x, velocity.y);
            isOutOfBounds = true;
        }

        if (_transform.position.y < _minY - _halfScale || _transform.position.y > _maxY + _halfScale)
        {
            velocity = new Vector2(velocity.x, -velocity.y);
            isOutOfBounds = true;
        }
        _rigidbody.velocity = velocity;
        if (isOutOfBounds)
        {
            Bounced();
        }
    }

    public void Reset()
    {
        _rigidbody.velocity = Vector2.zero;
        _transform.SetPositionAndRotation(new Vector3(0, 0, 0), _transform.rotation);
    }

    public void KickStart(PlayerSide side)
    {
        var angle = Random.Range(-30f, 30f);
        var rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        var direction = rotation * Vector2.left;
        if (side == PlayerSide.Right)
        {
            direction.x *= -1;
        }
        _rigidbody.AddForce(direction * kickStartForce, ForceMode2D.Impulse);
        _audioSource.PlayOneShot(startSound);
    }

    public void SetBounds(float minX, float maxX, float minY, float maxY)
    {
        Assert.AreNotApproximatelyEqual(minX, maxX);
        Assert.AreNotApproximatelyEqual(minY, maxY);
        
        _minX = minX;
        _maxX = maxX;
        _minY = minY;
        _maxY = maxY;
    }

    public void Bounced()
    {
        _audioSource.PlayOneShot(bounceSound);
    }

    public void Crashed()
    {
        _audioSource.PlayOneShot(crashSound);
    }
}

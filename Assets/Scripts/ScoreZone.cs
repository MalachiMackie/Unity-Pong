using System;
using System.Collections;
using System.Collections.Generic;
using Shared;
using UnityEngine;
using UnityEngine.Assertions;

public class ScoreZone : MonoBehaviour
{
    public PlayerSide side;

    private void Awake()
    {
        Assert.IsTrue(Enum.IsDefined(typeof(PlayerSide), side), $"{side} is not a valid value of {nameof(PlayerSide)}");
        Assert.AreNotEqual(side, PlayerSide.None);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag(Tags.Ball))
        {
            return;
        }

        GameManager.Instance.PlayerScored(PlayerSideHelpers.OtherSide(side));
    }
}

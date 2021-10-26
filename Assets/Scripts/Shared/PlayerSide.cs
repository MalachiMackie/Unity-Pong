using System;

namespace Shared
{
    public enum PlayerSide
    {
        None,
        Left,
        Right
    }
    
    public static class PlayerSideHelpers
    {
        public static PlayerSide OtherSide(PlayerSide side)
        {
            return side switch
            {
                PlayerSide.Left => PlayerSide.Right,
                PlayerSide.Right => PlayerSide.Left,
                _ => throw new InvalidOperationException($"{side} is not a valid {nameof(PlayerSide)}")
            };
        }
    }
}
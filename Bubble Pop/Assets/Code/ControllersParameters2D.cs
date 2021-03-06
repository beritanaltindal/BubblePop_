﻿using UnityEngine;
using System.Collections;

[System.Serializable]
public class ControllersParameters2D {

	public enum JumpBehaviour
    {
        CanJumpOnGround,
        CanJumpAnywhere,
        CantJump
    }
    public JumpBehaviour JumpRestrictions;

    public Vector2 MaxVelocity = new Vector2(float.MaxValue, float.MaxValue);
    [Range (0, 90)]
    public float SlopeLimit = 30;
    public float Gravity = -25f;
    public float JumpFrequency = 0.25f;
    public float JumpMagnitude = 16f;
}

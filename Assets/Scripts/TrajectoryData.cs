using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;


public struct TrajectoryData : IComponentData
{
    public float currentAngle;
    public float orbitRadius;
    public float3 sunPos;
    public float orbitSpeed;
    public float orbitAxisRatio;
    public int orbitCompletionTimeInDays;

    public bool generateTrail;
    public int generateTrailAfterFrames;
    public int trailCounter;
    public float destroyTrailAfter;


   
}

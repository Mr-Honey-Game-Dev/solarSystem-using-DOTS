using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class PlanetMoveSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float time = (float)UnityEngine.Time.realtimeSinceStartup;
        
        JobHandle jobHandle = new PositionCalculationJobs { DeltaTime = Time.DeltaTime }.ScheduleParallel();
        jobHandle.Complete();
       
        // Debug.Log("Time Taken to Calculate new positions for all plants"+ (((float)UnityEngine.Time.realtimeSinceStartup - time) * 1000) + "ms");
       
    }
}





[BurstCompile]
public partial struct PositionCalculationJobs : IJobEntity
{
    public float DeltaTime;
    public void Execute(ref Translation translation, ref TrajectoryData trajectoryData)
    {
        float x = trajectoryData.orbitRadius * Mathf.Cos(trajectoryData.currentAngle * Mathf.Deg2Rad);
        float y = trajectoryData.orbitRadius * Mathf.Sin(trajectoryData.currentAngle * Mathf.Deg2Rad) * trajectoryData.orbitAxisRatio;
        Vector3 newPosition = new float3(x, y, 0) + trajectoryData.sunPos;
        translation.Value = newPosition;

        trajectoryData.currentAngle += trajectoryData.orbitSpeed * DeltaTime;

        if (trajectoryData.currentAngle >= 360)
        {
            trajectoryData.currentAngle -= 360;
        }
    }

}





/*public struct PositionCalculateJob : IJob
{
    public float deltaTime;
    public float orbitRadius;
    public float orbitSpeed;
    public float currentAngle;
    public float orbitAxisRatio;
    public float3 sunPos;
    public float3 Value;
    public void Execute()
    {
        float x = orbitRadius * Mathf.Cos(currentAngle * Mathf.Deg2Rad);
        float y = orbitRadius * Mathf.Sin(currentAngle * Mathf.Deg2Rad) * orbitAxisRatio;
        float3 newPosition = new float3(x, y, 0) + sunPos;
        Value = newPosition;

        currentAngle += orbitSpeed * deltaTime;

        if (currentAngle >= 360)
        {
            currentAngle -= 360;
        }
    }
}*/

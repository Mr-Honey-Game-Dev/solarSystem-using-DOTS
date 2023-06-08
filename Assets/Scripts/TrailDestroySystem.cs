
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

public partial class TrailDestroySystem : SystemBase
{
    private EntityCommandBufferSystem _entityCommandBufferSystem;

    protected override void OnCreate()
    {
        _entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
    protected override void OnUpdate()
    {

        float time = (float)UnityEngine.Time.realtimeSinceStartup;

        float deltaTime = Time.DeltaTime;
        var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        JobHandle timerJobHandle = new TrailElementTimerJob
        {
            deltaTime = Time.DeltaTime,
            commandBuffer = commandBuffer
        }.ScheduleParallel();
        timerJobHandle.Complete();
         
        _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);

       // Debug.Log("Time Taken to update destroy timer for trail elements" + (((float)UnityEngine.Time.realtimeSinceStartup - time) * 1000) + "ms");

    }
}

[BurstCompile]
public partial struct TrailElementTimerJob : IJobEntity 
{
    public float deltaTime;
    public EntityCommandBuffer.ParallelWriter commandBuffer;
    public void Execute(Entity entity, ref DestroyAfterSeconds destroyAfterSeconds)
    {
        destroyAfterSeconds.timer -= deltaTime;
        if (destroyAfterSeconds.timer <= 0)
        {
            commandBuffer.DestroyEntity(0, entity);
        }
    }
}

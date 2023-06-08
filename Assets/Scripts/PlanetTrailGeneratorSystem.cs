using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;

public class PlanetTrailGeneratorSystem : ComponentSystem
{
    Material mat;
    Mesh mesh;
    protected override void OnCreate() 
    {
        mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        mat.enableInstancing = true;
        mesh = Resources.GetBuiltinResource<Mesh>("Quad.fbx");
    }
    protected override void OnUpdate()
    {      

        Entities.ForEach((ref Translation translation, ref TrajectoryData trajectoryData) =>
        {

            if (trajectoryData.generateTrail)
            {
                if (trajectoryData.trailCounter <= 0)
                {
                    GenerateTrail(translation.Value, mesh,mat , trajectoryData.destroyTrailAfter);
                    trajectoryData.trailCounter = trajectoryData.generateTrailAfterFrames;
                }
                else 
                {
                    trajectoryData.trailCounter--;
                }
            }

        });

     
    }

   
    void GenerateTrail(float3 position, Mesh trailMesh, Material mat, float destroyTrailAfter)
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityArchetype entityArchetype = entityManager.CreateArchetype(
            typeof(Translation),
            typeof(LocalToWorld),
            typeof(Scale),
            typeof(DestroyAfterSeconds)
            );
        Entity trailEntity = entityManager.CreateEntity(entityArchetype);

        var desc = new RenderMeshDescription(
                       trailMesh,
                       mat,
                       shadowCastingMode: ShadowCastingMode.Off,
                       receiveShadows: false);

        RenderMeshUtility.AddComponents(
        trailEntity,
        entityManager,
        desc);

        entityManager.SetComponentData(trailEntity, new Translation { Value = position });
        entityManager.SetComponentData(trailEntity, new Scale { Value = 0.5f });
        entityManager.SetComponentData(trailEntity, new DestroyAfterSeconds { timer = destroyTrailAfter });

    }
}


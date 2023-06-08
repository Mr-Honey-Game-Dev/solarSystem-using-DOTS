using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Mathematics;
using System.IO;

public class SolarSystemGenerator : MonoBehaviour
{
    [Header("Generator Properties")]
    [SerializeField] private bool generateRandom;
    [SerializeField] private bool generateFromFile;
    [SerializeField] private string fileName;

    [Header("SolarSystem Properties")]
    [SerializeField][Range(1,10)] private int simulationSpeed=4;
    [SerializeField] private Planet[] planets;
    [SerializeField] private Mesh planetMesh;


    [Header("Trail Properties")]
    [SerializeField] private bool generateTrail;
    [SerializeField][Range(0,30f)] private float destroyTrailAfter=5;
    [SerializeField][Range(1,5f)] private int generateTrailAfterFrames=2;


    [SerializeField] private Mesh sunMesh;
    [SerializeField] private float sunRadius=10f;
    [SerializeField] private Material sunMaterial;

    [Header("Random Parameters")]
    [SerializeField] private int numberOfPlanets = 8;
    [SerializeField] private Material[] materials;



    private void Start()
    {
        if (generateRandom) 
        {
            GenerateSun();
            GenerateRandomPlanets();
        } 
        else
        {
            if (generateFromFile)
                LoadPlanets();
            GenerateSun();
            GeneratePlanets();
        }
      
    }

    void LoadPlanets()
    {
        string filePath = Application.persistentDataPath+"/"+fileName;
        string jsonString = "";
        if (File.Exists(filePath))
        { 
             jsonString= File.ReadAllText(filePath); 
        }
        else
        {
            File.WriteAllText(filePath, "");
        }
        PlanetDataList planetDataList = JsonUtility.FromJson<PlanetDataList>(jsonString);

        planets = new Planet[planetDataList.planets.Count];

        for (int i = 0; i < planetDataList.planets.Count; i++)
        {
            PlanetData planetData = planetDataList.planets[i];
            Material material = Resources.Load<Material>("Materials/"+planetData.material);
            Mesh trailMesh = Resources.GetBuiltinResource<Mesh>(planetData.trailMesh+".fbx");
            Material trailMat = Resources.Load<Material>("Materials/" + planetData.trailMat);

            Planet planet = new Planet(
                planetData.name,
                material,
                planetData.distanceFromCenter,
                planetData.radius,
                planetData.orbitCompletionTimeInDays,
                planetData.orbitAxisRatio,
                planetData.generateTrail,
                trailMesh,
                trailMat
            );

            planets[i] = planet;
        }
    }
    void GenerateSun() 
    {

        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityArchetype entityArchetype = entityManager.CreateArchetype(
            typeof(Translation),
            typeof(LocalToWorld),
            typeof(Scale)
            );
        Entity sunEntity=entityManager.CreateEntity(entityArchetype);
       
        var desc = new RenderMeshDescription(
                       planetMesh,
                       sunMaterial,
                       shadowCastingMode: ShadowCastingMode.Off,
                       receiveShadows: false);

        RenderMeshUtility.AddComponents(
      sunEntity,
      entityManager,
      desc);

        entityManager.SetComponentData(sunEntity, new Scale { Value = sunRadius * 2f });

    }
    void GeneratePlanets() 
    {
        int numberOfPlanets = planets.Length;

        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityArchetype entityArchetype = entityManager.CreateArchetype(
            typeof(Translation),
            typeof(LocalToWorld),
            typeof(Scale),
            typeof(TrajectoryData)
            );

        NativeArray<Entity> entityArray = new NativeArray<Entity>(numberOfPlanets, Allocator.Temp);
        entityManager.CreateEntity(entityArchetype, entityArray);

        for (int i = 0; i < entityArray.Length; i++)
        {
            Entity entity = entityArray[i];

            var desc = new RenderMeshDescription(
                        planetMesh,
                        planets[i].material,
                        shadowCastingMode: ShadowCastingMode.Off,
                        receiveShadows: false);

            RenderMeshUtility.AddComponents(
          entity,
          entityManager,
          desc);
    
        entityManager.SetComponentData(entity, new Translation { Value = new float3(planets[i].distanceFromCenter,0,0) });
            entityManager.SetComponentData(entity, new TrajectoryData
            {
                orbitRadius = planets[i].distanceFromCenter,
                currentAngle = 0f,
                sunPos = new float3(transform.position.x, transform.position.y, transform.position.z),
                orbitSpeed = 40000 * simulationSpeed * Mathf.PI * planets[i].distanceFromCenter / (planets[i].orbitCompletionTimeInDays * 24 * 60 * 60),
                orbitAxisRatio=planets[i].orbitAxisRatio,
                generateTrail = generateTrail,
                destroyTrailAfter=destroyTrailAfter,
                orbitCompletionTimeInDays = planets[i].orbitCompletionTimeInDays,
                generateTrailAfterFrames=math.max(1,generateTrailAfterFrames),
                trailCounter= math.max(1, generateTrailAfterFrames)

            }) ;
        entityManager.SetComponentData(entity, new Scale { Value = planets[i].radius*5f });
   
        }
        entityArray.Dispose();
    }
    void GenerateRandomPlanets() 
    {
        List<Planet> planets  = new List<Planet>();
        int n = materials.Length;
        if (n <= 0) return;

        for (int i = 0; i < numberOfPlanets; i++) 
        {
            planets.Add(new Planet(materials[UnityEngine.Random.Range(0,n)]));
        }

      
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityArchetype entityArchetype = entityManager.CreateArchetype(
            typeof(Translation),
            typeof(LocalToWorld),
            typeof(Scale),
            typeof(TrajectoryData)
            );

        NativeArray<Entity> entityArray = new NativeArray<Entity>(numberOfPlanets, Allocator.Temp);
        entityManager.CreateEntity(entityArchetype, entityArray);

        for (int i = 0; i < entityArray.Length; i++)
        {
            Entity entity = entityArray[i];

            var desc = new RenderMeshDescription(
                        planetMesh,
                        planets[i].material,
                        shadowCastingMode: ShadowCastingMode.Off,
                        receiveShadows: false);

            RenderMeshUtility.AddComponents(
          entity,
          entityManager,
          desc);

            entityManager.SetComponentData(entity, new Translation { Value = new float3(planets[i].distanceFromCenter, 0, 0) });
            entityManager.SetComponentData(entity, new TrajectoryData
            {
                orbitRadius = planets[i].distanceFromCenter,
                currentAngle = 0f,
                sunPos = new float3(transform.position.x, transform.position.y, transform.position.z),
                orbitSpeed =40000* simulationSpeed * Mathf.PI * planets[i].distanceFromCenter / (planets[i].orbitCompletionTimeInDays * 24 * 60 * 60),
                orbitAxisRatio = UnityEngine.Random.Range(0.2f, 0.8f),
                generateTrail = generateTrail,
                destroyTrailAfter = destroyTrailAfter,
                orbitCompletionTimeInDays=planets[i].orbitCompletionTimeInDays,
                generateTrailAfterFrames = math.max(1, generateTrailAfterFrames),
                trailCounter = math.max(1, generateTrailAfterFrames)
            });
            entityManager.SetComponentData(entity, new Scale { Value = planets[i].radius * 5 });

      
        }
        entityArray.Dispose();

    }

}

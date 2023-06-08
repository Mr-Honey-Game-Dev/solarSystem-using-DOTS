using System;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PlanetData
{
    public string name;
    public string material;
    public float distanceFromCenter;
    public float radius;
    public int orbitCompletionTimeInDays;
    public float orbitAxisRatio;
    public bool generateTrail;
    public string trailMesh;
    public string trailMat;
}



public class PlanetDataList
{
    public List<PlanetData> planets;
}


[Serializable]
public class Planet 
{
    public string name;
    public Material material;
    
    public float distanceFromCenter=Constants.Ranges.defaultDistanceFromCenter;

    public float radius;
    public int orbitCompletionTimeInDays=365;
    public float orbitAxisRatio;

    public bool generateTrail;
    public Mesh trailMesh;
    public Material trailMat;


    public Planet(Material material) 
    {
        this.material = material;
        distanceFromCenter = UnityEngine.Random.Range(Constants.Ranges.minDistanceFromCenter, Constants.Ranges.maxDistanceFromCenter);
        radius= UnityEngine.Random.Range(Constants.Ranges.minRadius, Constants.Ranges.maxRadius);
        orbitCompletionTimeInDays = Math.Max(1,(int)(distanceFromCenter*UnityEngine.Random.Range(0.1f,1f))/2);
    }

    public Planet(string name, Material material, float distanceFromCenter, float radius, int orbitCompletionTimeInDays, float orbitAxisRatio, bool generateTrail, Mesh trailMesh, Material trailMat)
    {
        this.name = name;
        this.material = material;
        this.distanceFromCenter = distanceFromCenter;
        this.radius = radius;
        this.orbitCompletionTimeInDays = orbitCompletionTimeInDays;
        this.orbitAxisRatio = orbitAxisRatio;
        this.generateTrail = generateTrail;
        this.trailMesh = trailMesh;
        this.trailMat = trailMat;
    }

}

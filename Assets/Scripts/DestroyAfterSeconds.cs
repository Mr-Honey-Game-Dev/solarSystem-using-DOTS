using Unity.Entities;

public struct DestroyAfterSeconds : IComponentData
{
    public float timer;
}
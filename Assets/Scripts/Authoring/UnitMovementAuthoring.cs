using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class UnitMovementAuthoring : MonoBehaviour {
    public float MoveSpeed;
     public float RotationSpeed;

    public class Baker : Baker<UnitMovementAuthoring> {
        public override void Bake(UnitMovementAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new UnitMovement {
                MoveSpeed = authoring.MoveSpeed,
                RotationSpeed = authoring.RotationSpeed
            });
        }
    }
}

public struct UnitMovement : IComponentData {
    public float MoveSpeed;
    public float RotationSpeed;
    public float3 TargetPosition;
}
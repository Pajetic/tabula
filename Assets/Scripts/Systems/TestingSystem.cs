using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

partial struct TestingSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        // int unitCount = 0;
        // foreach (var (localTransform, unitMovement, physicsVelocity) in SystemAPI
        //              .Query<RefRW<LocalTransform>, RefRO<UnitMovement>, RefRW<PhysicsVelocity>>()) {
        //     unitCount++;
        // }
        // Debug.Log("total units " + unitCount);
    }
}

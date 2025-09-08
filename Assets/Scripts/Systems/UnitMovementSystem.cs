using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

partial struct UnitMovementSystem : ISystem {
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        foreach ((RefRW<LocalTransform> localTransform, RefRO<MoveSpeed> moveSpeed, RefRW<PhysicsVelocity> physicsVelocity) 
                 in SystemAPI.Query<RefRW<LocalTransform>, RefRO<MoveSpeed>, RefRW<PhysicsVelocity>>()) {

            float3 targetPosition = localTransform.ValueRO.Position + new float3(10, 0, 0);
            float3 moveDirection = math.normalize(targetPosition - localTransform.ValueRO.Position);
            
            localTransform.ValueRW.Rotation = quaternion.LookRotation(moveDirection, math.up());
            
            // Physics movement
            physicsVelocity.ValueRW.Linear = moveDirection * moveSpeed.ValueRO.Value;
            
            // Stop rotation on collisions
            physicsVelocity.ValueRW.Angular = float3.zero;
        } 
    }

}

using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

partial struct UnitMovementSystem : ISystem {
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        foreach ((RefRW<LocalTransform> localTransform, RefRO<UnitMovement> unitMovement, RefRW<PhysicsVelocity> physicsVelocity) 
                 in SystemAPI.Query<RefRW<LocalTransform>, RefRO<UnitMovement>, RefRW<PhysicsVelocity>>()) {

            float3 moveDirection = math.normalize(unitMovement.ValueRO.TargetPosition - localTransform.ValueRO.Position);
            
            localTransform.ValueRW.Rotation = 
                math.slerp(localTransform.ValueRO.Rotation,
                    quaternion.LookRotation(moveDirection, math.up()),
                    unitMovement.ValueRO.RotationSpeed * SystemAPI.Time.DeltaTime);
            
            // Physics movement
            physicsVelocity.ValueRW.Linear = moveDirection * unitMovement.ValueRO.MoveSpeed;
            
            // Stop rotation on collisions
            physicsVelocity.ValueRW.Angular = float3.zero;
        } 
    }

}

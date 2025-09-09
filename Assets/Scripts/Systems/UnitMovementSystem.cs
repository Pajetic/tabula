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

            float3 targetPosition = MouseWorldPosition.Instance.GetPosition();
            float3 moveDirection = math.normalize(targetPosition - localTransform.ValueRO.Position);

            float rotationFactor = 10f; // TODO remove hardcoded value
            localTransform.ValueRW.Rotation = 
                math.slerp(localTransform.ValueRO.Rotation,
                    quaternion.LookRotation(moveDirection, math.up()),
                    rotationFactor * SystemAPI.Time.DeltaTime);
            
            // Physics movement
            physicsVelocity.ValueRW.Linear = moveDirection * moveSpeed.ValueRO.Value;
            
            // Stop rotation on collisions
            physicsVelocity.ValueRW.Angular = float3.zero;
        } 
    }

}

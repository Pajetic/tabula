using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

partial struct UnitMovementSystem : ISystem {
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        new UnitMovementJob {
            deltaTime = SystemAPI.Time.DeltaTime,
        }.ScheduleParallel();
    }

    [BurstCompile]
    public partial struct UnitMovementJob : IJobEntity {
        public float deltaTime;
        public void Execute(RefRW<LocalTransform> localTransform, RefRO<UnitMovement> unitMovement, RefRW<PhysicsVelocity> physicsVelocity) {
            float destinationDistanceTolerance = 0.3f; // TODO refactor 
            
            float3 moveDirection = unitMovement.ValueRO.TargetPosition - localTransform.ValueRO.Position;

            // Stop movement when we are close enough
            if (math.lengthsq(moveDirection) < destinationDistanceTolerance) {
                physicsVelocity.ValueRW.Linear = float3.zero;
                physicsVelocity.ValueRW.Angular = float3.zero;
                return;
            }
            
            moveDirection = math.normalize(moveDirection);
            localTransform.ValueRW.Rotation = 
                math.slerp(localTransform.ValueRO.Rotation,
                    quaternion.LookRotation(moveDirection, math.up()),
                    unitMovement.ValueRO.RotationSpeed * deltaTime);
            
            // Physics movement
            physicsVelocity.ValueRW.Linear = moveDirection * unitMovement.ValueRO.MoveSpeed;
            
            // Stop rotation on collisions
            physicsVelocity.ValueRW.Angular = float3.zero;
        }
    }
}

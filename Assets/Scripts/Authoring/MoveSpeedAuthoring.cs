using UnityEngine;
using Unity.Entities;

public class MoveSpeedAuthoring : MonoBehaviour {
    public float Value;

    public class Baker : Baker<MoveSpeedAuthoring> {
        public override void Bake(MoveSpeedAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new MoveSpeed {
                Value = authoring.Value
            });
        }
    }
}

public struct MoveSpeed : IComponentData{
    public float Value;
}
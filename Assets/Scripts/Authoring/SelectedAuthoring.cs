using Unity.Entities;
using UnityEngine;

public class SelectedAuthoring : MonoBehaviour {
    public GameObject visualGameObject;
    public class Baker : Baker<SelectedAuthoring>
    {
        public override void Bake(SelectedAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Selected {
                VisualEntity = GetEntity(authoring.visualGameObject, TransformUsageFlags.Dynamic),
            });
            SetComponentEnabled<Selected>(entity, false);
        }
    }
}

public struct Selected : IComponentData, IEnableableComponent {
    public Entity VisualEntity;
}

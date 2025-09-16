using Unity.Entities;
using UnityEngine;

public class SelectedAuthoring : MonoBehaviour {
    public GameObject visualGameObject;
    public float ShowScale;
    public class Baker : Baker<SelectedAuthoring>
    {
        public override void Bake(SelectedAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Selected {
                VisualEntity = GetEntity(authoring.visualGameObject, TransformUsageFlags.Dynamic),
                ShowScale = authoring.ShowScale,
            });
            SetComponentEnabled<Selected>(entity, false);
        }
    }
}

public struct Selected : IComponentData, IEnableableComponent {
    // Events
    public bool OnSelected;
    public bool OnDeselected;

    public Entity VisualEntity;
    public float ShowScale;
}

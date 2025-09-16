using Unity.Entities;
using UnityEngine;

public class UnitAuthoring : MonoBehaviour {
    public Faction Faction;
    public class Baker : Baker<UnitAuthoring> {
        public override void Bake(UnitAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Unit {
                Faction = authoring.Faction,
            });
        }
    }
}

public struct Unit : IComponentData {
    public Faction Faction;
}

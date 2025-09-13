using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

partial struct SelectedVisualSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        foreach (RefRO<Selected> selected in SystemAPI.Query<RefRO<Selected>>()) {
            SystemAPI.SetComponentEnabled<MaterialMeshInfo>(selected.ValueRO.VisualEntity, true);
        }
        
        foreach (RefRO<Selected> selected in SystemAPI.Query<RefRO<Selected>>().WithDisabled<Selected>()) {
            SystemAPI.SetComponentEnabled<MaterialMeshInfo>(selected.ValueRO.VisualEntity, false);
        }
    }
}

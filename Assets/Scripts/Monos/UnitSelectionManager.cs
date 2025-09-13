using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class UnitSelectionManager : MonoBehaviour {

    private void Update() {
        // TODO Refactor input system
        if (Input.GetMouseButtonDown(1)) {
            Vector3 mouseWorldPosition = MousePositionManager.Instance.GetPosition();

            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<UnitMovement, Selected>().Build(entityManager);
            NativeArray<UnitMovement> unitMovementArray = entityQuery.ToComponentDataArray<UnitMovement>(Allocator.Temp);
            
            for (int i = 0; i < unitMovementArray.Length; i++) {
                UnitMovement unitMovement = unitMovementArray[i];
                unitMovement.TargetPosition = mouseWorldPosition;
                unitMovementArray[i] = unitMovement;
            }
            
            entityQuery.CopyFromComponentDataArray(unitMovementArray);
            unitMovementArray.Dispose();
        }
    }
}

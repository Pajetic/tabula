using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public class UnitSelectionManager : MonoBehaviour {

    public static UnitSelectionManager Instance { get; private set; }
    
    public event EventHandler OnSelectionStart;
    public event EventHandler OnSelectionEnd;

    private Vector2 selectionStartMousePos;

    private void Awake() {
        Instance = this;
    }

    private void Update() {
        // TODO Refactor input system
        if (Input.GetMouseButtonDown(0)) {
            selectionStartMousePos = Input.mousePosition;
            OnSelectionStart?.Invoke(this, EventArgs.Empty);
        }
        
        if (Input.GetMouseButtonUp(0)) {
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            
            // Deselect all units
            EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected>().Build(entityManager);
            NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
            NativeArray<Selected> selectedArray = entityQuery.ToComponentDataArray<Selected>(Allocator.Temp);
            for (int i = 0; i < entityArray.Length; i++) {
                entityManager.SetComponentEnabled<Selected>(entityArray[i], false);
                Selected selected = selectedArray[i];
                selected.OnDeselected = true;
                selectedArray[i] = selected;
                entityManager.SetComponentData(entityArray[i], selected);   // cannot use CopyFromComponentData cuz component already disabled
            }
            
            // Select units in selection rect
            entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform, Unit>().WithPresent<Selected>().Build(entityManager);
            NativeArray<LocalTransform> localTransformArray = entityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
            entityArray = entityQuery.ToEntityArray(Allocator.Temp);

            Rect selectionArea = GetSelectionAreaRect();
            float multiSelectMinSize = 40f; // TODO const
            float selectionSize = selectionArea.height + selectionArea.width;
            bool isMultiSelect = selectionSize > multiSelectMinSize;
            
            // Multi select with position check for larger boxes
            if (isMultiSelect) {
                for (int i = 0; i < localTransformArray.Length; i++) {
                    LocalTransform localTransform = localTransformArray[i];
                    Vector2 screenPosition = Camera.main.WorldToScreenPoint(localTransform.Position);
                    if (selectionArea.Contains(screenPosition)) {
                        entityManager.SetComponentEnabled<Selected>(entityArray[i], true);
                        Selected selected = entityManager.GetComponentData<Selected>(entityArray[i]);
                        selected.OnSelected = true;
                        entityManager.SetComponentData(entityArray[i], selected);
                    }
                }
            } else {    // Single selection with physics for clicks/small boxes
                entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
                PhysicsWorldSingleton physicsWorldSingleton = entityQuery.GetSingleton<PhysicsWorldSingleton>();
                CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
                UnityEngine.Ray cameraMouseRay = Camera.main.ScreenPointToRay(Input.mousePosition); // TODO refactor input
                int unitsLayer = 6; // Units layer
                RaycastInput raycastInput = new RaycastInput {
                    Start = cameraMouseRay.GetPoint(0),
                    End = cameraMouseRay.GetPoint(9999f),   // TODO const - just a large enough number to collide with ground plane
                    Filter = new CollisionFilter {
                        BelongsTo = ~0u,    // Every layer
                        CollidesWith = 1u << unitsLayer,
                        GroupIndex = 0,
                    }
                };
                
                if (collisionWorld.CastRay(raycastInput, out Unity.Physics.RaycastHit raycastHit)
                    && entityManager.HasComponent<Unit>(raycastHit.Entity)) {
                    entityManager.SetComponentEnabled<Selected>(raycastHit.Entity, true);
                    Selected selected = entityManager.GetComponentData<Selected>(raycastHit.Entity);
                    selected.OnSelected = true;
                    entityManager.SetComponentData(raycastHit.Entity, selected);
                }
            }

            OnSelectionEnd?.Invoke(this, EventArgs.Empty);
            localTransformArray.Dispose();
            entityArray.Dispose();
        }
        
        if (Input.GetMouseButtonDown(1)) {
            Vector3 mouseWorldPosition = MousePositionManager.Instance.GetPosition();

            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<UnitMovement, Selected>().Build(entityManager);
            NativeArray<UnitMovement> unitMovementArray = entityQuery.ToComponentDataArray<UnitMovement>(Allocator.Temp);
            NativeArray<float3> movePositionArray = GenerateMovePositionArray(mouseWorldPosition, unitMovementArray.Length);
            for (int i = 0; i < unitMovementArray.Length; i++) {
                UnitMovement unitMovement = unitMovementArray[i];
                unitMovement.TargetPosition = movePositionArray[i];
                unitMovementArray[i] = unitMovement;
            }
            
            entityQuery.CopyFromComponentDataArray(unitMovementArray);
            unitMovementArray.Dispose();
        }
    }

    // Get current mouse selection area
    public Rect GetSelectionAreaRect() {
        Vector2 currentMousePos = Input.mousePosition;
        Vector2 lowerLeftCorner = new Vector2(
            Mathf.Min(selectionStartMousePos.x, currentMousePos.x),
            MathF.Min(selectionStartMousePos.y, currentMousePos.y));
        Vector2 upperRightCorner = new Vector2(
            Mathf.Max(selectionStartMousePos.x, currentMousePos.x),
            MathF.Max(selectionStartMousePos.y, currentMousePos.y));

        return new Rect(
            lowerLeftCorner.x,
            lowerLeftCorner.y,
            upperRightCorner.x - lowerLeftCorner.x,
            upperRightCorner.y - lowerLeftCorner.y);
    }

    // Generate ring formation
    private NativeArray<float3> GenerateMovePositionArray(float3 targetPosition, int positionCount) {
        NativeArray<float3> positionArray = new NativeArray<float3>(positionCount, Allocator.Temp);

        if (positionCount == 0) {
            return positionArray;
        }
        
        positionArray[0] = targetPosition;
        if (positionCount == 1) {
            return positionArray;
        }

        float ringSize = 2.2f;
        int ring = 0;
        int positionIndex = 1;

        while (positionIndex < positionCount) {
            int ringPositionCount = 3 + ring * 2;

            for (int i = 0; i < ringPositionCount; i++) {
                float angle = i * (math.PI2 / ringPositionCount);
                float3 ringVector = math.rotate(quaternion.RotateY(angle), new float3(ringSize * (ring + 1), 0, 0)); // Position vector relative to center
                float3 ringPosition = targetPosition + ringVector;
                
                positionArray[positionIndex] = ringPosition;
                positionIndex++;

                if (positionIndex >= positionCount) {
                    break;
                }
            }
            ring++;
        }
        
        return positionArray;
    }
}

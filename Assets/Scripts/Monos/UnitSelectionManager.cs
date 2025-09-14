using System;
using Unity.Collections;
using Unity.Entities;
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
            EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform, Unit>().WithAll<Selected>().Build(entityManager);
            entityManager.SetComponentEnabled<Selected>(entityQuery, false);
            
            // Select units in selection rect
            entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform, Unit>().WithPresent<Selected>().Build(entityManager);
            NativeArray<LocalTransform> localTransformArray = entityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
            NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);

            Rect selectionArea = GetSelectionAreaRect();
            
            for (int i = 0; i < localTransformArray.Length; i++) {
                LocalTransform localTransform = localTransformArray[i];
                Vector2 screenPosition = Camera.main.WorldToScreenPoint(localTransform.Position);
                if (selectionArea.Contains(screenPosition)) {
                    entityManager.SetComponentEnabled<Selected>(entityArray[i], true);
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
            
            for (int i = 0; i < unitMovementArray.Length; i++) {
                UnitMovement unitMovement = unitMovementArray[i];
                unitMovement.TargetPosition = mouseWorldPosition;
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
}

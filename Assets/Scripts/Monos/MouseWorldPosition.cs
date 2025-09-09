using UnityEngine;

public class MouseWorldPosition : MonoBehaviour {
    
    public static MouseWorldPosition Instance { get; private set; }

    private void Awake() {
        Instance = this;
    }

    public Vector3 GetPosition() {
        // TODO Refator this later
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero);

        if (plane.Raycast(ray, out float enter)) {
            return ray.GetPoint(enter);
        }
        
        return Vector3.zero;
    }
}

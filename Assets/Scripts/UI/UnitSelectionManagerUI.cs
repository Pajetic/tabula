using UnityEngine;

public class UnitSelectionManagerUI : MonoBehaviour {
    [SerializeField] private RectTransform selectionAreaRectTransform;
    [SerializeField] private Canvas canvas;
    
    private void Start() {
        UnitSelectionManager.Instance.OnSelectionStart += OnSelectionStart;
        UnitSelectionManager.Instance.OnSelectionEnd += OnSelectionEnd;
        
        selectionAreaRectTransform.gameObject.SetActive(false);
    }

    private void Update() {
        if (selectionAreaRectTransform.gameObject.activeSelf) {
            UpdateSelectionAreaRect();
        }
    }

    private void OnSelectionStart(object sender, System.EventArgs e) {
        selectionAreaRectTransform.gameObject.SetActive(true);
        UpdateSelectionAreaRect();
    }
    
    private void OnSelectionEnd(object sender, System.EventArgs e) {
        selectionAreaRectTransform.gameObject.SetActive(false);
    }

    private void UpdateSelectionAreaRect() {
        Rect selectionAreaRect = UnitSelectionManager.Instance.GetSelectionAreaRect();
        float canvasScale = canvas.transform.localScale.x;
        selectionAreaRectTransform.anchoredPosition = new Vector2(selectionAreaRect.x, selectionAreaRect.y) / canvasScale;
        selectionAreaRectTransform.sizeDelta = new Vector2(selectionAreaRect.width, selectionAreaRect.height) / canvasScale;
    }
}

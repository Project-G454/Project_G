using UnityEngine;

public class ClickController : MonoBehaviour {
    public PlayerController playerController;
    public GridManager gridManager;
    
    private Camera mainCamera;

    void Start() {
        mainCamera = Camera.main;
    }
    
    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 clickPosition = new Vector2(Mathf.RoundToInt(mouseWorldPos.x), Mathf.RoundToInt(mouseWorldPos.y));
            
            Tile selectedTile = gridManager.GetTileAtPosition(clickPosition);
            
            if (selectedTile != null) {
                ClearAllHighlights();
                
                selectedTile.SetHighlight(true);
                
                playerController.MoveToTile(selectedTile);
            }
        }
    }
    
    private void ClearAllHighlights() {
        for (int x = 0; x < gridManager.width; x++) {
            for (int y = 0; y < gridManager.height; y++) {
                Tile tile = gridManager.GetTileAtPosition(new Vector2(x, y));
                if (tile != null) {
                    tile.SetHighlight(false);
                }
            }
        }
    }
}
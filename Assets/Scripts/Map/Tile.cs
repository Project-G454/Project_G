using UnityEngine;

public class Tile : MonoBehaviour {
    [SerializeField] private new SpriteRenderer renderer;
    [SerializeField] private GameObject highlight;

    public bool Walkable { get; private set; } = true;

    private bool forceHighlight = false;

    public Vector2 GridPosition { get; private set; }

    public void Init(Vector2 gridPosition) {
        this.GridPosition = gridPosition;
    }

    public void SetWalkable(bool isWalkable) {
        Walkable = isWalkable;
    }

    void OnMouseEnter() {
        if ( !forceHighlight )
            highlight.SetActive(true);    
    }

    void OnMouseExit() {
        if( !forceHighlight )
            highlight.SetActive(false);   
    }

    public void SetHighlight(bool active) {
        if (highlight != null && Walkable == true){
            forceHighlight = active;
            highlight.SetActive(active);
        }
    }
}

using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour
{

    public SpriteRenderer tile;
    public SpriteRenderer marking;

    private CombatMap map;
    private Point position;

    private void Start()
    {
        position = new Point(transform.localPosition);
        HideMarking();
    }

    public void SetMap(CombatMap map)
    {
        this.map = map;
    }

    public void SetTile(Sprite sprite)
    {
        tile.sprite = sprite;
    }

    public void SetMarking(Sprite sprite)
    {
        marking.sprite = sprite;
        marking.gameObject.SetActive(true);
    }

    public void HideMarking()
    {
        marking.gameObject.SetActive(false);
    }

    public void OnMouseUp()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            map.TileClicked(position);
        }
    }

    public void OnMouseEnter()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            map.TileMouseEnter(position);
        }
    }

    public void OnMouseExit()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            map.TileMouseExit(position);
        }
    }
}

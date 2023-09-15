using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

[ExecuteAlways]
public class TilemapGridPreoperties : MonoBehaviour
{
    //只在unity编辑器中才编译，与独立构建体分离
#if UNITY_EDITOR
    private Tilemap tilemap;
    [SerializeField] private SO_GridProperties gridProperties = null;
    [SerializeField] private GridBoolProperty gridBoolProperty = GridBoolProperty.diggable;

    private void OnEnable()
    {
        if (!Application.IsPlaying(gameObject))
        {
            tilemap = GetComponent<Tilemap>();
            if(gridProperties != null)
            {
                gridProperties.gridPropertyList.Clear();
            }
        }
    }

    private void OnDisable()
    {
        if (!Application.IsPlaying(gameObject))
        {
            UpdateGridProperties();
            if(gridProperties != null)
            {
                //将目标对象标记为“脏”（仅适用于非场景对象）
                EditorUtility.SetDirty(gridProperties);
            }
        }
    }

    private void UpdateGridProperties()
    {
        //压缩边界，让边界重新回到有绘制的tiles上
        tilemap.CompressBounds();

        if (!Application.IsPlaying(gameObject))
        {
            if (gridProperties != null)
            {
                Vector3Int startCell = tilemap.cellBounds.min;
                Vector3Int endCell = tilemap.cellBounds.max;

                for (int x = startCell.x; x < endCell.x; x++)
                {
                    for(int y = startCell.y; y < endCell.y; y++)
                    {

                        TileBase tile = tilemap.GetTile(new Vector3Int(x, y, 0));
                        if(tile != null)
                        {
                            gridProperties.gridPropertyList.Add(new GridProperty(new GridCoordinate(x, y), gridBoolProperty, true));
                        }
                    }
                }
            }
        }
    }

    private void Update()
    {
        if(!Application.IsPlaying(gameObject))
        {
            Debug.Log("禁止Tilemap属性");
        }
    }
#endif
}

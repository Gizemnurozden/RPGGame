using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NoDiagonalTile : Tile
{

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        Astar.NoDiagonalTiles.Add(position);

        return base.StartUp(position, tilemap, go);
    }


    [MenuItem("Assets/Create/Tiles/NoDiagonalTile")]
    public static void CreateWaterTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save NoDiagonalTile", "NoDiagonalTile", "asset", " Save NoDiagonalTile", "Assets");
        if (path == "")
        {
            return;
        }
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<NoDiagonalTile>(), path);
    }
}

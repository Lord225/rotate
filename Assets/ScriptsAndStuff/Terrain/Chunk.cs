using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public static int ChunkSize = 16;
    public static Dictionary<Vector2Int, Tile> toReload = new Dictionary<Vector2Int, Tile>(50);

    public Tile[,] Tiles;
    public Mesh mesh;
    public Vector2Int position;
    public bool isReady { get; private set; }
    public bool isFresh {get; set; }

    //TODO IMPORTANT *CREATE RELOADING HANDELER (with reassinging placable objects, ect)
    //GameplayData that include bool value DidPlayerInterfered to make sure that 

    public static Mesh[] meshes; //meshes pointer

    public Chunk(Vector2Int pos)
    {
        isReady = false;
        isFresh = true;
        position = pos;
        GenerateTiles();
    }

    public void OnDestroy()
    {
        if (!isFresh)
        {
            for (int i = 0; i < Tiles.GetLength(0); i++)
            {
                for (int j = 0; j < Tiles.GetLength(1); j++)
                {
                    if (Tiles[i, j].PlacedObject != null)
                    {
                        Tiles[i, j].PlacedObject.Hide();
                        toReload.Add(Tiles[i, j].PlacedObject.position, Tiles[i, j]);
                    }
                }
            }
        }
    }
    public Vector3 GetChunkGlobalPositon()
    {
        return new Vector3(position.x * ChunkSize, 0, position.y * ChunkSize);
    }
    public Tile GetTileOnGlobalPosition(Vector3 pos)
    {
        Vector2Int x = GlobalToChunkLocalPosition(pos);
        
        return Tiles[x.x,x.y];
    }
    public Vector2Int GlobalToChunkLocalPosition(Vector2 pos)
    {

        Vector2Int posRound = new Vector2Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));
        
        return new Vector2Int(mod(posRound.x,ChunkSize), mod(posRound.y, ChunkSize));
        
    }
    public void GenerateTiles()
    {
        Tiles = new Tile[ChunkSize, ChunkSize];

        var pos = new Vector2Int();
        for (int i = 0; i < Tiles.GetLength(0); i++)
        {
            for (int j = 0; j < Tiles.GetLength(1); j++)
            {
                pos.x = i + position.x * ChunkSize;
                pos.y = j + position.y * ChunkSize;
                if (toReload.TryGetValue(pos, out Tile tile))
                {
                    Tiles[i, j] = tile;
                    Tiles[i, j].PlacedObject.Show();
                    toReload.Remove(pos);
                    isFresh = false;
                }
                else
                {
                    Tiles[i, j] = new Tile(Terrain.GetBlockOnCordinates(pos.x, pos.y));      
                }
            }
        }
        
        //Debug.Log("Created Chunk");
    }

    public IEnumerator BuildMesh()
    {

        MeshBuilder builder = new MeshBuilder(position, Tiles);

        yield return builder.build((x) => mesh = x);

        isReady = true;

        //Debug.Log("Builded Chunk");
    }
    int mod(int x, int m)
    {
        int r = x % m;
        return r < 0 ? r + m : r;
    }



}

using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;

public class DebugVec
{
    public static void Log(Vector3 vec)
    {
        Debug.Log("["+vec.x + " " + vec.y + " " + vec.z+"]");
    }
    public static void Log(Vector2 vec)
    {
        Debug.Log("[" + vec.x + " " + vec.y +"]");
    }
}

public class MeshBuilder
{
    private Vector2Int pos;
    private Tile[,] tiles;
    private Mesh mesh;


    public MeshBuilder(Vector2Int _pos, Tile[,] _tiles)
    {
        pos = _pos;
        tiles = _tiles;
    }

    public IEnumerator build(Action<Mesh> mesh)
    {
        if (this.mesh == null)
        {
            this.mesh = new Mesh();
        }
        else
        {
            this.mesh.Clear();
        }
        CombineInstance[] combine = new CombineInstance[tiles.Length];

        int k = 0;
        Matrix4x4 movX = Matrix4x4.Translate(new Vector3(1, 0, 0));
        Matrix4x4 movYAndResetX = Matrix4x4.Translate(new Vector3(0, 1, 0))* Matrix4x4.Translate(new Vector3(-Chunk.ChunkSize, 0, 0));
        Matrix4x4 combined = Matrix4x4.Rotate(Quaternion.Euler(90,0,0))*Matrix4x4.Scale(new Vector3(1,1,-1));

        for (int i = 0; i < tiles.GetLength(0); i++)
        {
            for (int j = 0; j < tiles.GetLength(1); j++)
            {
                combine[k].mesh = Chunk.meshes[(int)tiles[j,i].type];
                combine[k].transform = combined;

                combined *= movX;
                k++;
            }
            combined *= movYAndResetX;
            yield return null;
        }
  
        this.mesh.CombineMeshes(combine,true,true,false);
        mesh(this.mesh);
    }
    
    
}

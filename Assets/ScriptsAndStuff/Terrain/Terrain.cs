using System.Collections;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;

public enum block
{
    NormalTerrain1,
    NormalTerrain2,
    NormalTerrain3,
    Sand,
    Sand2,
    Tree1,
    Tree2,
    TreeSpruce1,
    TreeSpruce2,
    TreeSpruce3,
    Water,
    MountainHigh1,
    MountainHigh2,
    MountainHigh3,
    MountainHigh4,
    MountainHigh5,
    MountainHigh7,
    MountainLow1,
    MountainLow2,
    MountainLow3,
    MountainLow4,
    MountainMed1,
    MountainMed2,
    MountainMed3,
    MountainMed4,
    MountainMed5,
    MountainLowTree1,
    MountainLowTree2,
    MountainLowTree3,
    MountainLowWater1,
    MountainLowWater2,
    MountainLowWater3
}

struct TerrainSettings
{
    public static float TreeScale = 30f;
    public static float TreeSize = 0.6f;

    public static float LakesScale = 80f;
    public static float LakesTreshold = 0.27f;
    public static float SandTreshold = 0.07f;
    public static float SandSubScale= 25f;

    public static float MountainsScale = 120.0f;
    public static float MountainsLowTresh = 0.68f;
    public static float MountainsMedTresh = 0.73f;
    public static float MountainsHighTresh = 0.82f;
    public static float MountainsScale2 = 3f;
}

public class Terrain : MonoBehaviour
{
    public Dictionary<Vector2Int,Chunk> chunks;
    //PUBLIC DURING DEBUG ^
    private Mesh[] meshes2;
    private Vector2 generatorPosition;

    public Material material;
    public Mesh[] meshes;
    public int radius = 4;
    public const int blockcount = 32;
    public float VanishDistance;

    public static Vector2 NoiseOffset;
    public static Vector2 MountainOffset;
    public static Vector2 MountainOffset2;
    public static Vector2 LakesOffset;
    public static Vector2 TreesOffset;
    
    static public readonly int SEED = 844;

    static Texture2D debug;

    

    private void Awake()
    {

        debug = new Texture2D(96, 96);
        meshes2 = new Mesh[blockcount];
        
        for (int i = 0; i < meshes2.Length; i++)
        {
            foreach (var mesh in meshes)
            {
                if(mesh.name == ((block)i).ToString())
                {
                    meshes2[i] = mesh;
                }
            }
        }
        Chunk.meshes = meshes2;
    }
    public Tile GetTileOnCordinates(Vector2 pos)
    {
        Chunk chunk;
        if(chunks == null)
        {
            //Try handle this error;
            return null;
        }
        if (chunks.TryGetValue(WorldToChunkCordinate(pos), out chunk))
        {
            return chunk.GetTileOnGlobalPosition(pos);
        }
        else
        {
            Debug.Log("Chunk Nie istnieje!");
            return null;
        }
    }

    public Chunk GetChunkOnCordinates(Vector2 pos)
    {
        Chunk chunk;
        if(chunks.TryGetValue(WorldToChunkCordinate(pos), out chunk))
        {
            return chunk;
        }
        else
        {
            Debug.Log("Chunk Nie istnieje!");
            return null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        RndSetup();
        
        chunks = new Dictionary<Vector2Int, Chunk>();

        StartCoroutine(LowFreqFunction());

        debug.Apply();
    }

    //Updating chunks
    IEnumerator LowFreqFunction()
    {
        while (true)
        {
            GenerateChunksAround();
            RemoveChunksThatAreFarAway();
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void SetPosition(Vector3 pos)
    {
        generatorPosition.x = pos.x;
        generatorPosition.y = pos.z;
    }
    static void RndSetup()
    {
        Random.InitState(SEED);
        NoiseOffset.x = Random.Range(0, 100000f);
        NoiseOffset.y = Random.Range(0, 100000f);
        MountainOffset.x = Random.Range(0, 100000f);
        MountainOffset.y = Random.Range(0, 100000f);
        MountainOffset2.x = Random.Range(0, 100000f);
        MountainOffset2.y = Random.Range(0, 100000f);
        LakesOffset.x = Random.Range(0, 100000f);
        LakesOffset.y = Random.Range(0, 100000f);
        TreesOffset.x = Random.Range(0, 100000f);
        TreesOffset.y = Random.Range(0, 100000f);
    }

    List<Vector2Int> ToRemove = new List<Vector2Int>();

    void RemoveChunksThatAreFarAway()
    {
        ToRemove.Clear();
        foreach (var chunk in chunks.Keys)
        {
            var chunkcords = WorldToChunkCordinate(generatorPosition);
            if (Vector2Int.Distance(chunk, chunkcords) > VanishDistance)
            {
                ToRemove.Add(chunk);
            }
        }
        foreach (var item in ToRemove)
        {
            chunks[item].OnDestroy();
            chunks.Remove(item);
        }
    }


    void GenerateChunksAround()
    {
        for (int x = -radius; x < radius; x++)
        {
            for (int y = -radius; y < radius; y++)
            {
                var chunkcords = WorldToChunkCordinate(generatorPosition, x, y);
                if (!chunks.ContainsKey(chunkcords)) {
                    Chunk chunk = new Chunk(chunkcords);
                    StartCoroutine(chunk.BuildMesh());
                    chunks.Add(chunkcords, chunk);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach(var chunk in chunks.Values)
        {
            if (chunk.isReady)
            {
                Graphics.DrawMesh(chunk.mesh, chunk.GetChunkGlobalPositon(), Quaternion.identity, material,0, Camera.current,0, new MaterialPropertyBlock(),true,true,true);
            }
        }
    }


    public Vector2 get2Dposition()
    {
        return new Vector2(transform.position.x, transform.position.z);
    }
    public static Vector2Int WorldToChunkCordinate(Vector2 world)
    {
        return new Vector2Int(Mathf.FloorToInt(world.x / Chunk.ChunkSize), Mathf.FloorToInt(world.y / Chunk.ChunkSize));
    }
    public static Vector2Int WorldToChunkCordinate(Vector2 worldChunk,int ChunkXOffset,int ChunkYOffset)
    {
        return new Vector2Int(Mathf.FloorToInt(worldChunk.x / Chunk.ChunkSize) + ChunkXOffset, Mathf.FloorToInt(worldChunk.y / Chunk.ChunkSize) + ChunkYOffset);
    }
    public static Vector3 ChunkToWorld(Vector2 chunk)
    {
        return new Vector3(chunk.x*Chunk.ChunkSize,0,chunk.x*Chunk.ChunkSize);
    }
    public static block GetBlockOnCordinates(int x, int y)
    {
        float noise = PerlinNoise(x, y, TerrainSettings.TreeScale,TreesOffset);
        float noise2 = PerlinNoise(x, y, TerrainSettings.LakesScale,LakesOffset);
        float noise3 = PerlinNoise(x, y, TerrainSettings.MountainsScale,MountainOffset);
        noise3 += PerlinNoise(x, y, TerrainSettings.MountainsScale2, MountainOffset)/8.0f;
        noise3 = Mathf.Pow(noise3, 1.5f);


        debug.SetPixel((int)(x)+48, (int)(y)+48, new Color(noise, noise, noise));
        if (noise3 < TerrainSettings.MountainsLowTresh)
        {
            if (noise2 > TerrainSettings.LakesTreshold)
            {
                //Land
                if (noise < TerrainSettings.TreeSize)
                {
                    return GetTerrainBlock();
                }
                else
                {
                    return GetTreeBlock();
                }
            }
            else
            {
                //Lakes
                if (noise2 > TerrainSettings.LakesTreshold - TerrainSettings.SandTreshold)
                {
                    return GetSandBlock();
                }
                else
                {
                    return block.Water;
                }
            }
        }
        else
        {
             if (noise3 > TerrainSettings.MountainsMedTresh)
             {
                 if (noise3 < TerrainSettings.MountainsHighTresh)
                 {
                     return GetMid();
                 }
                 else
                 {
                     return GetHigh();
                 }
             }
             else
             {
                if (noise2 < TerrainSettings.LakesTreshold && noise2 < TerrainSettings.LakesTreshold - TerrainSettings.SandTreshold)
                {
                    return GetWaterLow();
                }

                if (noise < TerrainSettings.TreeSize) {
                    return GetLow();
                }
                else
                {
                    return GetTreeLowBlock();
                }
             }
            //return block.NormalTerrain1;
        }
    }

    private static float PerlinNoise(float x,float y,float scale, Vector2 rnd_offset)
    {
        return Mathf.PerlinNoise(10000.0f + x / scale + NoiseOffset.x+ rnd_offset.x, 10000.0f + y / scale + NoiseOffset.y+ rnd_offset.x);
    }
    private static block GetTerrainBlock()
    {
        return (block)Random.Range(0, 3);
    }
    private static block GetHigh()
    {
        return (block)Random.Range(11, 17);
    }
    private static block GetMid()
    {
        return (block)Random.Range(21, 26);
    }
    private static block GetWaterLow()
    {
        return (block)Random.Range(29, 31);
    }
    private static block GetLow()
    {
        return (block)Random.Range(17, 22);
    }
    private static block GetSandBlock()
    {
        return (block)Random.Range(3, 4);
    }
    private static block GetTreeBlock()
    {
        return (block)Random.Range(5, 7);
    }
    private static block GetTreeLowBlock()
    {
        return (block)Random.Range(26,29);
    }
    public static bool IsTerrainBlock(block target)
    {
        switch (target)
        {
            case block.NormalTerrain1:
            case block.NormalTerrain2:
            case block.NormalTerrain3:
            case block.Sand:
            case block.Sand2:
                return true;
        }
        return false;
    }
    public static bool IsMountainBlock(block target)
    {
        switch (target)
        {
            case block.MountainHigh1:
            case block.MountainHigh2:
            case block.MountainHigh3:
            case block.MountainHigh4:
            case block.MountainHigh5:
            case block.MountainHigh7:
            case block.MountainLow1:
            case block.MountainLow2:
            case block.MountainLow3:
            case block.MountainLow4:
            case block.MountainLowTree1:
            case block.MountainLowTree2:
            case block.MountainLowTree3:
            case block.MountainLowWater1:
            case block.MountainLowWater2:
            case block.MountainLowWater3:
                return true;
        }
        return false;
    }
    public static bool IsTreeBlock(block target)
    {
        switch (target)
        {
            case block.Tree1:
            case block.Tree2:
            case block.TreeSpruce1:
            case block.TreeSpruce2:
            case block.TreeSpruce3:
                return true;
        }
        return false;
    }
    public static bool CanPlaceOnThisBlock(block target)
    {
        if (IsTerrainBlock(target)) //TODO: add sand, ores ect;
        {
            return true;
        }
        else
        {
            
        }

        return false;
    }
    public Tile[] get_neighbours(Vector2 pos)
    {
        Tile[] neighbours = new Tile[4];

        //Tile[] neighbours organization
        //******************************
        // x-1 ,y   left
        // x   ,y-1 down
        // x+1 ,y   right
        // x   ,y+1 up

        for (int i = 0; i < 4; i++)
        {
            neighbours[i] = GetTileOnCordinates(new Vector2(pos.x + ((i - 1) % 2), pos.y + ((i - 2) % 2)));
        }
        return neighbours;
    }
    private void OnGUI()
    {
        //GUI.DrawTexture(Rect.MinMaxRect(100, 100, 600, 600), debug);
    }
}

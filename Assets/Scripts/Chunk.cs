using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Threading;
using TreeEditor;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;
using static UnityEngine.Rendering.VolumeComponent;

public class Chunk
{
    World world;
    public int X;
    public int Z;
    public bool isActive;


    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();
    int vertexIndex = 0;

    public GameObject chunkObject;
    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    public Vector3 position;

    public bool isVoxelMapPopulated = false;
    public bool threadLocked = false;
    public Queue<VoxelMod> modifications = new Queue<VoxelMod>();

    public ChunkData chunkData;

    public Chunk(int _x,int _z, World _world, bool generateOnLoad)
    {
        X = _x;
        Z =_z;
        isActive = generateOnLoad;
        world = _world;
    }

    public void Init()
    {

        chunkObject = new GameObject();
        meshFilter = chunkObject.AddComponent<MeshFilter>();
        meshRenderer = chunkObject.AddComponent<MeshRenderer>();

        meshRenderer.material = World.Instance.material;
        chunkObject.transform.SetParent(world.transform);
        chunkObject.transform.position = new Vector3(X * VoxelData.ChunkWidth * VoxelData.BlockSize, 0f, Z * VoxelData.ChunkWidth * VoxelData.BlockSize);
        chunkObject.name = "Chunk " + X + ", " + Z;
        position = chunkObject.transform.position;

        chunkData = world.worldData.RequestChunk(new Vector2Int(X, Z), true);
        Debug.Log(chunkData.ChunkID);

        lock (World.Instance.ChunkUpdateThreadLock)
            World.Instance.chunksToUpdate.Add(this);
        Debug.Log(World.Instance.chunksToUpdate.Count);


    }
    
    public void ClearMeshData()
    {

        vertexIndex = 0;
        vertices.Clear();
        triangles.Clear();
        uvs.Clear();

    }

    public void CreateMesh()
    {

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }
    public void UpdateChunk()
    {
        ClearMeshData();

        while (modifications.Count > 0)
        {

            VoxelMod v = modifications.Dequeue();
            Vector3Int index = v.index -= new Vector3Int(X * VoxelData.ChunkWidth , 0, Z * VoxelData.ChunkWidth);
            int id = GetBlockIntID(index);
            chunkData.BlockList[id].SetBlockType(v.id);

        }


        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {
                    Vector3Int index = new Vector3Int(x, y, z);
                    int blockID = GetBlockIntID(index);
                    if (world.blocktype.BlockTypes[chunkData.BlockList[blockID].GetBlockType()].isSolid)

                        UpdateBlockFace(index,blockID);

                }
            }
        }
        world.chunksToDraw.Enqueue(this);
    }

    public void EditVoxel(Vector3 pos, byte newType)
    {
        Vector3Int worldindex = World.GetWorldIndexFromPos(pos);
        Vector2Int chunkindex = World.GetChunkIndexFromPos(pos);
        Vector3Int index = new Vector3Int(worldindex.x - (chunkindex.x * VoxelData.ChunkWidth), worldindex.y, worldindex.z - (chunkindex.y * VoxelData.ChunkWidth));
        int ID =GetBlockIntID(index);

        chunkData.BlockList[ID].SetBlockType(newType);

        world.worldData.AddToModifiedChunkList(chunkData);


        world.chunksToUpdate.Insert(0, this);
        UpdateSurroundingChunk(ID);

    }

    void UpdateSurroundingChunk(int ID)
    {

        for (int p = 0; p < 6; p++)
        {

            int neighborID = ID + VoxelData.faceChecks[p];

            if (!IsNeighborInChunk(neighborID,p))
            {
                Debug.Log("on board!"+p);
                if (p == 2 && !((chunkData.Z -1)<0)) 
                {
                    Debug.Log(X + " " + Z);
                    World.Instance.chunksToUpdate.Insert(0, world.Chunks[chunkData.X, chunkData.Z - 1]);
                }
                    
                else if (p == 3 && !((chunkData.Z + 1) >= VoxelData.WorldChunksSize)) 
                {
                    Debug.Log(X + " " + Z);
                    World.Instance.chunksToUpdate.Insert(0, world.Chunks[chunkData.X, chunkData.Z + 1]);
                }
                    
                else if (p == 4 && !((chunkData.X - 1) < 0)) 
                {
                    Debug.Log(X + " " + Z);
                    World.Instance.chunksToUpdate.Insert(0, world.Chunks[chunkData.X - 1, chunkData.Z]);
                }
                    
                else if (p == 5 && !((chunkData.X + 1 )>= VoxelData.WorldChunksSize))
                {
                    Debug.Log(X + " " + Z);
                    World.Instance.chunksToUpdate.Insert(0, world.Chunks[chunkData.X + 1, chunkData.Z]);
                }
                    
                else
                {
                    return;
                }
            }

        }

    }

    public void UpdateBlockFace(Vector3Int Chunkindex, int blockID)
    {
        Vector3 Pos = new Vector3(Chunkindex.x, Chunkindex.y, Chunkindex.z)*VoxelData.BlockSize;
        Block block = chunkData.BlockList[blockID];


        for (int p = 0; p < 6; p++)
        {
            int neighID = blockID + VoxelData.faceChecks[p];

            if (!world.blocktype.BlockTypes[block.GetBlockType()].isTransparent&& block.GetBlockType()!=0)
            {
                if (IsCoordTransparent(neighID, p))
                {


                vertices.Add(Pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 0]]);
                vertices.Add(Pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 1]]);
                vertices.Add(Pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 2]]);
                vertices.Add(Pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 3]]);

                AddTexture(world.blocktype.BlockTypes[block.GetBlockType()].GetTextureID(p));

                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 3);
                vertexIndex += 4;

                }
            }
            else 
            {
                if (!(IsCoordTransparent(neighID, p)&& IsCoordSame(blockID, neighID, p)))
                {


                    vertices.Add(Pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 0]]);
                    vertices.Add(Pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 1]]);
                    vertices.Add(Pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 2]]);
                    vertices.Add(Pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 3]]);

                    AddTexture(world.blocktype.BlockTypes[block.GetBlockType()].GetTextureID(p));

                    triangles.Add(vertexIndex);
                    triangles.Add(vertexIndex + 1);
                    triangles.Add(vertexIndex + 2);
                    triangles.Add(vertexIndex + 2);
                    triangles.Add(vertexIndex + 1);
                    triangles.Add(vertexIndex + 3);
                    vertexIndex += 4;

                }
            }
            
        }


    }

    public bool IsNeighborInChunk(int ID, int p)
    {

        if (p == 0)
        {
            if (ID < 0)
            {
                return false;
            }
            else  if ((ID + 1)% VoxelData.ChunkHeight == 127)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else if (p == 1)
        {
            if (ID > (VoxelData.ChunkWidth * VoxelData.ChunkWidth * VoxelData.ChunkHeight - 1))
            {
                return false;
            }
            else if ((ID + 1) % VoxelData.ChunkHeight == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else if (p == 2)
        {
            if (ID < 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else if (p == 3)
        {
            if (ID > (VoxelData.ChunkWidth * VoxelData.ChunkWidth * VoxelData.ChunkHeight - 1))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else if (p == 4)
        {
            if (ID < 0)
            {
                return false;
            }
            else if (Mathf.FloorToInt(ID / VoxelData.ChunkHeight) % VoxelData.ChunkWidth == 15)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else 
        {
            if (ID > VoxelData.ChunkWidth * VoxelData.ChunkWidth * VoxelData.ChunkHeight - 1)
            {
                return false;
            }
            else if (Mathf.FloorToInt((ID) / VoxelData.ChunkHeight) % VoxelData.ChunkWidth == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    public bool IsCoordTransparent(int ID, int p)
    {
        if (!IsNeighborInChunk(ID, p))
        {
            if (p == 0 || p == 1)
            {
                return true;
            }
            else if (p == 2)
            {
                if ((chunkData.Z - 1) < 0)
                {
                    return true;
                }
                return world.blocktype.BlockTypes[world.Chunks[chunkData.X, chunkData.Z - 1].chunkData.GetVoxel(ID + (VoxelData.ChunkWidth * VoxelData.ChunkWidth * VoxelData.ChunkHeight)).GetBlockType()].isTransparent;
            }
            else if (p == 3)
            {
                if ((chunkData.Z + 1) > VoxelData.WorldChunksSize)
                {
                    return true;
                }
                else
                {

                    Debug.Log(ID-(VoxelData.ChunkWidth * VoxelData.ChunkWidth * VoxelData.ChunkHeight));
                    Debug.Log(chunkData.X + " - " + (chunkData.Z + 1));


                    return world.blocktype.BlockTypes[world.Chunks[chunkData.X, chunkData.Z + 1].chunkData.GetVoxel(ID - (VoxelData.ChunkWidth * VoxelData.ChunkWidth * VoxelData.ChunkHeight)).GetBlockType()].isTransparent;
                }


            }
            else if (p == 4)
            {
                if ((chunkData.X - 1) < 0)
                {
                    return true;
                }
                return world.blocktype.BlockTypes[world.Chunks[chunkData.X - 1, chunkData.Z].chunkData.GetVoxel(ID + (VoxelData.ChunkWidth * VoxelData.ChunkHeight)).GetBlockType()].isTransparent;
            }
            else
            {
                if ((chunkData.X + 1) > VoxelData.WorldChunksSize)
                {
                    return true;
                }
                return world.blocktype.BlockTypes[world.Chunks[chunkData.X + 1, chunkData.Z].chunkData.GetVoxel(ID - (VoxelData.ChunkWidth * VoxelData.ChunkHeight)).GetBlockType()].isTransparent;
            }
        }
        else
        { return world.blocktype.BlockTypes[chunkData.GetVoxel(ID).GetBlockType()].isTransparent; }
    }
    public bool IsCoordSame(int ID, int neighID, int p)
    {
        byte type = chunkData.GetVoxel(ID).GetBlockType();
        byte neightype = chunkData.GetVoxel(neighID).GetBlockType();
        if (!IsNeighborInChunk(ID, p))
        {
            if (p == 0 || p == 1)
            {
                return false;
            }
            else if (p == 2)
            {
                if ((chunkData.Z - 1) < 0)
                {
                    return false;
                }
               neightype= world.Chunks[chunkData.X, chunkData.Z - 1].chunkData.GetVoxel(ID + (VoxelData.ChunkWidth * VoxelData.ChunkWidth * VoxelData.ChunkHeight)).GetBlockType();
            }
            else if (p == 3)
            {
                if ((chunkData.Z + 1 )> VoxelData.WorldChunksSize)
                {
                    return false;
                }
                neightype = world.Chunks[chunkData.X, chunkData.Z + 1].chunkData.GetVoxel(ID - (VoxelData.ChunkWidth * VoxelData.ChunkWidth * VoxelData.ChunkHeight)).GetBlockType();
            }
            else if (p == 4)
            {
                if ((chunkData.X - 1) < 0)
                {
                    return false;
                }
                neightype= world.Chunks[chunkData.X -1, chunkData.Z].chunkData.GetVoxel(ID + (VoxelData.ChunkWidth * VoxelData.ChunkHeight)).GetBlockType();
            }
            else
            {
                if ((chunkData.X + 1) > VoxelData.WorldChunksSize)
                {
                    return false;
                }
                neightype = world.Chunks[chunkData.X +1, chunkData.Z].chunkData.GetVoxel(ID - (VoxelData.ChunkWidth * VoxelData.ChunkHeight)).GetBlockType();
            }
        }

            return type==neightype;
    }


    public static Vector3Int GetBlockVector3Index(int blockID)
    {
        Vector3Int ID = new Vector3Int();
        ID.z = (int)MathF.Floor(blockID / (VoxelData.ChunkHeight * VoxelData.ChunkWidth));
        ID.x = (int)MathF.Floor((blockID - (ID.z * VoxelData.ChunkHeight * VoxelData.ChunkWidth)) / VoxelData.ChunkHeight);
        ID.y = blockID% VoxelData.ChunkHeight;
        return ID;
    }
    public static int GetBlockIntID(Vector3Int index)
    {
        int ID = index.z * VoxelData.ChunkHeight * VoxelData.ChunkWidth + index.x * VoxelData.ChunkHeight + index.y;
        return ID;
    }

    public static Vector2Int GetChunkVector2Index(int ChunkID)
    {
        Vector2Int ID = new Vector2Int();
        ID.y = (int)MathF.Floor(ChunkID / VoxelData.WorldChunksSize);
        ID.x = ChunkID - (ID.y * VoxelData.WorldChunksSize);

        return ID;
    }
    public static int GetChunkIntID(Vector2Int index)
    {
        int WorldIdx = index.y * VoxelData.WorldChunksSize + index.x ;
        return WorldIdx;
    }


    void AddTexture(int textureID)
    {
        float y = textureID / VoxelData.TextureAtlasSizeInBlocks;
        float x = textureID - (y * VoxelData.TextureAtlasSizeInBlocks);

        x *= VoxelData.NormalizedBlockTextureSize;
        y *= VoxelData.NormalizedBlockTextureSize;

        y = 1f - y - VoxelData.NormalizedBlockTextureSize;

        uvs.Add(new Vector2(x, y));
        uvs.Add(new Vector2(x, y + VoxelData.NormalizedBlockTextureSize));
        uvs.Add(new Vector2(x + VoxelData.NormalizedBlockTextureSize, y));
        uvs.Add(new Vector2(x + VoxelData.NormalizedBlockTextureSize, y + VoxelData.NormalizedBlockTextureSize));

    }

    public bool Equals(Vector2Int other)
    {

        if (other == null)
            return false;
        else if (other.x == X && other.y == X)
            return true;
        else
            return false;

    }
}

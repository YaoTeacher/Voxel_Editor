using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Threading;
using TreeEditor;
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

    ChunkData chunkData;

    public Chunk(int _x,int _z, World _world, bool generateOnLoad)
    {
        X = _x;
        Z =_z;
        isActive = generateOnLoad;
        world = _world;

        if (isActive==true)
        {
            Init();
            chunkObject.SetActive(false);
        }
            
        else if(isActive == false && triangles.Count>0)
        {
            ClearMeshData();
        }

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

        _UpdateChunk();


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

        Thread myThread = new Thread(new ThreadStart(_UpdateChunk));
        myThread.Start();

    }
    public void _UpdateChunk()
    {

        threadLocked = true;

        while (modifications.Count > 0)
        {

            VoxelMod v = modifications.Dequeue();
            Vector3Int index = v.index -= new Vector3Int(X * VoxelData.ChunkWidth , 0, Z * VoxelData.ChunkWidth);
            int id = GetBlockIntID(index);
            chunkData.BlockList[id].SetBlockType(v.id);

        }

        ClearMeshData();

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

        CreateMesh();

        lock (world.chunksToDraw)
        {
            world.chunksToDraw.Enqueue(this);
        }

        threadLocked = false;

    }
    public void UpdateBlockFace(Vector3Int Chunkindex, int blockID)
    {
        Vector3 Pos = new Vector3(Chunkindex.x, Chunkindex.y, Chunkindex.z)*VoxelData.BlockSize;
        Block block = chunkData.BlockList[blockID];


        for (int p = 0; p < 6; p++)
        {
            if (!world.blocktype.BlockTypes[block.GetBlockType()].isTransparent)
            {
                if (IsCoordTransparent(blockID + VoxelData.faceChecks[p],p))
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
                if (!(IsCoordTransparent(blockID + VoxelData.faceChecks[p],p)&& IsCoordSame(blockID, blockID + VoxelData.faceChecks[p],p)))
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


    public bool IsCoordTransparent(int index, int p)
    {

        if (index < 0 || index > VoxelData.ChunkWidth * VoxelData.ChunkWidth * VoxelData.ChunkHeight-1)
        {


            if (p == 0 || p == 1)
            {
                return true;
            }

            else if (p == 2)
            {
                    if (chunkData.ChunkID - VoxelData.WorldChunksSize < 0)
                    {
                        return false;
                    }

                    return world.blocktype.BlockTypes[world.worldData.Chunks[chunkData.ChunkID - VoxelData.WorldChunksSize].BlockList[index + (VoxelData.ChunkWidth * (VoxelData.ChunkWidth - 1) * VoxelData.ChunkHeight)].GetBlockType()].isTransparent;


            }
            else if (p == 3)
            {
                if (chunkData.ChunkID + VoxelData.WorldChunksSize > VoxelData.WorldChunksSize * VoxelData.WorldChunksSize)
                {
                    return false;
                }

                return world.blocktype.BlockTypes[world.worldData.Chunks[chunkData.ChunkID + VoxelData.WorldChunksSize].BlockList[index - (VoxelData.ChunkWidth * (VoxelData.ChunkWidth - 1) * VoxelData.ChunkHeight)].GetBlockType()].isTransparent;

            }
            else if (p == 4)
            {
                if (chunkData.ChunkID - 1 < 0)
                {
                    return false;
                }
                return world.blocktype.BlockTypes[world.worldData.Chunks[chunkData.ChunkID - 1].BlockList[index + ((VoxelData.ChunkWidth - 1) * VoxelData.ChunkHeight)].GetBlockType()].isTransparent;

            }
            else
            {
                if (chunkData.ChunkID + 1 > VoxelData.WorldChunksSize * VoxelData.WorldChunksSize)
                {
                    return false;
                }
                return world.blocktype.BlockTypes[world.worldData.Chunks[chunkData.ChunkID + 1].BlockList[index - ((VoxelData.ChunkWidth - 1) * VoxelData.ChunkHeight)].GetBlockType()].isTransparent;

            };

        }
        else
        { return world.blocktype.BlockTypes[chunkData.BlockList[index].GetBlockType()].isTransparent; }


    }

    public bool IsCoordSame(int index, int Coordindex,int p)
    {
        if (Coordindex < 0 ||Coordindex>VoxelData.ChunkWidth*VoxelData.ChunkWidth*VoxelData.ChunkHeight - 1)
        {
            if (p == 0 || p == 1)
            {
                return false;
            }

            else if (p == 2)
            {
                if (chunkData.BlockList[index].GetBlockType() == world.worldData.Chunks[chunkData.ChunkID - VoxelData.WorldChunksSize].BlockList[index + (VoxelData.ChunkWidth * (VoxelData.ChunkWidth - 1) * VoxelData.ChunkHeight)].GetBlockType())
                    return true;
                else
                    return false;
            }
            else if (p == 3)
            {
                if (chunkData.BlockList[index].GetBlockType() == world.worldData.Chunks[chunkData.ChunkID + VoxelData.WorldChunksSize].BlockList[index - (VoxelData.ChunkWidth * (VoxelData.ChunkWidth - 1) * VoxelData.ChunkHeight)].GetBlockType())
                    return true;
                else
                    return false;
            }
            else if (p == 4)
            {

                if (chunkData.BlockList[index].GetBlockType() == world.worldData.Chunks[chunkData.ChunkID - 1].BlockList[index + ((VoxelData.ChunkWidth - 1) * VoxelData.ChunkHeight)].GetBlockType())
                    return true;
                else
                    return false;
            }
            else 
            {
                if (chunkData.BlockList[index].GetBlockType() == world.worldData.Chunks[chunkData.ChunkID + 1].BlockList[index - ((VoxelData.ChunkWidth - 1) * VoxelData.ChunkHeight)].GetBlockType())
                    return true;
                else
                    return false;
            };




        }
        else
        {

            if (chunkData.BlockList[index].GetBlockType() == chunkData.BlockList[Coordindex].GetBlockType())
                return true;
            else
                return false;
        }


    }

    public static Vector3Int GetBlockVector3ID(int blockID)
    {
        Vector3Int ID = new Vector3Int();
        ID.z = (int)MathF.Floor(blockID / (VoxelData.ChunkHeight * VoxelData.ChunkWidth));
        ID.x = (int)MathF.Floor((blockID - (ID.z * VoxelData.ChunkHeight * VoxelData.ChunkWidth)) / VoxelData.ChunkHeight);
        ID.y = blockID - (ID.x * VoxelData.ChunkHeight * VoxelData.ChunkWidth) - (ID.x * VoxelData.ChunkHeight);
        return ID;
    }
    public static int GetBlockIntID(Vector3Int index)
    {
        int WorldIdx = index.z * VoxelData.ChunkHeight * VoxelData.ChunkWidth + index.x * VoxelData.ChunkHeight + index.y;
        return WorldIdx;
    }

    public static Vector2Int GetChunkVector2ID(int ChunkID)
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


   
    public bool isEditable
    {

        get
        {

            if (!isVoxelMapPopulated || threadLocked)
                return false;
            else
                return true;

        }

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

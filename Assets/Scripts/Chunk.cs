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
    Vector3Int[,,] ChunkIndex;

    public GameObject chunkObject;
    MeshRenderer meshRenderer;
    MeshFilter meshFilter;

    public bool isVoxelMapPopulated = false;
    public bool threadLocked = false;
    public Queue<VoxelMod> modifications = new Queue<VoxelMod>();

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



    public void GenerateBlock()
    {
        ChunkIndex = new Vector3Int[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];
        for (int y=0; y<VoxelData.ChunkHeight;y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {
                    Vector3Int index = new Vector3Int(x + X * VoxelData.ChunkWidth, y, z + Z* VoxelData.ChunkWidth);

                    ChunkIndex[x, y, z]=index;
                    Vector3 pos= new Vector3(x *VoxelData.BlockSize, y * VoxelData.BlockSize, z  * VoxelData.BlockSize);
                    world.BlockList[index]= new Block(index, World.BlockTypeList[index.x, y, index.z]);
                }
            }
        }
        isVoxelMapPopulated = true;
    }


    public void Init()
    {

        chunkObject = new GameObject();
        meshFilter = chunkObject.AddComponent<MeshFilter>();
        meshRenderer = chunkObject.AddComponent<MeshRenderer>();
        
        meshRenderer.material = world.material;
        chunkObject.transform.SetParent(world.transform);
        chunkObject.transform.position = new Vector3(X * VoxelData.ChunkWidth * VoxelData.BlockSize, 0f, Z * VoxelData.ChunkWidth * VoxelData.BlockSize);
        chunkObject.name = "Chunk " + X + ", " + Z;
        GenerateBlock();
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
            Vector3 pos = v.position -= chunkObject.transform.position;
            World.BlockTypeList[(int)pos.x, (int)pos.y, (int)pos.z] = v.id;

        }

        ClearMeshData();

        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {
                    Block b = world.BlockList[ChunkIndex[x, y, z]];
                    if (world.blocktype.BlockTypes[b.GetBlockType()].isSolid)

                        UpdateBlockFace(b, new Vector3(x, y, z) * VoxelData.BlockSize);

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
    public void UpdateBlockFace(Block block,Vector3 Pos)
    {
        Vector3Int index = block.GetIndex();

        for (int p = 0; p < 6; p++)
        {
            if (!world.blocktype.BlockTypes[block.GetBlockType()].isTransparent)
            {
                if (IsCoordTransparent(block.GetIndex() + VoxelData.faceChecks[p]))
                {


                vertices.Add(Pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 0]]);
                vertices.Add(Pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 1]]);
                vertices.Add(Pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 2]]);
                vertices.Add(Pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 3]]);

                AddTexture(world.blocktype.BlockTypes[World.BlockTypeList[index.x, index.y,index.z]].GetTextureID(p));

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
                if (!(IsCoordTransparent(block.GetIndex() + VoxelData.faceChecks[p])&& IsCoordSame(block.GetIndex(), block.GetIndex() + VoxelData.faceChecks[p])))
                {


                    vertices.Add(Pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 0]]);
                    vertices.Add(Pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 1]]);
                    vertices.Add(Pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 2]]);
                    vertices.Add(Pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 3]]);

                    AddTexture(world.blocktype.BlockTypes[World.BlockTypeList[index.x , index.y, index.z]].GetTextureID(p));

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


    public bool IsCoordTransparent(Vector3Int index)
   {
 
        if (index.x<0|| index.y < 0|| index.z < 0|| index.x >=VoxelData.ChunkWidth* (VoxelData.WorldChunksSize + 1)|| index.z >= VoxelData.ChunkWidth * (VoxelData.WorldChunksSize + 1) || index.y >= VoxelData.ChunkHeight)
        {
             return true;
        }
        else
        { return world.blocktype.BlockTypes[World.BlockTypeList[index.x, index.y, index.z]].isTransparent; }


   }

    public bool IsCoordSame(Vector3Int index, Vector3Int Coordindex)
    {
        if (Coordindex.x < 0 || Coordindex.y < 0 || Coordindex.z < 0 || Coordindex.x >= VoxelData.ChunkWidth * (VoxelData.WorldChunksSize + 1) || Coordindex.z >= VoxelData.ChunkWidth * (VoxelData.WorldChunksSize + 1) || Coordindex.y >= VoxelData.ChunkHeight)
        {
            return false;
        }
        else
        {
            if (world.blocktype.BlockTypes[World.BlockTypeList[index.x, index.y, index.z]] == world.blocktype.BlockTypes[World.BlockTypeList[Coordindex.x, Coordindex.y, Coordindex.z]])
                return true;
            else
                return false;
        }


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

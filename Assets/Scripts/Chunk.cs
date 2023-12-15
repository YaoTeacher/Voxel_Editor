using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;
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
                    int block_X = VoxelData.ChunkWidth * world.WorldChunkSize + index.x;
                    int block_Z = index.z + VoxelData.ChunkWidth * world.WorldChunkSize;
                    ChunkIndex[x, y, z]=index;
                    Vector3 pos= new Vector3(x + X * VoxelData.ChunkWidth, y, z + Z * VoxelData.ChunkWidth);
                    world.BlockList[index]= new Block(pos, index, World.BlockTypeList[block_X, y, block_Z]);
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
        chunkObject.transform.position = new Vector3(X * VoxelData.ChunkWidth, 0f, Z * VoxelData.ChunkWidth);
        chunkObject.name = "Chunk " + X + ", " + Z;
        GenerateBlock();
        UpdateChunk();


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

    void UpdateChunk()
    {

        ClearMeshData();

        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {
                    Block b = world.BlockList[ChunkIndex[x, y, z]];
                    if (world.BlockTypes[b.GetBlockType()].isSolid)

                        UpdateBlockFace(b, new Vector3(x, y, z));

                }
            }
        }

        CreateMesh();

    }
    public void UpdateBlockFace(Block block,Vector3 Pos)
    {
        Vector3Int index = block.GetIndex();

        for (int p = 0; p < 6; p++)
        {

            if (IsCoordTransparent(block.GetIndex() + VoxelData.faceChecks[p]))
            {


                vertices.Add(Pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 0]]);
                vertices.Add(Pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 1]]);
                vertices.Add(Pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 2]]);
                vertices.Add(Pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 3]]);

                AddTexture(world.BlockTypes[World.BlockTypeList[index.x+ VoxelData.ChunkWidth * world.WorldChunkSize, index.y,index.z+ VoxelData.ChunkWidth * world.WorldChunkSize]].GetTextureID(p));

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

    public void EditVoxel(Vector3 pos, byte newID)
    {

        int x_Check = Mathf.FloorToInt(pos.x);
        int y_Check = Mathf.FloorToInt(pos.y);
        int z_Check = Mathf.FloorToInt(pos.z);

        //int xCheck = x_Check - Mathf.FloorToInt(chunkObject.transform.position.x);
        //int zCheck = z_Check - Mathf.FloorToInt(chunkObject.transform.position.z);

        Vector3Int index = new Vector3Int(x_Check, y_Check,z_Check);

        if (!world.BlockList.ContainsKey(index))
        {
            Debug.Log("Out Of Range !");

        }
        else
        {
            Debug.Log(x_Check+"+" + y_Check+ "+" + z_Check);
            world.BlockList[index].SetBlockType(newID);
            World.BlockTypeList[x_Check+VoxelData.ChunkWidth * world.WorldChunkSize, y_Check, z_Check+VoxelData.ChunkWidth * world.WorldChunkSize] = newID;
            UpdateChunk();
        }


    }

   public bool IsCoordTransparent(Vector3Int index)
   {
        Vector3Int BlockIndex = index + new Vector3Int(VoxelData.ChunkWidth * world.WorldChunkSize,0, VoxelData.ChunkWidth * world.WorldChunkSize);
 
        if (BlockIndex.x<0|| BlockIndex.y < 0|| BlockIndex.z < 0|| BlockIndex.x >=VoxelData.ChunkWidth* (2 * world.WorldChunkSize+1)|| BlockIndex.z >= VoxelData.ChunkWidth * (2 * world.WorldChunkSize + 1) || BlockIndex.y >= VoxelData.ChunkHeight)
        {
             return true;
        }
        else
        { return world.BlockTypes[World.BlockTypeList[BlockIndex.x, BlockIndex.y, BlockIndex.z]].isTransparent; }


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

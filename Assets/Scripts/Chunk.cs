using System;
using System.Collections.Generic;

using UnityEngine;


public class Chunk
{
    World world;
    public int X;
    public int Z;
    public bool _isActive;


    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();
    int vertexIndex = 0;

    public GameObject chunkObject;
    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    public Vector3 position;

    public Queue<VoxelMod> modifications = new Queue<VoxelMod>();

    public ChunkData chunkData;


    public Chunk(int x,int z)
    {
        X = x;Z = z;
        chunkObject = new GameObject();
        meshFilter = chunkObject.AddComponent<MeshFilter>();
        meshRenderer = chunkObject.AddComponent<MeshRenderer>();

        meshRenderer.material = World.Instance.material;
        chunkObject.transform.SetParent(World.Instance.transform);
        chunkObject.transform.position = new Vector3(X * VoxelData.ChunkWidth * VoxelData.BlockSize, 0f, Z * VoxelData.ChunkWidth * VoxelData.BlockSize);
        chunkObject.name = "Chunk " + X + ", " + Z;
        position = chunkObject.transform.position;

        chunkData = World.Instance.worldData.RequestChunk(new Vector2Int(X, Z), true);

        World.Instance.AddChunkToUpdate(this);

           
    }

    public bool isActive
    {

        get { return _isActive; }
        set
        {

            _isActive = value;
            if (chunkObject != null)
                chunkObject.SetActive(value);

        }

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



        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {
                    Vector3Int index = new Vector3Int(x, y, z);
                    int blockID = GetBlockIntID(index);
                    if (World.Instance.blocktype.BlockTypes[chunkData.BlockList[blockID].GetBlockType()].isSolid)

                        UpdateBlockFace(index,blockID);

                }
            }
        }
        World.Instance.chunksToDraw.Enqueue(this);
    }

    public void EditVoxel(Vector3 pos, byte newType)
    {

        Vector3Int worldindex = World.GetWorldIndexFromPos(pos);
        Debug.Log("hi" + worldindex);
        //Vector2Int chunkindex = World.GetChunkIndexFromPos(pos);
        Vector3Int index = new Vector3Int(worldindex.x - (X * VoxelData.ChunkWidth), worldindex.y, worldindex.z - (Z* VoxelData.ChunkWidth));
        Debug.Log("hi" + index);
        int ID =GetBlockIntID(index);
        Debug.Log("hi"+ID);
        chunkData.BlockList[ID].SetBlockType(newType);

        World.Instance.worldData.AddToModifiedChunkList(chunkData);

        lock (World.Instance.ChunkUpdateThreadLock)
        {

            World.Instance.AddChunkToUpdate(this,true);
            UpdateSurroundingChunk(ID);
        }

    }

    public void EditVoxel(Vector3Int worldindex, byte newType)
    {

        Debug.Log("hi" + worldindex);
        //Vector2Int chunkindex = World.GetChunkIndexFromPos(pos);
        Debug.Log("hi" + X);
        Vector3Int index = new Vector3Int(worldindex.x - (X * VoxelData.ChunkWidth), worldindex.y, worldindex.z - (Z * VoxelData.ChunkWidth));
        Debug.Log("hi" + index);
        int ID = GetBlockIntID(index);
        Debug.Log("hi" + ID);
        chunkData.GetVoxel(ID).SetBlockType(newType);

        World.Instance.worldData.AddToModifiedChunkList(chunkData);

        lock (World.Instance.ChunkUpdateThreadLock)
        {

            World.Instance.AddChunkToUpdate(this, true);
            UpdateSurroundingChunk(ID);
        }

    }
    void UpdateSurroundingChunk(int ID)
    {

        for (int p = 0; p < 6; p++)
        {

            int neighborID = ID + VoxelData.faceChecks[p];

            if (!IsNeighborInChunk(neighborID, p))
            {
                Debug.Log("on board!" + p);
                switch (p)
                {
                    case 2:
                        {
                            if (!((chunkData.Z - 1) < 0))
                            {

                                Debug.Log(X + " " + Z);
                                World.Instance.AddChunkToUpdate(World.Instance.Chunks[chunkData.X, chunkData.Z - 1], true);
                            }
                            break;
                        }
                    case 3:
                        {
                            if (!((chunkData.Z + 1) >= VoxelData.WorldChunksSize))
                            {
                                Debug.Log(X + " " + Z);
                                World.Instance.AddChunkToUpdate(World.Instance.Chunks[chunkData.X, chunkData.Z + 1], true);
                            }
                            break;
                        }
                    case 4:
                        {
                            if (!((chunkData.X - 1) < 0))
                            {
                                Debug.Log(X + " " + Z);
                                World.Instance.AddChunkToUpdate( World.Instance.Chunks[chunkData.X - 1, chunkData.Z],true);
                            }
                            break;
                        }
                    case 5:
                        {
                            if (!((chunkData.X + 1) >= VoxelData.WorldChunksSize))
                            {
                                Debug.Log(X + " " + Z);
                                World.Instance.AddChunkToUpdate(World.Instance.Chunks[chunkData.X + 1, chunkData.Z],true);
                            }
                            break;
                        }
                    default:
                        {
                            return;
                        }
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

            if ((!World.Instance.blocktype.BlockTypes[block.GetBlockType()].isTransparent)&& block.GetBlockType()!=0)
            {
                if (IsCoordTransparent(neighID, p))
                {


                vertices.Add(Pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 0]]);
                vertices.Add(Pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 1]]);
                vertices.Add(Pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 2]]);
                vertices.Add(Pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 3]]);

                AddTexture(World.Instance.blocktype.BlockTypes[block.GetBlockType()].GetTextureID(p));

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
                if ((!IsCoordTransparent(neighID, p))|| (!IsCoordSame(blockID, neighID, p)))
                {


                    vertices.Add(Pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 0]]);
                    vertices.Add(Pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 1]]);
                    vertices.Add(Pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 2]]);
                    vertices.Add(Pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 3]]);

                    AddTexture(World.Instance.blocktype.BlockTypes[block.GetBlockType()].GetTextureID(p));

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

        switch (p)
        {
            case 0:
                {
                    if (ID < 0)
                    {
                        return false;
                    }
                    else if ((ID + 1) % VoxelData.ChunkHeight == 127)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }

                };
            case 1:
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
            case 2:
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
            case 3:
                {
                    if (ID > (VoxelData.ChunkWidth * VoxelData.ChunkWidth * VoxelData.ChunkHeight - 1))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                };
            case 4:
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
                };
            case 5:
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
                };
               default:
                {
                    return true;
                }
        } 
    }




    public bool IsCoordTransparent(int ID, int p)
    {
        if (!IsNeighborInChunk(ID, p))
        {



            switch (p)
            {
                case 0:
                    {
                        return true;
                    }
                case 1:
                    {
                        return true;
                    }
                case 2:
                    {
                        if ((chunkData.Z - 1) < 0)
                        {
                            return true;
                        }
                        return World.Instance.blocktype.BlockTypes[World.Instance.Chunks[chunkData.X, chunkData.Z - 1].chunkData.GetVoxel(ID + (VoxelData.ChunkWidth * VoxelData.ChunkWidth * VoxelData.ChunkHeight)).GetBlockType()].isTransparent;

                    }
                case 3:
                    {
                        if ((chunkData.Z + 1) >= VoxelData.WorldChunksSize)
                        {
                            return true;
                        }
                        else
                        {


                            return World.Instance.blocktype.BlockTypes[World.Instance.Chunks[chunkData.X, chunkData.Z + 1].chunkData.GetVoxel(ID - (VoxelData.ChunkWidth * VoxelData.ChunkWidth * VoxelData.ChunkHeight)).GetBlockType()].isTransparent;

                        }
                    }
                case 4:
                    {
                        if ((chunkData.X - 1) < 0)
                        {
                            return true;
                        }
                        return World.Instance.blocktype.BlockTypes[World.Instance.Chunks[chunkData.X - 1, chunkData.Z].chunkData.GetVoxel(ID + (VoxelData.ChunkWidth * VoxelData.ChunkHeight)).GetBlockType()].isTransparent;
                    }
                case 5:
                    {
                        if ((chunkData.X + 1) >= VoxelData.WorldChunksSize)
                        {
                            return true;
                        }
                        return World.Instance.blocktype.BlockTypes[World.Instance.Chunks[chunkData.X + 1, chunkData.Z].chunkData.GetVoxel(ID - (VoxelData.ChunkWidth * VoxelData.ChunkHeight)).GetBlockType()].isTransparent;
                    }
                default:
                    {
                        return false;
                    }

            };


        }
        else
        { return World.Instance.blocktype.BlockTypes[chunkData.GetVoxel(ID).GetBlockType()].isTransparent; }
    }
    public bool IsCoordSame(int ID, int neighID, int p)
    {
        byte type = chunkData.GetVoxel(ID).GetBlockType();
        byte neightype = new byte();
        if (!IsNeighborInChunk(neighID, p))
        {
            
            
            switch (p)
            {
                case 0:
                    {
                        return false;
                    }
                case 1:
                    {
                        return false;
                    }
                case 2:
                    {
                        if ((chunkData.Z - 1) < 0)
                        {
                            return false;
                        }
                        neightype = World.Instance.Chunks[chunkData.X, chunkData.Z - 1].chunkData.GetVoxel(ID + (VoxelData.ChunkWidth * VoxelData.ChunkWidth * VoxelData.ChunkHeight)).GetBlockType();
                        break;
                    }
                case 3:
                    {
                        if ((chunkData.Z + 1) >= VoxelData.WorldChunksSize)
                        {
                            return false;
                        }
                        neightype = World.Instance.Chunks[chunkData.X, chunkData.Z + 1].chunkData.GetVoxel(ID - (VoxelData.ChunkWidth * VoxelData.ChunkWidth * VoxelData.ChunkHeight)).GetBlockType();
                        break;
                    }
                case 4:
                    {
                        if ((chunkData.X - 1) < 0)
                        {
                            return false;
                        }
                        neightype = World.Instance.Chunks[chunkData.X - 1, chunkData.Z].chunkData.GetVoxel(ID + (VoxelData.ChunkWidth * VoxelData.ChunkHeight)).GetBlockType();
                        break;
                    }
                case 5:
                    {
                        if ((chunkData.X + 1) >= VoxelData.WorldChunksSize)
                        {
                            return false;
                        }
                        neightype = World.Instance.Chunks[chunkData.X + 1, chunkData.Z].chunkData.GetVoxel(ID - (VoxelData.ChunkWidth * VoxelData.ChunkHeight)).GetBlockType();
                        break;
                    }
                default:
                    {
                        break;
                    }

            };

        }
        else
        {
            neightype = chunkData.GetVoxel(neighID).GetBlockType();
        }

        return type == neightype;
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

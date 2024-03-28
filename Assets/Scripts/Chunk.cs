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
    List<Vector3> normals = new List<Vector3>();
    int vertexIndex = 0;

    public GameObject chunkObject;
    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    public Vector3 position;

    public Queue<VoxelMod> modifications = new Queue<VoxelMod>();

    public chunkData chunkData;


    public Chunk(int x,int z)
    {
        X = x;Z = z;

        /*World.Instance.transform.Find($"{chunkData.Name}") == null;*/
        chunkData = World.Instance.scenedata.RequestChunk(new Vector2Int(X, Z), true);
        if (System.Object.ReferenceEquals(World.Instance.transform.Find(chunkData.Name), null))
        {
            chunkObject = new GameObject();
            chunkObject.transform.SetParent(World.Instance.transform);
            meshFilter = chunkObject.AddComponent<MeshFilter>();
            meshRenderer = chunkObject.AddComponent<MeshRenderer>();

            meshRenderer.material = World.Instance.material;


        }
        else
        {
            Debug.Log("findob");
            chunkObject = World.Instance.transform.Find($"{chunkData.Name}").gameObject;

            if (chunkObject.GetComponent<MeshFilter>() != null)
            {
                meshFilter = chunkObject.GetComponent<MeshFilter>();
            }
            else
            {
                meshFilter = chunkObject.AddComponent<MeshFilter>();
            }
            if (chunkObject.GetComponent<MeshRenderer>() != null)
            {
                meshRenderer = chunkObject.GetComponent<MeshRenderer>();

                meshRenderer.material = World.Instance.material;
            }
            else
            {
                meshRenderer = chunkObject.AddComponent<MeshRenderer>();

                meshRenderer.material = World.Instance.material;
            }
        }




        chunkObject.transform.position = new Vector3(X * VoxelData.ChunkWidth * VoxelData.BlockSize, 0f, Z * VoxelData.ChunkWidth * VoxelData.BlockSize);
        //chunkObject.name = "Chunk " + X + ", " + Z;
        position = chunkObject.transform.position;

        
        chunkObject.name = chunkData.Name;
        //Debug.Log(chunkData.Name);

        World.Instance.AddChunkToUpdate(this);

           
    }

    //bool isObjectExist()
    //{

    //}
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
        normals.Clear();
        uvs.Clear();

    }

    public void CreateMesh()
    {

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.normals = normals.ToArray();
        //Debug.Log(mesh.triangles.Length);
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
                    if (World.Instance.blocktype.BlockTypes[chunkData.Blocks[blockID].GetBlockType()].isSolid)
                        try
                        {
                            UpdateBlockFace(index, blockID);
                        }
                        catch (Exception e)
                        {
                            Debug.Log(chunkData.Name+"+"+index);
                            Debug.LogException(e);
                        }

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
        if (!World.Instance.blocktype.BlockTypes[newType].isAllowXZorientation && !World.Instance.blocktype.BlockTypes[newType].isAllowXZorientation)
        {
            chunkData.GetVoxel(ID).SetBlockState(newType, 3);
        }
        else if (World.Instance.blocktype.BlockTypes[newType].isAllowXZorientation || World.Instance.blocktype.BlockTypes[newType].isAllowXZorientation)
        {
            chunkData.GetVoxel(ID).SetBlockState(newType, World.Instance._player.orientation);
        }

        World.Instance.scenedata.AddToModifiedChunkList(chunkData);

        lock (World.Instance.ChunkUpdateThreadLock)
        {

            World.Instance.AddChunkToUpdate(this,true);
            UpdateSurroundingChunk(ID);
        }

    }

    public void EditVoxel(Vector3Int worldindex, byte newType)
    {

        Debug.Log("hi" + worldindex);
        Debug.Log("hi" + X);
        Vector3Int index = new Vector3Int(worldindex.x - (X * VoxelData.ChunkWidth), worldindex.y, worldindex.z - (Z * VoxelData.ChunkWidth));
        int ID = GetBlockIntID(index);
        if ((!World.Instance.blocktype.BlockTypes[newType].isAllowXZorientation) && (!World.Instance.blocktype.BlockTypes[newType].isAllowYorientation))
        {
            chunkData.GetVoxel(ID).SetBlockState(newType, 2);
        }
        else if (World.Instance.blocktype.BlockTypes[newType].isAllowXZorientation)
        {
            if (!World.Instance.blocktype.BlockTypes[newType].isAllowYorientation)
            {
                int ori = 0;
                Debug.Log(chunkData.Blocks[ID].orientation);
                switch (World.Instance._player.orientation)
                {
                    
                    case 12:
                        ori = 2;
                        break;
                    case 13:
                        ori = 3;
                        break;
                    case 14:
                        ori = 4;
                        break;
                    case 15:
                        ori = 5;
                        break;
   
                    default:
                        
                        break;


                }
                Debug.Log(chunkData.Blocks[ID].orientation);
                chunkData.GetVoxel(ID).SetBlockState(newType, ori);
                Debug.Log(chunkData.Blocks[ID].orientation);
            }
            else
            {
                chunkData.GetVoxel(ID).SetBlockState(newType, World.Instance._player.orientation);
                Debug.Log(chunkData.Blocks[ID].orientation);
            }
        }
        chunkData.BlockstoUpdate[ID] = chunkData.Blocks[ID];


        World.Instance.scenedata.AddToModifiedChunkList(chunkData);

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
            Debug.Log(neighborID+""+p);

            if (!IsNeighborInChunk(neighborID, p))
            {
                Debug.Log(neighborID);
                Debug.Log("on board!" + p);
                switch (p)
                {
                    case 2:
                        {
                            if (!((chunkData.index_z - 1) < 0))
                            {

                                Debug.Log(X + " " + Z);
                                World.Instance.AddChunkToUpdate(World.Instance.Chunks[chunkData.index_x, chunkData.index_z - 1], true);
                            }
                            break;
                        }
                    case 3:
                        {
                            if (!((chunkData.index_z + 1) >= VoxelData.WorldChunksSize))
                            {
                                Debug.Log(X + " " + Z);
                                World.Instance.AddChunkToUpdate(World.Instance.Chunks[chunkData.index_x, chunkData.index_z + 1], true);
                            }
                            break;
                        }
                    case 4:
                        {
                            if (!((chunkData.index_x - 1) < 0))
                            {
                                Debug.Log(X + " " + Z);
                                World.Instance.AddChunkToUpdate( World.Instance.Chunks[chunkData.index_x - 1, chunkData.index_z],true);
                            }
                            break;
                        }
                    case 5:
                        {
                            if (!((chunkData.index_x + 1) >= VoxelData.WorldChunksSize))
                            {
                                Debug.Log(X + " " + Z);
                                World.Instance.AddChunkToUpdate(World.Instance.Chunks[chunkData.index_x + 1, chunkData.index_z],true);
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }

            }

        }

    }

    public void UpdateBlockFace(Vector3Int Chunkindex, int blockID)
    {
        Vector3 Pos = new Vector3(Chunkindex.x, Chunkindex.y, Chunkindex.z)*VoxelData.BlockSize;
        blockData block = chunkData.Blocks[blockID];

        Vector3 rot = new Vector3(0,0,0);
        switch (block.orientation)
        {

            case 3:
                rot = new Vector3(0,180f,0);
                break;
            case 5:
                rot = new Vector3(0, 270f, 0);
                break;
            case 2:
                rot = new Vector3(0,0, 0);
                break;
            case 12:
                rot = new Vector3(-90f, 0, 0);
                break;
            case 13:
                rot = new Vector3(90f, 0, 0);
                break;
            case 14:
                rot = new Vector3(0, 0, 90f);
                break;
            case 15:
                rot = new Vector3(0, 0, -90f);
                break;
            default:
                rot = new Vector3(0, 90f, 0);
                break;


        }


            for (int p = 0; p < 6; p++)
            {

            int translatedP = VoxelData.transP[new Vector2Int(block.orientation,p)];



            //if (block.orientation != 2)
            //{
            //    if (block.orientation == 3)
            //    {
            //        if (p == 3) translatedP = 2;
            //        else if (p == 2) translatedP = 3;
            //        else if (p == 4) translatedP = 5;
            //        else if (p == 5) translatedP = 4;
            //    }
            //    else if (block.orientation == 5)
            //    {
            //        if (p == 3) translatedP = 5;
            //        else if (p == 2) translatedP = 4;
            //        else if (p == 4) translatedP = 3;
            //        else if (p == 5) translatedP = 2;
            //    }
            //    else if (block.orientation == 4)
            //    {
            //        if (p == 3) translatedP = 4;
            //        else if (p == 2) translatedP = 5;
            //        else if (p == 4) translatedP = 2;
            //        else if (p == 5) translatedP = 3;
            //    }
            //    //���泯ǰ
            //    else if (block.orientation == 13)
            //    {
            //        if (p == 0) translatedP = 2; // ��-��
            //        if (p == 1) translatedP = 3; // ��-ǰ
            //        if (p == 2) translatedP = 1; // ǰ-��
            //        if (p == 3) translatedP = 0; // ��-��

            //    }

            //    // ���泯��
            //    else if (block.orientation == 12)
            //    {
            //        if (p == 0) translatedP = 3; // ��-ǰ
            //        if (p == 1) translatedP = 2; // ��-��
            //        if (p == 2) translatedP = 0; // ǰ-��
            //        if (p == 3) translatedP = 1; // ��-��

            //    }

            //    // ���泯��
            //    else if (block.orientation == 15)
            //    {
            //        if (p == 0) translatedP = 4; // ��-��
            //        if (p == 1) translatedP = 5; // ��-��
            //        if (p == 4) translatedP = 1; // ��-��
            //        if (p == 5) translatedP = 0; // ��-��
            //    }

            //    // ���泯��
            //    else if (block.orientation == 14)
            //    {
            //        if (p == 0) translatedP = 5; // ��-��
            //        if (p == 1) translatedP = 4; // ��-��
            //        if (p == 4) translatedP = 0; // ��-��
            //        if (p == 5) translatedP = 1; // ��-��
            //    }
            //}

            int neighID = blockID + VoxelData.faceChecks[translatedP];

            if ((!World.Instance.blocktype.BlockTypes[block.GetBlockType()].isTransparent)&& block.GetBlockType()!=0)
            {
                if (!IsCoordAllowRender(neighID, translatedP))
                {
                    if (IsCoordTransparent(neighID, translatedP))
                    {
                        int faceVertCount = 0;

                        for (int i = 0; i < block.properties.meshData.faces[p].vertData.Length; i++)
                        {
                            VertData vertData = block.properties.meshData.faces[p].GetVertData(i);

                            vertices.Add(Pos + vertData.GetRotatedPosition(rot));
                            normals.Add(block.properties.meshData.faces[p].normal);
                            AddTexture(block.properties.GetTextureID(p), vertData.uv);
                            faceVertCount++;


                        }
                        for (int i = 0; i < block.properties.meshData.faces[p].triangles.Length; i++)
                        {

                            triangles.Add(vertexIndex + block.properties.meshData.faces[p].triangles[i]);


                        }

                        vertexIndex += faceVertCount;

                    }
                }
                else
                {
                    //if ((!IsCoordSame(blockID, neighID, p)))
                    //{
                        int faceVertCount = 0;

                        for (int i = 0; i < block.properties.meshData.faces[p].vertData.Length; i++)
                        {
                        VertData vertData = block.properties.meshData.faces[p].GetVertData(i);
                        vertices.Add(Pos + (vertData.GetRotatedPosition(rot)));
                        normals.Add(block.properties.meshData.faces[p].normal);
                            AddTexture(block.properties.GetTextureID(p), vertData.uv);
                            faceVertCount++;


                        }
                        for (int i = 0; i < block.properties.meshData.faces[p].triangles.Length; i++)
                        {
                        triangles.Add(vertexIndex + block.properties.meshData.faces[p].triangles[i]);
                    }

                        vertexIndex += faceVertCount;

                    //}
                    //else { continue; }

                }


            }
            else 
            {


                if ((!IsCoordTransparent(neighID, p)) || (!IsCoordSame(blockID, neighID, p)))
                {
                    int faceVertCount = 0;

                    for (int i = 0; i < block.properties.meshData.faces[p].vertData.Length; i++)
                    {

                        //vertices.Add(Pos + (block.properties.meshData.faces[p].vertData[i].position * VoxelData.BlockSize));
                        VertData vertData = block.properties.meshData.faces[p].GetVertData(i);
                        vertices.Add(Pos + (vertData.GetRotatedPosition(rot)));
                        normals.Add(block.properties.meshData.faces[p].normal);
                        AddTexture(block.properties.GetTextureID(p), vertData.uv);
                        faceVertCount++;


                    }
                    for (int i = 0; i < block.properties.meshData.faces[p].triangles.Length; i++)
                    {
                        triangles.Add(vertexIndex + block.properties.meshData.faces[p].triangles[i]);
                    }

                    vertexIndex += faceVertCount;

                }
                else { continue; }
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
                    else if ((ID + 1) % VoxelData.ChunkHeight == VoxelData.ChunkHeight-1)
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
                    else if (Mathf.FloorToInt(ID / VoxelData.ChunkHeight) % VoxelData.ChunkWidth == VoxelData.ChunkWidth-1)
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
                        if ((chunkData.index_z - 1) < 0)
                        {
                            return true;
                        }
                        return World.Instance.blocktype.BlockTypes[World.Instance.Chunks[chunkData.index_x, chunkData.index_z - 1].chunkData.GetVoxelType(ID + (VoxelData.ChunkWidth * VoxelData.ChunkWidth * VoxelData.ChunkHeight))].isTransparent;

                    }
                case 3:
                    {
                        if ((chunkData.index_z + 1) >= VoxelData.WorldChunksSize)
                        {
                            return true;
                        }
                        else
                        {

                            return World.Instance.blocktype.BlockTypes[World.Instance.Chunks[chunkData.index_x, chunkData.index_z + 1].chunkData.GetVoxelType(ID - (VoxelData.ChunkWidth * VoxelData.ChunkWidth * VoxelData.ChunkHeight))].isTransparent;

                        }
                    }
                case 4:
                    {
                        if ((chunkData.index_x - 1) < 0)
                        {
                            return true;
                        }
                        return World.Instance.blocktype.BlockTypes[World.Instance.Chunks[chunkData.index_x - 1, chunkData.index_z].chunkData.GetVoxelType(ID + (VoxelData.ChunkWidth * VoxelData.ChunkHeight))].isTransparent;
                    }
                case 5:
                    {
                        if ((chunkData.index_x + 1) >= VoxelData.WorldChunksSize)
                        {
                            return true;
                        }
                        return World.Instance.blocktype.BlockTypes[World.Instance.Chunks[chunkData.index_x + 1, chunkData.index_z].chunkData.GetVoxelType(ID - (VoxelData.ChunkWidth * VoxelData.ChunkHeight))].isTransparent;
                    }
                default:
                    {
                        return false;
                    }

            };


        }
        else
        { return World.Instance.blocktype.BlockTypes[chunkData.GetVoxelType(ID)].isTransparent; }
    }

    public bool IsCoordAllowRender(int ID, int p)
    {
        if (!IsNeighborInChunk(ID, p))
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
                        if ((chunkData.index_z - 1) < 0)
                        {
                            return false;
                        }
                        return World.Instance.blocktype.BlockTypes[World.Instance.Chunks[chunkData.index_x, chunkData.index_z - 1].chunkData.GetVoxelType(ID + (VoxelData.ChunkWidth * VoxelData.ChunkWidth * VoxelData.ChunkHeight))].isRenderNeibor;

                    }
                case 3:
                    {
                        if ((chunkData.index_z + 1) >= VoxelData.WorldChunksSize)
                        {
                            return false;
                        }
                        else
                        {

                            return World.Instance.blocktype.BlockTypes[World.Instance.Chunks[chunkData.index_x, chunkData.index_z + 1].chunkData.GetVoxelType(ID - (VoxelData.ChunkWidth * VoxelData.ChunkWidth * VoxelData.ChunkHeight))].isRenderNeibor;

                        }
                    }
                case 4:
                    {
                        if ((chunkData.index_x - 1) < 0)
                        {
                            return false;
                        }
                        return World.Instance.blocktype.BlockTypes[World.Instance.Chunks[chunkData.index_x - 1, chunkData.index_z].chunkData.GetVoxelType(ID + (VoxelData.ChunkWidth * VoxelData.ChunkHeight))].isRenderNeibor;
                    }
                case 5:
                    {
                        if ((chunkData.index_x + 1) >= VoxelData.WorldChunksSize)
                        {
                            return false;
                        }
                        return World.Instance.blocktype.BlockTypes[World.Instance.Chunks[chunkData.index_x + 1, chunkData.index_z].chunkData.GetVoxelType(ID - (VoxelData.ChunkWidth * VoxelData.ChunkHeight))].isRenderNeibor;
                    }
                default:
                    {
                        return false;
                    }

            };


        }
        else
        { return World.Instance.blocktype.BlockTypes[chunkData.GetVoxelType(ID)].isRenderNeibor; }
    }
    public bool IsCoordSame(int ID, int neighID, int p)
    {
        byte type = chunkData.GetVoxelType(ID);
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
                        if ((chunkData.index_z - 1) < 0)
                        {
                            return false;
                        }
                        neightype = World.Instance.Chunks[chunkData.index_x, chunkData.index_z - 1].chunkData.GetVoxelType(ID + (VoxelData.ChunkWidth * VoxelData.ChunkWidth * VoxelData.ChunkHeight));
                        break;
                    }
                case 3:
                    {
                        if ((chunkData.index_z + 1) >= VoxelData.WorldChunksSize)
                        {
                            return false;
                        }
                        neightype = World.Instance.Chunks[chunkData.index_x, chunkData.index_z + 1].chunkData.GetVoxelType(ID - (VoxelData.ChunkWidth * VoxelData.ChunkWidth * VoxelData.ChunkHeight));
                        break;
                    }
                case 4:
                    {
                        if ((chunkData.index_x - 1) < 0)
                        {
                            return false;
                        }
                        neightype = World.Instance.Chunks[chunkData.index_x - 1, chunkData.index_z].chunkData.GetVoxelType(ID + (VoxelData.ChunkWidth * VoxelData.ChunkHeight));
                        break;
                    }
                case 5:
                    {
                        if ((chunkData.index_x + 1) >= VoxelData.WorldChunksSize)
                        {
                            return false;
                        }
                        neightype = World.Instance.Chunks[chunkData.index_x + 1, chunkData.index_z].chunkData.GetVoxelType(ID - (VoxelData.ChunkWidth * VoxelData.ChunkHeight));
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
            neightype = chunkData.GetVoxelType(neighID);
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


    void AddTexture(int textureID, Vector2 uv)
    {

        float y = textureID / VoxelData.TextureAtlasSizeInBlocks;
        float x = textureID - (y * VoxelData.TextureAtlasSizeInBlocks);

        x *= VoxelData.NormalizedBlockTextureSize;
        y *= VoxelData.NormalizedBlockTextureSize;

        y = 1f - y - VoxelData.NormalizedBlockTextureSize;

        x += VoxelData.NormalizedBlockTextureSize * uv.x;
        y += VoxelData.NormalizedBlockTextureSize * uv.y;

        uvs.Add(new Vector2(x, y));

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

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;
using static BlockInfo;
using static UnityEditor.PlayerSettings;
//[ExecuteInEditMode]
public class World : MonoBehaviour
{
    public int seed;
    public Vector3 spawnPosition;
    public int WorldChunkSize;
    public Texture2DArray[] block;

    public Transform player;
    public Material material;
    public GameObject debugScreen;

    public Vector2Int playerChunk;
    Vector2Int playerLastChunk;

    public Dictionary<Vector3Int, Block> BlockList = new Dictionary<Vector3Int, Block>();
    public Dictionary<Vector2Int, Chunk> Chunks = new Dictionary<Vector2Int, Chunk>();

    List<Chunk> ActiveChunkList = new List<Chunk>();

    public static byte[,,] BlockTypeList ;
    public BlockType[] BlockTypes;

    public Queue<Chunk> chunksToDraw = new Queue<Chunk>();
    List<Chunk> chunksToUpdate = new List<Chunk>();

    private void Start()
    {

        BlockTypeList = new byte[VoxelData.ChunkWidth * (2 * WorldChunkSize + 1), VoxelData.ChunkHeight, VoxelData.ChunkWidth * (2 * WorldChunkSize + 1)];
        GenerateBlock();
        spawnPosition = new Vector3((VoxelData.WorldSizeInChunks * VoxelData.ChunkWidth) / 2f, VoxelData.ChunkHeight - 50f, (VoxelData.WorldSizeInChunks * VoxelData.ChunkWidth) / 2f);
        GenerateWorldChunk();
        GenerateActiveWorldChunk();
        ActiveWorldChunk();
        playerLastChunk = GetChunkIndexFromVector3(player.position);

    }

    private void Update()
    {
        playerChunk = GetChunkIndexFromVector3(player.position);

        // Only update the chunks if the player has moved from the chunk they were previously on.
        if (Chunks.ContainsKey(playerChunk)&& Chunks.ContainsKey(playerLastChunk))
        {
            if (!Chunks[playerChunk].Equals(playerLastChunk))
            {
                CheckViewDistance();
                ActiveWorldChunk();
            }
        }




        if (chunksToDraw.Count > 0)
            lock (chunksToDraw)
            {

                if (chunksToDraw.Peek().isEditable)
                    chunksToDraw.Dequeue().CreateMesh();

            }



    }
    public void GenerateBlock()
    {
        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = -VoxelData.ChunkWidth* WorldChunkSize; x < VoxelData.ChunkWidth * ( WorldChunkSize + 1) ; x++)
            {
                for (int z = -VoxelData.ChunkWidth *  WorldChunkSize ; z < VoxelData.ChunkWidth * (WorldChunkSize + 1) ; z++)
                {
                    Vector3Int index = new Vector3Int(x + VoxelData.ChunkWidth * WorldChunkSize, y, z + VoxelData.ChunkWidth * WorldChunkSize);
                    //Vector3 pos = new Vector3(x , y, z);
                    //BlockList[index] = new Block(pos, index, blocktypes[GetVoxel(pos)]);
                    BlockTypeList[x+ VoxelData.ChunkWidth * WorldChunkSize, y, z+ VoxelData.ChunkWidth * WorldChunkSize] = (byte)GetVoxel(index);
                }
            }
        }
    }


    public void GenerateWorldChunk()
    {
        for (int x = -WorldChunkSize; x <= WorldChunkSize; x++)
        {
            for (int z = -WorldChunkSize; z <= WorldChunkSize; z++)
            {
                Vector2Int ChunkIndex = new Vector2Int(x, z);
                Chunks.Add(ChunkIndex, new Chunk(x, z, this, true));

            }
        }
    }
    public void GenerateActiveWorldChunk()
    {
        for (int x = -VoxelData.ViewDistanceInChunks; x <= VoxelData.ViewDistanceInChunks; x++)
        {
            for (int z = -VoxelData.ViewDistanceInChunks; z <= VoxelData.ViewDistanceInChunks; z++)
            {
                Vector2Int ChunkIndex = new Vector2Int(x, z);

                ActiveChunkList.Add(Chunks[ChunkIndex]);

            }
        }

    }


    //void UpdateChunks()
    //{

    //    if (chunksToUpdate.Count > 0)
    //    foreach (var chunk in chunksToUpdate)
    //    {

    //            chunk._UpdateChunk();
    //            chunksToUpdate.Remove(chunk);

    //    }



    //}
    public int GetVoxel(Vector3Int pos)
    {

        int yPos = Mathf.FloorToInt(pos.y);

        if (yPos == 0)
        { return 1; }

        else if (yPos <= 20 && yPos > 0)
        { return 7; }

        else 
        { return 0; }


    }


    public void ActiveWorldChunk()
    {
        foreach (Chunk c in ActiveChunkList)
        {
            c.chunkObject.SetActive(true);
        }
    }


    public Vector2Int PositionInChunks(Vector3 pos)
    {
        int Chunk_x;
        int Chunk_z;
        if (pos.x >= 0)
        {
            Chunk_x = Mathf.FloorToInt(pos.x / (WorldChunkSize * VoxelData.ChunkWidth));
        }
        else
        {
            Chunk_x = Mathf.FloorToInt(pos.x / (WorldChunkSize * VoxelData.ChunkWidth)) - 1;
        }

        if (pos.z >= 0)
        {
            Chunk_z = Mathf.FloorToInt(pos.z / (WorldChunkSize * VoxelData.ChunkWidth));
        }
        else
        {
            Chunk_z = Mathf.FloorToInt(pos.z / (WorldChunkSize * VoxelData.ChunkWidth)) - 1;
        }

        return new Vector2Int(Chunk_x, Chunk_z);
    }

    public Vector3Int PositionInBlock(Vector3 pos)
    {
        int Chunk_x = Mathf.FloorToInt(pos.x);
        int Chunk_y = Mathf.FloorToInt(pos.y);
        int Chunk_z = Mathf.FloorToInt(pos.z);

        return new Vector3Int(Chunk_x, Chunk_y, Chunk_z);
    }

    public bool CheckVoxelSolid(Vector3 pos)
    {

        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);
       
        if (!BlockList.ContainsKey(new Vector3Int(x, y, z)))
            return false;
        else
        {
            return BlockTypes[BlockTypeList[x+ VoxelData.ChunkWidth * WorldChunkSize, y,z+ VoxelData.ChunkWidth * WorldChunkSize]].isSolid;
        }


    }

    public bool CheckIfVoxelTransparent(Vector3 pos)
    {

        Vector2Int chunkindex = GetChunkIndexFromVector3(pos);
        Vector3Int blockindex = PositionInBlock(pos);
        Vector3Int blocktypeindex = PositionInBlock(pos)+new Vector3Int(VoxelData.ChunkWidth * WorldChunkSize,0,VoxelData.ChunkWidth * WorldChunkSize);

        if (!IsChunkInWorld(chunkindex) || pos.y < 0 || pos.y > VoxelData.ChunkHeight)
            return false;

        if (Chunks[chunkindex] != null && Chunks[chunkindex].isVoxelMapPopulated)
            
            return BlockTypes[BlockTypeList[blocktypeindex.x+ VoxelData.ChunkWidth * WorldChunkSize, blocktypeindex.y,blocktypeindex.z+ VoxelData.ChunkWidth * WorldChunkSize]].isTransparent;

        return BlockTypes[GetVoxel(blockindex)].isTransparent;

    }
    public void UpdateChunks(Vector3 pos, byte newID)
    {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);
        Vector2Int ChunckIndex = GetChunkIndexFromVector3(pos);
        Vector3Int index = new Vector3Int(x,y,z);
        BlockList[index].SetBlockType(newID);
        BlockTypeList[x + VoxelData.ChunkWidth * WorldChunkSize, y ,z + VoxelData.ChunkWidth * WorldChunkSize] = newID;
        if (BlockList.ContainsKey(index))
        {
            
            //if ((x - (ChunckIndex.x * VoxelData.ChunkWidth)) != 0 && (x - ((ChunckIndex.x) * VoxelData.ChunkWidth)) != (VoxelData.ChunkWidth - 1) && (z - ((ChunckIndex.y) * VoxelData.ChunkWidth)) != 0 && (z - ((ChunckIndex.y) * VoxelData.ChunkWidth)) != (VoxelData.ChunkWidth - 1))
            //{
            //    chunksToUpdate.Add(Chunks[ChunckIndex]);
            //}
            if (x - (ChunckIndex.x * VoxelData.ChunkWidth) == 0)
            {
                Chunks[new Vector2Int(ChunckIndex.x - 1, ChunckIndex.y)]._UpdateChunk();
            }
            if ((x - (ChunckIndex.x * VoxelData.ChunkWidth)) == (VoxelData.ChunkWidth - 1))
            {
                Chunks[new Vector2Int(ChunckIndex.x + 1, ChunckIndex.y)]._UpdateChunk();
            }
            if ((z - (ChunckIndex.y * VoxelData.ChunkWidth)) == 0)
            {
                Chunks[new Vector2Int(ChunckIndex.x, ChunckIndex.y - 1)]._UpdateChunk();
            }
            if ((z - (ChunckIndex.y * VoxelData.ChunkWidth)) == (VoxelData.ChunkWidth - 1))
            {
                Chunks[new Vector2Int(ChunckIndex.x, ChunckIndex.y + 1)]._UpdateChunk();
            }
            Chunks[ChunckIndex]._UpdateChunk();
            //else if (((x - (ChunckIndex.x * VoxelData.ChunkWidth)) == 0)&& ((z - (ChunckIndex.y * VoxelData.ChunkWidth)) == 0))
            //{
            //    chunksToUpdate.Add(Chunks[ChunckIndex]);
            //    chunksToUpdate.Add(Chunks[new Vector2Int(ChunckIndex.x - 1, ChunckIndex.y)]);
            //    chunksToUpdate.Add(Chunks[new Vector2Int(ChunckIndex.x, ChunckIndex.y - 1)]);

            //}
            //else if (((x - (ChunckIndex.x * VoxelData.ChunkWidth)) == 0) && ((z - (ChunckIndex.y * VoxelData.ChunkWidth)) == (VoxelData.ChunkWidth - 1)))
            //{
            //    chunksToUpdate.Add(Chunks[ChunckIndex]);
            //    chunksToUpdate.Add(Chunks[new Vector2Int(ChunckIndex.x - 1, ChunckIndex.y)]);
            //    chunksToUpdate.Add(Chunks[new Vector2Int(ChunckIndex.x, ChunckIndex.y + 1)]);

            //}
            //else if ((x - (ChunckIndex.x * VoxelData.ChunkWidth)) == (VoxelData.ChunkWidth - 1) && ((z - (ChunckIndex.y * VoxelData.ChunkWidth)) == 0))
            //{
            //    chunksToUpdate.Add(Chunks[ChunckIndex]);
            //    chunksToUpdate.Add(Chunks[new Vector2Int(ChunckIndex.x + 1, ChunckIndex.y)]);
            //    chunksToUpdate.Add(Chunks[new Vector2Int(ChunckIndex.x, ChunckIndex.y - 1)]);

            //}
            //else if ((x - (ChunckIndex.x * VoxelData.ChunkWidth)) == (VoxelData.ChunkWidth - 1) && ((z - (ChunckIndex.y * VoxelData.ChunkWidth)) == (VoxelData.ChunkWidth - 1)))
            //{
            //    chunksToUpdate.Add(Chunks[ChunckIndex]);
            //    chunksToUpdate.Add(Chunks[new Vector2Int(ChunckIndex.x + 1, ChunckIndex.y)]);
            //    chunksToUpdate.Add(Chunks[new Vector2Int(ChunckIndex.x, ChunckIndex.y + 1)]);

            //}
        }
        else
        {
            Debug.Log("Out Of Range !");
        }


    }

    public Chunk GetChunkFromVector3(Vector3 pos)
    {

        int x = Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth);
        int z = Mathf.FloorToInt(pos.z / VoxelData.ChunkWidth);

        return Chunks[new Vector2Int(x, z)];

    }

    public Vector2Int GetChunkIndexFromVector3(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth);
        int z = Mathf.FloorToInt(pos.z / VoxelData.ChunkWidth);

        return new Vector2Int(x, z);
    }

    void CheckViewDistance()
    {

        Vector2Int coord = GetChunkIndexFromVector3(player.position);
        playerLastChunk = playerChunk;

        List<Chunk> previouslyActiveChunks = new List<Chunk>(ActiveChunkList);
        ActiveChunkList.Clear();
        // Loop through all chunks currently within view distance of the player.
        for (int x = coord.x - VoxelData.ViewDistanceInChunks; x <= coord.x + VoxelData.ViewDistanceInChunks; x++)
        {
            for (int z = coord.y - VoxelData.ViewDistanceInChunks; z <= coord.y + VoxelData.ViewDistanceInChunks; z++)
            {
                
                Vector2Int index = new Vector2Int(x, z);
                // If the current chunk is in the world...
                if (IsChunkInWorld(index))
                {

                    if (!Chunks[index].isActive)
                    {
                        Chunks[index].isActive = true;
                    }
                    ActiveChunkList.Add(Chunks[index]);

                    // Check through previously active chunks to see if this chunk is there. If it is, remove it from the list.
                    for (int i = 0; i < previouslyActiveChunks.Count; i++)
                    {

                        if (previouslyActiveChunks[i].Equals(Chunks[index]))
                        {
                            previouslyActiveChunks.RemoveAt(i);
                        }
                            

                    }
                }

                foreach (Chunk c in previouslyActiveChunks)
                {
                    c.chunkObject.SetActive(false);
                }
                

            }
        }


    }

    bool IsChunkInWorld(Vector2Int coord)
    {

        if (Chunks.ContainsKey(coord))
            return true;
        else
            return
                false;

    }

    

}

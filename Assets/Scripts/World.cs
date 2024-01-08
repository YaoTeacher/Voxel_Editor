using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;
using static BlockInfo;
using static UnityEditor.PlayerSettings;
using static UnityEngine.Rendering.VolumeComponent;
//[ExecuteInEditMode]
public class World : MonoBehaviour
{
    public int seed;
    public Vector3 spawnPosition;
    public int WorldChunkSize;
    //public Texture2DArray[] block;

    public Transform player;
    public Material material;
    public GameObject debugScreen;
    public BlockInfo blocktype;

    Vector2Int playerChunk;
    Vector2Int playerLastChunk;

    public Dictionary<Vector3Int, Block> BlockList = new Dictionary<Vector3Int, Block>();
    public  Chunk[,] Chunks ;

    List<Chunk> ActiveChunkList = new List<Chunk>();

    bool applyingModifications = false;

    public static byte[,,] BlockTypeList ;
    

    public Queue<Chunk> chunksToDraw = new Queue<Chunk>();
    List<Chunk> chunksToUpdate = new List<Chunk>();

    Queue<Queue<VoxelMod>> modifications = new Queue<Queue<VoxelMod>>();

    public bool _inUI = false;

    private void Awake()
    {
        spawnPosition = new Vector3(((WorldChunkSize + 1) * VoxelData.ChunkWidth) *VoxelData.BlockSize/ 2f, (VoxelData.ChunkHeight - 50) * VoxelData.BlockSize, ((WorldChunkSize + 1) * VoxelData.ChunkWidth) * VoxelData.BlockSize / 2f);
    }
    private void Start()
    {
        
        BlockTypeList = new byte[VoxelData.ChunkWidth * (WorldChunkSize + 1), VoxelData.ChunkHeight, VoxelData.ChunkWidth * (WorldChunkSize + 1)];
        Chunks =new Chunk[WorldChunkSize + 1, WorldChunkSize + 1];
        GenerateBlock();
        print(spawnPosition);
        player.transform.position = spawnPosition;
        print(player.position);
        playerLastChunk = GetChunkIndexFromVector3(player.position);
        GenerateWorldChunk();
        GenerateActiveWorldChunk();
        ActiveWorldChunk();
        

    }

    private void Update()
    {
        playerChunk = GetChunkIndexFromVector3(player.position);
        // Only update the chunks if the player has moved from the chunk they were previously on.
        if(playerChunk.x>WorldChunkSize|| playerChunk.y > WorldChunkSize||playerChunk.x <0 || playerChunk.y < 0)
        {
            return;
        }
        else
        {
            if (Chunks[playerChunk.x, playerChunk.y] != null)
            {

                if (!Chunks[playerChunk.x, playerChunk.y].Equals(playerLastChunk))
                {
                    CheckViewDistance();
                    ActiveWorldChunk();
                }
            }
        }


        if (!applyingModifications)
            ApplyModifications();

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
            for (int x = 0; x < VoxelData.ChunkWidth * ( WorldChunkSize + 1) ; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth * (WorldChunkSize + 1) ; z++)
                {
                    Vector3Int index = new Vector3Int(x , y, z );
                    //Vector3 pos = new Vector3(x , y, z);
                    //BlockList[index] = new Block(pos, index, blocktypes[GetVoxel(pos)]);
                    BlockTypeList[x ,y, z] = (byte)GetVoxel(index);
                }
            }
        }
    }


    public void GenerateWorldChunk()
    {
        for (int x = 0; x <= WorldChunkSize; x++)
        {
            for (int z = 0; z <= WorldChunkSize; z++)
            {
                Vector2Int ChunkIndex = new Vector2Int(x, z);
                Chunks[x,z]=new Chunk(x, z, this, true);

            }
        }
    }
    public void GenerateActiveWorldChunk()
    {
        print("!");
        print(playerLastChunk);
        for (int x = playerLastChunk.x - VoxelData.ViewDistanceInChunks; x <= playerLastChunk.x + VoxelData.ViewDistanceInChunks; x++)
        {
            
            if (x < 0 || x > WorldChunkSize)
            {
                print(x);
                continue;
            }
             for (int z = playerLastChunk.y - VoxelData.ViewDistanceInChunks; z <= playerLastChunk.y + VoxelData.ViewDistanceInChunks; z++)
             {
                if (z < 0 || z > WorldChunkSize)
                {
                    continue;
                }


                    ActiveChunkList.Add(Chunks[x, z]);
                    print(Chunks[x, z].X + "+" + Chunks[x, z].Z);




             }
            
        }

    }
    void ApplyModifications()
    {

        applyingModifications = true;

        while (modifications.Count > 0)
        {

            Queue<VoxelMod> queue = modifications.Dequeue();

            while (queue.Count > 0)
            {

                VoxelMod v = queue.Dequeue();

                Chunk c = GetChunkFromVector3(v.position);

                if (Chunks[c.X,c.Z] == null)
                {
                    Chunks[c.X, c.Z] = c;
                    ActiveChunkList.Add(c);
                }

                Chunks[c.X, c.Z].modifications.Enqueue(v);

                if (!chunksToUpdate.Contains(Chunks[c.X, c.Z]))
                    chunksToUpdate.Add(Chunks[c.X, c.Z]);

            }
        }

        applyingModifications = false;

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

        else if (yPos <= 70 && yPos > 0)
        { return 6; }

        else 
        { return 0; }


    }


    public void ActiveWorldChunk()
    {
        foreach (Chunk c in ActiveChunkList)
        {
            c.chunkObject.SetActive(true);
            print(c.X + "+" + c.Z);
        }
    }


    public Vector2Int PositionInChunks(Vector3 pos)
    {
        int Chunk_x;
        int Chunk_z;
        if (pos.x >= 0)
        {
            Chunk_x = Mathf.FloorToInt(pos.x / (WorldChunkSize * VoxelData.ChunkWidth) / VoxelData.BlockSize);
        }
        else
        {
            Chunk_x = Mathf.FloorToInt(pos.x / (WorldChunkSize * VoxelData.ChunkWidth) / VoxelData.BlockSize) - 1;
        }

        if (pos.z >= 0)
        {
            Chunk_z = Mathf.FloorToInt(pos.z / (WorldChunkSize * VoxelData.ChunkWidth) / VoxelData.BlockSize);
        }
        else
        {
            Chunk_z = Mathf.FloorToInt(pos.z / (WorldChunkSize * VoxelData.ChunkWidth) / VoxelData.BlockSize) - 1;
        }

        return new Vector2Int(Chunk_x, Chunk_z);
    }

    public Vector3Int PositionInBlock(Vector3 pos)
    {
        pos = pos /VoxelData.BlockSize;
        int Chunk_x = Mathf.FloorToInt(pos.x );
        int Chunk_y = Mathf.FloorToInt(pos.y );
        int Chunk_z = Mathf.FloorToInt(pos.z );


        return new Vector3Int(Chunk_x, Chunk_y, Chunk_z);
    }

    public bool CheckVoxelSolid(Vector3 pos)
    {
        Vector3Int index = PositionInBlock(pos);
       
        if (!BlockList.ContainsKey(index))
            return false;
        else
        {
            return blocktype.BlockTypes[BlockTypeList[index.x , index.y, index.z]].isSolid;
        }


    }

    public bool CheckIfVoxelTransparent(Vector3 pos)
    {

        Vector2Int chunkindex = GetChunkIndexFromVector3(pos);
        Vector3Int blockindex = PositionInBlock(pos);
        Vector3Int blocktypeindex = PositionInBlock(pos);

        if (!IsChunkInWorld(chunkindex) || pos.y < 0 || pos.y > VoxelData.ChunkHeight)
            return false;

        if (Chunks[chunkindex.x,chunkindex.y] != null && Chunks[chunkindex.x, chunkindex.y].isVoxelMapPopulated)
            
            return blocktype.BlockTypes[BlockTypeList[blocktypeindex.x, blocktypeindex.y,blocktypeindex.z]].isTransparent;

        return blocktype.BlockTypes[GetVoxel(blockindex)].isTransparent;

    }
    public void UpdateChunks(Vector3 pos, byte newID)
    {
        Vector3Int index = PositionInBlock(pos);
        print("1:" + index);
        Vector2Int ChunckIndex = GetChunkIndexFromVector3(pos);
        BlockList[index].SetBlockType(newID);
        BlockTypeList[index.x, index.y, index.z] = newID;
        if (BlockList.ContainsKey(index))
        {
            
            //if ((x - (ChunckIndex.x * VoxelData.ChunkWidth)) != 0 && (x - ((ChunckIndex.x) * VoxelData.ChunkWidth)) != (VoxelData.ChunkWidth - 1) && (z - ((ChunckIndex.y) * VoxelData.ChunkWidth)) != 0 && (z - ((ChunckIndex.y) * VoxelData.ChunkWidth)) != (VoxelData.ChunkWidth - 1))
            //{
            //    chunksToUpdate.Add(Chunks[ChunckIndex]);
            //}
            if (index.x - (ChunckIndex.x * VoxelData.ChunkWidth) == 0)
            {
                Chunks[ChunckIndex.x - 1, ChunckIndex.y]._UpdateChunk();
            }
            if ((index.x - (ChunckIndex.x * VoxelData.ChunkWidth)) == (VoxelData.ChunkWidth - 1))
            {
                Chunks[ChunckIndex.x + 1, ChunckIndex.y]._UpdateChunk();
            }
            if ((index.z-((ChunckIndex.y) * VoxelData.ChunkWidth)) == 0)
            {
                Chunks[ChunckIndex.x, ChunckIndex.y - 1]._UpdateChunk();
            }
            if ((index.z - ((ChunckIndex.y) * VoxelData.ChunkWidth)) == (VoxelData.ChunkWidth - 1))
            {
                Chunks[ChunckIndex.x, ChunckIndex.y + 1]._UpdateChunk();
            }
            Chunks[ChunckIndex.x, ChunckIndex.y]._UpdateChunk();
        }
        else
        {
            Debug.Log("Out Of Range !");
        }


    }

    public Chunk GetChunkFromVector3(Vector3 pos)
    {
        Vector2Int index = GetChunkIndexFromVector3(pos);

        return Chunks[index.x,index.y];

    }

    public Vector2Int GetChunkIndexFromVector3(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth/VoxelData.BlockSize);
        int z = Mathf.FloorToInt(pos.z / VoxelData.ChunkWidth/VoxelData.BlockSize);

        return new Vector2Int(x, z);
    }

    void CheckViewDistance()
    {

        Vector2Int coord = GetChunkIndexFromVector3(player.position);
        print(coord);
        playerLastChunk = playerChunk;

        List<Chunk> previouslyActiveChunks = new List<Chunk>(ActiveChunkList);
        ActiveChunkList.Clear();
        // Loop through all chunks currently within view distance of the player.
        for (int x = coord.x- VoxelData.ViewDistanceInChunks; x <= coord.x + VoxelData.ViewDistanceInChunks; x++)
        {
            if (x < 0||x> WorldChunkSize)
            {
                continue;
            }
            for (int z = coord.y - VoxelData.ViewDistanceInChunks; z <= coord.y + VoxelData.ViewDistanceInChunks; z++)
            {
                if (z < 0 || z > WorldChunkSize)
                {
                    continue;
                }
                // If the current chunk is in the world...
                if (IsChunkInWorld(new Vector2Int(x,z)))
                {

                    if (!Chunks[x,z].isActive)
                    {
                        Chunks[x, z].isActive = true;
                    }
                    print(x +","+ z);
                    ActiveChunkList.Add(Chunks[x, z]);

                    // Check through previously active chunks to see if this chunk is there. If it is, remove it from the list.
                    for (int i = 0; i < previouslyActiveChunks.Count; i++)
                    {

                        if (previouslyActiveChunks[i].Equals(Chunks[x, z]))
                        {
                            previouslyActiveChunks.RemoveAt(i);
                        }
                            

                    }
                }

                foreach (Chunk c in previouslyActiveChunks)
                {
                    c.chunkObject.SetActive(false);
                    print(c.X+"+"+c.Z);
                }
                

            }
        }

    }

    public bool inUI
    {
        get { return _inUI; }
        set
        {
            _inUI = value;
        }
    }

    bool IsChunkInWorld(Vector2Int coord)
    {

        if (Chunks[coord.x,coord.y]!=null)
            return true;
        else
            return
                false;

    }

    


}

public class VoxelMod
{

    public Vector3 position;
    public byte id;

    public VoxelMod()
    {

        position = new Vector3();
        id = 0;

    }

    public VoxelMod(Vector3 _position, byte _id)
    {

        position = _position;
        id = _id;

    }

}

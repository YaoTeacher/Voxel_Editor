using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;
using System.IO;
using static UnityEngine.Rendering.VolumeComponent;

//[ExecuteInEditMode]
public class World : MonoBehaviour
{
    public int seed;
    public Vector3 spawnPosition;

    public Settings settings;
    //public Texture2DArray[] block;

    public Transform player;
    public Material material;
    public GameObject debugScreen;
    public BlockInfo blocktype;

    Vector2Int playerChunk;
    Vector2Int playerLastChunk;

    public Chunk[,] Chunks = new Chunk[VoxelData.WorldChunksSize + 1, VoxelData.WorldChunksSize + 1];

    List<Chunk> ActiveChunks = new List<Chunk>();

    bool applyingModifications = false;

    //public byte[,,] BlockTypeList = new byte[VoxelData.ChunkWidth * (VoxelData.WorldChunksSize + 1), VoxelData.ChunkHeight, VoxelData.ChunkWidth * (VoxelData.WorldChunksSize + 1)];

    List<Chunk> chunksToCreate = new List<Chunk>();
    public Queue<Chunk> chunksToDraw = new Queue<Chunk>();
    public List<Chunk> chunksToUpdate = new List<Chunk>();

    Queue<Queue<VoxelMod>> modifications = new Queue<Queue<VoxelMod>>();

    public bool _inUI = false;

    Thread ChunkUpdateThread;
    public object ChunkUpdateThreadLock = new object();

    public object ChunkListThreadLock = new object();

    private static World _instance;
    public static World Instance { get { return _instance; } }

    public WorldData worldData;

    public string appPath;
    private void Awake()
    {

        // If the instance value is not null and not *this*, we've somehow ended up with more than one World component.
        // Since another one has already been assigned, delete this one.
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        // Else set instance to this.
        else
            _instance = this;

        appPath = Application.persistentDataPath;

        spawnPosition = new Vector3(((VoxelData.WorldChunksSize + 1) * VoxelData.ChunkWidth) *VoxelData.BlockSize/ 2f, (VoxelData.ChunkHeight - 50) * VoxelData.BlockSize, ((VoxelData.WorldChunksSize + 1) * VoxelData.ChunkWidth) * VoxelData.BlockSize / 2f);
    }
    private void Start()
    {
        worldData = SaveSystem.LoadWorld("Testing");

        string jsonImport = File.ReadAllText("D:\\Unity Project\\Voxel_Editor\\Voxel_Editor\\Assets\\settings.cfg");
        settings = JsonUtility.FromJson<Settings>(jsonImport);
        LoadWorld();
        player.transform.position = spawnPosition;
        GenerateWorldChunk();
        playerLastChunk = GetChunkIndexFromPos(player.position);


        if (settings.enableThreading)
        {
            ChunkUpdateThread = new Thread(new ThreadStart(ThreadedUpdate));
            ChunkUpdateThread.Start();
        }

    }

    private void Update()
    {
        playerChunk = GetChunkIndexFromPos(player.position);

        // Only update the chunks if the player has moved from the chunk they were previously on.
        if (!playerChunk.Equals(playerLastChunk))
            CheckViewDistance();

        if (Instance.chunksToCreate.Count > 0)
            CreateChunk();

        if (Instance.chunksToDraw.Count > 0)
        {
            Debug.Log(chunksToDraw.Count);
            Instance.chunksToDraw.Dequeue().CreateMesh();
        }


        if (!settings.enableThreading)
        {

            if (!applyingModifications)
                ApplyModifications();

            if (Instance.chunksToUpdate.Count > 0)
            {
                Debug.Log(chunksToUpdate.Count);
                UpdateChunks();
            }
                

        }

        if (Input.GetKeyDown(KeyCode.F3))
            debugScreen.SetActive(!debugScreen.activeSelf);

        if (Input.GetKeyDown(KeyCode.B))
            SaveSystem.SaveWorld(worldData);



    }

    void UpdateChunks()
    {

        lock (ChunkUpdateThreadLock)
        {

            Instance.chunksToUpdate[0].UpdateChunk();
            if (!ActiveChunks.Contains(chunksToUpdate[0]))
                ActiveChunks.Add(chunksToUpdate[0]);
            Instance. chunksToUpdate.RemoveAt(0);

        }
    }
    void LoadWorld()
    {

        for (int x = 0; x < VoxelData.WorldChunksSize; x++)
        {
            for (int z = 0; z < VoxelData.WorldChunksSize; z++)
            {
                int ChunkID = Chunk.GetChunkIntID(new Vector2Int(x, z));
                worldData.LoadChunk(ChunkID);

            }
        }
    }


    public void GenerateWorldChunk()
    {
        for (int x = 0; x < VoxelData.WorldChunksSize; x++)
        {
            for (int z = 0; z < VoxelData.WorldChunksSize; z++)
            {
                Chunks[x,z]=new Chunk(x, z, this, true);
                Instance.chunksToCreate.Add(Chunks[x, z]);
                Debug.Log(Instance.chunksToCreate.Count);
            }
        }

        player.position = spawnPosition;
        CheckViewDistance();
    }

    void CreateChunk()
    {

        Chunk c = Instance.chunksToCreate[0];
        Instance.chunksToCreate.RemoveAt(0);
        Chunks[c.X, c.Z].Init();

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

                Chunk c = GetChunkFromPos(v.index);

                if (Chunks[c.X,c.Z] == null)
                {
                    Chunks[c.X, c.Z] = c;
                    ActiveChunks.Add(c);
                }

                Chunks[c.X, c.Z].modifications.Enqueue(v);

                if (!chunksToUpdate.Contains(Chunks[c.X, c.Z]))
                    chunksToUpdate.Add(Chunks[c.X, c.Z]);

            }
        }

        applyingModifications = false;

    }

    void ThreadedUpdate()
    {

        while (true)
        {

            if (!applyingModifications)
                ApplyModifications();

            if (chunksToUpdate.Count > 0)
                UpdateChunks();

        }

    }

    private void OnDisable()
    {

        if (settings.enableThreading)
        {
            ChunkUpdateThread.Abort();
        }

    }

    public static byte GetVoxel(Vector3Int pos)
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
        foreach (Chunk c in ActiveChunks)
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
            Chunk_x = Mathf.FloorToInt(pos.x / (VoxelData.WorldChunksSize * VoxelData.ChunkWidth) / VoxelData.BlockSize);
        }
        else
        {
            Chunk_x = Mathf.FloorToInt(pos.x / (VoxelData.WorldChunksSize * VoxelData.ChunkWidth) / VoxelData.BlockSize) - 1;
        }

        if (pos.z >= 0)
        {
            Chunk_z = Mathf.FloorToInt(pos.z / (VoxelData.WorldChunksSize * VoxelData.ChunkWidth) / VoxelData.BlockSize);
        }
        else
        {
            Chunk_z = Mathf.FloorToInt(pos.z / (VoxelData.WorldChunksSize * VoxelData.ChunkWidth) / VoxelData.BlockSize) - 1;
        }

        return new Vector2Int(Chunk_x, Chunk_z);
    }



    public bool CheckVoxelSolid(Vector3 pos)
    {
        Vector2Int ChunkIndex = GetChunkIndexFromPos(pos);
        int ChunkID = Chunk.GetChunkIntID(ChunkIndex);
        Vector3Int index = GetWorldIndexFromPos(pos);
        index = index - new Vector3Int(ChunkIndex.x*VoxelData.ChunkWidth,0, ChunkIndex.y * VoxelData.ChunkWidth);
        int BlockIDinChunk = Chunk.GetBlockIntID(index);

        if (ChunkID<0||ChunkID>VoxelData.WorldChunksSize* VoxelData.WorldChunksSize)
        {
            return false;
        }

        if (!worldData.Chunks[ChunkID].BlockList.ContainsKey(BlockIDinChunk))
            return false;
        else
        {
            return blocktype.BlockTypes[worldData.Chunks[ChunkID].BlockList[BlockIDinChunk].GetBlockType()].isSolid;
        }


    }

    public bool CheckIfVoxelTransparent(Vector3 pos)
    {

        Vector2Int chunkindex = GetChunkIndexFromPos(pos);
        Vector3Int blockindex = GetWorldIndexFromPos(pos);
        int chunkID = Chunk.GetChunkIntID(chunkindex);
        blockindex = new Vector3Int(blockindex.x - (chunkindex.x * VoxelData.ChunkWidth), 0, blockindex.z - (chunkindex.y * VoxelData.ChunkWidth));
        int blockID = Chunk.GetBlockIntID(blockindex);

        if (!IsChunkInWorld(chunkID) || pos.y < 0 || pos.y > VoxelData.ChunkHeight)
            return false;

        else if (Chunks[chunkindex.x,chunkindex.y] != null && Chunks[chunkindex.x, chunkindex.y].isVoxelMapPopulated)
            
            return blocktype.BlockTypes[worldData.Chunks[chunkID].BlockList[blockID].GetBlockType()].isTransparent;
        else { return false; }
    }

    public static Vector3Int GetWorldIndexFromPos(Vector3 pos)
    {
        pos = pos / VoxelData.BlockSize;
        int Chunk_x = Mathf.FloorToInt(pos.x);
        int Chunk_y = Mathf.FloorToInt(pos.y);
        int Chunk_z = Mathf.FloorToInt(pos.z);


        return new Vector3Int(Chunk_x, Chunk_y, Chunk_z);
    }

    public static Vector3Int GetIndexInChunkFromPos(Vector3 pos)
    {
        Vector3Int WorldIndex = GetWorldIndexFromPos(pos);
        Vector2Int ChunkIndex = GetChunkIndexFromPos(pos);
        Vector3Int Index = new Vector3Int(WorldIndex.x-(ChunkIndex.x*VoxelData.ChunkWidth), WorldIndex.y,WorldIndex.z-(ChunkIndex.y * VoxelData.ChunkWidth));
        return Index;
        
    }

    public Chunk GetChunkFromPos(Vector3 pos)
    {
        Vector2Int index = GetChunkIndexFromPos(pos);

        return Chunks[index.x,index.y];

    }

    public static Vector2Int GetChunkIndexFromPos(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth/VoxelData.BlockSize);
        int z = Mathf.FloorToInt(pos.z / VoxelData.ChunkWidth/VoxelData.BlockSize);

        return new Vector2Int(x, z);
    }

    void CheckViewDistance()
    {

        Vector2Int coord = GetChunkIndexFromPos(player.position);
        print(coord);
        playerLastChunk = playerChunk;

        List<Chunk> previouslyActiveChunks = new List<Chunk>(ActiveChunks);
        ActiveChunks.Clear();
        // Loop through all chunks currently within view distance of the player.
        for (int x = coord.x- VoxelData.ViewDistanceInChunks; x <= coord.x + VoxelData.ViewDistanceInChunks; x++)
        {
            if (x < 0||x>= VoxelData.WorldChunksSize)
            {
                continue;
            }
            for (int z = coord.y - VoxelData.ViewDistanceInChunks; z <= coord.y + VoxelData.ViewDistanceInChunks; z++)
            {
                if (z < 0 || z >= VoxelData.WorldChunksSize)
                {
                    continue;
                }
                // If the current chunk is in the world...
                int chunkID = Chunk.GetChunkIntID(new Vector2Int(x, z));
                if (IsChunkInWorld(chunkID))
                {

                    if (!Chunks[x,z].isActive)
                    {
                        Chunks[x, z].isActive = true;
                    }
                    ActiveChunks.Add(Chunks[x, z]);

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
                    Chunks[c.X, c.Z].isActive = false;
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

    bool IsChunkInWorld(int ID)
    {

        return worldData.Chunks.ContainsKey(ID);

    }

    


}

public class VoxelMod
{

    public Vector3Int index;
    public byte id;

    public VoxelMod()
    {

        index = new Vector3Int();
        id = 0;

    }

    public VoxelMod(Vector3Int _index, byte _id)
    {

        index = _index;
        id = _id;

    }

}

[System.Serializable]
public class Settings
{

    [Header("Game Data")]
    public string version = "0.0.0.01";

    [Header("Performance")]
    public int viewDistance = VoxelData.ViewDistanceInChunks;
    public int loadDistance = VoxelData.WorldChunksSize; // Cannot be lower than viewDistance, validation in Settings Menu to come...
    public bool enableThreading = true;
    public bool enableAnimatedChunks = false;

    [Header("Controls")]
    [Range(0.1f, 10f)]
    public float mouseSensitivity = 2.0f;

}


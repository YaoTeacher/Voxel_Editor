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

    }
    private void Start()
    {
        worldData = SaveSystem.LoadWorld("Testing");

        string jsonImport = File.ReadAllText("D:\\Unity Project\\Voxel_Editor\\Voxel_Editor\\Assets\\settings.cfg");
        settings = JsonUtility.FromJson<Settings>(jsonImport);
        LoadWorld();
        GenerateWorld();
        spawnPosition = new Vector3(VoxelData.WorldCentre, VoxelData.ChunkHeight - 50f, VoxelData.WorldCentre);
        player.transform.position = spawnPosition;
        CheckViewDistance();
        playerLastChunk = GetChunkIndexFromPos(player.position);


        if (settings.enableThreading)
        {
            Debug.Log("!!"+chunksToUpdate.Count);
            ChunkUpdateThread = new Thread(new ThreadStart(ThreadedUpdate));
            ChunkUpdateThread.Start();
        }

    }

    private void Update()
    {
        playerChunk = GetChunkIndexFromPos(player.position);

        // Only update the chunks if the player has moved from the chunk they were previously on.
        if (!playerChunk.Equals(playerLastChunk))
        { CheckViewDistance(); }
            


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

    void GenerateWorld()
    {

        for (int x = 0; x < VoxelData.WorldChunksSize ; x++)
        {
            for (int z = 0; z < VoxelData.WorldChunksSize; z++)
            {

                Chunks[x, z] = new Chunk(x,z);

            }
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

    void ApplyModifications()
    {

        applyingModifications = true;

        while (modifications.Count > 0)
        {

            Queue<VoxelMod> queue = modifications.Dequeue();

            while (queue.Count > 0)
            {

                VoxelMod v = queue.Dequeue();

                worldData.SetVoxel(v.index, v.id);

            }
        }

        applyingModifications = false;

    }
    public void AddChunkToUpdate(Chunk chunk)
    {

        AddChunkToUpdate(chunk, false);

    }

    public void AddChunkToUpdate(Chunk chunk, bool insert)
    {

        // Lock list to ensure only one thing is using the list at a time.
        lock (ChunkUpdateThreadLock)
        {

            // Make sure update list doesn't already contain chunk.
            if (!chunksToUpdate.Contains(chunk))
            {
                // If insert is true, chunk gets inserted at the top of the list.
                if (insert)
                    chunksToUpdate.Insert(0, chunk);
                else
                    chunksToUpdate.Add(chunk);

            }
        }
    }

    void UpdateChunks()
    {

        lock (ChunkUpdateThreadLock)
        {

            Instance.chunksToUpdate[0].UpdateChunk();
            if (!ActiveChunks.Contains(chunksToUpdate[0]))
                ActiveChunks.Add(chunksToUpdate[0]);
            Instance.chunksToUpdate.RemoveAt(0);

        }
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


    public bool CheckForVoxel(Vector3 pos)
    {

        Block block = worldData.GetVoxel(pos);

        if (block != null)
        {
            if (blocktype.BlockTypes[block.GetBlockType()].isSolid)
                return true;
            else
                return false;
        }
        else
        {
            return false;
        }



    }

    //public bool CheckVoxelSolid(Vector3 pos)
    //{
    //    Vector2Int ChunkIndex = GetChunkIndexFromPos(pos);
    //    int ChunkID = Chunk.GetChunkIntID(ChunkIndex);
    //    Vector3Int index = GetWorldIndexFromPos(pos);
    //    index = index - new Vector3Int(ChunkIndex.x*VoxelData.ChunkWidth,0, ChunkIndex.y * VoxelData.ChunkWidth);
    //    int BlockIDinChunk = Chunk.GetBlockIntID(index);

    //    if (ChunkID<0||ChunkID>VoxelData.WorldChunksSize* VoxelData.WorldChunksSize)
    //    {
    //        return false;
    //    }

    //    if (!worldData.Chunks[ChunkID].BlockList.ContainsKey(BlockIDinChunk))
    //        return false;
    //    else
    //    {
    //        return blocktype.BlockTypes[worldData.Chunks[ChunkID].BlockList[BlockIDinChunk].GetBlockType()].isSolid;
    //    }


    //}

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

    public static Vector2Int GetChunkIndexFromWorldIndex(Vector3Int index)
    {
        int x = Mathf.FloorToInt(index.x / VoxelData.ChunkWidth);
        int z = Mathf.FloorToInt(index.z / VoxelData.ChunkWidth);

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

                    Chunks[x, z].isActive = true;

                    ActiveChunks.Add(Chunks[x, z]);

                }
                for (int i = 0; i < previouslyActiveChunks.Count; i++)
                {

                    if (previouslyActiveChunks[i].Equals(Chunks[x, z]))
                    {
                        previouslyActiveChunks.RemoveAt(i);
                    }

                }

                

            }
        }

        foreach (Chunk c in previouslyActiveChunks)
        {
            Chunks[c.X, c.Z].isActive = false;
        }
        Debug.Log(ActiveChunks.Count);

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


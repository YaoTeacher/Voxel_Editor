using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BlockInfo :ScriptableObject
{
    [SerializeField]
    public BlockType[] BlockTypes;

}

[System.Serializable]
public class BlockType
{
    [field: SerializeField]
    public int ID;
    [field: SerializeField]
    public string blockName;
    [field: SerializeField]
    public bool isSolid;
    [field: SerializeField]
    public float rough;
    [field: SerializeField]
    public bool isRenderNeibor;
    [field: SerializeField]
    public bool isTransparent;
    [field: SerializeField]
    public Sprite icon;
    [field: SerializeField]
    public VoxelMeshData meshData;



    [Header("Texture Values")]
    public int backFaceTexture;
    public int frontFaceTexture;
    public int topFaceTexture;
    public int bottomFaceTexture;
    public int leftFaceTexture;
    public int rightFaceTexture;

    // Back, Front, Top, Bottom, Left, Right

    public int GetTextureID(int faceIndex)
    {

        switch (faceIndex)
        {

            case 0:
                return backFaceTexture;
            case 1:
                return frontFaceTexture;
            case 2:
                return topFaceTexture;
            case 3:
                return bottomFaceTexture;
            case 4:
                return leftFaceTexture;
            case 5:
                return rightFaceTexture;
            default:
                Debug.Log("Error in GetTextureID; invalid face index");
                return 0;


        }

    }

}

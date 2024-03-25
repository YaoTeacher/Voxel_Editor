using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Linq;
using System.Collections.Generic;
using static UnityEditor.IMGUI.Controls.MultiColumnHeader;

public class MainMapEditor : EditorWindow
{
    World world;
    bool isEditMap=false;
    bool isSettingWayPoints=false;
    int selectedBlock=0;

    BlockInfo blockInfo;
    BlockType[] blockTypes;

    //���л�����
    protected SerializedObject _blockList;
    ReorderableList _blockArray;

    [MenuItem("EditorTool/MapEditor", priority = 0)]

    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(MainMapEditor));
    }

    //private void OnEnable()
    //{
    //    blockTypes = blockInfo.BlockTypes;
    //    List<BlockType> list = blockTypes.ToList<BlockType>();
    //    _blockArray = new ReorderableList(list, typeof(BlockType)
    //        , true, true, false, false);

    //    //�Զ����б�����
    //    _blockArray.drawHeaderCallback = (Rect rect) =>
    //    {
    //        GUI.Label(rect, "block Array");
    //    };

    //    //����Ԫ�صĸ߶�
    //    _blockArray.elementHeight = 40;

    //    //���Ƶ���Ԫ��
    //    _blockArray.drawElementCallback = OnElementCallback;

    //    //����ɫ
    //    //_blockArray.drawElementBackgroundCallback = OnElementBackgroundCallback;

    //    //ͷ��
    //    _blockArray.drawHeaderCallback = OnHeaderCallback;

    // }
    //private void OnHeaderCallback(Rect rect)
    //{
    //    EditorGUI.LabelField(rect, "�����б�");
    //}

    //private void OnElementBackgroundCallback(Rect rect, int index, bool isActive, bool isFocused)
    //{
    //    GUI.color = Color.black;
    //}

    //private void OnElementCallback(Rect rect, int index, bool isActive, bool isFocused)
    //{
    //    if (blockTypes.Length < 0)
    //        return;
    //    blockTypes[index].icon = (Sprite)EditorGUI.ObjectField(new Rect(rect.x, rect.y, 32, 32), blockTypes[index].icon, typeof(Sprite), false);
    //    blockTypes[index].blockName = EditorGUI.TextField(new Rect(rect.x+32, rect.y, rect.width -150, 20), blockTypes[index].blockName);
    //}


        void OnGUI()
        {
        world = EditorGUILayout.ObjectField(world, typeof(World), true) as World;
        blockInfo = EditorGUILayout.ObjectField(blockInfo, typeof(BlockInfo), true) as BlockInfo;
        blockTypes = blockInfo.BlockTypes;


        GUILayout.Label($"�����б�");
        EditorGUILayout.BeginHorizontal();
        foreach (var b in blockTypes)
        {
            EditorGUILayout.BeginVertical();
            if (GUILayout.Button(b.icon.texture, GUILayout.Width(32), GUILayout.Height(32)))
            {
                selectedBlock = b.ID;
            }
            GUILayout.Label($"{b.blockName}");
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Label($"{selectedBlock}");


        if (GUILayout.Button("�༭��ͼ"))
        {
            if (isEditMap != true&&isSettingWayPoints==false)
            {
                isEditMap = true;
            }
            else if(isSettingWayPoints == true)
            {
                Debug.Log("�����˳�·���༭");
                
            }
            else
            {
                isEditMap = false;
            }
        }
        GUILayout.Label($"{isEditMap}");

        if (GUILayout.Button("�༭·��"))
        {
            if (isSettingWayPoints != true && isEditMap == false)
            {
                isSettingWayPoints = true;
            }
            else if (isEditMap == true)
            {
                Debug.Log("�����˳���ͼ�༭");

            }
            else
            {
                isSettingWayPoints = false;
            }
        }
        GUILayout.Label($"{isSettingWayPoints}");


        if (GUILayout.Button("���µ�ͼ"))
        {

        }
            ;
        if (GUILayout.Button("�����ͼ"))
        {

            WorldDataManager.SaveWorld();

        }

        void LoadMap()
        {
            //string loadPath = World.Instance.appPath + "/saves/" + worldName + "/";
        }


        }

    void BlockList()
    {
        GUILayout.Label($"�����б�");
        foreach (var b in blockTypes)
        {
            EditorGUILayout.BeginHorizontal(); 
            if (GUILayout.Button(b.icon.texture, GUILayout.Width(32), GUILayout.Height(32)))
            {
                selectedBlock = b.ID;
            }
            EditorGUILayout.EndHorizontal();
        }
        GUILayout.Label($"{selectedBlock}");
        //_blockArray.DoLayoutList();
    }
};
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

    //序列化对象
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

    //    //自定义列表名称
    //    _blockArray.drawHeaderCallback = (Rect rect) =>
    //    {
    //        GUI.Label(rect, "block Array");
    //    };

    //    //定义元素的高度
    //    _blockArray.elementHeight = 40;

    //    //绘制单个元素
    //    _blockArray.drawElementCallback = OnElementCallback;

    //    //背景色
    //    //_blockArray.drawElementBackgroundCallback = OnElementBackgroundCallback;

    //    //头部
    //    _blockArray.drawHeaderCallback = OnHeaderCallback;

    // }
    //private void OnHeaderCallback(Rect rect)
    //{
    //    EditorGUI.LabelField(rect, "建造列表");
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


        GUILayout.Label($"建造列表");
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


        if (GUILayout.Button("编辑地图"))
        {
            if (isEditMap != true&&isSettingWayPoints==false)
            {
                isEditMap = true;
            }
            else if(isSettingWayPoints == true)
            {
                Debug.Log("需先退出路径编辑");
                
            }
            else
            {
                isEditMap = false;
            }
        }
        GUILayout.Label($"{isEditMap}");

        if (GUILayout.Button("编辑路径"))
        {
            if (isSettingWayPoints != true && isEditMap == false)
            {
                isSettingWayPoints = true;
            }
            else if (isEditMap == true)
            {
                Debug.Log("需先退出地图编辑");

            }
            else
            {
                isSettingWayPoints = false;
            }
        }
        GUILayout.Label($"{isSettingWayPoints}");


        if (GUILayout.Button("更新地图"))
        {

        }
            ;
        if (GUILayout.Button("保存地图"))
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
        GUILayout.Label($"建造列表");
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
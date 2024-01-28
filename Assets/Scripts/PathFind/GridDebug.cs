using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEditor;
using UnityEngine;

public enum FlowFieldDisplayType { None, AllIcons, DestinationIcon, CostField, IntegrationField };

public class GridDebug : MonoBehaviour
{
    public GridController gridController;
    public bool displayGrid;

    public FlowFieldDisplayType curDisplayType;

    private FlowField curFlowField;

    private Sprite[] ffIcons;

    private void Start()
    {
        ffIcons = Resources.LoadAll<Sprite>("Sprites/FFicons");
    }

    public void SetFlowField(FlowField newFlowField)
    {
        curFlowField = newFlowField;
    }

    //public void DrawFlowField()
    //{
    //    ClearCellDisplay();

    //    switch (curDisplayType)
    //    {
    //        case FlowFieldDisplayType.AllIcons:
    //            DisplayAllCells();
    //            break;

    //        case FlowFieldDisplayType.DestinationIcon:
    //            DisplayDestinationCell();
    //            break;

    //        default:
    //            break;
    //    }
    //}

    //private void DisplayAllCells()
    //{
    //    if (curFlowField == null) { return; }
    //    foreach (FlowFieldCellData curCell in curFlowField.grid.Values)
    //    {
    //        DisplayCell(curCell);
    //    }
    //}

    //private void DisplayDestinationCell()
    //{
    //    if (curFlowField == null) { return; }
    //    DisplayCell(curFlowField.destinationCell);
    //}

    //private void DisplayCell(FlowFieldCellData cell)
    //{
    //    GameObject iconGO = new GameObject();
    //    SpriteRenderer iconSR = iconGO.AddComponent<SpriteRenderer>();
    //    iconGO.transform.parent = transform;
    //    iconGO.transform.position = cell.worldPos;

    //    if (cell.cost == 0)
    //    {
    //        iconSR.sprite = ffIcons[3];
    //        Quaternion newRot = Quaternion.Euler(90, 0, 0);
    //        iconGO.transform.rotation = newRot;
    //    }
    //    else if (cell.cost == byte.MaxValue)
    //    {
    //        iconSR.sprite = ffIcons[2];
    //        Quaternion newRot = Quaternion.Euler(90, 0, 0);
    //        iconGO.transform.rotation = newRot;
    //    }
    //    else if (cell.bestDirection == GridDirection.North)
    //    {
    //        iconSR.sprite = ffIcons[0];
    //        Quaternion newRot = Quaternion.Euler(90, 0, 0);
    //        iconGO.transform.rotation = newRot;
    //    }
    //    else if (cell.bestDirection == GridDirection.South)
    //    {
    //        iconSR.sprite = ffIcons[0];
    //        Quaternion newRot = Quaternion.Euler(90, 180, 0);
    //        iconGO.transform.rotation = newRot;
    //    }
    //    else if (cell.bestDirection == GridDirection.East)
    //    {
    //        iconSR.sprite = ffIcons[0];
    //        Quaternion newRot = Quaternion.Euler(90, 90, 0);
    //        iconGO.transform.rotation = newRot;
    //    }
    //    else if (cell.bestDirection == GridDirection.West)
    //    {
    //        iconSR.sprite = ffIcons[0];
    //        Quaternion newRot = Quaternion.Euler(90, 270, 0);
    //        iconGO.transform.rotation = newRot;
    //    }
    //    else if (cell.bestDirection == GridDirection.NorthEast)
    //    {
    //        iconSR.sprite = ffIcons[1];
    //        Quaternion newRot = Quaternion.Euler(90, 0, 0);
    //        iconGO.transform.rotation = newRot;
    //    }
    //    else if (cell.bestDirection == GridDirection.NorthWest)
    //    {
    //        iconSR.sprite = ffIcons[1];
    //        Quaternion newRot = Quaternion.Euler(90, 270, 0);
    //        iconGO.transform.rotation = newRot;
    //    }
    //    else if (cell.bestDirection == GridDirection.SouthEast)
    //    {
    //        iconSR.sprite = ffIcons[1];
    //        Quaternion newRot = Quaternion.Euler(90, 90, 0);
    //        iconGO.transform.rotation = newRot;
    //    }
    //    else if (cell.bestDirection == GridDirection.SouthWest)
    //    {
    //        iconSR.sprite = ffIcons[1];
    //        Quaternion newRot = Quaternion.Euler(90, 180, 0);
    //        iconGO.transform.rotation = newRot;
    //    }
    //    else
    //    {
    //        iconSR.sprite = ffIcons[0];
    //    }
    //}

    public void ClearCellDisplay()
    {
        foreach (Transform t in transform)
        {
            GameObject.Destroy(t.gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        if (displayGrid)
        {
            if (curFlowField == null)
            {
                DrawGrid(Color.yellow);
            }
            else
            {
                DrawGrid(Color.green);
            }
        }

        if (curFlowField == null) { return; }

        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.MiddleCenter;

        switch (curDisplayType)
        {
            case FlowFieldDisplayType.CostField:

                foreach (FlowFieldCellData curCell in curFlowField.grid.Values)
                {
                    Handles.Label(curCell.WorldIndex, curCell.cost.ToString(), style);
                }
                break;

            case FlowFieldDisplayType.IntegrationField:

                foreach (FlowFieldCellData curCell in curFlowField.grid.Values)
                {
                    Handles.Label(curCell.WorldIndex, curCell.finalcost.ToString(), style);
                }
                break;

            default:
                break;
        }
    }

    private void DrawGrid(Color drawColor)
    {
        Gizmos.color = drawColor;
        if (curFlowField == null) { return; }
        foreach(FlowFieldCellData f in curFlowField.GroundData.Values)
        {
            Vector3 center = new Vector3(f.WorldIndex.x+0.5f, f.WorldIndex.y + 0.5f, f.WorldIndex.z + 0.5f) * VoxelData.BlockSize;
            Vector3 size = Vector3.one * VoxelData.BlockSize;
            Gizmos.DrawWireCube(center, size);
        }

    }
}
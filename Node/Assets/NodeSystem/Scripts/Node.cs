using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Node : MonoBehaviour, IClickable, IDraggable, IObject
{
    public enum NodeTypeEnum { input, output };
    public NodeTypeEnum nodeType;

    [HideInInspector] public Entity entity;

    public int MaxConnections;

    [Header("Visual")]
    public Sprite iconUnconnected;
    public Sprite iconConnected;

    public Color iconColorDefault;
    public Color iconColorHover;
    public Color iconColorSelected;
    public Color iconColorConnected;

    public Text text;
    public Image imageCurrentIcon;

    [Header("Elements")]
    public Transform ControlPointTranform;
    public bool DisableClick => false;
    public bool EnableDrag { get; set; }

    public int Priority => 2;
    public Color objectColour { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string ID => string.Format("{0}'s Node ({1})", entity.name, name);


    public bool HaveSpots
    {
        get
        {
            return MaxConnections == 0 || ConnectionList.Count < MaxConnections;
        }
    }

    Line ConnectionLine;
    Spline ConnectionSpline;
    UILineRenderer uiLineRenderer;

    [HideInInspector] public Node lastNode;
    Node closestNode;

    public List<Connection> ConnectionList;

    private void OnValidate()
    {
        Init();
    }

    private void Start()
    {
        Init();

        imageCurrentIcon.color = iconColorDefault;
        ConnectionList = new List<Connection>();

        ConnectionLine = new Line();
    }

    public void Init()
    {
        text = transform.GetChild(0).GetComponent<Text>();
        imageCurrentIcon = transform.GetChild(1).GetComponent<Image>();
        ControlPointTranform = transform.GetChild(2);
        ControlPointTranform.name = "ControlPoint";

        if (nodeType == NodeTypeEnum.input)
        {
            name = "Input Node";
            text.text = "In";
        }
        else if (nodeType == NodeTypeEnum.output)
        {
            name = "Output Node";
            text.text = "Out";
        }

        imageCurrentIcon.sprite = iconUnconnected;

        entity = GetParentEntity(transform);

        if (entity != null && !entity.NodeList.Contains(this))
        {
            entity.NodeList.Add(this);
        }

        ConnectionSpline = new Spline();

        uiLineRenderer = FindObjectOfType<UILineRenderer>();
    }

    private Entity GetParentEntity(Transform child)
    {
        if (child && child.gameObject.activeSelf)
        {
            Entity parentEntity = GetComponentInParent<Entity>();
            if (parentEntity != null) return parentEntity;
            else return GetParentEntity(child.parent);
        }
        else return null;
    }

    public void SetIcon()
    {
        if (ConnectionList.Count > 0)
        {
            imageCurrentIcon.sprite = iconConnected;
            imageCurrentIcon.color = iconColorConnected;
        }
        else
        {
            imageCurrentIcon.sprite = iconUnconnected;
            imageCurrentIcon.color = iconColorDefault;
        }
    }

    public Connection Connect(Node target)
    {
        Connection c = null;

        if (target.HaveSpots && this.HaveSpots)
        {
            c = entity.NodeManager.AddConnection(this, target, entity.NodeManager.GlobalLineType);
            target.SetIcon();
        }
        SetIcon();

        return c;
    }

    public void OnDrag()
    {
        Manager m = entity.NodeManager;

        if (lastNode)
        {
            lastNode.SetIcon();
        }
        closestNode = m.pointer.RaycastClosestNode(this);

        if (closestNode)
        {
            closestNode.imageCurrentIcon.color = closestNode.iconColorSelected;
            lastNode = closestNode;
        }

        if (m.GlobalLineType == Connection.LineTypeEnum.Spline)
        {
            ConnectionSpline.SetControlPoints(
                LinePoints[0],
                LinePoints[1],
                LinePoints[2],
                LinePoints[3]
                );

            ConnectionLine.SetPoints(ConnectionSpline.points);
        }

        else if (m.GlobalLineType == Connection.LineTypeEnum.Z_Shape)
        {
            ConnectionLine.SetPoints(new Vector2[]{
                LinePoints[0],
                (LinePoints[1] + LinePoints[0])/2,
                (LinePoints[2] + LinePoints[3])/2,
                LinePoints[3]
            });
        }

        else if (m.GlobalLineType == Connection.LineTypeEnum.Line)
        {
            ConnectionLine.SetPoints(new Vector2[]{
                LinePoints[0],
                LinePoints[3]
            });
        }
    }

    public void OnPointerDown()
    {
        Manager m = entity.NodeManager;
        imageCurrentIcon.color = iconColorSelected;

        m.AddLine(ConnectionLine);

        ConnectionLine.width = m.GlobalLineWidth;
        ConnectionLine.defaultColor = m.GlobalDefaultColour;
        ConnectionLine.color = m.GlobalDefaultColour;

        ConnectionLine.SetCap(Line.CapIDEnum.Start, m.GlobalCapStartType, m.GlobalCapStartSize, m.GlobalCapStartColor, m.GlobalCapStartAngleOffset);
        ConnectionLine.SetCap(Line.CapIDEnum.End, m.GlobalCapEndType, m.GlobalCapEndSize, m.GlobalCapEndColor, m.GlobalCapEndAngleOffset);
    }

    public void OnPointerUp()
    {
        imageCurrentIcon.color = iconColorDefault;
        ConnectionLine.points.Clear();
        entity.NodeManager.RemoveLine(ConnectionLine);
        Manager m = entity.NodeManager;

        // m.RemoveLine(ConnectionLine);

        closestNode = m.pointer.RaycastClosestNode(this);

        if (closestNode)
        {
            closestNode.imageCurrentIcon.color = closestNode.iconColorDefault;

            if (nodeType == NodeTypeEnum.input || closestNode.nodeType == NodeTypeEnum.output)
            {
                closestNode.Connect(this);
            }
            else
            {
                Connect(closestNode);
            }
        }

        SetIcon();

        lastNode = null;
    }

    public void Remove()
    {
        RemoveAllConnections();
        entity.NodeList.Remove(this);
        Destroy(gameObject);
    }

    public void RemoveAllConnections()
    {
        for (int i = ConnectionList.Count - 1; i >= 0; i--)
        {
            ConnectionList[i].Remove();
        }

        SetIcon();
    }

    Vector3[] LinePoints
    {
        get
        {
            Manager m = entity.NodeManager;
            Vector3 pointerPosition = m.pointer.PointerPosition;
            if (m.canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                return new Vector3[] {
                    transform.position,
                    ControlPointTranform.position,
                    closestNode ? closestNode.ControlPointTranform.position : pointerPosition,
                    closestNode ? closestNode.transform.position : pointerPosition };
            }
            else if (m.canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                return new Vector3[] {
                    m.MainCam.WorldToScreenPoint(transform.position),
                    m.MainCam.WorldToScreenPoint(ControlPointTranform.position),
                    closestNode ? m.MainCam.WorldToScreenPoint(closestNode.ControlPointTranform.position) : pointerPosition,
                    closestNode ? m.MainCam.WorldToScreenPoint(closestNode.transform.position) : pointerPosition };
            }
            else if (m.canvas.renderMode == RenderMode.WorldSpace)
            {
                return new Vector3[] {
                    Util.WorldToScreenPointInCanvas(transform.position, m),
                    Util.WorldToScreenPointInCanvas(ControlPointTranform.position, m),
                    closestNode ? Util.WorldToScreenPointInCanvas(closestNode.ControlPointTranform.position, m) : Util.WorldToScreenPointInCanvas(m.pointer.transform.position, m),
                    closestNode ? Util.WorldToScreenPointInCanvas(closestNode.transform.position, m) : Util.WorldToScreenPointInCanvas(m.pointer.transform.position, m) };
            }

            return new Vector3[] {
                transform.position,
                ControlPointTranform.position,
                closestNode ? closestNode.ControlPointTranform.position : pointerPosition,
                closestNode ? closestNode.transform.position : pointerPosition };
        }
    }
}

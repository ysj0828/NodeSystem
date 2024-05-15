using UnityEngine;

[System.Serializable]
public class Connection : ISelectable, IClickable, IObject, IDraggable
{
    public Node node0;
    public Node node1;

    public enum LineTypeEnum { Spline, Z_Shape, Line }
    public LineTypeEnum lineType;

    public string ID => string.Format("Connection ({0} - {1})", node0.entity ? node0.entity.name : "null", node1.entity ? node1.entity.name : "null");

    public Color objectColour
    {
        get => line.defaultColor;
        set
        {
            line.color = value;
            line.defaultColor = value;
        }
    }

    public int Priority => 1;

    public bool DisableClick => node0.entity.NodeManager.DisableConnectionClick;

    public Line line;

    Vector3[] handles = new Vector3[2];
    public Vector3[] Handles
    {
        get
        {
            handles[0] = node0.ControlPointTranform.position;
            handles[1] = node1.ControlPointTranform.position;
            return handles;
        }

        set => handles = value;
    }

    public Spline connectionSpline;

    public Connection(Node node0, Node node1, LineTypeEnum lineType)
    {
        this.node0 = node0;
        this.node1 = node1;
        this.lineType = lineType;
    }

    Vector3[] LinePoints
    {
        get
        {
            Manager m = node0.entity.NodeManager;
            Camera cam = m.MainCam;
            if (m.canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                return new Vector3[] {
                    node0.transform.position,
                    Handles[0],
                    Handles[1],
                    node1.transform.position
                    };
            }
            else if (m.canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                return new Vector3[] {
                    cam.WorldToScreenPoint(node0.transform.position),
                    cam.WorldToScreenPoint(Handles[0]),
                    cam.WorldToScreenPoint(Handles[1]),
                    cam.WorldToScreenPoint(node1.transform.position)
                    };
            }
            else if (m.canvas.renderMode == RenderMode.WorldSpace)
            {
                return new Vector3[] {
                    Util.WorldToScreenPointInCanvas(node0.transform.position, m),
                    Util.WorldToScreenPointInCanvas(Handles[0], m),
                    Util.WorldToScreenPointInCanvas(Handles[1], m),
                    Util.WorldToScreenPointInCanvas(node1.transform.position, m)
                    };
            }
            return new Vector3[] {
                node0.transform.position,
                Handles[0],
                Handles[1],
                node1.transform.position };
        }
    }

    public bool EnableDrag { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public void UpdateLine()
    {
        if (lineType == LineTypeEnum.Spline)
        {
            connectionSpline = new Spline();
            connectionSpline.SetControlPoints(
                LinePoints[0],
                LinePoints[1],
                LinePoints[2],
                LinePoints[3]);
            line.SetPoints(connectionSpline.points);
        }
        if (lineType == LineTypeEnum.Z_Shape)
        {
            line.SetPoints(new Vector2[] {
                LinePoints[0],
                (LinePoints[1] + LinePoints[0])/2,
                (LinePoints[2] + LinePoints[3])/2,
                LinePoints[3] });
        }
        if (lineType == LineTypeEnum.Line)
        {
            line.SetPoints(new Vector2[] {
                LinePoints[0],
                LinePoints[3] });
        }
    }

    public void OnPointerDown()
    {
        if (!node0.entity.NodeManager.SelectedObjectList.Contains(this))
        {
            Select();
        }
        else
        {
            Deselect();
        }

        if (line.CurrentlySelected)
        {
            node0.OnPointerDown();
            line.NotRemoved = true;
        }


    }

    public void OnPointerUp()
    {
        if (line.CurrentlySelected)
        {
            if (!line.NotRemoved)
                line.NotRemoved = true;
            node0.OnPointerUp();
        }
    }

    public void OnDrag()
    {
        if (line.NotRemoved)
        {
            node0.RemoveConnection(this);
            node1.RemoveConnection(this);
            Manager.instance.ConnectionsList.Remove(this);

            // node0.ConnectionLine.points.Clear();
            // node1.ConnectionLine.points.Clear();

            line.points.Clear();
            line.NotRemoved = false;
        }
        node0.OnDrag();
    }

    public void Select()
    {
        line.color = line.selectedColor;
        line.CurrentlySelected = true;

        if (!node0.entity.NodeManager.SelectedObjectList.Contains(this))
        {
            node0.entity.NodeManager.SelectedObjectList.Add(this);
        }
    }

    public void Deselect()
    {
        line.color = line.defaultColor;
        line.CurrentlySelected = false;

        if (node0.entity.NodeManager.SelectedObjectList.Contains(this))
        {
            node0.entity.NodeManager.SelectedObjectList.Remove(this);
        }
    }

    public void Remove()
    {
        Deselect();

        node0.ConnectionList.Remove(this);
        node1.ConnectionList.Remove(this);

        node0.SetIcon();
        node1.SetIcon();

        node0.entity.NodeManager.RemoveLine(this.line);
        node0.entity.NodeManager.ConnectionsList.Remove(this);
    }
}

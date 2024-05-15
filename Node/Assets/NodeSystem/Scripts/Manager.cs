using UnityEngine;
using System.Collections.Generic;

public class Manager : MonoBehaviour
{
    public static Manager instance { get; private set; }

    List<Entity> entityList;
    public List<Entity> EntityList { get => entityList; }

    // v2.0 - made not static to localize UIC to children
    // list of connections in the scene, used to improve performance of detections
    List<Connection> connectionsList;
    public List<Connection> ConnectionsList { get => connectionsList; }

    // v2.0 - made not static to localize UIC to children
    public UILineRenderer uiLineRenderer;
    public List<Line> UILinesList { get => uiLineRenderer.LineList; }

    // v2.0 - made not static to localize UIC to children
    public Canvas canvas;
    public RectTransform canvasRectTransform;
    public Camera MainCam;

    public List<ISelectable> SelectedObjectList = new List<ISelectable>();
    public IObject clickedObject;

    public Pointer pointer;

    [Header("Global Settings")]
    public bool DisableConnectionClick = false;
    public int GlobalLineWidth = 2;
    public Color GlobalDefaultColour = Color.white;
    public Connection.LineTypeEnum GlobalLineType;
    public Line.CapTypeEnum GlobalCapStartType;
    public float GlobalCapStartSize;
    public Color GlobalCapStartColor;
    public float GlobalCapStartAngleOffset;
    public Line.CapTypeEnum GlobalCapEndType;
    public float GlobalCapEndSize;
    public Color GlobalCapEndColor;
    public float GlobalCapEndAngleOffset;


    private void OnEnable()
    {
        InitLine();
    }

    private void OnValidate()
    {
        Awake();
    }

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(this);

        instance = this;

        canvas = GetComponent<Canvas>();
        canvasRectTransform = canvas.GetComponent<RectTransform>();
        pointer = GetComponentInChildren<Pointer>();
    }

    private void Start()
    {
        entityList = new List<Entity>();
        UpdateEntityList();
        connectionsList = new List<Connection>();

        InitLine();
    }


    public void InitLine()
    {
        uiLineRenderer = GetComponentInChildren<UILineRenderer>();
        if (!uiLineRenderer)
        {
            uiLineRenderer = FindObjectOfType<UILineRenderer>();

            if (!uiLineRenderer)
            {
                Debug.Log("todo?");
                //todo : load prefab?
                // uiLineRenderer = Instantiate(Resources.Load("UIC_LineRenderer") as UILineRenderer, transform.position, Quaternion.identity, transform);
                // uiLineRenderer.name = "UIC_LineRenderer";
            }
        }
    }

    public void AddLine(Line line)
    {
        if (!uiLineRenderer.LineList.Contains(line))
        {
            uiLineRenderer.LineList.Add(line);
        }
    }
    public void RemoveLine(Line line)
    {
        if (uiLineRenderer.LineList.Contains(line))
        {
            uiLineRenderer.LineList.Remove(line);
        }
    }
    public void AddEntity(Entity entity)
    {
        if (!EntityList.Contains(entity))
        {
            EntityList.Add(entity);
        }
    }
    public void UpdateEntityList()
    {
        entityList = new List<Entity>();
        entityList.AddRange(GetComponentsInChildren<Entity>());
    }

    public void RemoveConnection(Node n0, Node n1, Connection c)
    {
        n0.ConnectionList.Remove(c);
        n1.ConnectionList.Remove(c);
        ConnectionsList.Remove(c);
    }

    public Connection AddConnection(Node n0, Node n1, Connection.LineTypeEnum lineType = Connection.LineTypeEnum.Spline)
    {
        Connection prev = NodesConnected(n0, n1);
        if (prev != null)
        {
            return prev;
        }

        Connection c = CreateConnection(n0, n1, lineType);

        ConnectionsList.Add(c);
        n0.ConnectionList.Add(c);
        n1.ConnectionList.Add(c);

        AddLine(c.line);

        c.line.width = GlobalLineWidth;
        c.line.defaultColor = GlobalDefaultColour;
        c.line.color = GlobalDefaultColour;

        c.line.SetCap(Line.CapIDEnum.Start, GlobalCapStartType, GlobalCapStartSize, GlobalCapStartColor, GlobalCapStartAngleOffset);
        c.line.SetCap(Line.CapIDEnum.End, GlobalCapEndType, GlobalCapEndSize, GlobalCapEndColor, GlobalCapEndAngleOffset);

        c.UpdateLine();
        return c;
    }

    private Connection CreateConnection(Node n0, Node n1, Connection.LineTypeEnum lineType)
    {
        Connection c = new Connection(n0, n1, lineType);
        c.line = new Line();
        c.line.width = 2;

        return c;
    }

    public Connection NodesConnected(Node n0, Node n1)
    {
        foreach (Connection c in ConnectionsList)
        {
            if ((n0 == c.node0 && n1 == c.node1) || (n0 == c.node1 && n1 == c.node0))
            {
                return c;
            }
        }
        return null;
    }
    public Connection FindClosestConnection(Vector3 pos, float maxDistance)
    {
        float minDistance = Mathf.Infinity;
        Connection closestConnection = null;
        foreach (Connection c in ConnectionsList)
        {
            int count = c.line.points.Count;
            if (count > 0)
            {
                for (int i = 1; i < count; i++)
                {
                    float d = Util.DistanceToConnection(c, pos, maxDistance);
                    if (d < minDistance)
                    {
                        closestConnection = c;
                        minDistance = d;
                    }
                }
            }
        }

        return closestConnection;
    }

    public void InstantiateEntity(Entity entity, Vector3 pos)
    {
        GameObject go = Instantiate(entity.gameObject, new Vector3(200, 100), Quaternion.identity, canvas.transform);
        AddEntity(go.GetComponent<Entity>());

        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            go.transform.position = pos + new Vector3(15, 15, 0);
        }
        else
        {
            pos.z = 0;
            go.transform.localPosition = pos + new Vector3(1, 1, 0);
        }
    }



}
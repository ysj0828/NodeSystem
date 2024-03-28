using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class Entity : MonoBehaviour, IObject, ISelectable, IDraggable, IClickable
{
    public Manager NodeManager;

    public string ID => name;
    Image image;
    [SerializeField] private Transform parent;
    private RectTransform rectTrasnform;

    public Color objectColour { get => image.color; set => image.color = value; }
    public int Priority => 0;

    [SerializeField] private bool enableDrag;
    public bool EnableDrag { get => enableDrag; set => enableDrag = value; }
    public bool EnableSelfConnection = false;

    public bool DisableClick { get; set; }
    public Vector3[] Handles { get; set; }

    private Outline outline;

    public List<Node> NodeList;


    private void OnValidate()
    {
        Init();
        Awake();
    }

    private void Awake()
    {
        NodeManager = GetComponentInParent<Manager>();
        DisableClick = false;
        image = GetComponent<Image>();
        parent = transform.parent;
    }

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        outline = outline ? outline : gameObject.GetComponent<Outline>() ? gameObject.GetComponent<Outline>() : gameObject.AddComponent<Outline>();

        if (outline)
        {
            //todo : set colour
            outline.effectColor = Color.red;
            outline.effectDistance = new Vector2(3, -3);
            outline.enabled = false;
        }

        rectTrasnform = rectTrasnform ? rectTrasnform : GetComponent<RectTransform>();

        NodeList = new List<Node>();
        NodeList.AddRange(transform.GetComponentsInChildren<Node>());
    }

    public void UpdateConnections()
    {
        foreach (Node n in NodeList)
        {
            foreach (Connection c in n.ConnectionList)
            {
                c.UpdateLine();
            }
        }
    }

    public void Select()
    {
        outline.enabled = true;
        if (!NodeManager.SelectedObjectList.Contains(this))
        {
            NodeManager.SelectedObjectList.Add(this);
        }
    }
    public void Deselect()
    {
        outline.enabled = false;
        if (NodeManager.SelectedObjectList.Contains(this))
        {
            NodeManager.SelectedObjectList.Remove(this);
        }
    }

    public void OnDrag()
    {
        if (EnableDrag)
        {
            Select();
            transform.SetParent(NodeManager.pointer.transform.GetChild(0));

            UpdateConnections();
        }
    }

    public void OnPointerDown()
    {
        if (!NodeManager.SelectedObjectList.Contains(this))
        {
            Select();
            parent = transform.parent;
        }
        else
        {
            Deselect();
        }
    }

    public void OnPointerUp()
    {
        transform.SetParent(parent);

        UpdateConnections();
    }

    public void Remove()
    {
        Deselect();

        foreach (Node n in NodeList)
        {
            n.RemoveAllConnections();
        }

        if (NodeManager.EntityList.Contains(this))
        {
            NodeManager.EntityList.Remove(this);
        }

        Destroy(gameObject);
    }


    public List<Entity> GetConnectedEntities(Node.NodeTypeEnum type)
    {
        List<Entity> entities = new List<Entity>();
        foreach (Node n in NodeList)
        {
            if (n.nodeType == type)
            {
                foreach (Connection c in n.ConnectionList)
                {
                    Entity e = c.node0 != n ? c.node0.entity : c.node1.entity;
                    entities.Add(e);
                }
            }
        }
        return entities;
    }
}
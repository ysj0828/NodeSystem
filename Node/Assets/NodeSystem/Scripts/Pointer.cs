using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class Pointer : MonoBehaviour
{
    [SerializeField] Manager nodeManager;
    [SerializeField] Camera mainCam;

    public KeyCode PrimaryKey;
    public KeyCode SecondaryKey;

    Image image;
    Canvas canvas;

    GraphicRaycaster graphicRaycaster;
    PointerEventData pointerEventData;
    EventSystem eventSystem;

    Vector3 initialMousePos;

    List<IObject> objectList = new List<IObject>();

    public Vector3 PointerPosition
    {
        get => Input.mousePosition;
    }

    public UnityEvent OnPointerDownFirst;
    public UnityEvent OnPointerDownLast;

    public UnityEvent OnDragFirst;
    public UnityEvent OnDragLast;

    public UnityEvent OnPointerUpFirst;
    public UnityEvent OnPointerUpLast;

    private void OnDisable()
    {
        OnPointerDownFirst.RemoveAllListeners();
        OnPointerDownLast.RemoveAllListeners();
        OnDragFirst.RemoveAllListeners();
        OnDragLast.RemoveAllListeners();
        OnPointerUpFirst.RemoveAllListeners();
        OnPointerUpLast.RemoveAllListeners();
    }

    private void OnValidate()
    {
        Init();
        Awake();
    }

    private void Awake()
    {
        nodeManager = GetComponentInParent<Manager>();
    }

    private void Start()
    {
        OnPointerDownFirst = OnPointerDownFirst ?? new UnityEvent();
        OnPointerDownLast = OnPointerDownLast ?? new UnityEvent();
        OnDragFirst = OnDragFirst ?? new UnityEvent();
        OnDragLast = OnDragLast ?? new UnityEvent();
        OnPointerUpFirst = OnPointerUpFirst ?? new UnityEvent();
        OnPointerUpLast = OnPointerUpLast ?? new UnityEvent();

        // Cursor.visible = false;
        FollowCursor();
        Init();

        graphicRaycaster = FindObjectOfType<GraphicRaycaster>();
        eventSystem = FindObjectOfType<EventSystem>();

        mainCam = nodeManager.MainCam;
    }

    private void Init()
    {
        image = image ? image : GetComponent<Image>() ? GetComponent<Image>() : gameObject.AddComponent<Image>();
        image.raycastTarget = false;
        canvas = canvas ? canvas : GetComponent<Canvas>() ? GetComponent<Canvas>() : gameObject.AddComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = 999;
    }

    private void Update()
    {
        // if (nodeManager == null) nodeManager = GetComponentInParent<Manager>();
        FollowCursor();

        if (Input.GetKeyDown(PrimaryKey))
        {
            OnPointerDown();
        }
        if (Input.GetKey(PrimaryKey))
        {
            if (initialMousePos != transform.position)
            {
                OnDrag();
            }
        }
        if (Input.GetKeyUp(PrimaryKey))
        {
            OnPointerUp();
        }
        // if(Input.GetKeyDown(SecondaryKey)){

        // }
    }

    private void FollowCursor()
    {

        if (nodeManager.canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            transform.position = PointerPosition;
        }
        else
        {
            Vector3 screenPoint = PointerPosition;
            screenPoint.z = nodeManager.transform.position.z - mainCam.transform.position.z;
            transform.position = mainCam.ScreenToWorldPoint(screenPoint);
        }
    }

    public void OnPointerDown()
    {
        OnPointerDownFirst.Invoke();

        SelectCloserObject();

        initialMousePos = transform.position;
        OnPointerDownLast.Invoke();
    }
    public void OnPointerUp()
    {
        OnPointerUpFirst.Invoke();

        if (nodeManager.clickedObject is IClickable)
        {
            (nodeManager.clickedObject as IClickable).OnPointerUp();
        }

        foreach (IObject obj in nodeManager.SelectedObjectList)
        {
            if (obj is IClickable)
            {
                (obj as IClickable).OnPointerUp();
            }
        }

        OnPointerUpLast.Invoke();
    }
    public void OnDrag()
    {
        OnDragFirst.Invoke();

        if (nodeManager.clickedObject is IDraggable)
        {
            (nodeManager.clickedObject as IDraggable).OnDrag();
        }

        if (nodeManager.clickedObject is Entity)
        {
            foreach (ISelectable obj in nodeManager.SelectedObjectList)
            {
                if (obj is Entity)
                {
                    (obj as IDraggable).OnDrag();
                }
            }
        }

        OnDragLast.Invoke();
    }

    public void DeselectAllObjects()
    {
        if (!Input.GetKey(SecondaryKey))
        {
            for (int i = nodeManager.SelectedObjectList.Count - 1; i >= 0; i--)
            {
                nodeManager.SelectedObjectList[i].Deselect();
            }
        }
    }

    public void SelectCloserObject()
    {
        nodeManager.clickedObject = FindObjectCloserToPointer();
        if (nodeManager.clickedObject is IClickable)
        {
            (nodeManager.clickedObject as IClickable).OnPointerDown();
        }
    }

    public IObject FindObjectCloserToPointer()
    {
        objectList = ObjectsUnderPointer();

        if (objectList.Count > 0)
        {

            DeselectAllObjects();

            return objectList[0];
        }

        DeselectAllObjects();
        return null;
    }

    public List<IObject> ObjectsUnderPointer()
    {
        List<IObject> orderedList = new List<IObject>();

        List<RaycastResult> result = RaycastResultAll();

        IObject obj = null;

        foreach (RaycastResult r in result)
        {
            obj = r.gameObject.GetComponent<IObject>();

            if (obj != null)
            {
                if (!(obj is IClickable) || !(obj as IClickable).DisableClick)
                {
                    orderedList.Add(obj);
                }
            }
        }

        if (nodeManager.canvas.renderMode != RenderMode.WorldSpace)
        {
            obj = (IObject)nodeManager.FindClosestConnection(PointerPosition, 15);
        }
        else
        {
            obj = (IObject)nodeManager.FindClosestConnection(Util.WorldToScreenPointInCanvas(transform.position, nodeManager), 15);
        }

        if (obj != null)
        {
            if (!(obj as IClickable).DisableClick)
            {
                orderedList.Add(obj);
            }
        }

        orderedList.Sort(SortByPriority);

        return orderedList;
    }

    public List<RaycastResult> RaycastResultAll()
    {
        List<RaycastResult> result = new List<RaycastResult>();
        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = PointerPosition;

        List<RaycastResult> local = new List<RaycastResult>();

        GraphicRaycaster[] raycastersArray = FindObjectsOfType<GraphicRaycaster>();
        foreach (GraphicRaycaster gr in raycastersArray)
        {
            gr.Raycast(pointerEventData, local);
            result.AddRange(local);
        }

        return result;
    }

    public Node RaycastClosestNode(Node draggedNode)
    {
        Node closestNode = null;

        List<RaycastResult> result = nodeManager.pointer.RaycastResultAll();
        IObject obj = null;

        foreach (RaycastResult r in result)
        {
            obj = r.gameObject.GetComponent<IObject>();

            if (obj != null)
            {
                if (!(obj is IClickable) || !(obj as IClickable).DisableClick)
                {
                    if (obj is Node)
                    {
                        Node n = obj as Node;
                        if (draggedNode != n && n.HaveSpots && draggedNode.HaveSpots)
                        {
                            if ((n.entity == draggedNode.entity && n.entity.EnableSelfConnection) || n.entity != draggedNode.entity)
                            {
                                if (n.nodeType != draggedNode.nodeType)
                                {
                                    return n;
                                }
                            }
                        }
                    }
                }
            }
        }
        return closestNode;
    }

    private static int SortByPriority(IObject o1, IObject o2)
    {
        return o2.Priority.CompareTo(o1.Priority);
    }

    public List<IObject> OrderFoundObjects(List<IObject> list)
    {
        objectList.Sort(SortByPriority);
        return objectList;
    }


}
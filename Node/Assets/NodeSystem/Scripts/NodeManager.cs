using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    private List<NodeBody> nodeBodies;
    public List<NodeBody> NodeBodies
    {
        get => nodeBodies;
    }


    public NodeLineRenderer _NodeLineRenderer;

    public Canvas _Canvas;
    public RectTransform _RectTransform;

    private void Awake()
    {
        _Canvas = GetComponent<Canvas>();
        _RectTransform = GetComponent<RectTransform>();

    }

    private void Start()
    {
        if (!_NodeLineRenderer)
        {
            _NodeLineRenderer = GetComponentInChildren<NodeLineRenderer>();
        }

    }
}

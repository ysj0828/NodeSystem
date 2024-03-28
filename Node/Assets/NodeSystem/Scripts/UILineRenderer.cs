using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILineRenderer : Graphic
{
    Manager nodeManager;
    Canvas canvas;
    private List<Line> lineList;
    public List<Line> LineList { get => lineList; }

    RectTransform rectTransform;
    float rectScale;
    float D2R = Mathf.Deg2Rad * 90f;

    Vector2 lastPos0, lastPos1;

    VertexHelper _vh;

    Vector2 uv0 = new Vector2(0, 0);
    Vector2 uv1 = new Vector2(0, 1);
    Vector2 uv2 = new Vector2(1, 1);
    Vector2 uv3 = new Vector2(1, 0);

    Vector2 pos0, pos1, pos2, pos3;

    [SerializeField] private Texture texture;
    public override Texture mainTexture
    {
        get => texture == null ? s_WhiteTexture : texture;
    }

    public Texture t
    {
        get => texture;
        set
        {
            if (texture == value) return;
            texture = value;
            SetVerticesDirty();
            SetMaterialDirty();
        }
    }

    [HideInInspector]
    public int vertCount = 0;
    public bool IsVertLimitReached { get => (vertCount >= 64000); }

    protected UIVertex[] SetVertex(Vector2[] vertices, Vector2[] uvs, Color _color)
    {
        UIVertex[] v = new UIVertex[4];
        for (int i = 0; i < vertices.Length; i++)
        {
            var vert = UIVertex.simpleVert;
            vert.color = _color;

            vert.position = vertices[i];
            vert.uv0 = uvs[i];
            v[i] = vert;
        }

        return v;
    }

    protected override void OnEnable()
    {
        lineList = lineList == null ? new List<Line>() : lineList;
        rectTransform = GetComponent<RectTransform>();
        rectScale = rectTransform.localScale.x;

        canvas = GetComponentInParent<Canvas>();

        Awake();
    }
    private void Awake()
    {
        nodeManager = GetComponentInParent<Manager>();
    }

    private void Update()
    {
        if (nodeManager.canvas.renderMode != RenderMode.WorldSpace)
        {
            rectTransform.localScale = Vector3.one / canvas.scaleFactor;

        }
        else
        {
            rectTransform.localScale = Vector3.one;
        }
        SetVerticesDirty();
        SetMaterialDirty();
    }

    private void DrawLine(Line line)
    {
        int count = line.points.Count;

        for (int i = 1; i < count; i++)
        {
            Vector2 p0 = line.points[i - 1];
            Vector2 p1 = line.points[i];

            float angle = Mathf.Atan2(p0.y - p1.y, p0.x - p1.x) + (D2R);

            float cos = Mathf.Cos(angle);   // calc cos
            float sin = Mathf.Sin(angle);   // calc sin

            float _halfLineWidth = ((line.width / rectScale) / 2);
            float _wCos = (_halfLineWidth * cos);
            float _wSin = (_halfLineWidth * sin);

            pos0 = new Vector2((p0.x) + _wCos, (p0.y) + _wSin);
            pos1 = new Vector2((p0.x) - _wCos, (p0.y) - _wSin);
            pos2 = new Vector2((p1.x) - _wCos, (p1.y) - _wSin);
            pos3 = new Vector2((p1.x) + _wCos, (p1.y) + _wSin);

            _vh.AddUIVertexQuad(SetVertex(new[] { pos0, pos1, pos2, pos3 }, new[] { uv0, uv1, uv2, uv3 }, line.color));

            if (i > 1)
            {
                _vh.AddUIVertexQuad(SetVertex(new[] { lastPos1, lastPos0, pos1, pos0 }, new[] { uv0, uv1, uv2, uv3 }, line.color));
            }

            if (i == 1)
                DrawCap(line.points[i - 1], angle + (line.StartCapAngleOffset * Mathf.Deg2Rad), line.StartCapSize, line.StartCapEnum, line.StartCapColour);
            if (i == count - 1)
                DrawCap(line.points[i], angle + (line.EndCapAngleOffset * Mathf.Deg2Rad), line.EndCapSize, line.EndCapEnum, line.EndCapColour);

            lastPos0 = pos2;
            lastPos1 = pos3;
        }
    }

    public void DrawCap(Vector2 pos, float angle, float size, Line.CapTypeEnum type, Color color)
    {
        float cos = Mathf.Cos(angle);   // calc cos
        float sin = Mathf.Sin(angle);   // calc sin
        size /= rectScale;

        float sizeSin = (size * sin);
        float sizeCos = (size * cos);

        if (type == Line.CapTypeEnum.Circle)
        {
            DrawCircle(pos, size, color);
        }
    }

    public void DrawCircle(Vector2 pos, float r, Color c)
    {
        Vector2 v = pos;
        Vector2 prev = v;

        float circleSlice = 360 / 10;

        for (int i = 0; i <= 10; i++)
        {
            float angle = circleSlice * i;

            angle = Mathf.Deg2Rad * angle;
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);

            pos2 = v;
            pos3 = v;
            pos0 = prev;
            pos1 = new Vector2(v.x + (r * cos), v.y + (r * sin));

            prev = pos1;

            _vh.AddUIVertexQuad(SetVertex(new[] { pos0, pos1, pos2, pos3 }, new[] { uv3, uv0, uv1, uv2 }, color));
        }
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        _vh = _vh ?? vh;

        if (lineList != null)
        {
            foreach (Line l in lineList)
            {
                if (l.points != null && l.points.Count > 0)
                {
                    if (!IsVertLimitReached)
                    {
                        DrawLine(l);
                    }
                    else
                    {
                        Debug.LogWarning("Vert Limit reached");
                    }

                    vertCount = _vh.currentVertCount;
                }
            }
        }
    }
}

public class Spline
{
    public Vector2[] controlPoints = new Vector2[4];
    public Vector2[] points = new Vector2[1];
    public int steps;
    public static int minPoints = 5;
    public static int maxPoints = 15;

    public void SetControlPoints(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        steps = CalculateNumberOfPointsInLine(p0, p1, p2, p3);

        controlPoints = new Vector2[] { p0, p1, p2, p3 };
        points = GetCurve(steps);
    }

    public int CalculateNumberOfPointsInLine(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        float a0 = Mathf.Abs(Mathf.Sin(Vector2.Angle(p0 - p1, p3 - p1) * Mathf.Deg2Rad));
        float a1 = Mathf.Abs(Mathf.Sin(Vector2.Angle(p3 - p2, p0 - p2) * Mathf.Deg2Rad));
        float r0 = a0 > a1 ? a0 : a1;

        return (int)((r0 * (maxPoints - minPoints)) + minPoints);
    }

    Vector2[] GetCurve(int steps)
    {
        points = new Vector2[steps + 1];
        for (int i = 0; i < steps; i++)
        {
            points[i] = GetPoint(controlPoints[0], controlPoints[1], controlPoints[2], controlPoints[3], (float)i / (float)steps);
        }
        points[steps] = controlPoints[3];

        return points;
    }

    public Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        t = Mathf.Clamp01(t);
        float u = 1f - t;
        return
            u * u * u * p0 +
            3f * u * u * t * p1 +
            3f * u * t * t * p2 +
            t * t * t * p3;
    }
}
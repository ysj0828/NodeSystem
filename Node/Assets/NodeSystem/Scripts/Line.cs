using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Line
{
    public enum CapTypeEnum { none, Circle };
    public enum CapIDEnum { Start, End };
    public CapTypeEnum StartCapEnum;
    public float StartCapSize;
    public Color StartCapColour;
    public float StartCapAngleOffset;
    public CapTypeEnum EndCapEnum;
    public float EndCapSize;
    public Color EndCapColour;
    public float EndCapAngleOffset;


    public float width;
    public float length;
    public Color color;
    public List<Vector2> points;

    public Color selectedColor = new Color32(0x7f, 0x5a, 0xf0, 0xff);
    public Color hoverColor = new Color32(0x2c, 0xb6, 0x7d, 0xff);
    public Color defaultColor = new Color32(0xff, 0xff, 0xfe, 0xff);

    float D2R = Mathf.Deg2Rad * 90f;


    public Line()
    {
        color = defaultColor;
        width = 2;
        points = new List<Vector2>();
    }

    public float GetLength()
    {
        float l = 0;
        for (int i = 1; i < points.Count; i++)
        {
            l += Vector2.Distance(points[i - 1], points[i]);
        }
        return l;
    }

    public void SetPoints(Vector2[] newPoints)
    {
        if (newPoints != null)
        {
            points.Clear();
            points.AddRange(newPoints);
        }

        length = GetLength();
    }

    public void SetCap(CapIDEnum capID, CapTypeEnum capType, float capSize = 5, Color? capColor = null, float angleOffset = 0)
    {
        if (capID == CapIDEnum.Start)
        {
            this.StartCapSize = capSize;
            this.StartCapEnum = capType;
            this.StartCapColour = capColor ?? Color.white;
            this.StartCapAngleOffset = angleOffset;
        }
        else
        {
            this.EndCapSize = capSize;
            this.EndCapEnum = capType;
            this.EndCapColour = capColor ?? Color.white;
            this.EndCapAngleOffset = angleOffset;
        }
    }

    System.Tuple<int, float> GetPreviousPointELength(float pos)
    {
        System.Tuple<int, float> t = new System.Tuple<int, float>(0, 0);
        float l = 0;
        float prevL = 0;
        for (int i = 1; i < points.Count; i++)
        {
            l += Vector2.Distance(points[i - 1], points[i]);
            if (l >= pos * length)
            {
                t = new System.Tuple<int, float>(i - 1, prevL);
                break;
            }
            prevL = l;
        }
        return t;
    }

    public System.Tuple<Vector2, float> LerpLine(float pos)
    {
        System.Tuple<int, float> t = GetPreviousPointELength(pos);
        Vector2 p0 = points[t.Item1];
        Vector2 p1 = points[t.Item1 + 1];
        float ppDistance = Vector2.Distance(p0, p1);

        Vector2 position = Vector2.Lerp(p0, p1, ((pos * length) - t.Item2) / ppDistance);
        float angle = Mathf.Atan2(p0.y - p1.y, p0.x - p1.x) + D2R;

        return new System.Tuple<Vector2, float>(position, angle);
    }
}
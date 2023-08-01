using System.Collections.Generic;
using UnityEngine;

public class LagrangeCurve
{
    struct ControlPoint{
        public Vector2 vector;
        public float distanceOnCurve;
    }

    List<ControlPoint> controlPoints = new List<ControlPoint>();
    
    /// <summary>
    /// Magic
    /// </summary>
    /// <param name="i"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    float Li(int i, float t)
    {
        float Li = 1.0f;
        for (int index = 0; index < controlPoints.Count; index++)
        {
            if (index!=i)
            {
                Li *= (t - controlPoints[index].distanceOnCurve) / (controlPoints[i].distanceOnCurve - controlPoints[index].distanceOnCurve);
            }
        }
        return Li;
    }

    public void AddControlPoint(Vector2 controlPointVector)
    {
        ControlPoint newPoint = new ControlPoint();
        newPoint.vector = controlPointVector;
        newPoint.distanceOnCurve = controlPoints.Count;
        controlPoints.Add(newPoint);
    }

    /// <summary>
    /// Calculates the 2D position of the curve's point given by the distance proportion
    /// </summary>
    /// <param name="distanceProportion"> The distance proportion is a float between 0 and 1 (each inclusive). 0 means the first control point's position, 1 means the last control point's position. </param>
    /// <returns> 2D point of the curve at the given distance proportion </returns>
    public Vector2 Point(float distanceProportion)
    {
        float curveEndDistance = controlPoints[controlPoints.Count - 1].distanceOnCurve;

        Vector2 pointAtDistanceProportion = Vector2.zero;
        for (int i = 0; i < controlPoints.Count; i++)
        {
            pointAtDistanceProportion += controlPoints[i].vector * Li(i, distanceProportion * curveEndDistance);
        }
        return pointAtDistanceProportion;
    }
}

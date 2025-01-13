using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
    [SerializeField] MatchersLibrary MatchersLibrary;
    [SerializeField] Transform Crib;

    [SerializeField] Transform NorthBoundary;
    [SerializeField] Transform SouthBoundary;
    [SerializeField] Transform WestBoundary;
    [SerializeField] Transform EastBoundary;
    [SerializeField] Transform BottomBoundary;
    [SerializeField] float MaxHeightToFall;

    private const float minGap = 1.5f;
    private float minX;
    private float maxX;
    private float minY;
    private float maxY;
    private float minZ;
    private float maxZ;

    private System.Random random = new System.Random();

    private void SetBoundaries()
    {
        minY = BottomBoundary.position.y + minGap;
        maxY = minY + MaxHeightToFall;

        minX = WestBoundary.position.x + minGap;
        maxX = EastBoundary.position.x - minGap;

        minZ = SouthBoundary.position.z + minGap;
        maxZ = NorthBoundary.position.z - minGap;
    }

    private Vector3 GeneratePositionForObject()
    {
        var x = random.NextDouble() * (maxX - minX) + minX;
        var y = random.NextDouble() * (maxY - minY) + minY;
        var z = random.NextDouble() * (maxZ - minZ) + minZ;
        return new Vector3((float)x, (float)y, (float)z);
    }
    private Quaternion GenerateRotationForObject()
    {
        var x = random.NextDouble() * 360;
        var y = random.NextDouble() * 360;
        var z = random.NextDouble() * 360;
        return Quaternion.Euler((float)x, (float)y, (float)z);
    }

    public void Generate(Dictionary<MatchingType, int> objectsToGenerate, Action<MatchingObject> clickCallback)
    {
        SetBoundaries();
        foreach ((MatchingType type, int count) in objectsToGenerate)
        {
            if (type == MatchingType.EMPTY || type == MatchingType.MATCHED) { continue; }
            GameObject prefab = MatchersLibrary.GetMatcherByType(type);
            if (prefab != null)
            {
                /*Vector3 prefabSize = prefab.GetComponent<Renderer>().bounds.size;
                float maxDimension = Mathf.Max(prefabSize.x, prefabSize.y, prefabSize.y);
                float minDimension = Mathf.Min(prefabSize.x, prefabSize.y, prefabSize.y);
                float scaleCoeff = 1;
                if (minDimension > minGap * 2) { scaleCoeff *= minGap * 2 / minDimension; }
                if (maxDimension < minGap / 2) { scaleCoeff *= minGap / 2 / maxDimension; }*/
                for (int i = 0; i < count; i++)
                {
                    GameObject obj = Instantiate(prefab, GeneratePositionForObject(), GenerateRotationForObject(), Crib.transform);
                    obj.transform.localScale = Vector3.one/* * scaleCoeff*/;
                    obj.GetComponent<MatchingObject>().clickCallback = clickCallback;
                }
            }
        }
    }
}

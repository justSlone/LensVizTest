﻿using UnityEngine;
using System;
using System.Collections.Generic;

public class SurfaceChart : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public static void Render(GameObject gameObject, float[] x, float[] y, float[] z)
    {
        if (!(x.Length == y.Length && y.Length == z.Length))
        {
            throw new ArgumentException("Length of x, y, and z to surface chart must must all be equal");
        }

        int numPoints = x.Length;
        float xMin = float.MaxValue;
        float xMax = float.MinValue;
        float yMin = float.MaxValue;
        float yMax = float.MinValue;
        float zMin = float.MaxValue;
        float zMax = float.MinValue;
        for (long i = 0; i < numPoints; i++)
        {
            if (x[i] < xMin)
            {
                xMin = x[i];
            }
            if (x[i] > xMax)
            {
                xMax = x[i];
            }

            if (y[i] < yMin)
            {
                yMin = y[i];
            }
            if (y[i] > yMax)
            {
                yMax = y[i];
            }

            if (z[i] < zMin)
            {
                zMin = z[i];
            }
            if (z[i] > zMax)
            {
                zMax = z[i];
            }
        }
        int xRange = ((int)xMax - (int)xMin) + 1;
        int yRange = ((int)yMax - (int)yMin) + 1;
        
        // Find the right mesh filter.
        MeshFilter meshFilter = null;
        var meshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
        for(int i = 0; i < meshFilters.Length; i++)
        {
            if(meshFilters[i].name == "SurfaceRenderer")
            {
                meshFilter = meshFilters[i];
                break;
            }
        }
        var mesh = new Mesh();
        meshFilter.mesh = mesh;

        // Set vertices of surface mesh
        var vertices = new Vector3[xRange * yRange];
        for(int i = 0; i < xRange; i++)
        {
            for(int j = 0; j < yRange; j++)
            {
                var xVal = (x[i + j * xRange] - xMin) / (xMax - xMin) - 0.5f;
                var yVal = (y[i + j * xRange] - yMin) / (yMax - yMin) - 0.5f;
                var zVal = (z[i + j * xRange] - zMin) / (zMax - zMin) - 0.5f;
                vertices[i + j * xRange] = new Vector3(xVal, zVal, yVal);
            }
        }
        mesh.vertices = vertices;

        // Set triangles of mesh (it's not visible without them!)
        // Also, double the number of triangles you would normally use,
        // since we want surface "solid" from both sides
        var triangles = new int[2 * (xRange * yRange * 6)];
        for(int i = 0, ti = 0; i < xRange - 1; i++)
        {
            for(int j = 0; j < yRange - 1; j++, ti += 12)
            {
                triangles[ti] = i + j * xRange;
                triangles[ti + 1] = i + j * xRange + xRange;
                triangles[ti + 2] = i + j * xRange + 1;
                triangles[ti + 3] = i + j * xRange + 1;
                triangles[ti + 4] = i + j * xRange + xRange;
                triangles[ti + 5] = i + j * xRange + xRange + 1;
            }
        }
        mesh.triangles = triangles;

        var colors = new Color[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            var h = vertices[i].z + 0.5;
            var r = (float)(255 * h);
            var b = (float)(255 * (1.0f - h));
            colors[i] = new Color(r, 0, b);
        }
        mesh.SetColors(new List<Color>(colors));

        mesh.RecalculateNormals();
    }
}
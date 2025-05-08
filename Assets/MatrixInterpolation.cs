using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vectors;

[ExecuteAlways, RequireComponent(typeof(VectorRenderer))]
public class MatrixInterpolation : MonoBehaviour
{
    private VectorRenderer vectors;

    private List<Vector3> _transformedVerts;
    
    private readonly List<Vector3> _originalVerts = new()
    {
        new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-0.5f, -0.5f, 0.5f),
        new Vector3(0.5f, -0.5f, -0.5f),  new Vector3(0.5f, -0.5f, 0.5f),
        new Vector3(-0.5f, 0.5f, -0.5f),  new Vector3(-0.5f, 0.5f, 0.5f),
        new Vector3(0.5f, 0.5f, -0.5f),   new Vector3(0.5f, 0.5f, 0.5f),
        new Vector3(0, 0, 0), new Vector3(1, 0, 0),
        new Vector3(0, 1, 0), new Vector3(0, 0, 1)
    };

    [SerializeField, HideInInspector] internal Vector3 startMatrixTarget;
    [SerializeField, HideInInspector] internal Matrix4x4 startMatrix;
    [SerializeField, HideInInspector] internal Matrix4x4 endMatrix;
    [SerializeField, HideInInspector] internal Matrix4x4 changeMatrix;

    [SerializeField][Range(0, 1.0f)] private float interpolationFactor;

    [Header("Toggle Interpolation")]
    [SerializeField] private bool interpolateRotation;
    [SerializeField] private bool interpolateScale;
    [SerializeField] private bool interpolateTranslation;

    // Start is called before the first frame update
    private void Start()
    {
        vectors = GetComponent<VectorRenderer>() ?? gameObject.AddComponent<VectorRenderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        
        ApplyMatrixInterpolation();

        // Multiply the original vertices of the cube with the changeMatrix to get the transformed vertices
        _transformedVerts = MultiplyVertsMatrix(changeMatrix, _originalVerts);

        // Draw the transformed cube
        DrawCube();
        
    }

    private void ApplyMatrixInterpolation()
    {
        changeMatrix = startMatrix;

        // Calculate the rotationMatrix using Quaternion interpolation
        Matrix4x4 rotationMatrix = Matrix4x4Extensions.QuaternionToMatrix(Matrix4x4Extensions.MatrixToQuaternion(startMatrix));

        // Check if interpolation of rotation is enabled
        if (interpolateRotation)
        {
            // Calculate Quaternion values for startMatrix and endMatrix
            Quaternion q = Matrix4x4Extensions.MatrixToQuaternion(startMatrix);
            Quaternion b = Matrix4x4Extensions.MatrixToQuaternion(endMatrix);

            // Interpolate the rotation using Slerp and update the rotationMatrix
            rotationMatrix = Matrix4x4Extensions.QuaternionToMatrix(MatrixMath.Slerp(q, b, interpolationFactor));
        }
        else
        {
            // If interpolation of rotation is disabled, use the startMatrix's rotation directly
            Quaternion q = Matrix4x4Extensions.MatrixToQuaternion(startMatrix);
            rotationMatrix = Matrix4x4Extensions.QuaternionToMatrix(q);
        }

        // Invert the rotationMatrix
        rotationMatrix = rotationMatrix.inverse;

        // Calculate the scaleMatrix for the startMatrix
        float scaleXstart = startMatrix.GetColumn(0).magnitude;
        float scaleYstart = startMatrix.GetColumn(1).magnitude;
        float scaleZstart = startMatrix.GetColumn(2).magnitude;
        Matrix4x4 scaleMatrix = MatrixMath.ScaleMatrix4X4(scaleXstart, scaleYstart,scaleZstart);

        // Check if interpolation of scale is enabled
        if (interpolateScale)
        {
            // Calculate scale values for both startMatrix and endMatrix
            scaleXstart = startMatrix.GetColumn(0).magnitude;
            scaleYstart = startMatrix.GetColumn(1).magnitude;
            scaleZstart = startMatrix.GetColumn(2).magnitude;
            float scaleXend = endMatrix.GetColumn(0).magnitude;
            float scaleYend = endMatrix.GetColumn(1).magnitude;
            float scaleZend = endMatrix.GetColumn(2).magnitude;

            // Interpolate the scale using Lerp and update the scaleMatrix
            float scaleX = MatrixMath.Lerp(scaleXstart, scaleXend, interpolationFactor);
            float scaleY = MatrixMath.Lerp(scaleYstart, scaleYend, interpolationFactor);
            float scaleZ = MatrixMath.Lerp(scaleZstart, scaleZend, interpolationFactor);
            scaleMatrix = MatrixMath.ScaleMatrix4X4(scaleX, scaleY, scaleZ);
        }

        // Combine the scaleMatrix and rotationMatrix
        scaleMatrix *= rotationMatrix;

        // Transpose the changeMatrix
        changeMatrix = scaleMatrix;
        changeMatrix = changeMatrix.Transpose();

        // Clear the translation components of the changeMatrix
        Vector4 mRow0 = changeMatrix.GetColumn(0);
        Vector4 mRow1 = changeMatrix.GetColumn(1);
        Vector4 mRow2 = changeMatrix.GetColumn(2);
        mRow0.w = 0; mRow1.w = 0; mRow2.w = 0;
        changeMatrix.SetColumn(0, mRow0);
        changeMatrix.SetColumn(1, mRow1);
        changeMatrix.SetColumn(2, mRow2);

        // Check if interpolation of translation is enabled
        if (interpolateTranslation)
        {
            // Interpolate the translation values and update the changeMatrix
            Vector3 lerpResult = MatrixMath.Lerp( new Vector3(startMatrix.m03, startMatrix.m13, startMatrix.m23),  new Vector3(endMatrix.m03, endMatrix.m13, endMatrix.m23),interpolationFactor);
            changeMatrix.m03 = lerpResult.x;
            changeMatrix.m13 = lerpResult.y;
            changeMatrix.m23 = lerpResult.z;
        }
        else
        {
            // If interpolation of translation is disabled, use the startMatrix's translation directly
            changeMatrix.m03 = startMatrix.m03;
            changeMatrix.m13 = startMatrix.m13;
            changeMatrix.m23 = startMatrix.m23;
        }
    }

    private void DrawCube()
    {
        using (vectors.Begin())
        {
            vectors.Draw(_transformedVerts[0], _transformedVerts[1], Color.red);
            vectors.Draw(_transformedVerts[2], _transformedVerts[3], Color.red);
            vectors.Draw(_transformedVerts[0], _transformedVerts[2], Color.yellow);
            vectors.Draw(_transformedVerts[1], _transformedVerts[3], Color.yellow);
            vectors.Draw(_transformedVerts[0], _transformedVerts[4], Color.blue);
            vectors.Draw(_transformedVerts[1], _transformedVerts[5], Color.blue);
            vectors.Draw(_transformedVerts[2], _transformedVerts[6], Color.blue);
            vectors.Draw(_transformedVerts[3], _transformedVerts[7], Color.blue);
            vectors.Draw(_transformedVerts[4], _transformedVerts[5], Color.red);
            vectors.Draw(_transformedVerts[6], _transformedVerts[7], Color.red);
            vectors.Draw(_transformedVerts[4], _transformedVerts[6], Color.yellow);
            vectors.Draw(_transformedVerts[5], _transformedVerts[7], Color.yellow);
            
            vectors.Draw(_transformedVerts[8], _transformedVerts[9], Color.red);
            vectors.Draw(_transformedVerts[8],_transformedVerts[10], Color.green);
            vectors.Draw(_transformedVerts[8], _transformedVerts[11], Color.blue);
        }
    }

    // Method to transform a list of Vector3 vertices using a given Matrix4x4
    private List<Vector3> MultiplyVertsMatrix(Matrix4x4 mat, List<Vector3> vertList)
    {
        // Iterate through each vertex in the original vertex list and
        // return the list of transformed vertices
        return vertList.Select(t => mat.MultiplyPoint(t)).ToList();
    }
}
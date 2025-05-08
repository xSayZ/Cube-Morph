using UnityEngine;

public static class Matrix4x4Extensions
{
    public static Vector3 GetTranslation(this Matrix4x4 matrix)
    {
        return matrix.GetColumn(3);
    }

    public static Vector3 GetScale(this Matrix4x4 matrix)
    {
        Vector3 scale;
        scale.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
        scale.y = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
        scale.z = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;
        return scale;
    }

    public static Quaternion GetRotation(this Matrix4x4 matrix)
    {
        // Extract rotation from the matrix using LookRotation method
        // Normalize the result as LookRotation can return a non-normalized quaternion
        return Quaternion.LookRotation(matrix.GetColumn(2), matrix.GetColumn(1)).normalized;
    }
    
    // Converts a Matrix4x4 to a normalized Quaternion.
    public static Quaternion MatrixToQuaternion(Matrix4x4 m)
    {
        m.SetColumn(0, m.GetColumn(0).normalized);
        m.SetColumn(1, m.GetColumn(1).normalized);
        m.SetColumn(2, m.GetColumn(2).normalized);

        float traceW = m.m00 + m.m11 + m.m22;
        float traceX = m.m00 - m.m11 - m.m22;
        float traceY = m.m11 - m.m00 - m.m22;
        float traceZ = m.m22 - m.m00 - m.m11;

        int biggestIndex = 0;
        float traceBiggest = traceW;
        if (traceX > traceBiggest) { traceBiggest = traceX; biggestIndex = 1; }
        if (traceY > traceBiggest) { traceBiggest = traceY; biggestIndex = 2; }
        if (traceZ > traceBiggest) { traceBiggest = traceZ; biggestIndex = 3; }

        float sqrtTrace = Mathf.Sqrt(traceBiggest + 1f) * 0.5f;
        float factor = 0.25f / sqrtTrace;

        float w = 0, x = 0, y = 0, z = 0;

        switch (biggestIndex)
        {
            case 0:
                w = sqrtTrace;
                x = (m.m21 - m.m12) * factor;
                y = (m.m02 - m.m20) * factor;
                z = (m.m10 - m.m01) * factor;
                break;
            case 1:
                x = sqrtTrace;
                w = (m.m21 - m.m12) * factor;
                y = (m.m10 + m.m01) * factor;
                z = (m.m02 + m.m20) * factor;
                break;
            case 2:
                y = sqrtTrace;
                w = (m.m02 - m.m20) * factor;
                x = (m.m10 + m.m01) * factor;
                z = (m.m21 + m.m12) * factor;
                break;
            case 3:
                z = sqrtTrace;
                w = (m.m10 - m.m01) * factor;
                x = (m.m02 + m.m20) * factor;
                y = (m.m21 + m.m12) * factor;
                break;
        }

        return new Quaternion(x, y, z, w);
    }
    
    // Converts a Quaternion to a Matrix4x4.
    public static Matrix4x4 QuaternionToMatrix(Quaternion q)
    {
        float x2 = q.x * 2, y2 = q.y * 2, z2 = q.z * 2;
        float xx = q.x * x2, yy = q.y * y2, zz = q.z * z2;
        float xy = q.x * y2, xz = q.x * z2, yz = q.y * z2;
        float wx = q.w * x2, wy = q.w * y2, wz = q.w * z2;

        Matrix4x4 m = Matrix4x4.identity;
        m.m00 = 1 - (yy + zz); m.m01 = xy + wz;     m.m02 = xz - wy;
        m.m10 = xy - wz;     m.m11 = 1 - (xx + zz); m.m12 = yz + wx;
        m.m20 = xz + wy;     m.m21 = yz - wx;     m.m22 = 1 - (xx + yy);

        return m.transpose;
    }
}
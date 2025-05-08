using System.Collections.Generic;
using UnityEngine;

public static class MatrixMath
{
    // Method to create a scaling Matrix4x4
    public static Matrix4x4 ScaleMatrix4X4(float scaleX, float scaleY, float scaleZ)
    {
        // Create a new identity Matrix4x4, representing no transformation initially
        Matrix4x4 m = Matrix4x4.identity;

        // Set the scaling factors on the diagonal elements of the Matrix4x4
        // The diagonal elements represent scaling along the x, y, and z axes
        m.m00 = scaleX; // Scaling factor for the x-axis
        m.m11 = scaleY; // Scaling factor for the y-axis
        m.m22 = scaleZ; // Scaling factor for the z-axis

        // Return the resulting scaling Matrix4x4
        return m;
    }

    // Method to perform spherical linear interpolation (SLERP) between two Quaternions
    public static Quaternion Slerp(Quaternion q0, Quaternion q1, float t)
    {
        // Create a new Quaternion to store the result of the interpolation
        Quaternion finalQ = new Quaternion();

        // Calculate the dot product between the two Quaternions
        float cosW = Quaternion.Dot(q0, q1);

        // Check if the angle between the two Quaternions is greater than 90 degrees (cosine < 0)
        if (cosW < 0.0f)
        {
            // If the angle is greater than 90 degrees, negate one of the Quaternions
            // to ensure the shortest path for the interpolation
            ScalarQuaternionMultiplication(q1, -1);
        }

        // Variables to store the interpolation weights for q0 and q1
        float k0, k1;

        // Check if the angle between the two Quaternions is very close to 0 degrees (cosine ~ 1)
        if (cosW > 0.9999f)
        {
            // If the angle is very close to 0 degrees, perform a simple linear interpolation
            k0 = 1.0f - t;
            k1 = t;
        }
        else
        {
            // Calculate the angle (omega) and sine of the angle (sinOmega) between the Quaternions
            float sinOmega = Mathf.Sqrt(1.0f - cosW *cosW);

            // Calculate the reciprocal of sinOmega
            float omega = Mathf.Atan2(sinOmega, cosW);
            float inverseSinOmega = 1.0f / sinOmega;

            // Calculate the interpolation weights using spherical linear interpolation formula
            k0 = Mathf.Sin((1.0f - t) * omega) * inverseSinOmega;
            k1 = Mathf.Sin(t * omega) * inverseSinOmega;
        }

        // Interpolate the components of the Quaternions and store the result in finalQ
        finalQ.x = q0.x * k0 + q1.x * k1;
        finalQ.y = q0.y * k0 + q1.y * k1;
        finalQ.z = q0.z * k0 + q1.z * k1;
        finalQ.w = q0.w * k0 + q1.w * k1;

        // Return the resulting interpolated Quaternion
        return finalQ;
    }

    // Method to perform scalar multiplication on a Quaternion
    public static void ScalarQuaternionMultiplication(Quaternion q, float scalar)
    {
        // Scale the individual components of the Quaternion by the scalar value
        q.x *= scalar;
        q.y *= scalar;
        q.z *= scalar;
        q.w *= scalar;
    }

    // Linear interpolation method for Vector3
    public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
    {
        // Perform linear interpolation between vectors 'a' and 'b' based on 't'
        // The resulting vector is (1 - t) times vector 'a' plus 't' times vector 'b'
        return (1 - t) * a + t * b;
    }

    // Linear interpolation method for float values
    public static float Lerp(float a, float b, float t)
    {
        // Perform linear interpolation between 'a' and 'b' based on 't'
        // The result is (1 - t) times 'a' plus 't' times 'b'
        return (1 - t) * a + t * b;
    }
}
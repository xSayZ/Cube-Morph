using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MatrixInterpolation))]
public class MatrixInterpolationEditor : Editor
{
    private void OnSceneGUI()
    {
        var script = (MatrixInterpolation)target;
        if (!script) return;

        DrawHandles(ref script.startMatrix, "Start Matrix");
        DrawHandles(ref script.endMatrix, "End Matrix");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var script = (MatrixInterpolation)target;
        if (!script) return;

        DrawMatrixProperty("Start Matrix", ref script.startMatrix);
        DrawMatrixProperty("End Matrix", ref script.endMatrix);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Start Matrix Determinant", script.startMatrix.determinant.ToString());
        EditorGUILayout.LabelField("Change Matrix Determinant", script.changeMatrix.determinant.ToString());
        EditorGUILayout.LabelField("End Matrix Determinant", script.endMatrix.determinant.ToString());
    }
    
    private void DrawHandles(ref Matrix4x4 matrix, string label)
    {
        EditorGUI.BeginChangeCheck();

        Vector3 pos = matrix.GetTranslation();
        Quaternion rot = matrix.GetRotation();
        Vector3 scale = matrix.GetScale();

        pos = Handles.PositionHandle(pos, rot);
        scale = Handles.ScaleHandle(scale, pos, rot, 0.5f);
        rot = Handles.RotationHandle(rot, pos);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, $"Modify {label}");
            matrix = Matrix4x4.TRS(pos, rot, scale);
            EditorUtility.SetDirty(target);
        }
    }

    private void DrawMatrixProperty(string label, ref Matrix4x4 matrix)
    {
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
        var newMatrix = matrix;

        for (int i = 0; i < 4; i++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int j = 0; j < 4; j++)
            {
                newMatrix[i, j] = EditorGUILayout.FloatField(newMatrix[i, j]);
            }
            EditorGUILayout.EndHorizontal();
        }

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, $"Edit {label}");
            matrix = newMatrix;
            EditorUtility.SetDirty(target);
        }
    }
}

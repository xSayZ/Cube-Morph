using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Vectors;

[ExecuteAlways, RequireComponent(typeof(VectorRenderer))]
public class CubeRenderer : MonoBehaviour
{
    private VectorRenderer vectors;
    public Vector3 Target = Vector3.forward;
    [Range(0, 1)] public float Time = 0.0f;

    [SerializeField, HideInInspector] internal Matrix4x4 A;

    // Start is called before the first frame update
    void Start()
    {
        if (!TryGetComponent<VectorRenderer>(out vectors))
        {
            vectors = gameObject.AddComponent<VectorRenderer>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        using (vectors.Begin())
        {
            var aPos = new Vector3(A.m03, A.m13, A.m23);
            var pos = (1.0f - Time) * aPos + Time * Target;
            vectors.Draw(pos, pos + transform.up, Color.green);

            
        }
    }

}

[CustomEditor(typeof(CubeRenderer))]
public class CubeEditor: Editor
{
    private void OnSceneGUI()
    {
        var cube = target as CubeRenderer;
        if (!cube) return;

        EditorGUI.BeginChangeCheck();

        var aPos = new Vector3(cube.A.m03, cube.A.m13, cube.A.m23);
        var newTarget = Handles.PositionHandle(aPos, cube.transform.rotation);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, "Vector Positions");
            var copy = cube.A;
            copy.m03 = newTarget.x;
            copy.m13 = newTarget.y;
            copy.m23 = newTarget.z;
            cube.A = copy;
            EditorUtility.SetDirty(target);
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        var cube = target as CubeRenderer;
        if (!cube) return;

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Matrix A");
        EditorGUILayout.BeginVertical();

        Matrix4x4 result = Matrix4x4.identity;
        for (int i = 0; i < 4; i++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int j = 0; j < 4; j++)
            {
                EditorGUILayout.BeginHorizontal();
                result[i, j] = EditorGUILayout.FloatField(cube.A[i, j]);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(cube, "Change Matrix");
            cube.A = result;
            EditorUtility.SetDirty(cube);
        }
    }
}

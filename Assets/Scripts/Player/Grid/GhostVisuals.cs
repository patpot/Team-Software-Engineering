using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostVisuals : MonoBehaviour
{
    [SerializeField] private Mesh _mesh;
    [SerializeField] private Material _canBuildMaterial;
    [SerializeField] private Material _cannotBuildMaterial;

    private Vector3 _position;
    private Quaternion _rotation;
    [SerializeField] private Vector3 _scale;
    private Matrix4x4 _matrix;

    void Update()
    {
        _position = this.transform.position;
        _rotation = Quaternion.identity;
        _matrix = Matrix4x4.TRS(_position, _rotation, _scale);

        if (BuildingCollisionCheck.CannotBuild)
        {
            Graphics.DrawMesh(_mesh, _matrix, _cannotBuildMaterial, 0, null, 0);

            if (_mesh.subMeshCount > 1)
            {
                Graphics.DrawMesh(_mesh, _matrix, _cannotBuildMaterial, 0, null, 1);
            }
        }
        else if (!BuildingCollisionCheck.CannotBuild)
        {
            Graphics.DrawMesh(_mesh, _matrix, _canBuildMaterial, 0, null, 0);

            if (_mesh.subMeshCount > 1)
            {
                Graphics.DrawMesh(_mesh, _matrix, _canBuildMaterial, 0, null, 1);
            }
        }
    }
}

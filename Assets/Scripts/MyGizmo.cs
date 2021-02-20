using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGizmo : MonoBehaviour
{
    public enum m_Gizmos
    {
        sphere, box, mesh
    };

    [SerializeField]
    m_Gizmos gizmo = m_Gizmos.box;
    [SerializeField]
    Mesh g_mesh = null;
    [SerializeField]
    Color gizmo_color;
    [SerializeField][Range(0f,1f)]
    float wire_transparency = 1f;
    [SerializeField]
    Vector3 gizmo_size = Vector3.one;
    [SerializeField]
    bool wire = true;
    [SerializeField]
    bool pos_half_size = true;

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmo_color;
        Vector3 g_position;
        if (pos_half_size)
        {
            g_position = new Vector3(transform.position.x, transform.position.y + (gizmo_size.y * 0.5f), transform.position.z);
        }
        else
        {
            g_position = transform.position;
        }

        switch (gizmo)
        {
            case m_Gizmos.box:
                Gizmos.DrawCube(g_position, gizmo_size);

                Gizmos.color = new Color(gizmo_color.r, gizmo_color.g, gizmo_color.b, wire_transparency);
                if (wire)
                    Gizmos.DrawWireCube(g_position, gizmo_size);
                break;
            case m_Gizmos.sphere:
                Gizmos.DrawSphere(g_position, gizmo_size.x);

                Gizmos.color = new Color(gizmo_color.r, gizmo_color.g, gizmo_color.b, wire_transparency);
                if (wire)
                    Gizmos.DrawWireSphere(g_position, gizmo_size.x);
                break;
            case m_Gizmos.mesh:
                Gizmos.DrawMesh(g_mesh, g_position, transform.rotation, gizmo_size);

                Gizmos.color = new Color(gizmo_color.r, gizmo_color.g, gizmo_color.b, wire_transparency);
                if (wire)
                    Gizmos.DrawWireMesh(g_mesh, g_position, transform.rotation, gizmo_size);
                break;
        }
    }
}

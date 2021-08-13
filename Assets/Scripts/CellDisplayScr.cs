using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class CellDisplayScr : MonoBehaviour
{

    #region Public Fields
    public float cellSize;
    #endregion

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    private Transform playerTrans;
 
    #region Unity Methods
 
    void Start()
    {
        playerTrans = CamControlScr.Instance.playerTrans;
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        ResetVertices();
        UpdateMesh();
    }
 
    void FixedUpdate()
    {
        Vector3 pos = playerTrans.position;
        Vector3 bot = new Vector3(Mathf.Floor(pos.x / cellSize),
                                        Mathf.Floor(pos.y / cellSize),
                                        Mathf.Floor(pos.z / cellSize));
        transform.position = bot * cellSize;
    }

 
    #endregion
 
    #region Private Methods
    private void ResetVertices()
    {
        vertices = new Vector3[]{
            new Vector3(0, 0, 0) * cellSize,
            new Vector3(0, 0, 1) * cellSize,
            new Vector3(0, 1, 0) * cellSize,
            new Vector3(0, 1, 1) * cellSize,
            new Vector3(1, 0, 0) * cellSize,
            new Vector3(1, 0, 1) * cellSize,
            new Vector3(1, 1, 0) * cellSize,
            new Vector3(1, 1, 1) * cellSize
        };

        triangles = new int[]
        {
            0, 1, 5,
            0, 2, 3,
            0, 3, 1,
            0, 4, 6,
            0, 5, 4,
            0, 6, 2,
            1, 3, 7,
            1, 7, 5,
            2, 6, 7,
            2, 7, 3,
            4, 5, 7,
            4, 7, 6
        };
    }

    private void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }
    #endregion
}

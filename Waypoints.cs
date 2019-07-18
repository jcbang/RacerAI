using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Waypoints : MonoBehaviour
{
    /// <summary>
    /// Waypoints class controls the logic for visually linking waypoints throughout
	/// the procedurally generated map. Spawning for the waypoints themselves
	/// is controlled in Map Generation.
    /// </summary>
	
    public Color lineColor = Color.red;

    private List<Transform> nodes = new List<Transform>();

	// Draw a line mapping the path
    void OnDrawGizmosSelected()
	{
        Gizmos.color = lineColor;

        Transform [] pathTransforms = GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();

        for (int i = 0; i < pathTransforms.Length; i++)
		{
            if(pathTransforms[i] != transform)
			{
                nodes.Add(pathTransforms[i]);
            }
        }

        for (int i = 0; i < nodes.Count; i++)
		{
            Vector3 currentNode = nodes[i].position;
            Vector3 previousNode = Vector3.zero;

            if (i > 0)
			{
                previousNode = nodes[i - 1].position;

				Gizmos.DrawLine(previousNode, currentNode);
				Gizmos.DrawWireSphere(currentNode, 0.3f);
			}
        }
    }
}

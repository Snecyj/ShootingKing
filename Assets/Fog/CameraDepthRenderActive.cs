using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraDepthRenderActive : MonoBehaviour
{
    public Camera camera;

	private void OnValidate()
	{
		if (camera == null)
			camera = GetComponent<Camera>();
	}

	// Start is called before the first frame update
	void Start()
    {
        //Debug.Log(camera.actualRenderingPath);
      if (camera.actualRenderingPath == RenderingPath.Forward || camera.actualRenderingPath == RenderingPath.VertexLit)
            camera.depthTextureMode = DepthTextureMode.Depth;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Controller : MonoBehaviour
{
    public Car car;
    public float minZoom = 1f;
    public float maxZoom = 5f;
    public float zoomStep = 0.1f;
    public float smoothness = 0.1f;

    private void LateUpdate()
    {
        StartCoroutine(ZoomOutSmoothly());
    }

    private IEnumerator ZoomOutSmoothly()
    {
        WaitForSeconds myWait = new WaitForSeconds(smoothness);
        Camera.main.orthographicSize = minZoom; //you could also comment this line out and just use your current zoomValue... It's just for having a start position for the zoomanimation
        while (Camera.main.orthographicSize < maxZoom)
        {
            Camera.main.orthographicSize += zoomStep * car.speedKPH; //take your players one for the velocity  
            yield return myWait;
        }
        Camera.main.orthographicSize = maxZoom; //ensure it is exactly set to the maxZoom value after the animation
    }

}

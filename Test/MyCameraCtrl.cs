using CameraViewPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCameraCtrl : MonoBehaviour
{
    public List<string> cameraviewName;
    public Transform camera;
    private int index=0;
    private int indexs = 0;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (Singleton<CameraViewManager>.Instance.MoveTager(cameraviewName[index], MoveType.DoTween))
            {
                Debug.Log("1111111111");
                index++;
            }
         
               
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (Singleton<CameraViewManager>.Instance.MoveTager(cameraviewName[indexs], MoveType.none))
            {
                Debug.Log("22222222222");
                indexs++;
            }
            
           
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace CameraViewPro
{
    /// <summary>
    /// 相机的点位信息
    /// </summary>
    [Serializable]
    public class CameraViewMessage
    {
        [Header("相机的视角点位名字")]
        public string idName;
        [Header("相机的视角点位信息")]
        [SerializeField]
        public List<CameraPosData> cameraPosDatas;
        public CameraViewMessage() { }
    }
}
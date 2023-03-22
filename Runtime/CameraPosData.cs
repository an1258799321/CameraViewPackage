
using System;
using UnityEngine;
namespace CameraViewPro
{
    /// <summary>
    /// 相机的位置信息类
    /// </summary>
    [Serializable]
    public class CameraPosData
    {
        [Header("相机位置的序号")]
        public int id;
        [Header("相机位置")]
        public Vector3 pos;
        [Header("相机角度")]
        public Vector3 rot;
        public CameraPosData() { }
    }
}
using System.Collections.Generic;
using UnityEngine;
using System;
using NaughtyAttributes;

namespace CameraViewPro
{
    /// <summary>
    /// 存储相机的点位类
    /// </summary
    [Serializable]
    public class CameraViewData:ScriptableObject
    {     
        [Header("相机的点位信息")]
        [SerializeField] 
        [ReadOnly]
        public List<CameraViewMessage> cameraViewMessages;
        public CameraViewData() { }
    }


}
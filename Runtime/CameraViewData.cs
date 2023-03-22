using System.Collections.Generic;
using UnityEngine;
using System;
using NaughtyAttributes;

namespace CameraViewPro
{
    /// <summary>
    /// �洢����ĵ�λ��
    /// </summary
    [Serializable]
    public class CameraViewData:ScriptableObject
    {     
        [Header("����ĵ�λ��Ϣ")]
        [SerializeField] 
        [ReadOnly]
        public List<CameraViewMessage> cameraViewMessages;
        public CameraViewData() { }
    }


}
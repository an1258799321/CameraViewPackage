using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace CameraViewPro
{
    /// <summary>
    /// ����ĵ�λ��Ϣ
    /// </summary>
    [Serializable]
    public class CameraViewMessage
    {
        [Header("������ӽǵ�λ����")]
        public string idName;
        [Header("������ӽǵ�λ��Ϣ")]
        [SerializeField]
        public List<CameraPosData> cameraPosDatas;
        public CameraViewMessage() { }
    }
}
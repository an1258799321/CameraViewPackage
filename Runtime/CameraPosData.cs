
using System;
using UnityEngine;
namespace CameraViewPro
{
    /// <summary>
    /// �����λ����Ϣ��
    /// </summary>
    [Serializable]
    public class CameraPosData
    {
        [Header("���λ�õ����")]
        public int id;
        [Header("���λ��")]
        public Vector3 pos;
        [Header("����Ƕ�")]
        public Vector3 rot;
        public CameraPosData() { }
    }
}
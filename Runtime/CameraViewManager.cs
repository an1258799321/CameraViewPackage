using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CameraViewPro
{
    /// <summary>
    /// 相机移动管理 目前功能 控制相机移动到视角所在的一个或者多个点位（移动完毕执行回调）方法MoveTager（） 所作限制 必须等待视角内所有点位移动完毕才可再次执行该方法
    /// 
    /// 
    /// TO  DO 功能待定
    /// </summary>
    public class CameraViewManager : Singleton<CameraViewManager>
    {
        private CameraViewData cameraViewData;
        [Header("多个点位移动所间隔的时间")]
        public float time=2f;
        [Header("DownTween执行的时间")]
        public float posdataDeleytime=2;
        [Header("当前相机移动的状态")]
        public CameraState cameraState;
        [Header("当前相机")]
        public Transform cameramain;
        private void Awake()
        {
            DontDestroyOnLoad(this);
            cameraViewData = Resources.Load<CameraViewData>("CameraViewData");
            cameraState = CameraState.none;
            cameramain = Camera.main.transform;
        }
        /// <summary>
        /// 相机移动
        /// </summary>
        /// <param name="camera">所要移动的相机</param>
        /// <param name="viewIdName">视角的名字</param>
        /// <param name="moveType">移动的类型 </param>
        /// <param name="posdataDeleytime">DoTween播放的时间 moveType=MoveType.DoTween 可用 </param>
        /// <param name="action">回调 所有点位移动完毕可用 </param>
        public bool MoveTager( string viewIdName,MoveType moveType,Action action=null) {
            if (cameraState == CameraState.Move) return false;
            MoveViewCtrl( viewIdName, moveType, action);
            return true;
        }
        private void MoveViewCtrl(string viewIdName, MoveType moveType, Action action = null) {
            List<CameraPosData> cameraPosDatas = new List<CameraPosData>();
            cameraPosDatas = GetCameraPosDatas(viewIdName);
            if (cameraPosDatas!=null)
            {
                if (cameraPosDatas.Count==1)
                {
                    MoveType(moveType,  cameraPosDatas[0].pos, cameraPosDatas[0].rot,true, action);
                }
                else
                {
                    StartCoroutine(Moveposs( cameraPosDatas, moveType, action));
                }
            }
        }

        IEnumerator Moveposs(List<CameraPosData> cameraPosDatas, MoveType moveType, Action action) {
            MoveType(moveType,  cameraPosDatas[0].pos, cameraPosDatas[0].rot,false, action);
            for (int i = 1; i < cameraPosDatas.Count; i++)
            {
                yield return new WaitForSeconds(time);
                if (i< cameraPosDatas.Count)
                {
                    MoveType(moveType,  cameraPosDatas[i].pos, cameraPosDatas[i].rot,true, action);
                }
                else
                {
                    MoveType(moveType,  cameraPosDatas[i].pos, cameraPosDatas[i].rot, false, action);
                }
                             
            }
        }
        /// <summary>
        /// 返回True 表示移动完毕
        /// </summary>
        /// <param name="moveType"></param>
        /// <param name="targe"></param>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <param name="posdataDeleytime"></param>
        /// <returns></returns>
        public void MoveType(MoveType moveType,Vector3 pos,Vector3 rot,bool isrunAction, Action action) {
            cameraState = CameraState.Move;
            switch (moveType)
            {
                case CameraViewPro.MoveType.none:
                    cameramain.position = pos;
                    cameramain.localEulerAngles = rot;
                   
                    if (isrunAction)
                    {
                        Debug.Log("执行回调");
                        cameraState = CameraState.none;
                        if (action != null)
                        {
                            action();
                        }
                    }
                    break;
                case CameraViewPro.MoveType.DoTween:
                    cameramain.DORotate(rot, posdataDeleytime, RotateMode.Fast);
                    cameramain.DOMove(pos, posdataDeleytime).OnComplete(()=> {
                      
                        if (isrunAction)
                        {
                            Debug.Log("执行回调");
                            cameraState = CameraState.none;
                            if (action != null)
                            {
                                action();
                            }
                        }

                    });
                    break;
            }
        }


        /// <summary>
        /// 获取CameraViewMessage
        /// </summary>
        /// <returns></returns>
        private CameraViewMessage GetCameraViewMessage(string viewIdName) {
            CameraViewMessage cameraViewMessage = cameraViewData.cameraViewMessages.Find(x => x.idName == viewIdName);
            if (cameraViewMessage==null)
                Debug.LogError("当前viewIdName在配置文件中没找到，请检查配置文件！！！");

            return cameraViewMessage;
        }
        /// <summary>
        /// 获取所有点位
        /// </summary>
        /// <returns></returns>
        private List<CameraPosData> GetCameraPosDatas(string viewIdName)
        {
            CameraViewMessage cameraViewMessage = GetCameraViewMessage(viewIdName);
            if (cameraViewMessage!=null)
            {
                return cameraViewMessage.cameraPosDatas;
            }
            return null;
           
        }
    }

    /// <summary>
    /// 相机移动的类型
    /// </summary>
    public enum MoveType { 
    none,
    DoTween,
    }
    /// <summary>
    /// 相机移动的类型
    /// </summary>
    public enum CameraState
    {
        none,
        Move,
    }
}
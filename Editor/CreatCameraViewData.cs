using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;
using System.Linq;

namespace CameraViewPro {

    public class CreatCameraViewData : EditorWindow
    {
        [MenuItem("Tools/CameraPro/CreatCameraViewData")]
        public static void ShowExample()
        {
            CreatCameraViewData wnd = GetWindow<CreatCameraViewData>();
            wnd.titleContent = new GUIContent("CreatCameraViewData");
        }
        public VisualTreeAsset visualTree;
        private CameraViewData cameraViewData;
        private ScrollView scrollView;
        private Dictionary<VisualElement, List<VisualElement>> keyValuePairs = new Dictionary<VisualElement, List<VisualElement>>();

        private VisualElement nowVisualElement;
        private VisualElement nowVisualElementCameraDataIndex;

        private List<VisualElement> NowvisualElementschild = new List<VisualElement>();
        private DropdownField dropdownFieldCameraView;
        private DropdownField dropdownFieldCameraData;
        private VisualElement dropdownParent;
        /// <summary>
        /// 是否处理下拉框改变 事件出发
        /// </summary>
        private bool isdisposeCameraViewdropdown=false;
        private bool isdisposeCameraDatadropdown=false;

        public Transform camera;

        private SerializedProperty propertypos;
        private SerializedProperty propertyrot;


        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;


            // Import UXML
            //var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/CreatCameraViewData.uxml");
            VisualElement labelFromUXML = visualTree.Instantiate();
            root.Add(labelFromUXML);

            Initproperty();


            dropdownParent = rootVisualElement.Q<VisualElement>("CreatData");
            Button addDataBut = rootVisualElement.Q<Button>("AddCameraViewData");
            Button addmessageDataBut = rootVisualElement.Q<Button>("AddCameraViewMessageData");
            Button RemoveDataBut = rootVisualElement.Q<Button>("RemoveCameraViewData");
            Button RemovemessageDataBut = rootVisualElement.Q<Button>("RemoveCameraViewMessageData");
            scrollView = rootVisualElement.Q<ScrollView>("CameraViewDataMessage");

            dropdownFieldCameraData = rootVisualElement.Q<DropdownField>("NowCameraData");

            Button saveButton = rootVisualElement.Q<Button>("Yes");
            ObjectField objectField= rootVisualElement.Q<ObjectField>("NowData");
            addDataBut.clicked += AddDataBut_clicked;
            saveButton.clicked += YseButton_clicked;
            addmessageDataBut.clicked += AddmessageDataBut_clicked;
            RemoveDataBut.clicked += RemoveDataBut_clicked;
            RemovemessageDataBut.clicked += RemovemessageDataBut_clicked;
            List<string> vs = new List<string>();

            dropdownFieldCameraView = new DropdownField("当前视角", vs, 0, SelectCameraView);           
            dropdownFieldCameraData = new DropdownField("当前点位", vs, 0, SelectCameraData);
            dropdownParent.Add(dropdownFieldCameraView);
            dropdownParent.Add(dropdownFieldCameraData);
            dropdownFieldCameraView.style.width = 300;
            dropdownFieldCameraView.style.left = 150;
            dropdownFieldCameraView.style.top = 22;


            dropdownFieldCameraData.style.width = 300;
            dropdownFieldCameraData.style.left = 550;

            CreatCameraViewDataAsset();
            
            objectField.value = cameraViewData;
            objectField.SetEnabled(false);


            RefreshPanel();

           
        }
        /// <summary>
        /// 移除当前选择的视角数据
        /// </summary>
        private void RemovemessageDataBut_clicked()
        {
            if (keyValuePairs.Count>0)
            {
                if (keyValuePairs.ContainsKey(nowVisualElement))
                {
                    keyValuePairs.Remove(nowVisualElement);
                    scrollView.Remove(nowVisualElement);
                    if (keyValuePairs.Count > 0)
                    {
                        foreach (var item in keyValuePairs)
                        {
                            nowVisualElement = item.Key;
                            NowvisualElementschild = item.Value;
                            break;
                        }

                    }
                    else
                    {
                        nowVisualElement = null;
                        NowvisualElementschild = new List<VisualElement>();
                    }
                    RefreshPanel();
                }
                else
                {
                    ShowNotification(new GUIContent("删除视角不存在，请检查！！！"));
                }
            }
            else
            {
                ShowNotification(new GUIContent("当前数据为空！！！"));
            }
          
            

        }
        /// <summary>
        /// 移除当前点位
        /// </summary>
        private void RemoveDataBut_clicked()
        {
            if (keyValuePairs.Count > 0)
            {
                if (keyValuePairs.ContainsKey(nowVisualElement))
                {
                    if (NowvisualElementschild.Count > 1)
                    {
                        Debug.Log("删除前当前字典里面的数据  " + keyValuePairs[nowVisualElement].Count);
                        NowvisualElementschild.Remove(nowVisualElementCameraDataIndex);
                        nowVisualElement.Remove(nowVisualElementCameraDataIndex);

                        keyValuePairs[nowVisualElement] = NowvisualElementschild;
                        Debug.Log("删除后当前字典里面的数据  " + keyValuePairs[nowVisualElement].Count);

                        RefreshDropDownCameraData();
                    }
                    else
                    {
                        ShowNotification(new GUIContent("删除失败！请确保视角中存在一个点位信息"));
                    }
                }
                else
                {
                    ShowNotification(new GUIContent("删除点位视角不存在，请检查！！！"));
                }
            }
            else
            {
                ShowNotification(new GUIContent("当前数据为空！！！"));
            }
        }

        /// <summary>
        /// 添加相机数据
        /// </summary>
        private void AddmessageDataBut_clicked()
        {
            VisualElement visualElement1 = SetCameraViewMessageVisualElement();
            nowVisualElement = visualElement1;
            VisualElement visualElement2 = SetCameraPosDataVisualElement();

            nowVisualElement.Add(visualElement2);
            NowvisualElementschild = new List<VisualElement>();
            NowvisualElementschild.Add(visualElement2);

            scrollView.Add(nowVisualElement);
            keyValuePairs.Add(nowVisualElement, NowvisualElementschild);

            

            RefreshPanel();
        }



        /// <summary>
        /// 添加点位数据
        /// </summary>
        private void AddDataBut_clicked()
        {
            if (nowVisualElement==null)
            {
                ShowNotification(new GUIContent("当前视角列表为空，请先添加视角！！"));
                return;
            }
            if (keyValuePairs.ContainsKey(nowVisualElement))
            {
                VisualElement visualElement2 = SetCameraPosDataVisualElement();
                nowVisualElement.Add(visualElement2);

                NowvisualElementschild = new List<VisualElement>();
                NowvisualElementschild = keyValuePairs.GetValueOrDefault(nowVisualElement);
                NowvisualElementschild.Add(visualElement2);
                keyValuePairs.Remove(nowVisualElement);
                keyValuePairs.Add(nowVisualElement, NowvisualElementschild);

                RefreshPanel();
            }
           
        }
        /// <summary>
        /// 确认按钮(保存)
        /// </summary>
        private void YseButton_clicked()
        {
            SaveData();
        }
        /// <summary>
        /// 获取生成的相机点位数据
        /// </summary>
        public VisualElement SetCameraPosDataVisualElement() {
            VisualElement visualElement = new VisualElement();
            IntegerField integerField = new IntegerField("点位id");
            integerField.name = "id";
            integerField.value = keyValuePairs.ContainsKey(nowVisualElement)==true? NowvisualElementschild.Count:0;
            Vector3Field vector3Fieldpos = new Vector3Field("点位pos");
            vector3Fieldpos.name = "pos";
            Vector3Field vector3FieldRot = new Vector3Field("点位rot");
            vector3FieldRot.name = "rot";
            visualElement.Add(integerField);
            visualElement.Add(vector3Fieldpos);
            visualElement.Add(vector3FieldRot);
            return visualElement;
        }
        /// <summary>
        /// 获取生成的相机视角信息
        /// </summary>
        public VisualElement SetCameraViewMessageVisualElement()
        {
            VisualElement visualElement = new VisualElement();
            TextField textField = new TextField("视角名字");
            textField.value = "视角"+ keyValuePairs.Count;
            textField.name = "idName";
            visualElement.Add(textField);
            return visualElement;
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        public void SaveData() {
            if (!CheckData()) { this.ShowNotification(new GUIContent("数据检查出现错误请查看错误信息！！！！")); return;  }
            cameraViewData.cameraViewMessages = new List<CameraViewMessage>();
            foreach (var item in keyValuePairs)
            {
                CameraViewMessage cameraViewMessage = new CameraViewMessage();
                cameraViewMessage.idName = GetIdName(item.Key);
                cameraViewMessage.cameraPosDatas = GetCameraPosDatas(item.Value);
                cameraViewData.cameraViewMessages.Add(cameraViewMessage);
            }
            RefreshPanel();
            EditorUtility.SetDirty(cameraViewData);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            this.ShowNotification(new GUIContent("数据保存成功！！！！"));
        }

        /// <summary>
        /// 获取数据（视角）
        /// </summary>
        /// <param name="visualElement"></param>
        /// <returns></returns>
        public string GetIdName(VisualElement visualElement) {
            TextField textField = visualElement.Q<TextField>("idName");
            return textField.value;

        }
        /// <summary>
        /// 生成数据（视角）
        /// </summary>
        /// <param name="visualElement"></param>
        /// <returns></returns>
        public void SetIdName(VisualElement visualElement,string value)
        {
            TextField textField = visualElement.Q<TextField>("idName");
            textField.value = value;
        }
        /// <summary>
        /// 获取数据（点位）
        /// </summary>
        /// <param name="visualElements"></param>
        /// <returns></returns>
        public List<CameraPosData> GetCameraPosDatas(List<VisualElement> visualElements) {

            List<CameraPosData> cameraPosDatas = new List<CameraPosData>(); 
            for (int i = 0; i < visualElements.Count; i++)
            {
                CameraPosData cameraPosData = new CameraPosData();

                IntegerField integerField = visualElements[i].Q <IntegerField>("id");
                Vector3Field vector3Fieldpos = visualElements[i].Q<Vector3Field>("pos");
                Vector3Field vector3FieldRot = visualElements[i].Q<Vector3Field>("rot");
                cameraPosData.id = integerField.value;
                cameraPosData.pos = vector3Fieldpos.value;
                cameraPosData.rot = vector3FieldRot.value;
                cameraPosDatas.Add(cameraPosData);
            }
            return cameraPosDatas;
        }
        /// <summary>
        /// 生成数据（点位）
        /// </summary>
        /// <param name="visualElements"></param>
        /// <returns></returns>
        public void SetCameraPosDatas(VisualElement visualElement, CameraPosData cameraPosData)
        {
            IntegerField integerField = visualElement.Q<IntegerField>("id");
            Vector3Field vector3Fieldpos = visualElement.Q<Vector3Field>("pos");
            Vector3Field vector3FieldRot = visualElement.Q<Vector3Field>("rot");
            integerField.value = cameraPosData.id;
            vector3Fieldpos.value = cameraPosData.pos;
            vector3FieldRot.value = cameraPosData.rot;
        }
        /// <summary>
        /// 创建存储相机视角的文件
        /// </summary>
        public  void CreatCameraViewDataAsset()
        {
            if (!Resources.Load<CameraViewData>("CameraViewData"))
            {
                cameraViewData = ScriptableObject.CreateInstance<CameraViewData>();
                AssetDatabase.CreateAsset(cameraViewData, "Assets/Resources/CameraViewData.asset");
                AssetDatabase.SaveAssets();
            }
            else
            {
                cameraViewData = Resources.Load<CameraViewData>("CameraViewData");//
                InitReadData();
            }
            

        }
        /// <summary>
        /// 选择相机视角下拉框
        /// </summary>
        public string SelectCameraView(string value) {
            if (!isdisposeCameraViewdropdown) {
                isdisposeCameraViewdropdown = true;
                return null;
            }

            Debug.Log("视角 下拉框 更新！！！！");
            if (value!=null)
            {
                nowVisualElement = FinKeyVisualElement(value);
                if (nowVisualElement!=null)
                {
                    if (keyValuePairs.ContainsKey(nowVisualElement))
                    {
                        NowvisualElementschild = keyValuePairs[nowVisualElement];
                    }
                    RefreshDropDownCameraData();
                }
               
            }        
            return value;
        }
        /// <summary>
        /// 选择相机点位下拉框
        /// </summary>
        public string SelectCameraData(string value) {
            if (!isdisposeCameraDatadropdown)
            {
                isdisposeCameraDatadropdown = true;
                return null;
            }
            Debug.Log("点位 下拉框 更新！！！！");
            if (value != null)
            {
                nowVisualElementCameraDataIndex = FinCameraDataVisualElement(value);
                if (nowVisualElementCameraDataIndex != null)
                {
                    SetPosAndRot();
                }
            }       
            return value;
        }
        /// <summary>
        /// 刷新面板
        /// </summary>
        public void RefreshPanel() {
            RefreshDropDownCameraView();
            RefreshDropDownCameraData();
        }
        /// <summary>
        /// 刷新相机视角下拉框
        /// </summary>
        public void RefreshDropDownCameraView() {
            List<string> values = new List<string>();
            foreach (var item in keyValuePairs)
            {
                values.Add(GetIdName(item.Key));
            }
            isdisposeCameraViewdropdown = false;
            dropdownFieldCameraView.choices = values;
          
            if (nowVisualElement != null)
            {
                Debug.Log("跟新视角下拉框初始值！！");
                if (dropdownFieldCameraView.value != null && dropdownFieldCameraView.value == GetIdName(nowVisualElement))
                {
                    isdisposeCameraDatadropdown = false;
                    dropdownFieldCameraView.value = null;
                }
                dropdownFieldCameraView.value = GetIdName(nowVisualElement);
            }
            
        }
        /// <summary>
        /// 刷新相机点位下拉框
        /// </summary>
        public void RefreshDropDownCameraData()
        {
            List<string> values = new List<string>();
            //if (NowvisualElementschild!=null&& NowvisualElementschild.Count>0)
            //{
            List<CameraPosData> cameraPosDatas = GetCameraPosDatas(NowvisualElementschild);

            for (int i = 0; i < cameraPosDatas.Count; i++)
            {
                values.Add(cameraPosDatas[i].id.ToString());
            }
            isdisposeCameraDatadropdown = false;
            dropdownFieldCameraData.choices = values;
            if (cameraPosDatas.Count > 0)
            {
                if (dropdownFieldCameraData.value != null && dropdownFieldCameraData.value == values[cameraPosDatas.Count - 1])
                {
                    isdisposeCameraDatadropdown = false;
                    dropdownFieldCameraData.value = null;
                }
                dropdownFieldCameraData.value = values[cameraPosDatas.Count - 1];
            }

            //}


        }
        /// <summary>
        /// 初始化面板的数据
        /// </summary>
        public void InitReadData() {
            if (cameraViewData.cameraViewMessages == null) return;
            for (int i = 0; i < cameraViewData.cameraViewMessages.Count; i++)
            {
                VisualElement visualElement = SetCameraViewMessageVisualElement();
                nowVisualElement = visualElement;
                SetIdName(visualElement, cameraViewData.cameraViewMessages[i].idName);
                NowvisualElementschild = new List<VisualElement>();
                for (int j = 0; j < cameraViewData.cameraViewMessages[i].cameraPosDatas.Count; j++)
                {
                    VisualElement visualElementdata = SetCameraPosDataVisualElement();
                    NowvisualElementschild.Add(visualElementdata);
                    SetCameraPosDatas(visualElementdata, cameraViewData.cameraViewMessages[i].cameraPosDatas[j]);
                    visualElement.Add(visualElementdata);
                }
                keyValuePairs.Add(nowVisualElement, NowvisualElementschild);
                scrollView.Add(visualElement);
            }

        }
        /// <summary>
        /// 查找当前选择的视角
        /// </summary>
        /// <param name="idname"></param>
        /// <returns></returns>
        public VisualElement FinKeyVisualElement(string idname) {
            if (idname == null) return null;
            foreach (var item in keyValuePairs)
            {
                if (GetIdName(item.Key) == idname)
                {
                    return item.Key;
                }
            }
            Debug.Log("没有查找到对应的Key(通过名字)");
            return null;
        }
        /// <summary>
        /// 查找当前选择视角对应的点位
        /// </summary>
        /// <param name="idname"></param>
        /// <returns></returns>
        public VisualElement FinCameraDataVisualElement(string id)
        {
            if (id == null) return null;
            for (int i = 0; i < NowvisualElementschild.Count; i++)
            {
                //Debug.Log(NowvisualElementschild[i].Q<IntegerField>("id").value+"   "+ int.Parse(id));
                if (NowvisualElementschild[i].Q<IntegerField>("id").value==int.Parse(id))
                {
                    return NowvisualElementschild[i];
                }
            }
            Debug.Log("没有查找到对应的Value(通过名字)");
            return null;
        }

        /// <summary>
        /// 检查是否有重复的IDName 或者id
        /// </summary>
        /// <returns></returns>
        public bool CheckData() {
            List<CameraViewMessage> cameraViewMessages = new List<CameraViewMessage>();
            foreach (var item in keyValuePairs)
            {
                CameraViewMessage cameraViewMessage = new CameraViewMessage();
                cameraViewMessage.idName = GetIdName(item.Key);
                cameraViewMessage.cameraPosDatas = GetCameraPosDatas(item.Value);
                cameraViewMessages.Add(cameraViewMessage);
            }
            List<string> idname = new List<string>();
            for (int i = 0; i < cameraViewMessages.Count; i++)
            {
                idname.Add(cameraViewMessages[i].idName);
            }
            if (idname.Distinct().Count() != idname.Count())
            {
                Debug.Log("视角ID有重复请检查！！   ！");
                return false;
            }

            List<string> id;
            for (int i = 0; i < cameraViewMessages.Count; i++)
            {
                id = new List<string>();
                for (int j = 0; j < cameraViewMessages[i].cameraPosDatas.Count; j++)
                {
                    id.Add(cameraViewMessages[i].cameraPosDatas[j].id.ToString());
                }
                if (id.Distinct().Count() != id.Count())
                {
                    Debug.Log("请检查视角名字为  "+ cameraViewMessages[i].idName+"  里面数据id重复");
                    return false;
                }
            }
            return true;
        }


        Vector3Field vector3Fieldpos;
        Vector3Field vector3FieldRot;
        /// <summary>
        /// 生成点位信息
        /// </summary>
        public void SetPosAndRot() {
            if (vector3Fieldpos!=null|| vector3FieldRot != null)
            {
                vector3Fieldpos.Unbind();
                vector3FieldRot.Unbind();
            }
            vector3Fieldpos = nowVisualElementCameraDataIndex.Q<Vector3Field>("pos");
            vector3FieldRot = nowVisualElementCameraDataIndex.Q<Vector3Field>("rot");
            
            vector3Fieldpos.BindProperty(propertypos);         
            vector3FieldRot.BindProperty(propertyrot);
        }
        /// <summary>
        /// 初始化绑定
        /// </summary>
        public void Initproperty() {
            camera = Camera.main.transform;
            SerializedObject so = new SerializedObject(camera);
            propertypos = so.FindProperty("m_LocalPosition");
            propertyrot = so.FindProperty("m_LocalEulerAnglesHint");
        }

    }
}
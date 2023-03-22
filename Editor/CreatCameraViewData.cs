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
        /// �Ƿ���������ı� �¼�����
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

            dropdownFieldCameraView = new DropdownField("��ǰ�ӽ�", vs, 0, SelectCameraView);           
            dropdownFieldCameraData = new DropdownField("��ǰ��λ", vs, 0, SelectCameraData);
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
        /// �Ƴ���ǰѡ����ӽ�����
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
                    ShowNotification(new GUIContent("ɾ���ӽǲ����ڣ����飡����"));
                }
            }
            else
            {
                ShowNotification(new GUIContent("��ǰ����Ϊ�գ�����"));
            }
          
            

        }
        /// <summary>
        /// �Ƴ���ǰ��λ
        /// </summary>
        private void RemoveDataBut_clicked()
        {
            if (keyValuePairs.Count > 0)
            {
                if (keyValuePairs.ContainsKey(nowVisualElement))
                {
                    if (NowvisualElementschild.Count > 1)
                    {
                        Debug.Log("ɾ��ǰ��ǰ�ֵ����������  " + keyValuePairs[nowVisualElement].Count);
                        NowvisualElementschild.Remove(nowVisualElementCameraDataIndex);
                        nowVisualElement.Remove(nowVisualElementCameraDataIndex);

                        keyValuePairs[nowVisualElement] = NowvisualElementschild;
                        Debug.Log("ɾ����ǰ�ֵ����������  " + keyValuePairs[nowVisualElement].Count);

                        RefreshDropDownCameraData();
                    }
                    else
                    {
                        ShowNotification(new GUIContent("ɾ��ʧ�ܣ���ȷ���ӽ��д���һ����λ��Ϣ"));
                    }
                }
                else
                {
                    ShowNotification(new GUIContent("ɾ����λ�ӽǲ����ڣ����飡����"));
                }
            }
            else
            {
                ShowNotification(new GUIContent("��ǰ����Ϊ�գ�����"));
            }
        }

        /// <summary>
        /// ����������
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
        /// ��ӵ�λ����
        /// </summary>
        private void AddDataBut_clicked()
        {
            if (nowVisualElement==null)
            {
                ShowNotification(new GUIContent("��ǰ�ӽ��б�Ϊ�գ���������ӽǣ���"));
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
        /// ȷ�ϰ�ť(����)
        /// </summary>
        private void YseButton_clicked()
        {
            SaveData();
        }
        /// <summary>
        /// ��ȡ���ɵ������λ����
        /// </summary>
        public VisualElement SetCameraPosDataVisualElement() {
            VisualElement visualElement = new VisualElement();
            IntegerField integerField = new IntegerField("��λid");
            integerField.name = "id";
            integerField.value = keyValuePairs.ContainsKey(nowVisualElement)==true? NowvisualElementschild.Count:0;
            Vector3Field vector3Fieldpos = new Vector3Field("��λpos");
            vector3Fieldpos.name = "pos";
            Vector3Field vector3FieldRot = new Vector3Field("��λrot");
            vector3FieldRot.name = "rot";
            visualElement.Add(integerField);
            visualElement.Add(vector3Fieldpos);
            visualElement.Add(vector3FieldRot);
            return visualElement;
        }
        /// <summary>
        /// ��ȡ���ɵ�����ӽ���Ϣ
        /// </summary>
        public VisualElement SetCameraViewMessageVisualElement()
        {
            VisualElement visualElement = new VisualElement();
            TextField textField = new TextField("�ӽ�����");
            textField.value = "�ӽ�"+ keyValuePairs.Count;
            textField.name = "idName";
            visualElement.Add(textField);
            return visualElement;
        }

        /// <summary>
        /// ��������
        /// </summary>
        public void SaveData() {
            if (!CheckData()) { this.ShowNotification(new GUIContent("���ݼ����ִ�����鿴������Ϣ��������")); return;  }
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
            this.ShowNotification(new GUIContent("���ݱ���ɹ���������"));
        }

        /// <summary>
        /// ��ȡ���ݣ��ӽǣ�
        /// </summary>
        /// <param name="visualElement"></param>
        /// <returns></returns>
        public string GetIdName(VisualElement visualElement) {
            TextField textField = visualElement.Q<TextField>("idName");
            return textField.value;

        }
        /// <summary>
        /// �������ݣ��ӽǣ�
        /// </summary>
        /// <param name="visualElement"></param>
        /// <returns></returns>
        public void SetIdName(VisualElement visualElement,string value)
        {
            TextField textField = visualElement.Q<TextField>("idName");
            textField.value = value;
        }
        /// <summary>
        /// ��ȡ���ݣ���λ��
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
        /// �������ݣ���λ��
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
        /// �����洢����ӽǵ��ļ�
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
        /// ѡ������ӽ�������
        /// </summary>
        public string SelectCameraView(string value) {
            if (!isdisposeCameraViewdropdown) {
                isdisposeCameraViewdropdown = true;
                return null;
            }

            Debug.Log("�ӽ� ������ ���£�������");
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
        /// ѡ�������λ������
        /// </summary>
        public string SelectCameraData(string value) {
            if (!isdisposeCameraDatadropdown)
            {
                isdisposeCameraDatadropdown = true;
                return null;
            }
            Debug.Log("��λ ������ ���£�������");
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
        /// ˢ�����
        /// </summary>
        public void RefreshPanel() {
            RefreshDropDownCameraView();
            RefreshDropDownCameraData();
        }
        /// <summary>
        /// ˢ������ӽ�������
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
                Debug.Log("�����ӽ��������ʼֵ����");
                if (dropdownFieldCameraView.value != null && dropdownFieldCameraView.value == GetIdName(nowVisualElement))
                {
                    isdisposeCameraDatadropdown = false;
                    dropdownFieldCameraView.value = null;
                }
                dropdownFieldCameraView.value = GetIdName(nowVisualElement);
            }
            
        }
        /// <summary>
        /// ˢ�������λ������
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
        /// ��ʼ����������
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
        /// ���ҵ�ǰѡ����ӽ�
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
            Debug.Log("û�в��ҵ���Ӧ��Key(ͨ������)");
            return null;
        }
        /// <summary>
        /// ���ҵ�ǰѡ���ӽǶ�Ӧ�ĵ�λ
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
            Debug.Log("û�в��ҵ���Ӧ��Value(ͨ������)");
            return null;
        }

        /// <summary>
        /// ����Ƿ����ظ���IDName ����id
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
                Debug.Log("�ӽ�ID���ظ����飡��   ��");
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
                    Debug.Log("�����ӽ�����Ϊ  "+ cameraViewMessages[i].idName+"  ��������id�ظ�");
                    return false;
                }
            }
            return true;
        }


        Vector3Field vector3Fieldpos;
        Vector3Field vector3FieldRot;
        /// <summary>
        /// ���ɵ�λ��Ϣ
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
        /// ��ʼ����
        /// </summary>
        public void Initproperty() {
            camera = Camera.main.transform;
            SerializedObject so = new SerializedObject(camera);
            propertypos = so.FindProperty("m_LocalPosition");
            propertyrot = so.FindProperty("m_LocalEulerAnglesHint");
        }

    }
}
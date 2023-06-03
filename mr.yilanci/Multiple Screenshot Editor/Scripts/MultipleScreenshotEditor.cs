using System;
using System.IO;
using System.Collections;
using UnityEngine;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Compilation;


namespace MultipleScreenshot.Editor
{
    using Enumerators;
    public class MultipleScreenshotEditor : EditorWindow
    {

        public  static MultipleScreenshotManager _MultipleScreenshotManager;
        
        
        private GUIStyle _guiStyle = new GUIStyle();
        private Vector2 _scroll = Vector2.zero;
        
        public DeviceList _Root ;
        public SerializedObject _soData;
        

        
        [MenuItem("Window/mr.yilanci/Multiple Screenshot Editor #t")]
        public static void ShowWindow()
        {
           
            
            MultipleScreenshotEditor window = (MultipleScreenshotEditor) GetWindow(typeof(MultipleScreenshotEditor), false, "Multiple Screenshot Editor");
            window.minSize = new Vector2(340, 445);
            window.Show();
        }

        
        
        private void OnEnable()
        {
            _MultipleScreenshotManager = new MultipleScreenshotManager();
            _MultipleScreenshotManager.StartingReadJson(_Root);
            ScriptableObject target = _MultipleScreenshotManager._target;
            
            _soData = new SerializedObject(target);
            
            _MultipleScreenshotManager.StartingReadJson_2();
           
            
        }
/*
      void OnDisable()
      {
          _MultipleScreenshotManager.ClosingWriteJson(_MultipleScreenshotManager._Root);
      }
*/
        private void OnGUI()
        {
            // Save Current Device Data  update 
            //_MultipleScreenshotManager.SaveCurrentDevice(_Root);
         
           
            Debug.Log("......"+_Root.Device.Count);
            
            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            


            
            
            GUILayout.BeginVertical();
            _guiStyle.fontSize = 11;
            _guiStyle.normal.textColor = Color.white;
            EditorGUILayout.LabelField(("Outfile Folder: "+_MultipleScreenshotManager._dataPath ),_guiStyle,GUILayout.Height(15));
            GUILayout.Space(2);
            EditorStyles.helpBox.fontSize = 12;
            EditorStyles.helpBox.normal.textColor=Color.yellow;
            EditorStyles.helpBox.fontStyle = FontStyle.Bold;
            EditorGUILayout.LabelField(_MultipleScreenshotManager._guide, EditorStyles.helpBox,GUILayout.Height(30));
            GUILayout.Space(2);
            EditorGUILayout.LabelField("Number of the output folder : "+_MultipleScreenshotManager._saveSetting.counter,_guiStyle,GUILayout.Height(15));
            GUILayout.EndVertical();
            
            if (_MultipleScreenshotManager._dataPath == "")
            {
                _MultipleScreenshotManager._guide = "You have to select folder for save Screenshot";

            }
            else if (_Root.Device.Count == 0)
            {
                _MultipleScreenshotManager._guide = "You have to add device resolution ";

            }
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical();
            
            _soData.Update();
            SerializedProperty stringsProperty = _soData.FindProperty("_Root");
            EditorGUILayout.PropertyField(stringsProperty, new GUIContent("Device_Root"), true); // True means show children
            _soData.ApplyModifiedProperties();
            
          
            
            EditorGUILayout.EndVertical();
            
            GUILayout.Space(27);
            GUILayout.BeginHorizontal();
            
            if (EditorApplication.isPlaying != true &  _Root.Device.Count ==0 )
            {
                GUI.backgroundColor = Color.red;
            }
            if (EditorApplication.isPlaying != true & (_MultipleScreenshotManager._dataPath !=""|| _Root.Device.Count > 0) )
            {
                GUI.backgroundColor = Color.green;
            }
            else
            {
                GUI.backgroundColor = Color.yellow;
                if (_MultipleScreenshotManager._dataPath == "")
                {
                    Debug.Log(_MultipleScreenshotManager._dataPath +"You have to select folder for save Screenshot");
                    _MultipleScreenshotManager._guide = "You have to select folder for save Screenshot";
                }
            }
            
            if (GUILayout.Button( "Play Unity",GUILayout.Height(50),GUILayout.MinWidth(160)))
            {
                if (EditorApplication.isPlaying != true)
                    EditorApplication.isPlaying = true;
            }

            GUI.backgroundColor = EditorApplication.isPlaying != true ? Color.red : Color.green;
            if (GUILayout.Button( "Take Screenshot",GUILayout.Height(50),GUILayout.MinWidth(160)))
            {
            
                if (EditorApplication.isPlaying & _Root.Device.Count !=0)
                {
                    _MultipleScreenshotManager._guide = "Editor Working";
                    _MultipleScreenshotManager.DelayUseAsync(_Root);
                    
                }
                else if (EditorApplication.isPlaying & _Root.Device.Count ==0)
                {
                    _MultipleScreenshotManager._guide = "You have to add device resolution ";
                }
                else
                {
                    _MultipleScreenshotManager._guide = "You have to Play Unity";
                }

            }
           
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
       
            GUI.backgroundColor = Color.yellow;

            
            
            if (GUILayout.Button ("Load Json",GUILayout.Height(50),GUILayout.MinWidth(160)))
            {
                _MultipleScreenshotManager.LoadSavedDeviceJson(_Root);
            }

        
            GUI.backgroundColor = _Root.Device.Count == 0 ? Color.red : Color.yellow;
            if (GUILayout.Button ( "Save to Json",GUILayout.Height(50),GUILayout.MinWidth(160)))
            {
                _MultipleScreenshotManager.SaveDeviceJson(_Root);
           
            }
          
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            
            GUI.backgroundColor = _MultipleScreenshotManager._dataPath == "" ? Color.red : Color.green;
       
            if(GUILayout.Button ( "Show Folder",GUILayout.Height(50),GUILayout.MinWidth(160)))
            {

                _MultipleScreenshotManager.ShowFolder();

            }


            GUI.backgroundColor = _MultipleScreenshotManager._saveSetting.location != ""? Color.yellow : Color.green;

            if (GUILayout.Button ( "Where Save SS",GUILayout.Height(50),GUILayout.MinWidth(160)))
            {
                _MultipleScreenshotManager.WhereSave(_Root);

            }
            GUILayout.EndHorizontal();
      
            
            GUI.backgroundColor = Time.timeScale == 0 ? Color.green : Color.red;
          
            if (GUILayout.Button ( "Time Scale 1 ",GUILayout.Height(50)))
            {
                _MultipleScreenshotManager.TimeScale_1();
            }
            
            EditorGUILayout.EndScrollView();
            
        }

    }

}

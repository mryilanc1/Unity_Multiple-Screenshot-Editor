using System;
using System.IO;
using System.Collections;
using UnityEngine;
using System.Threading.Tasks;
using UnityEditor.Recorder.Input;
using UnityEditor.Recorder;
using UnityEditor;
using UnityEditor.Compilation;


namespace MultipleScreenshot.Editor
{
    using Enumerators;
    public class MultipleScreenshotManager : EditorWindow
    {
        
        public  string _guide = "Guide";
       
        public  SaveSetting.SaveSetting _saveSetting;
        
        public  string _location;
        public  string _dataPath;
        public  string _counterHash;


        public ScriptableObject _target ;
        
        


        public int _rootElementCount;


        

        public void StartingReadJson( DeviceList _Root )
        {
           
            // Load Currently Device Data from Json
            TextAsset json_yazi = new TextAsset(File.ReadAllText(Application.dataPath +"/mr.yilanci/Multiple Screenshot Editor/Json_Device_Currently_Save_List.json"));
            _Root = JsonUtility.FromJson<DeviceList>(json_yazi.text);
            _target = this;

            /*
            
            // Load Editor data  - Counter - location - click link bool for pop-up data
            TextAsset json_setting = new TextAsset(File.ReadAllText(Application.dataPath +"/mr.yilanci/Multiple Screenshot Editor/Json_Setting.json"));
            _saveSetting = JsonUtility.FromJson<SaveSetting.SaveSetting>(json_setting.text);
            */
        }
        public void StartingReadJson_2( )
        {
            // Load Editor data  - Counter - location - click link bool for pop-up data
            TextAsset json_setting = new TextAsset(File.ReadAllText(Application.dataPath +"/mr.yilanci/Multiple Screenshot Editor/Json_Setting.json"));
            _saveSetting = JsonUtility.FromJson<SaveSetting.SaveSetting>(json_setting.text);
            
        }

        public void ClosingWriteJson( DeviceList _Root)
        {
            // Save Currently Device Data from Json
            string Json_added = JsonUtility.ToJson(_Root);
            File.WriteAllText(Application.dataPath +"/mr.yilanci/Multiple Screenshot Editor/Json_Device_Currently_Save_List.json",Json_added);
            
            // Save Editor data  - Counter - location - click link bool  for pop-up data
            string Json_setting = JsonUtility.ToJson(_saveSetting);
            File.WriteAllText(Application.dataPath +"/mr.yilanci/Multiple Screenshot Editor/Json_Setting.json",Json_setting);
        }

        public void SaveCurrentDevice( DeviceList _Root)
        {
            // Save Currently Device List 
            if (_Root.Device.Count != _rootElementCount )
            {
                string Json_added = JsonUtility.ToJson(_Root);
                File.WriteAllText(Application.dataPath +"/mr.yilanci/Multiple Screenshot Editor/Json_Device_Currently_Save_List.json",Json_added);
                _rootElementCount = _Root.Device.Count;
            }
            
            // Assign Editor data  (Counter - location - click for pop-up data) to _dataPath
            _dataPath = _saveSetting.location;
        }

        public void PlayUnity()
        {
            if (EditorApplication.isPlaying != true)
                EditorApplication.isPlaying = true;
        }

        public void CheckAndTakeSS( DeviceList _Root)
        {
            if (EditorApplication.isPlaying & _Root.Device.Count !=0)
            {
                _guide = "Editor Working";
                DelayUseAsync(_Root);
            }
            else if (EditorApplication.isPlaying & _Root.Device.Count ==0)
            {
                _guide = "You have to add device resolution ";
            }
            else
            {
                _guide = "You have to Play Unity";
            }
        }

        public void LoadSavedDeviceJson( DeviceList _Root)
        {   /// Loading Saved Device Data from json
            TextAsset json_yazi = new TextAsset(File.ReadAllText(Application.dataPath +"/mr.yilanci/Multiple Screenshot Editor/Json_Device_Save_List.json"));
            _Root = JsonUtility.FromJson<DeviceList>(json_yazi.text);
                
            _guide = "You have to Play Unity after Click to Take ScreenShot";
            Debug.Log("Loaded Device List From Json ");
        }


        public void SaveDeviceJson( DeviceList _Root)
        {
            string Json_added = JsonUtility.ToJson(_Root);
            File.WriteAllText(Application.dataPath +"/mr.yilanci/Multiple Screenshot Editor/Json_Device_Save_List.json",Json_added);
        }
        
        public void ShowFolder()
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                Application.OpenURL(_saveSetting.location);
            }

            else if (Application.platform == RuntimePlatform.OSXEditor)
            {
                System.Diagnostics.Process.Start("open", (_saveSetting.location));
            }
        }

        public void WhereSave( DeviceList _Root)
        {
            _location = EditorUtility.OpenFolderPanel(_location, "", "");
            _saveSetting.location = _location;
                
            if (_Root.Device.Count == 0)
            {
                _guide = "You have to add device resolution ";
                Debug.Log("You have to add device resolution");
            }
            else 
            {
                _guide = "You have to Play Unity after Click to Take ScreenShot";
            }

        }

        public void TimeScale_1()
        {
            Time.timeScale = 1;
        }
        

       

         public async void DelayUseAsync( DeviceList _Root)
         {
             _counterHash = _saveSetting.counter.ToString();
             
             if (Path.IsPathRooted(_dataPath) )
             {
                 float counter = 0;
                 float DevicesCounts = _Root.Device.Count;
                     
                     foreach (var unit in _Root.Device)
                     { 
                         
                         EditorUtility.DisplayProgressBar("MultipleScreenshotEditor.cs", "Editor Working - Device in Progress: ''"+unit.DeviceName+"''" , counter /DevicesCounts);
                         Time.timeScale = 0;
                         Take_SS(unit.DeviceName, unit.Height, unit.Width,_dataPath,_counterHash);
                         await Task.Delay(1250);
                         counter++;
                     }
                 _guide = "Completed";
                 ShowFolder();

                 _saveSetting.counter++;
                 
                 
                 
                 
                 if (_saveSetting.clickDownload == false)
                 {
                     PopUp.Init();
                     
                 }
                
             }

             else
             {
                 _guide = "!!! Write Path of outfile";
             }
             
             string Json_setting = JsonUtility.ToJson(_saveSetting);
             File.WriteAllText(Application.dataPath +"/mr.yilanci/Multiple Screenshot Editor/Json_Setting.json",Json_setting);
             
             EditorUtility.ClearProgressBar();
         }
         
         public static void Take_SS(string name, int Height, int width,string _dataPath,string _counterHash)
         {
             var controllerSettings = CreateInstance<RecorderControllerSettings>();
             var m_RecorderController = new RecorderController(controllerSettings);

             // Image sequence
             var imageRecorder = CreateInstance<ImageRecorderSettings>();
             imageRecorder.name = "My Image Recorder";
             imageRecorder.Enabled = true;
             imageRecorder.OutputFormat = ImageRecorderSettings.ImageRecorderOutputFormat.PNG;
             imageRecorder.CaptureAlpha = false;


             imageRecorder.imageInputSettings = new GameViewInputSettings
             {
                 OutputWidth = width,
                 OutputHeight = Height,
             };
             // Setup Recording

             controllerSettings.AddRecorderSettings(imageRecorder);
             controllerSettings.SetRecordModeToFrameInterval(0, 1);
             m_RecorderController.PrepareRecording();

             m_RecorderController.StartRecording();
               
               
             Directory.CreateDirectory(_dataPath +"/"+ _counterHash); // returns a DirectoryInfo object
             
             imageRecorder.OutputFile = Path.Combine(_dataPath +"/"+_counterHash, name) + DefaultWildcard.Frame;
             var recorderWindow = UnityEditor.EditorWindow.GetWindow<RecorderWindow>();
             recorderWindow.StopRecording();
             recorderWindow.Close();
         }

         
         
    }
    public class PopUp : EditorWindow
    {

        private GUIStyle _guiStyle2 = new GUIStyle();
        private GUIStyle _guiStyle3 = new GUIStyle();
        public static SaveSetting.SaveSetting _saveSetting;
        
        [MenuItem("Window/mr.yilanci/Give Rate Pop-up")]
        public static void Init()
        {
            PopUp window = ScriptableObject.CreateInstance<PopUp>();
            window.minSize = new Vector2(600, 335);
            window.maxSize = new Vector2(600, 335);
            window.maximized = false;
            window.Show();
            
        }

        private void OnEnable()
        {
            TextAsset json_setting = new TextAsset(File.ReadAllText(Application.dataPath +"/mr.yilanci/Multiple Screenshot Editor/Json_Setting.json"));
            _saveSetting = JsonUtility.FromJson<SaveSetting.SaveSetting>(json_setting.text);
            Debug.Log(_saveSetting.clickDownload+"0");
        }

        private void OnDisable()
        {
            string Json_setting = JsonUtility.ToJson(_saveSetting);
            File.WriteAllText(Application.dataPath +"/mr.yilanci/Multiple Screenshot Editor/Json_Setting.json",Json_setting);
            Debug.Log(_saveSetting.clickDownload+"1");
        }

        void OnGUI()
        {
         
            _guiStyle2.fontSize = 14;
            _guiStyle2.normal.textColor = Color.white;
            _guiStyle2.fontStyle = FontStyle.Bold;
            
            _guiStyle3.fontSize = 11;
            _guiStyle3.normal.textColor = Color.white;
            _guiStyle3.fontStyle = FontStyle.Bold;
 
            

            EditorGUI.LabelField(new Rect(5, 2 , position.width, 20),"Multiple Screenshot Editor - Rate Pop-Up ",_guiStyle3 );
            
            EditorGUI.LabelField(new Rect(280, 12 , position.width, 20),"Please",_guiStyle2);
        
            EditorGUI.LabelField(new Rect(120, 32 , position.width, 20),"Give Rate for Multiple Screenshot Editor on Asset Store",_guiStyle2);
            
            Texture banner = (Texture)AssetDatabase.LoadAssetAtPath("Assets/mr.yilanci/Multiple Screenshot Editor/Logo/ss_icon.png", typeof(Texture));
            
            
            EditorGUI.DrawPreviewTexture(new Rect(239, 58 , 128 , 128),banner); 
           
            
            
            if (GUI.Button(new Rect(50, 202 , 600 , 15),"https://assetstore.unity.com/packages/tools/utilities/multiple-screenshot-editor-235183",EditorStyles.linkLabel))
            {
                _saveSetting.clickDownload = true;

                Application.OpenURL("https://assetstore.unity.com/packages/tools/utilities/multiple-screenshot-editor-235183");
                Debug.Log(_saveSetting.clickDownload+"3");
  
            }
            
            
            
            GUILayout.Space(235f);
            
            
            if (GUILayout.Button("Yes,I Will Give Rate - Open The Asset's Page",GUILayout.Height(40)))
            {
                Application.OpenURL("https://assetstore.unity.com/packages/tools/utilities/multiple-screenshot-editor-235183");
                _saveSetting.clickDownload = true;
                Debug.Log(_saveSetting.clickDownload+"4");

                this.Close();
            }
            
            if (GUILayout.Button("No,Close Window",GUILayout.Height(40))) this.Close();
            
            
        }
    }
    
}
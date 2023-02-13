using System.Collections.Generic;
using MultipleScreenshot.Unit;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultipleScreenshot.Enumerators
{
    [System.Serializable]
    
    public class DeviceList
    { [FormerlySerializedAs("Devices")] [NonReorderable]
      public List<DeviceUnit> Device = new List<DeviceUnit>();
    }
}
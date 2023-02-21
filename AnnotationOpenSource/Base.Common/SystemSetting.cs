using Base.ConfigServer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace Base.Common
{
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class SystemSetting
    {
     /*   [Category("Cameras")]
        [DisplayName("Cameras")]
        public List<Camera> CameraList { get; set; }*/
        //public List<ServerSockets> SocketServerList;
        public List<SequenceConfig> SequenceConfigList;
        //[Category("Lighting")]
        //[DisplayName("Lighting")]
        //public List<LightingControllerConfig> LigthingControllerList { get; set; }
        public SystemSetting()
        {
            Machine = new Machine();
           // ImageArchive = new ImageArchive();
          //  SocketServerList = new List<ServerSockets>();
            SequenceConfigList = new List<SequenceConfig>();
          //  CameraList = new List<Camera>();
            FolderPath = new FolderPath();
           // LigthingControllerList = new List<LightingControllerConfig>();


        }
        [Category("Machine")]
        [DisplayName("Machine Info")]
        public Machine Machine
        {
            get; set;
        }

        [Category("Folder Path")]
        [DisplayName("Folder Path")]
        public FolderPath FolderPath
        {
            get; set;
        }
       /* [Category("Image Archive")]
        [DisplayName("Image Archive")]
        public ImageArchive ImageArchive
        {
            get;set;
        }*/
        public void Save(SystemSetting sys)
        {
            Serializer.XmlSave(sys, FolderPath.SystemFile);
        }
        public void Open()
        {

        }
    }

    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Machine
    {
        public List<string> ModuleName;
        public Machine()
        {
            ModuleName = new List<string>();
        }
        [DisplayName("MachineName")]
        public string MachineName { get; set; } = "AnnotationsCreator";
        [DisplayName("Machine Mode")]
        public bool Standalone { get; set; } = true;
        [DisplayName("Number of Module")]
        public int NumCoreModule { get; set; } = 0;
        [DisplayName("Number of Sequence")]
        public int NumOfSeq { get; set; } = 1;
        [DisplayName("Simulation")]
        public bool Simulation { get; set; } = true;
    }

    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ServerSockets : NetworkDevice
    {
        public ServerSockets()
        {
            IPAddress = "127.0.0.1";
            Port = 5000;
        }
    }

    public class NetworkDevice
    {
        public NetworkDevice()
        {

        }
        [DisplayName("Name")]
        public string Name { get; set; } = "DEFAULT";
        [DisplayName("IPAddress")]
        public string IPAddress { get; set; } = "10.10.10.10";
        [DisplayName("Port")]
        public int Port { get; set; } = 5000;
    }
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ImageArchive
    {
        public ImageArchive()
        {

        }
        [DisplayName("Image Archive Folder")]
        public string ImgArchFolder{ get; set; } = @"..\Vision Setting\Image";
        [DisplayName("Archive All Image")]
        public bool ImgArchiveAll { get; set; } = true;
        [DisplayName("Archive Fail Image")]
        public bool ImgArchiveFail { get; set; } = false;
        [DisplayName("Clean Up Archive Image Days")]
        public int ImgArchiveCleanUpDay { get; set; } = 30;
        [DisplayName("Enable Auto Clean Up")]
        public bool EnableAutoCleanUp { get; set; } = false;

    }
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class FolderPath
    {
        public FolderPath()
        {

        }
        [DisplayName("Security File")]
        public string SecurityConfig { get; set; } = @"D:\TridentVision\System Setting\pwd.Config";
        [DisplayName("Machine Log")]
        public string MachineLog { get; set; } = @"..\Vision Setting\Log\System.txt";
        [DisplayName("Error Log")]
        public string ErrorLog { get; set; } = @"..\Vision Setting\Log\Error.txt";
        [DisplayName("Communication Log")]
        public string CommunicationLog { get; set; } = @"..\Vision Setting\Log\Communication.txt";
        [DisplayName("System File")]
        public string SystemFile { get; set; } = @"..\Vision Setting\System Setting\SystemSetting.Config";
        [DisplayName("Inspection Recipe Folder")]
        public string InspRecipeFolder { get; set; } = @"..\Vision Setting\Recipe";
        [DisplayName("Log Folder")]
        public string LogFolder { get; set; } = @"..\Vision Setting\Log";
    }

    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Camera
    {
        public Camera()
        {

        }
        public Camera(int i)
        {
            ID = i;
        }
        public Camera(int i, string camerabrand, string model, string serialNumber, string m_interface, string address)
        {
            ID = i;
            CameraBrand = camerabrand;
            Model = model;
            SerialNumber = serialNumber;
            Interface = m_interface;
            Address = address;
        }
        [Category("Camera")]
        [DisplayName("ID")]
        [PropertyOrder(1)]
        public int ID { get; set; } = 0;
        [Category("Camera")]
        [DisplayName("Camera Brand")]
        [PropertyOrder(2)]
        public string CameraBrand { get; set; } = "Basler";
        [Category("Camera")]
        [DisplayName("Model")]
        [PropertyOrder(3)]
        public string Model { get; set; } = "Null";
        [Category("Camera")]
        [DisplayName("Serial Number")]
        [PropertyOrder(4)]
        public string SerialNumber { get; set; } = "Null";
        [Category("Camera")]
        [DisplayName("Interface")]
        [PropertyOrder(5)]
        public string Interface { get; set; } = "Null";
        [Category("Camera")]
        [DisplayName("Address")]
        [PropertyOrder(6)]
        public string Address { get; set; } = "Null";
        [Category("Camera")]
        [DisplayName("Full Name")]
        [PropertyOrder(7)]
        public string FullName { get; set; } = "Null";
        [Category("Camera")]
        [DisplayName("Enable Rotation")]
        [PropertyOrder(9)]
        public bool EnableRotation { get; set; } = false;
        [Category("Camera")]
        [DisplayName("Rotation Angle")]
        [PropertyOrder(10)]
        public double RotationAngle { get; set; } = 0.0;


    }
    [Serializable]
    public class Parameter
    {
        public Parameter()
        {

        }
        public Parameter(int i)
        {
            ID = i;
        }
        public Parameter(int i, string paramName, string value, string types)
        {
            ID = i;
            Param = paramName;
            Value = value;
            Types = types;
        }
        [DisplayName("ID")]
        public int ID { get; set; } = 0;
        [DisplayName("Param")]
        public string Param { get; set; } = "Null";
        [DisplayName("Value")]
        public string Value { get; set; } = "Null";
        [DisplayName("Types")]
        public string Types { get; set; } = "Null";
    }
    [Serializable]
    public class SequenceConfig
    {
        public SequenceConfig()
        {

        }

        [DisplayName("ID")]
        public int ID { get; set; } = 0;
        [DisplayName("Reference")]
        public string Reference { get; set; } = "Null";

    }
}

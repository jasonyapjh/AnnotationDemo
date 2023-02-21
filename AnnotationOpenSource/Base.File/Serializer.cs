using Base.Common;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace Base.ConfigServer
{
    static public class Serializer
    {
        static public string LastErrorMessage;
        static public bool XmlSave(object inputObj, string path)
        {
            XmlSerializer serializer = null;
            System.IO.StreamWriter fWriter = null;
            try
            {
                serializer = new XmlSerializer(inputObj.GetType());
                fWriter = new System.IO.StreamWriter(path);
                serializer.Serialize(fWriter, inputObj);
                fWriter.Close();
                return true;
            }
            catch (Exception ex)
            {
                if (fWriter != null)
                    fWriter.Close();
                //LastErrorMessage = ex.GetDetailMessage();
                //return false;
                throw ex;
            }
        }
        static public bool XmlSave(object inputObj, string path, Type type)
        {
            XmlSerializer serializer = null;
            System.IO.StreamWriter fWriter = null;
            try
            {
                serializer = new XmlSerializer(type);
                fWriter = new System.IO.StreamWriter(path);
                serializer.Serialize(fWriter, inputObj);
                fWriter.Close();
                return true;
            }
            catch (Exception ex)
            {
                if (fWriter != null)
                    fWriter.Close();
                //LastErrorMessage = ex.GetDetailMessage(); ;
                //return false;
                throw ex;
            }
        }

        static public object XmlLoad(object inputObj, string path)
        {
            XmlSerializer serializer = null;
            FileStream fStream = null;
            TextReader reader = null;
            try
            {
                serializer = new XmlSerializer(inputObj.GetType());
                fStream = new FileStream(path, FileMode.Open);
                fStream.Position = 0;
                reader = new StreamReader(fStream);

                inputObj = serializer.Deserialize(reader);
                fStream.Close();
                reader.Close();
                return inputObj;
            }
            catch (Exception ex)
            {
                //LastErrorMessage = ex.GetDetailMessage();
                if (fStream != null)
                    fStream.Close();
                if (reader != null)
                    reader.Close();
                //return null;
                throw ex;
            }
        }

        static public object XmlLoad(Type type, string path)
        {
            XmlSerializer serializer = null;
            FileStream fStream = null;
            TextReader reader = null;
            object returnObj;
            try
            {
                serializer = new XmlSerializer(type);
                fStream = new FileStream(path, FileMode.Open);
                fStream.Position = 0;
                reader = new StreamReader(fStream);

                returnObj = serializer.Deserialize(reader);
                fStream.Close();
                reader.Close();
                return returnObj;
            }
            catch (Exception ex)
            {
                //LastErrorMessage = ex.GetDetailMessage();
                if (fStream != null)
                    fStream.Close();
                if (reader != null)
                    reader.Close();
                //return null;
                throw ex;
            }
        }
        static public object BinaryLoad(string path)
        {
            // Construct a BinaryFormatter and use it to serialize the data to the stream.
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fs = null;
            try
            {
                fs = new FileStream(path, FileMode.Open);
                object obj = formatter.Deserialize(fs);
                fs.Close();
                return obj;
            }
            catch (Exception ex)
            {
                if (fs != null)
                    fs.Close();
                LastErrorMessage = 
                    ex.GetDetailMessage();
                return false;
            }
        }

        static public bool BinarySave(object inputObj, string path)
        {
            // Construct a BinaryFormatter and use it to serialize the data to the stream.
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fs = null;
            try
            {
                fs = new FileStream(path, FileMode.Create);
                formatter.Serialize(fs, inputObj);
                fs.Close();
                return true;
            }
            catch (Exception ex)
            {
                if (fs != null)
                    fs.Close();
                LastErrorMessage = ex.GetDetailMessage();
                return false;
            }
        }

        // Sample clone function from Code Project
        public static T Clone<T>(this T RealObject)
        {
            using (Stream objectStream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, RealObject);
                objectStream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(objectStream);
            }
        }

        public static object XmlClone(this object input)
        {
            using (System.IO.MemoryStream Mems = new System.IO.MemoryStream())
            {
                XmlSerializer Serializer = new XmlSerializer(input.GetType());
                //var fWriter = new System.IO.StreamWriter("temp");
                Serializer.Serialize(Mems, input);
                Mems.Position = 0;
                object Deserialized = Serializer.Deserialize(Mems);
                return Deserialized;
            }
        }

        // http://www.gamedev.net/community/forums/topic.asp?topic_id=306333
        public static byte[] SerializeToBinary(object anything)
        {
            int structsize = Marshal.SizeOf(anything);
            IntPtr buffer = Marshal.AllocHGlobal(structsize);
            Marshal.StructureToPtr(anything, buffer, false);
            byte[] streamdatas = new byte[structsize];
            Marshal.Copy(buffer, streamdatas, 0, structsize);
            Marshal.FreeHGlobal(buffer);
            return streamdatas;
        }

        // http://www.gamedev.net/community/forums/topic.asp?topic_id=306333
        public static object DeserializeFromBinary(byte[] rawdatas, Type anytype)
        {
            int rawsize = Marshal.SizeOf(anytype);
            if (rawsize > rawdatas.Length) return null;
            IntPtr buffer = Marshal.AllocHGlobal(rawsize);
            Marshal.Copy(rawdatas, 0, buffer, rawsize);
            object retobj = Marshal.PtrToStructure(buffer, anytype);
            Marshal.FreeHGlobal(buffer);
            return retobj;
        }


    }
}

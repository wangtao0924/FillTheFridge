using ProtoBuf;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Protobuf
{
    public static class ProtobufTool
    {
        static string localSaveDataPath = Application.persistentDataPath + "/LocalSaveData/";
        public static byte[] Serialize<T>(T serializaData)
        {
            byte[] data = null;
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            try
            {
                bf.Serialize(ms, serializaData);
                ms.Seek(0, SeekOrigin.Begin);
                data = ms.ToArray();
            }
            catch (SerializationException e)
            {
                Debug.LogError("数据序列化失败:" + e.Message);
            }
            finally
            {
                ms.Close();
            }
            return data;
        }

        public static T DeSerialize<T>(byte[] bytes)
        {
            T data = default(T);
            MemoryStream ms = new MemoryStream(bytes);
            BinaryFormatter bf = new BinaryFormatter();
            try
            {
                data = (T)bf.Deserialize(ms);
            }
            catch (SerializationException e)
            {
                Debug.LogError("数据反序列化失败:" + e.Message);
            }
            finally
            {
                ms.Close();
            }
            return data;
        }

        public static void WriteLocalSaveData<T>(T data,string fileName)
        {
            if (!Directory.Exists(localSaveDataPath))
            {
                Directory.CreateDirectory(localSaveDataPath);
            }
            string filePath = localSaveDataPath + fileName + ".bin";
            using (FileStream file = new FileStream(filePath, FileMode.Create,FileAccess.Write))
            {
                Serializer.Serialize(file, data);
            }
        }

        public static T ReadLocalSaveData<T>(string fileName)
        {
            T data=default;
            if (!Directory.Exists(localSaveDataPath))
            {
                Directory.CreateDirectory(localSaveDataPath);
            }
            string filePath = localSaveDataPath + fileName + ".bin";
            if (!File.Exists(filePath))
            {
                return data;
            }
            using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                if (file!=null)
                {
                   data = Serializer.Deserialize<T>(file);
                }
                return data;
            }
        }
    }
    /*
[ProtoContract]
public class Person
{
[ProtoMember(1)]
public int Id { get; set; }
[ProtoMember(2)]
public string Name { get; set; }
}
*/
}
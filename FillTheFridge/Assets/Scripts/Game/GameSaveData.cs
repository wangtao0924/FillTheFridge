using Protobuf;
using ProtoBuf;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSaveData
{
    private static GameSaveData instance;
    public LocalSaveData localSaveData;
    public GameSaveData()
    {
        localSaveData = ProtobufTool.ReadLocalSaveData<LocalSaveData>("localSaveData");
        if (localSaveData == null)
        {
            localSaveData = new LocalSaveData();
        }
    }


    public static GameSaveData Instance
    {
        get 
        {
            if (instance==null)
            {
                instance = new GameSaveData();
            }
            return instance;
        }
    }

    public void SaveData()
    {
        ProtobufTool.WriteLocalSaveData(localSaveData, "localSaveData");
    }

}
[ProtoContract]
public class LocalSaveData
{
    [ProtoMember(1)]
    public bool voiceOff;
    [ProtoMember(2)]
    public bool shakeOff;
    [ProtoMember(3)]
    public int gameLevel=1;
}
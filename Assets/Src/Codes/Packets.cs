using UnityEngine;
using ProtoBuf;
using System.IO;
using System.Buffers;
using System.Collections.Generic;
using System;

public class Packets : MonoBehaviour
{
    public enum PacketType { Ping = 0, Normal = 1, Pong = 2, Location = 3 }

    public enum HandlerIds {
        Init = 0,
        Ping = 1,
        LocationUpdate = 11
    }

    public static void Serialize<T>(IBufferWriter<byte> writer, T data)
    {
        Serializer.Serialize(writer, data);
    }

    public static T Deserialize<T>(byte[] data) {
        try {
            using (var stream = new MemoryStream(data)) {
                return ProtoBuf.Serializer.Deserialize<T>(stream);
            }
        } catch (Exception ex) {
            Debug.LogError($"Deserialize: Failed to deserialize data. Exception: {ex}");
            throw;
        }
    }
    public static string ExtractValue(string json, string key)
    {
        string pattern = $"\"{key}\":";
        int startIndex = json.IndexOf(pattern) + pattern.Length;
        int endIndex;

        if (json[startIndex] == '\"')
        {
            startIndex++;
            endIndex = json.IndexOf('\"', startIndex);
        }
        else
        {
            endIndex = json.IndexOfAny(new char[] { ',', '}' }, startIndex);
        }

        return json.Substring(startIndex, endIndex - startIndex).Trim('\"', ' ', '\t', '\n', '\r');
    }
}

[ProtoContract]
public class InitialPayload
{
    [ProtoMember(1, IsRequired = true)]
    public string deviceId { get; set; }

    [ProtoMember(2, IsRequired = true)]
    public uint playerId { get; set; }
    
    [ProtoMember(3, IsRequired = true)]
    public float latency { get; set; }
}

[ProtoContract]
public class CommonPacket
{
    [ProtoMember(1)]
    public uint handlerId { get; set; }

    [ProtoMember(2)]
    public string userId { get; set; }

    [ProtoMember(3)]
    public string version { get; set; }

    [ProtoMember(4)]
    public uint sequence { get; set; }

    [ProtoMember(5)]
    public byte[] payload { get; set; }
}

[ProtoContract]
public class LocationUpdatePayload {
    [ProtoMember(1, IsRequired = true)]
    public float x { get; set; }
    [ProtoMember(2, IsRequired = true)]
    public float y { get; set; }
}

[ProtoContract]
public class LocationUpdate
{
    [ProtoMember(1)]
    public List<UserLocation> users { get; set; }

    [ProtoContract]
    public class UserLocation
    {
        [ProtoMember(1)]
        public string id { get; set; }

        [ProtoMember(2)]
        public uint playerId { get; set; }

        [ProtoMember(3)]
        public float x { get; set; }

        [ProtoMember(4)]
        public float y { get; set; }
    }
}

[ProtoContract]
public class Response {
    [ProtoMember(1)]
    public uint handlerId { get; set; }

    [ProtoMember(2)]
    public uint responseCode { get; set; }

    [ProtoMember(3)]
    public long timestamp { get; set; }

    [ProtoMember(4)]
    public byte[] data { get; set; }

    [ProtoMember(5)]
    public uint sequence { get; set; }
}

public class InitialData
{
    public string userId { get; set; }
    public float x { get; set; }
    public float y { get; set; }
}

[ProtoContract]
public class Ping
{
    [ProtoMember(1)]
    public long timestamp { get; set; }
}

[ProtoContract]
public class Pong
{
    [ProtoMember(1)]
    public long timestamp { get; set; }
}

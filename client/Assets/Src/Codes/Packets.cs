using UnityEngine;
using ProtoBuf;
using System.IO;
using System.Buffers;
using System.Collections.Generic;
using System;
using JetBrains.Annotations;

public class Packets : MonoBehaviour
{
    public enum PacketType
    {
        PING = 0,
        NORMAL = 1,
        GAME_START = 2,
        LOCATION = 3,
        GAME_END = 4,
        CHATTING = 5,
        MATCHMAKING = 6,
        ATTACK = 40,
        SKILL = 50,
    }

    public static void Serialize<T>(IBufferWriter<byte> writer, T data)
    {
        Serializer.Serialize(writer, data);
    }

    public static T Deserialize<T>(byte[] data)
    {
        try
        {
            using (var stream = new MemoryStream(data))
            {
                return ProtoBuf.Serializer.Deserialize<T>(stream);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Deserialize: Failed to deserialize data. Exception: {ex}");
            throw;
        }
    }
}

[ProtoContract]
public class RegisterPayload
{
    [ProtoMember(1, IsRequired = true)]
    public string playerId { get; set; }

    [ProtoMember(2, IsRequired = true)]
    public string password { get; set; }

    [ProtoMember(3, IsRequired = true)]
    public string name { get; set; }
}

[ProtoContract]
public class LoginPayload
{
    [ProtoMember(1, IsRequired = true)]
    public string playerId { get; set; }

    [ProtoMember(2, IsRequired = true)]
    public string password { get; set; }

    [ProtoMember(3, IsRequired = true)]
    public float frame { get; set; }
}

[ProtoContract]
public class JoinLobbyPayload
{
    [ProtoMember(1, IsRequired = true)]
    public uint characterId { get; set; }
}

[ProtoContract]
public class CharacterEarnPayload
{
    [ProtoMember(1, IsRequired = true)]
    public uint characterId { get; set; }
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
    public byte[] payload { get; set; }

    [ProtoMember(5)]
    public uint sequence { get; set; }
}

[ProtoContract]
public class Ping
{
    [ProtoMember(1)]
    public long timestamp { get; set; }
}

[ProtoContract]
public class LocationUpdatePayload
{
    [ProtoMember(1, IsRequired = true)]
    public float x { get; set; }
    [ProtoMember(2, IsRequired = true)]
    public float y { get; set; }
    [ProtoMember(3, IsRequired = true)]
    public bool isLobby { get; set; }
}

[ProtoContract]
public class ChattingPayload
{
    [ProtoMember(1, IsRequired = true)]
    public string message { get; set; }
    [ProtoMember(2, IsRequired = true)]
    public uint type { get; set; }
    [ProtoMember(3, IsRequired = true)]
    public bool isLobby { get; set; }
}

[ProtoContract]
public class SkillPayload
{
    [ProtoMember(1, IsRequired = true)]
    public float x { get; set; }

    [ProtoMember(2, IsRequired = true)]
    public float y { get; set; }

    [ProtoMember(3, IsRequired = true)]
    public float rangeX { get; set; }

    [ProtoMember(4, IsRequired = true)]
    public float rangeY { get; set; }
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
        public string playerId { get; set; }

        [ProtoMember(2)]
        public uint characterId { get; set; }

        [ProtoMember(3)]
        public float x { get; set; }

        [ProtoMember(4)]
        public float y { get; set; }
        
        [ProtoMember(5)]
        public float direction { get; set; }
    }
}

[ProtoContract]
public class ChattingUpdate
{
    [ProtoMember(1)]
    public string playerId { get; set; }

    [ProtoMember(2)]
    public string message { get; set; }

    [ProtoMember(3)]
    public uint type { get; set; }
}

[ProtoContract]
public class SkillUpdate
{
    [ProtoMember(1)]
    public string playerId { get; set; }

    [ProtoMember(2)]
    public float x { get; set; }

    [ProtoMember(3)]
    public float y { get; set; }

    [ProtoMember(4)]
    public float rangeX { get; set; }

    [ProtoMember(5)]
    public float rangeY { get; set; }
}

[ProtoContract]
public class AttackedSuccess
{
    [ProtoMember(1)]
    public string playerId { get; set; }

    [ProtoMember(2)]
    public float hp { get; set; }
}

[ProtoContract]
public class Response
{
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


[ProtoContract]
public class CharacterChoice
{
    [ProtoMember(1)]
    public string playerId { get; set; }

    [ProtoMember(2)]
    public string name { get; set; }

    [ProtoMember(3)]
    public string sessionId { get; set; }
}

[ProtoContract]
public class CharacterSelect
{
    [ProtoMember(1)]
    public string playerId { get; set; }

    [ProtoMember(2)]
    public string name { get; set; }

    [ProtoMember(3)]
    public string sessionId { get; set; }

    [ProtoMember(4)]
    public List<Possession> possession { get; set; }

    [ProtoContract]
    public class Possession
    {
        [ProtoMember(1)]
        public long playerId { get; set; }

        [ProtoMember(2)]
        public long characterId { get; set; }
    }
}

[ProtoContract]
public class GameEndPayload
{
    [ProtoMember(1)]
    public string result { get; set; }

    [ProtoMember(2)]
    public List<UserState> users { get; set; }

    [ProtoContract]
    public class UserState
    {
        [ProtoMember(1)]
        public string name { get; set; }

        [ProtoMember(2)]
        public uint kill { get; set; }

        [ProtoMember(3)]
        public uint death { get; set; }

        [ProtoMember(4)]
        public uint damage { get; set; }
    }
}

[ProtoContract]
public class ReturnLobbyRequestPayload
{
    [ProtoMember(1, IsRequired = true)]
    public string message { get; set; }
}


[ProtoContract]
public class ExitPayload
{
    [ProtoMember(1, IsRequired = true)]
    public string message { get; set; }
}


[ProtoContract]
public class MatchingPayload
{
    [ProtoMember(1, IsRequired = true)]
    public string sessionId { get; set; }

}

[ProtoContract]
public class MatchMakingComplete
{
    [ProtoMember(1, IsRequired = true)]
    public string message { get; set; }

}


[ProtoContract]
public class BattleStart
{
    [ProtoMember(1)]
    public List<UserTeam> users { get; set; }

    [ProtoMember(2)]
    public string message { get; set; }

    [ProtoContract]
    public class UserTeam
    {
        [ProtoMember(1)]
        public string playerId { get; set; }

        [ProtoMember(2)]
        public string team { get; set; }

        [ProtoMember(3)]
        public float hp { get; set; }

        [ProtoMember(4)]
        public float x { get; set; }

        [ProtoMember(5)]
        public float y { get; set; }

    }

}

[ProtoContract]
public class StoreOpenRequestPayload
{
    [ProtoMember(1, IsRequired = true)]
    public string message { get; set; }
}

[ProtoContract]
public class PurchaseCharacterRequestPayload
{
    [ProtoMember(1, IsRequired = true)]
    public string name { get; set; }

    [ProtoMember(2, IsRequired = true)]
    public string price { get; set; }
}

[ProtoContract]
public class PurchaseEquipmentRequestPayload
{
    [ProtoMember(1, IsRequired = true)]
    public string name { get; set; }

    [ProtoMember(2, IsRequired = true)]
    public string price { get; set; }
}


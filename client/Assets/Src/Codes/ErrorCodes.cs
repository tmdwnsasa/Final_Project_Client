using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErrorCodes : MonoBehaviour
{
    public enum ErrorCode
    {
        SOCKET_ERROR = 10000,
        CLIENT_VERSION_MISMATCH = 10001,
        UNKNOWN_HANDLER_ID = 10002,
        PACKET_DECODE_ERROR = 10003,
        PACKET_STRUCTURE_MISMATCH = 10004,
        MISSING_FIELDS = 10005,
        USER_NOT_FOUND = 10006,
        INVALID_PACKET = 10007,
        INVALID_SEQUENCE = 10008,
        GAME_NOT_FOUND = 10009,
        SESSION_ID_MISMATCH = 10010,
        LOBBY_NOT_FOUND = 10011,
        PLAYERID_NOT_FOUND = 10012,
        CHARACTERID_NOT_FOUND = 10013,
        INVENTORY_NOT_FOUND = 10014,

        // 추가적인 에러 코드들
        VALIDATE_ERROR = 10020,
        ALREADY_EXIST_ID = 10021,
        ALREADY_EXIST_NAME = 10022,
        LOGGED_IN_ALREADY = 10023,
        MISMATCH_PASSWORD = 10024,

        MISMATCH_COOLTIME = 10030,
        BULLQUEUE_ERROR = 10050,

        API_ERROR = 10060,

        SERVER_REBOOT = 99999,
    }
}

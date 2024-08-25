using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TextCore.Text;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;

    private string port = "5000";
    // private string ip = "127.0.0.1";
    private string ip = "34.47.111.128";
    public GameObject uiNotice;
    private TcpClient tcpClient;
    private NetworkStream stream;

    private uint sequence = 0;

    WaitForSecondsRealtime wait;

    private byte[] receiveBuffer = new byte[4096];
    private List<byte> incompleteData = new List<byte>();

    public bool isLobby;

    void Awake()
    {
        instance = this;
        wait = new WaitForSecondsRealtime(5);
        Handlers.instance = new Handlers();

        if (IsValidPort(port))
        {
            int portNumber = int.Parse(port);

            if (ConnectToServer(ip, portNumber))
            {
                StartGame();
            }
            else
            {
                AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp);
                StartCoroutine(NoticeRoutine(1));
            }

        }
        else
        {
            AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp);
            StartCoroutine(NoticeRoutine(0));
        }

        isLobby = true;
    }

    void OnApplicationQuit()
    {
        if (stream != null)
        {
            stream.Close();
            stream = null;
        }

        if (tcpClient != null)
        {
            tcpClient.Close();
            tcpClient = null;
        }

        Debug.Log("Disconnected server");
    }

    bool IsValidIP(string ip)
    {
        // 간단한 IP 유효성 검사
        return System.Net.IPAddress.TryParse(ip, out _);
    }

    bool IsValidPort(string port)
    {
        // 간단한 포트 유효성 검사 (0 - 65535)
        if (int.TryParse(port, out int portNumber))
        {
            return portNumber > 0 && portNumber <= 65535;
        }
        return false;
    }

    bool ConnectToServer(string ip, int port)
    {
        try
        {
            tcpClient = new TcpClient(ip, port);
            stream = tcpClient.GetStream();
            Debug.Log($"Connected to {ip}:{port}");

            return true;
        }
        catch (SocketException e)
        {
            Debug.LogError($"SocketException: {e}");
            return false;
        }
    }

    string GenerateUniqueID()
    {
        return System.Guid.NewGuid().ToString();
    }

    void StartGame()
    {
        // 게임 시작 코드 작성
        Debug.Log("Game Started");
        StartReceiving(); // Start receiving data
    }

    IEnumerator NoticeRoutine(int index)
    {

        uiNotice.SetActive(true);
        uiNotice.transform.GetChild(index).gameObject.SetActive(true);

        yield return wait;

        uiNotice.SetActive(false);
        uiNotice.transform.GetChild(index).gameObject.SetActive(false);
    }

    public static byte[] ToBigEndian(byte[] bytes)
    {
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }
        return bytes;
    }

    byte[] CreatePacketHeader(int dataLength, Packets.PacketType packetType)
    {
        int packetLength = 4 + 1 + dataLength; // 전체 패킷 길이 (헤더 포함)
        byte[] header = new byte[5]; // 4바이트 길이 + 1바이트 타입

        // 첫 4바이트: 패킷 전체 길이
        byte[] lengthBytes = BitConverter.GetBytes(packetLength);
        lengthBytes = ToBigEndian(lengthBytes);
        Array.Copy(lengthBytes, 0, header, 0, 4);

        // 다음 1바이트: 패킷 타입
        header[4] = (byte)packetType;

        return header;
    }

    // 공통 패킷 생성 함수
    async void SendPacket<T>(T payload, uint handlerId)
    {
        // ArrayBufferWriter<byte>를 사용하여 직렬화
        var payloadWriter = new ArrayBufferWriter<byte>();
        Packets.Serialize(payloadWriter, payload);
        byte[] payloadData = payloadWriter.WrittenSpan.ToArray();

        CommonPacket commonPacket = new CommonPacket
        {
            handlerId = handlerId,
            userId = GameManager.instance.playerId,
            version = GameManager.instance.version,
            payload = payloadData,
            sequence = sequence,
        };

        // ArrayBufferWriter<byte>를 사용하여 직렬화
        var commonPacketWriter = new ArrayBufferWriter<byte>();
        Packets.Serialize(commonPacketWriter, commonPacket);
        byte[] data = commonPacketWriter.WrittenSpan.ToArray();

        // 헤더 생성
        byte[] header = CreatePacketHeader(data.Length, Packets.PacketType.NORMAL);

        // 패킷 생성
        byte[] packet = new byte[header.Length + data.Length];
        Array.Copy(header, 0, packet, 0, header.Length);
        Array.Copy(data, 0, packet, header.Length, data.Length);

        // 패킷 전송
        stream.Write(packet, 0, packet.Length);
    }

    public void SendRegisterPacket(string id, string password, string name, int guild)
    {
        RegisterPayload registerPayload = new RegisterPayload
        {
            playerId = id,
            password = password,
            name = name,
            guild = guild,
        };
        // handlerId는 0으로 가정
        SendPacket(registerPayload, (uint)Handlers.HandlerIds.REGISTER);
    }

    public void SendCharacterEarnPacket(uint characterId)
    {
        CharacterEarnPayload characterEarnPayload = new CharacterEarnPayload
        {
            characterId = characterId,
        };

        // handlerId는 0으로 가정
        SendPacket(characterEarnPayload, (uint)Handlers.HandlerIds.GIVE_CHARACTER);
    }

    public void SendLoginPacket(string id, string password)
    {
        LoginPayload loginPayload = new LoginPayload
        {
            playerId = id,
            password = password,
        };

        // handlerId는 0으로 가정
        SendPacket(loginPayload, (uint)Handlers.HandlerIds.LOGIN);
    }

    public void SendJoinLobbyPacket(uint characterId)
    {
        JoinLobbyPayload joinLobbyPayload = new JoinLobbyPayload
        {
            characterId = characterId,
        };

        // handlerId는 0으로 가정
        SendPacket(joinLobbyPayload, (uint)Handlers.HandlerIds.JOIN_LOBBY);
    }


    async void SendPongPacket(byte[] packetData)
    {
        Ping response = Packets.Deserialize<Ping>(packetData);
        Ping ping = new Ping
        {
            timestamp = response.timestamp,
        };

        var pingPacketWriter = new ArrayBufferWriter<byte>();
        Packets.Serialize(pingPacketWriter, ping);
        byte[] data = pingPacketWriter.WrittenSpan.ToArray();

        // 헤더 생성
        byte[] header = CreatePacketHeader(data.Length, Packets.PacketType.PING);

        // 패킷 생성
        byte[] packet = new byte[header.Length + data.Length];
        Array.Copy(header, 0, packet, 0, header.Length);
        Array.Copy(data, 0, packet, header.Length, data.Length);

        // 패킷 전송
        stream.Write(packet, 0, packet.Length);
    }

    public void SendLocationUpdatePacket(float x, float y)
    {
        LocationUpdatePayload locationUpdatePayload = new LocationUpdatePayload
        {
            x = x,
            y = y,
            isLobby = isLobby,
        };

        SendPacket(locationUpdatePayload, (uint)Handlers.HandlerIds.UPDATE_LOCACTION);
    }

    public void SendSkillUpdatePacket(float x, float y, bool isDirectionX, uint skill_id)
    {
        SkillPayload SkillPayload = new SkillPayload
        {
            x = x,
            y = y,
            isDirectionX = isDirectionX,
            skill_id = skill_id,
            timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };
        SendPacket(SkillPayload, (uint)Handlers.HandlerIds.SKILL);
    }

    public void SendRemoveSkillPacket(string prefabNum, uint skillType)
    {
        RemoveSkillPayload RemoveSkillPayload = new RemoveSkillPayload
        {
            prefabNum = prefabNum,
            skillType = skillType
        };

        SendPacket(RemoveSkillPayload, (uint)Handlers.HandlerIds.REMOVESKILL);
    }


    public void SendChattingPacket(string message, uint type)
    {
        ChattingPayload ChattingPayload = new ChattingPayload
        {
            message = message,
            type = type,
            isLobby = isLobby,
        };

        SendPacket(ChattingPayload, (uint)Handlers.HandlerIds.CHATTING);
    }

    public void SendMatchPacket(string sessionId, uint handlerId)
    {
        MatchingPayload MatchingPayload = new MatchingPayload
        {
            sessionId = sessionId
        };
        SendPacket(MatchingPayload, handlerId);
    }

    public void SendReturnLobbyPacket()
    {
        ReturnLobbyRequestPayload ReturnLobbyRequestPayload = new ReturnLobbyRequestPayload
        {
            message = "returnLobby"
        };

        SendPacket(ReturnLobbyRequestPayload, (uint)Handlers.HandlerIds.RETURN_LOBBY);
    }

    public void SendExitPacket()
    {
        ExitPayload exitPayload = new ExitPayload
        {
            message = "exit"
        };
        SendPacket(exitPayload, (uint)Handlers.HandlerIds.EXIT);

    }

     public void SendInventoryPacket()
    {
        InventoryPayload inventoryPayload = new InventoryPayload
        {
            message = "open inventory"
        };

        SendPacket(inventoryPayload, (uint)Handlers.HandlerIds.INVENTORY);
    }


    public void SendEquipItemPacket(string itemId)
    {
        EquipItemPayload equipPayload = new EquipItemPayload
        {
            itemId = itemId,
        };

        SendPacket(equipPayload, (uint)Handlers.HandlerIds.EQUIP_ITEM);
    }

    public void SendUnequipItemPacket(string itemId)
    {
        UnequipItemPayload unequipPayload = new UnequipItemPayload
        {
            itemId = itemId,
        };

        SendPacket(unequipPayload, (uint)Handlers.HandlerIds.UNEQUIP_ITEM);
    }

    public void SendStoreOpenPacket(string sessionId)
    {
        StoreOpenRequestPayload StoreOpenRequestPayload = new StoreOpenRequestPayload
        {
            sessionId = sessionId
        };
        SendPacket(StoreOpenRequestPayload, (uint)Handlers.HandlerIds.OPEN_STORE);
    }

    public void SendPurchaseCharacterPacket(string name, string price, string sessionId)
    {
        PurchaseCharacterRequestPayload purchaseCharacterRequestPayload = new PurchaseCharacterRequestPayload
        {
            name = name,
            price = price,
            sessionId = sessionId
        };
        SendPacket(purchaseCharacterRequestPayload, (uint)Handlers.HandlerIds.PURCHASE_CHARACTER);
    }

    public void SendPurchaseEquipmentPacket(string name, string price, string sessionId)
    {
        PurchaseEquipmentRequestPayload purchaseEquipmentRequestPayload = new PurchaseEquipmentRequestPayload
        {
            name = name,
            price = price,
            sessionId = sessionId
        };
        SendPacket(purchaseEquipmentRequestPayload, (uint)Handlers.HandlerIds.PURCHASE_EQUIPMENT);
    }

    public void SendOpenMapPacket()
    {
        OpenMapPayload openMapPayload = new OpenMapPayload
        {
            message = "mapOpen"
        };
        SendPacket(openMapPayload, (uint)Handlers.HandlerIds.OPEN_MAP);
    }

    public void SendReselectCharacterPacket()
    {
        ReselectCharacterPayload reselectCharacterPayload = new ReselectCharacterPayload
        {
            message = "reselect"
        };
        SendPacket(reselectCharacterPayload, (uint)Handlers.HandlerIds.RESELECTCHARACTER);
    }

    void StartReceiving()
    {
        _ = ReceivePacketsAsync();
    }

    async System.Threading.Tasks.Task ReceivePacketsAsync()
    {
        while (tcpClient.Connected)
        {
            try
            {
                int bytesRead = await stream.ReadAsync(receiveBuffer, 0, receiveBuffer.Length);
                if (bytesRead > 0)
                {
                    ProcessReceivedData(receiveBuffer, bytesRead);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Receive error: {e.Message}");
                break;
            }
        }
    }

    void ProcessReceivedData(byte[] data, int length)
    {
        incompleteData.AddRange(data.AsSpan(0, length).ToArray());

        while (incompleteData.Count >= 5)
        {
            // 패킷 길이와 타입 읽기
            byte[] lengthBytes = incompleteData.GetRange(0, 4).ToArray();
            int packetLength = BitConverter.ToInt32(ToBigEndian(lengthBytes), 0);
            Packets.PacketType packetType = (Packets.PacketType)incompleteData[4];

            if (incompleteData.Count < packetLength)
            {
                // 데이터가 충분하지 않으면 반환
                return;
            }

            // 패킷 데이터 추출
            byte[] packetData = incompleteData.GetRange(5, packetLength - 5).ToArray();
            incompleteData.RemoveRange(0, packetLength);

            //Debug.Log($"Received packet: Length = {packetLength}, Type = {packetType}");

            switch (packetType)
            {
                case Packets.PacketType.PING:
                    SendPongPacket(packetData);
                    break;
                case Packets.PacketType.NORMAL:
                    HandleNormalPacket(packetData);
                    break;
                case Packets.PacketType.LOCATION:
                    HandleLocationPacket(packetData);
                    break;
                case Packets.PacketType.CHATTING:
                    HandleChattingPacket(packetData);
                    break;
                case Packets.PacketType.MATCHMAKING:
                    HandleMatchMakingPacket(packetData);
                    break;
                case Packets.PacketType.CREATE_USER:
                    HandleCreateUserPacket(packetData);
                    break;
                case Packets.PacketType.REMOVE_USER:
                    HandleRemoveUserPacket(packetData);
                    break;
                case Packets.PacketType.GAME_START:
                    HandleBattleStartPacket(packetData);
                    break;
                case Packets.PacketType.GAME_END:
                    HandleGameEndPacket(packetData);
                    break;
                case Packets.PacketType.SKILL:
                    HandleSkillPacket(packetData);
                    break;

                case Packets.PacketType.ATTACK:
                    HandleAttackPacket(packetData);
                    break;
            }
        }
    }

    void HandleNormalPacket(byte[] packetData)
    {
        // 패킷 데이터 처리
        var response = Packets.Deserialize<Response>(packetData);
        Debug.Log($"HandlerId: {response.handlerId}, responseCode: {response.responseCode}, timestamp: {response.timestamp}");

        HandleErrorResponsePacket(response);

        if (response.responseCode != 0 && !uiNotice.activeSelf)
        {
            AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp);
            StartCoroutine(NoticeRoutine(2));
            return;
        }

        if (response.data != null && response.data.Length > 0)
        {
            sequence = response.sequence;
            switch (response.handlerId)
            {
                case (uint)Handlers.HandlerIds.LOGIN:
                    break;
                case (uint)Handlers.HandlerIds.REGISTER:
                    GameManager.instance.GoLogin();
                    break;
                case (uint)Handlers.HandlerIds.UPDATE_LOCACTION:
                    break;
                case (uint)Handlers.HandlerIds.CREATE_GAME:
                    break;
                case (uint)Handlers.HandlerIds.JOIN_GAME:
                    break;
                case (uint)Handlers.HandlerIds.JOIN_LOBBY:
                    GameManager.instance.GameStart();
                    Handlers.instance.SetCharacterStats(response.data);
                    break;
                case (uint)Handlers.HandlerIds.CHOICE_CHARACTER:
                    Handlers.instance.GetCharacterChoice(response.data);
                    break;
                case (uint)Handlers.HandlerIds.SELECT_CHARACTER:
                    Handlers.instance.GetCharacterSelect(response.data);
                    break;
                case (uint)Handlers.HandlerIds.MATCHMAKING:
                    GameManager.instance.matchStartUI.transform.GetChild(0).GetComponent<Button>().interactable = true;
                    GameManager.instance.matchStartUI.SetActive(false);
                    GameManager.instance.matchCancelUI.SetActive(true);
                    GameManager.instance.isMatching = true;
                    GameManager.instance.reselectCharacterBtn.transform.GetComponent<Button>().interactable = false;
                    break;
                case (uint)Handlers.HandlerIds.MATCHINGCANCEL:
                    GameManager.instance.matchCancelUI.transform.GetChild(0).GetComponent<Button>().interactable = true;
                    GameManager.instance.matchStartUI.SetActive(true);
                    GameManager.instance.matchCancelUI.SetActive(false);
                    GameManager.instance.isMatching = false;
                    GameManager.instance.reselectCharacterBtn.transform.GetComponent<Button>().interactable = true;
                    break;
                case (uint)Handlers.HandlerIds.RESELECTCHARACTER:
                    GameManager.instance.GoCharacterSelect();
                    GameManager.instance.characterSelectUI.transform.GetChild(1).GetComponent<Button>().interactable = true;
                    break;
                case (uint)Handlers.HandlerIds.RETURN_LOBBY:
                    Handlers.instance.ReturnLobbySetting(response.data);
                    break;
                case (uint)Handlers.HandlerIds.SKILL:
                    break;
                case (uint)Handlers.HandlerIds.INVENTORY:
                    Handlers.instance.UpdateInventoryAndStats(response.data);
                    break;
                case (uint)Handlers.HandlerIds.EQUIP_ITEM:
                    Handlers.instance.UpdateInventoryAndStats(response.data);
                    break;
                case (uint)Handlers.HandlerIds.UNEQUIP_ITEM:
                    Handlers.instance.UpdateInventoryAndStats(response.data);
                    break;
                case (uint)Handlers.HandlerIds.EXIT:
                    Application.Quit();
                    break;
                case (uint)Handlers.HandlerIds.OPEN_STORE:
                    Handlers.instance.StoreOpen(response.data);
                    break;
                case (uint)Handlers.HandlerIds.PURCHASE_CHARACTER:
                    Handlers.instance.PurchaseMessage(response.data);
                    break;
                    case (uint)Handlers.HandlerIds.PURCHASE_EQUIPMENT:
                    Handlers.instance.PurchaseMessage(response.data);
                    break;
                case (uint)Handlers.HandlerIds.REMOVESKILL:
                    break;
                case (uint)Handlers.HandlerIds.OPEN_MAP:
                    Handlers.instance.OpenMap(response.data);
                    break;
            }
            ProcessResponseData(response.data);
        }
    }

    void ProcessResponseData(byte[] data)
    {
        try
        {
            // var specificData = Packets.Deserialize<SpecificDataType>(data);
            string jsonString = Encoding.UTF8.GetString(data);
            Debug.Log($"Processed SpecificDataType: {jsonString}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error processing response data: {e.Message}");
        }
    }

    void HandleLocationPacket(byte[] data)
    {
        try
        {
            LocationUpdate response;

            if (data.Length > 0)
            {
                // 패킷 데이터 처리
                response = Packets.Deserialize<LocationUpdate>(data);
            }
            else
            {
                // data가 비어있을 경우 빈 배열을 전달
                response = new LocationUpdate { users = new List<LocationUpdate.UserLocation>() };
            }

            CharacterManager.instance.MoveAllPlayers(response);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error HandleLocationPacket: {e.Message}");
        }
    }

    void HandleChattingPacket(byte[] packetData)
    {
        var response = Packets.Deserialize<ChattingUpdate>(packetData);

        GameManager.instance.chatting.updateChatting($"{response.playerId} : {response.message} / {response.type}");
        Debug.Log($"{response.playerId} : {response.message} / {response.type}");
    }

    void HandleSkillPacket(byte[] packetData)
    {
        var response = Packets.Deserialize<SkillUpdate>(packetData);
        CharacterManager.instance.UpdateAttack(response);
    }

    void HandleMatchMakingPacket(byte[] packetData)
    {
        var response = Packets.Deserialize<MatchMakingComplete>(packetData);

    }

    void HandleCreateUserPacket(byte[] packetData)
    {
        var response = Packets.Deserialize<CreateUser>(packetData);
        CharacterManager.instance.CreateOtherPlayers(response);
    }
    void HandleRemoveUserPacket(byte[] packetData)
    {
        var response = Packets.Deserialize<RemoveUser>(packetData);
        CharacterManager.instance.RemoveOtherPlayers(response);
    }

    void HandleGameEndPacket(byte[] packetData)
    {
        var response = Packets.Deserialize<GameEndPayload>(packetData);
        GameManager.instance.GameEnd(response.result, response.users);
    }

    void HandleAttackPacket(byte[] packetData)
    {
        var response = Packets.Deserialize<AttackedSuccess>(packetData);
        Debug.Log($"{response.playerId} : {response.hp}");

        CharacterManager.instance.UpdateCharacterState(response);
    }

    //recieve GAME_START packet
    void HandleBattleStartPacket(byte[] packetData)
    {
        var response = Packets.Deserialize<BattleStart>(packetData);
        foreach (var user in response.users)
        {
            Debug.Log($"Player ID: {user.playerId}, Team: {user.team}, HP : {user.hp}, Position: ({user.x}, {user.y})");
            if (user.playerId == GameManager.instance.player.name)
            {
                // GameManager.instance.player.transform.position = new Vector2(user.x, user.y);
            }
        }
        Text announcementMap = GameManager.instance.AnnouncementMap.transform.GetChild(0).GetComponent<Text>();
        announcementMap.text = $"대전 지역 이름: {response.mapName}";
        Debug.Log(response.mapName);

        if(GameManager.instance.storeUI.activeSelf) {
            GameManager.instance.storeUI.SetActive(false);
            GameManager.instance.storeUI.transform.GetChild(0).gameObject.SetActive(true);
            GameManager.instance.storeUI.transform.GetChild(1).gameObject.SetActive(false);
            GameManager.instance.storeUI.transform.GetChild(2).GetComponent<Button>().interactable = true;
            GameManager.instance.storeUI.transform.GetChild(3).GetComponent<Button>().interactable = true;
        }

        if(GameManager.instance.mapUI.activeSelf) {
            GameManager.instance.mapUI.SetActive(false);
        }

        if(GameManager.instance.inventoryUI.activeSelf) {
            GameManager.instance.inventoryUI.SetActive(false);
        }

        AudioManager.instance.StopBgm(AudioManager.Bgm.Lobby);
        AudioManager.instance.PlayBgm(AudioManager.Bgm.Game);
        isLobby = false;
        GameManager.instance.matchStartUI.SetActive(false);
        GameManager.instance.matchCancelUI.SetActive(false);
        GameManager.instance.exitBtn.SetActive(false);
        GameManager.instance.storeBtn.SetActive(false);
        GameManager.instance.mapBtn.SetActive(false);
        GameManager.instance.inventoryButton.SetActive(false);
        GameManager.instance.reselectCharacterBtn.SetActive(false);
        GameManager.instance.AnnouncementMap.SetActive(true);
        GameManager.instance.chattingUI.SetActive(true);
        CharacterManager.instance.SetCharacterHp(response);
        CharacterManager.instance.SetCharacterTag(response);
    }

    void HandleErrorResponsePacket(Response response)
    {
        if (response.responseCode == (uint)ErrorCodes.ErrorCode.CLIENT_VERSION_MISMATCH)
        {
            AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp);
            StartCoroutine(NoticeRoutine(5));
        }

        if (response.responseCode == (uint)ErrorCodes.ErrorCode.INVALID_SEQUENCE)
        {
            Application.Quit();
        }

        if (response.responseCode == (uint)ErrorCodes.ErrorCode.VALIDATE_ERROR ||
        response.responseCode == (uint)ErrorCodes.ErrorCode.ALREADY_EXIST_ID ||
        response.responseCode == (uint)ErrorCodes.ErrorCode.ALREADY_EXIST_NAME)
        {
            GameManager.instance.registerUI.transform.GetChild(3).GetComponent<Button>().interactable = true;
            AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp);
            StartCoroutine(NoticeRoutine(3));
        }

        if (response.responseCode == (uint)ErrorCodes.ErrorCode.LOGGED_IN_ALREADY ||
        response.responseCode == (uint)ErrorCodes.ErrorCode.USER_NOT_FOUND ||
        response.responseCode == (uint)ErrorCodes.ErrorCode.MISMATCH_PASSWORD)
        {
            GameManager.instance.loginUI.transform.GetChild(3).GetComponent<Button>().interactable = true;
            AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp);
            StartCoroutine(NoticeRoutine(4));
        }

        if (response.responseCode == (uint)ErrorCodes.ErrorCode.PLAYERID_NOT_FOUND ||
        response.responseCode == (uint)ErrorCodes.ErrorCode.LOBBY_NOT_FOUND)
        {
            GameManager.instance.characterChoiceUI.transform.GetChild(1).GetComponent<Button>().interactable = true;
            GameManager.instance.characterSelectUI.transform.GetChild(1).GetComponent<Button>().interactable = true;
        }
    }
}

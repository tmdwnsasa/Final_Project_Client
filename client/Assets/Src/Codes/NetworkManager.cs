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
using static Packets;
using static Handlers;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;

    private string port = "5000";
    private string ip = "127.0.0.1";
    public InputField loginIdInputField;
    public InputField loginPasswordInputField;
    public InputField registerIdInputField;
    public InputField registerPasswordInputField;
    public InputField registerNameInputField;
    public GameObject uiNotice;
    private TcpClient tcpClient;
    private NetworkStream stream;
    
    private uint sequence = 0;
    
    WaitForSecondsRealtime wait;

    private byte[] receiveBuffer = new byte[4096];
    private List<byte> incompleteData = new List<byte>();

    void Awake() {        
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
    }


    //로그인 버튼
    public void OnLoginButtonClicked()
    {
        string id = loginIdInputField.text;
        string password = loginPasswordInputField.text;

        if (id != "" && password != "" && name != "")
            SendLoginPacket(id, password);
    }

    // 계정 생성 버튼
    public void OnRegisterButtonClicked()
    {
        string id = registerIdInputField.text;
        string password = registerPasswordInputField.text;
        string name = registerNameInputField.text;

        if(id != "" && password != "" && name != "")
            SendRegisterPacket(id, password, name);
    }

    // 케릭터 선택 버튼
    public void OnCharacterChoiceButtonClicked()
    {
        uint characterId = GameManager.instance.characterId;

        SendCharacterEarnPacket(characterId);
        SendJoinLobbyPacket(characterId);
    }

    // 케릭터 고르기 버튼
    public void OnCharacterSelectButtonClicked()
    {
        uint characterId = GameManager.instance.characterId;

        SendJoinLobbyPacket(characterId);
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

     bool ConnectToServer(string ip, int port) {
        try {
            tcpClient = new TcpClient(ip, port);
            stream = tcpClient.GetStream();
            Debug.Log($"Connected to {ip}:{port}");

            return true;
        } catch (SocketException e) {
            Debug.LogError($"SocketException: {e}");
            return false;
        }
    }

    string GenerateUniqueID() {
        return System.Guid.NewGuid().ToString();
    }

    void StartGame()
    {
        // 게임 시작 코드 작성
        Debug.Log("Game Started");
        StartReceiving(); // Start receiving data
    }

    IEnumerator NoticeRoutine(int index) {
        
        uiNotice.SetActive(true);
        uiNotice.transform.GetChild(index).gameObject.SetActive(true);

        yield return wait;

        uiNotice.SetActive(false);
        uiNotice.transform.GetChild(index).gameObject.SetActive(false);
    }

    public static byte[] ToBigEndian(byte[] bytes) {
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }
        return bytes;
    }

    byte[] CreatePacketHeader(int dataLength, Packets.PacketType packetType) {
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

        await Task.Delay(GameManager.instance.latency);
        
        // 패킷 전송
        stream.Write(packet, 0, packet.Length);
    }

    //void SendInitialPacket() {
    //    InitialPayload initialPayload = new InitialPayload
    //    {
    //        playerId = GameManager.instance.playerId,
    //        characterId = GameManager.instance.characterId,
    //        frame = GameManager.instance.targetFrameRate,
    //    };

    //    // handlerId는 0으로 가정
    //    SendPacket(initialPayload, (uint)Packets.HandlerIds.Init);
    //}

    void SendRegisterPacket(string id, string password, string name)
    {
        RegisterPayload registerPayload = new RegisterPayload
        {
            playerId = id,
            password = password,
            name = name,
        };

        // handlerId는 0으로 가정
        SendPacket(registerPayload, (uint)Handlers.HandlerIds.REGISTER);
    }

    void SendCharacterEarnPacket(uint characterId)
    {
        CharacterEarnPayload characterEarnPayload = new CharacterEarnPayload
        {
            characterId = characterId,
        };

        // handlerId는 0으로 가정
        SendPacket(characterEarnPayload, (uint)Handlers.HandlerIds.GIVE_CHARACTER);
    }   

    void SendLoginPacket(string id, string password)
    {
        LoginPayload loginPayload = new LoginPayload
        {
            playerId = id,
            password = password,
            frame = GameManager.instance.targetFrameRate,
        };

        // handlerId는 0으로 가정
        SendPacket(loginPayload, (uint)Handlers.HandlerIds.LOGIN);
    }

    void SendJoinLobbyPacket(uint characterId)
    {
        JoinLobbyPayload joinLobbyPayload = new JoinLobbyPayload
        {
            characterId = characterId,
        };

        // handlerId는 0으로 가정
        SendPacket(joinLobbyPayload, (uint)Handlers.HandlerIds.JOIN_LOBBY);
    }


    async void SendPongPacket(byte[] packetData) {
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

        await Task.Delay(GameManager.instance.latency);
        
        // 패킷 전송
        stream.Write(packet, 0, packet.Length);
    }

    public void SendLocationUpdatePacket(float x, float y) {
        LocationUpdatePayload locationUpdatePayload = new LocationUpdatePayload
        {
            x = x,
            y = y,
            isLobby = true,
        };

        SendPacket(locationUpdatePayload, (uint)Handlers.HandlerIds.UPDATE_LOCACTION);
    }

    public void SendChattingPacket(string message, uint type) {
        ChattingPayload ChattingPayload = new ChattingPayload
        {
            message = message,
            type = type,
        };

        SendPacket(ChattingPayload, (uint)Handlers.HandlerIds.CHATTING);
    }


    void StartReceiving() {
        _ = ReceivePacketsAsync();
    }

    async System.Threading.Tasks.Task ReceivePacketsAsync() {
        while (tcpClient.Connected) {
            try {
                int bytesRead = await stream.ReadAsync(receiveBuffer, 0, receiveBuffer.Length);
                if (bytesRead > 0) {
                    ProcessReceivedData(receiveBuffer, bytesRead);
                }
            } catch (Exception e) {
                Debug.LogError($"Receive error: {e.Message}");
                break;
            }
        }
    }

    void ProcessReceivedData(byte[] data, int length) {
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

            // Debug.Log($"Received packet: Length = {packetLength}, Type = {packetType}");

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
            }
        }
    }

    void HandleNormalPacket(byte[] packetData) {
        // 패킷 데이터 처리
        var response = Packets.Deserialize<Response>(packetData);
        // Debug.Log($"HandlerId: {response.handlerId}, responseCode: {response.responseCode}, timestamp: {response.timestamp}");

        if (response.responseCode != 0 && !uiNotice.activeSelf) {
            AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp);
            StartCoroutine(NoticeRoutine(2));
            return;
        }

        if (response.data != null && response.data.Length > 0) {
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
                    break;
                case (uint)Handlers.HandlerIds.CHOICE_CHARACTER:
                    Handlers.instance.GetCharacterChoice(response.data);
                    break;
                case (uint)Handlers.HandlerIds.SELECT_CHARACTER:
                    Handlers.instance.GetCharacterSelect(response.data);
                    break;
            }
            ProcessResponseData(response.data);
        }
    }

    void ProcessResponseData(byte[] data) {
        try {
            // var specificData = Packets.Deserialize<SpecificDataType>(data);
            string jsonString = Encoding.UTF8.GetString(data);
            Debug.Log($"Processed SpecificDataType: {jsonString}");
        } catch (Exception e) {
            Debug.LogError($"Error processing response data: {e.Message}");
        }
    }

    void HandleLocationPacket(byte[] data) {
        try {
            LocationUpdate response;

            if (data.Length > 0) {
                // 패킷 데이터 처리
                response = Packets.Deserialize<LocationUpdate>(data);
            } else {
                // data가 비어있을 경우 빈 배열을 전달
                response = new LocationUpdate { users = new List<LocationUpdate.UserLocation>() };
            }

            Spawner.instance.Spawn(response);
        } catch (Exception e) {
            Debug.LogError($"Error HandleLocationPacket: {e.Message}");
        }
    }

    void HandleChattingPacket(byte[] packetData) {
        var response = Packets.Deserialize<ChattingUpdate>(packetData);

        GameManager.instance.chatting.updateChatting($"{response.playerId} : {response.message} / {response.type}");
        Debug.Log($"{response.playerId} : {response.message} / {response.type}");
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
}

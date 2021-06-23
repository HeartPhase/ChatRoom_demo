using UnityEngine;
using System;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Text;
using UnityEngine.UI;
using System.Threading;

public class chatRoom : MonoBehaviour
{
    private byte[] data = new byte[1024];
    public InputField inputcontext;
    public Socket TcpClient;
    //op��ͷ�ǲ�����
    private byte op0 = 0;//����
    private byte op1 = 1;//������Ϣ
    private byte op6 = 6;//����˱�����
    string getmsg, oldmessage;
    private Thread thread;
    public Text te;
    public ScrollRect scrollRect;
    public String userName;
    // Use this for initialization

    public chatRoom(Socket s,String userName) {
        this.TcpClient = s;
        this.userName = userName;
    }

    void Start()
    {
        startChat();

    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (getmsg != null && getmsg != oldmessage)
        {
            te.text += getmsg + "\n";
            oldmessage = getmsg;
        }
        */
        if (getmsg != null )
        {
            te.text += getmsg + "\n";
            getmsg = null;
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
            Canvas.ForceUpdateCanvases();
        }
        
        if (Input.GetKeyUp(KeyCode.Return)) {
            ButtonOnClickEvent();
        }

        
    }
    void startChat()
    {
        //�������߳���������Ϣ
        thread = new Thread(GetmessageFromServer);
        thread.Start();
    }

    public void SendMessageToServer(string message)//��ʼ���������Ϣ 01 | userName | message
    {
        byte[] op = new byte[] {0,1};
        byte[] name = Encoding.UTF8.GetBytes(this.userName);
        byte[] mes = Encoding.UTF8.GetBytes(message);
        byte[] data = new byte[op.Length+name.Length+mes.Length+1];
        op.CopyTo(data, 0);
        name.CopyTo(data, op.Length);
        data[op.Length + name.Length] = op0;
        mes.CopyTo(data,op.Length+name.Length+1);
        try { 
            TcpClient.Send(data); 
        } catch (Exception ex) { 
        
        }
        
    }

    public void initialConnect() {
        //this.TcpClient = s;
        //this.userName = userName;
        this.GetComponent<Canvas>().enabled = true;
        Debug.Log(this.TcpClient);
    }

    public void GetmessageFromServer()
    {

        while (true)
        {
            //�ڽ�������֮ǰ  �ж�һ��socket�����Ƿ�Ͽ�
            if (TcpClient.Connected == false)
            {
                Debug.Log("disconnect");
                TcpClient.Close();
                break;//����ѭ�� ��ֹ�̵߳�ִ��
            }
            TcpClient.ReceiveTimeout = 100000;
            int length = TcpClient.Receive(data);
            if (length==0) {
                Debug.Log("disconnect");
                TcpClient.Close();
                break; 
            }
            if (data[1]==op1) {
                Debug.Log("reciev a b mes from server");
                int i = 2;
                while (data[i] != op0)
                {
                    i++;
                }
                String name = Encoding.UTF8.GetString(data, 2, i-1);
                String message = Encoding.UTF8.GetString(data, i + 1, length);
                Debug.Log(name+" : "+message);
                if (name==userName) {
                    Debug.Log("My message");
                    getmsg = "<alignment=right>" + name + "</color>: " + message;
                }
                getmsg =  "<color=red>" + name + "</color>: " + message;
            }
            data = new byte[1024];
            //Debug.Log(getmsg);

        }
    }

    public void ButtonOnClickEvent()
    {
        //Debug.Log("button is clicked");
        string getmessages = inputcontext.text;

        if (getmessages != "")
        {
            SendMessageToServer(getmessages);
            inputcontext.text = null;
            Debug.Log(getmessages);
        }
        //else { Debug.Log("no word"); }
    }
    //�ú���ֻ�д��֮������ã�ƽʱ������Ч
    public void quit() {
        
        Application.Quit();
    }
}

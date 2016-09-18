using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class DrawingsManager : MonoBehaviour
{
    public string ip;
    public int port;
    public Vector2 resolution;
    public Drawing drawingPrefab;

    private UdpClient client;
    private IPEndPoint ipep;
    private string data;
    private string[] dataDrawings;
    private string[] dataPoints;
    private string[] point;
    private Drawing[] drawings;


    void Start ()
    {
        ipep = new IPEndPoint(IPAddress.Parse(ip), port);
        client = new UdpClient();
        client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        client.Client.Bind(ipep);
    }
	
	void Update ()
    {
        // Retrieve data from openframeworks
        if (client.Available > 0)
        {
            data = Encoding.UTF8.GetString(client.Receive(ref ipep));
            if (data != null && data.Length > 0)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    Destroy(transform.GetChild(i).gameObject);
                }

                dataDrawings = data.Split(';');
                drawings = new Drawing[dataDrawings.Length];
                for (int i = 0; i < dataDrawings.Length; i++)
                {
                    dataPoints = dataDrawings[i].Split(',');
                    drawings[i] = Instantiate(drawingPrefab);
                    drawings[i].transform.parent = transform;
                    for (int j = 0; j < dataPoints.Length; j++)
                    {
                        point = dataPoints[j].Split('&');
                        drawings[i].addPoint(float.Parse(point[0]), float.Parse(point[1]));
                    }
                    drawings[i].draw();
                }
            }
        }
    }

    void OnApplicationQuit()
    {
        client.Close();
    }
}

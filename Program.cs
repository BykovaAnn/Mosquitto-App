using System;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MqttApp
{
    class Program
    {
        static string brokenHostName = "192.168.99.100";
        static void Main(string[] args)
        {
            string topic = "test";
            string message = "Message1";
            SendMessage(topic, message);
            GetMessages(topic);
        }

        static void SendMessage (string topic, string message) 
        {
            try
            {
                MqttClient localClient = new MqttClient(brokenHostName);
                string clientId = Guid.NewGuid().ToString();
                localClient.Connect(clientId);
                localClient.Publish(topic, Encoding.UTF8.GetBytes(message), uPLibrary.Networking.M2Mqtt.Messages.MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Connection Failed: " + ex.Message);
            }
        }

        static void GetMessages(string topic) 
        {
            try
            {
                MqttClient localClient = new MqttClient(brokenHostName);
                localClient.MqttMsgPublishReceived += recievedMessage;
                string clientId = Guid.NewGuid().ToString();
                localClient.Connect(clientId);
                localClient.Subscribe( new String [] { topic }, new byte [] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connection Failed: " + ex.Message);
            }
        }

        static void recievedMessage(object sender, MqttMsgPublishEventArgs e)
        {
            string message = new string(Encoding.UTF8.GetChars(e.Message));
            Console.WriteLine(message);
        }

    }
}

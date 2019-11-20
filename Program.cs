using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MqttApp
{
    class Program
    {
        static string brokenHostName = "192.168.99.100";
        static async Task Main(string[] args)
        {
            string topic = "test";
            string message = "Message from console application";
            //SendMessage(topic, message);
            await CreateManagedClient(topic);
            //GetMessages(topic);
        }

        static void SendMessage(string topic, string message)
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
                localClient.Subscribe(new String[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
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

        static async Task CreateManagedClient(string topic)
        {
            string clientId = "ManagedClient";
            var messageBuilder = new MqttClientOptionsBuilder()
                                    .WithClientId(clientId)
                                    .WithTcpServer(brokenHostName, port: 1883)
                                    .WithCleanSession()
                                    .Build();
            var managedOptions = new ManagedMqttClientOptionsBuilder()
                                        .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                                        .WithClientOptions(messageBuilder)
                                        .Build();
            var managedClient = new MqttFactory().CreateManagedMqttClient();
            //Subscribe to the topic
            //await managedClient.SubscribeAsync(new TopicFilterBuilder()
            //                                        .WithTopic(topic)
            //                                        .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce)
            //                                        .Build());
            await managedClient.StartAsync(managedOptions);
            await managedClient.PublishAsync(topic, "Hello from managed client, lets count from 0 to 99");
            //Show the received messege in form topic: message
            //managedClient.UseApplicationMessageReceivedHandler(e =>
            //{
            //    try
            //    {
            //        string topic = e.ApplicationMessage.Topic;
            //        if (string.IsNullOrWhiteSpace(topic) == false)
            //        {
            //            string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            //            Console.WriteLine($"Topic: {topic}. Message Received: {payload}");
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine(ex.Message, ex);
            //    }
            //});
            
            //Sending messages 100 times every 3 seconds
            for (int i = 0; i < 100; i++)
            {
                var result = await managedClient.PublishAsync(topic, i.ToString(), MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce);
                Thread.Sleep(3000);
            }
            Console.ReadLine();

        }

    }
}

using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJD.AccessServiceTask.Helper
{
    public class RabbitMqContext
    {
        #region 初始

        public RabbitMqContext()
        {

        }

        public RabbitMqContext(string queueKey, string host, string hostuid, string hostpw)
        {
            QueueKey = queueKey;
            RabbitMqHostName = host;
            RabbitMqUserName = hostuid;
            RabbitMqPassword = hostpw;

            InitSendChannel();

            InitListenChannel();
        }

        public IConnection SendConnection { get; set; }
        public IModel SendChannel { get; set; }
        public IConnection ListenConnection { get; set; }
        public IModel ListenChannel { get; set; }
        public QueueingBasicConsumer BasicConsumer { get; set; }

        public string QueueKey { get; set; }
        public string RabbitMqHostName { get; set; }
        public string RabbitMqUserName { get; set; }
        public string RabbitMqPassword { get; set; }

        private bool InitSendChannelState = true;

        private void InitSendChannel()
        {
            if (InitSendChannelState)
            {
                var factory = new ConnectionFactory();
                factory.HostName = RabbitMqHostName;
                factory.UserName = RabbitMqUserName;
                factory.Password = RabbitMqPassword;

                SendConnection = factory.CreateConnection();
                SendChannel = SendConnection.CreateModel();
                SendChannel.QueueDeclare(QueueKey, false, false, false, null);

                InitSendChannelState = !InitSendChannelState;
            }
        }

        private bool InitListenChannelState = true;

        private void InitListenChannel()
        {
            if (InitListenChannelState)
            {
                var factory = new ConnectionFactory();
                factory.HostName = RabbitMqHostName;
                factory.UserName = RabbitMqUserName;
                factory.Password = RabbitMqPassword;

                ListenConnection = factory.CreateConnection();
                ListenChannel = ListenConnection.CreateModel();
                ListenChannel.QueueDeclare(QueueKey, false, false, false, null);

                InitListenChannelState = !InitListenChannelState;
            }
        }

        #endregion

        #region 进

        public void Publish(object pubValueObject)
        {
            var pubValStr = JsonConvert.SerializeObject(pubValueObject);

            SendChannel.BasicPublish("", QueueKey, null, Encoding.UTF8.GetBytes(pubValStr));
        }

        public void Publish(string pubValueString)
        {
            SendChannel.BasicPublish("", QueueKey, null, Encoding.UTF8.GetBytes(pubValueString));
        }

        #endregion

        #region 出

        public void OnLitening()
        {
            BasicConsumer = new QueueingBasicConsumer(ListenChannel);
            ListenChannel.BasicConsume(QueueKey, false, BasicConsumer);
            ListenChannel.BasicQos(0, 1, false);
        }

        #endregion

        #region 回收

        public void CloseAll()
        {
            CloseSend();
            CloseListen();
        }

        public void CloseSend()
        {
            SendConnection.Close();
            SendChannel.Close();
        }

        public void CloseListen()
        {
            ListenConnection.Close();
            ListenChannel.Close();
        }

        #endregion

        #region 配置参数

        public static string RabbitmqHostName
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["RabbitmqHostName"];
            }
        }

        public static string RabbitmqUserName
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["RabbitmqUserName"];
            }
        }

        public static string RabbitmqPassword
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["RabbitmqPassword"];
            }
        }

        public static string BehaviorTxtDownloadPath
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["BehaviorTxtDownloadPath"];
            }
        }

        public static string BehaviorCsvDownloadPath
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["BehaviorCsvDownloadPath"];
            }
        }

        public static string WeixinChatRecordTxtDownloadPath
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["WeixinChatRecordTxtDownloadPath"];
            }
        }

        public static string WeixinChatRecordCsvDownloadPath
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["WeixinChatRecordCsvDownloadPath"];
            }
        }

        #endregion
    }
}

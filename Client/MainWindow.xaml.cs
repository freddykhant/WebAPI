using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Http;

namespace Client
{
    public partial class MainWindow : Window
    {
        private Thread networkingThread;
        private Thread serverThread;
        private int completedJobsCount = 0;
        private IRemoteService remoteService;
        private object lockObject = new object();

        public MainWindow()
        {
            InitializeComponent();
            StartThreads();
        }

        private void StartThreads()
        {
            networkingThread = new Thread(new ThreadStart(NetworkingThreadMethod));
            networkingThread.Start();

            serverThread = new Thread(new ThreadStart(ServerThreadMethod));
            serverThread.Start();
        }

        private void NetworkingThreadMethod()
        {
            while (true)
            {
                try
                {
                    var client = new RestClient("http://localhost:5080");
                    var request = new RestRequest("api/Clients/getAll", Method.Get);
                    var response = client.Execute(request);
                    var clientsList = JsonConvert.DeserializeObject<List<ClientClass>>(response.Content);

                    foreach (var clientInfo in clientsList)
                    {
                        remoteService = (IRemoteService)Activator.GetObject(typeof(IRemoteService), $"http://{clientInfo.IPAddress}:{clientInfo.Port}/RemoteService");

                        if (remoteService.HasJob())
                        {
                            string job = remoteService.GetJob();
                            string result = ExecutePythonJob(job);
                            remoteService.SubmitResult(result);
                            UpdateJobStatus(false);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogError($"Error in NetworkingThread: {ex.Message}");
                }

                Thread.Sleep(10000);
            }
        }

        private void ServerThreadMethod()
        {
            try
            {
                HttpChannel channel = new HttpChannel(int.Parse(portTextBox.Text));
                ChannelServices.RegisterChannel(channel, false);
                RemotingConfiguration.RegisterWellKnownServiceType(typeof(RemoteService), "RemoteService", WellKnownObjectMode.Singleton);
            }
            catch (Exception ex)
            {
                LogError($"Error in ServerThread: {ex.Message}");
            }
        }

        private string ExecutePythonJob(string pythonCode)
        {
            try
            {
                ScriptEngine engine = Python.CreateEngine();
                ScriptScope scope = engine.CreateScope();
                dynamic result = engine.Execute(pythonCode, scope);
                return result.ToString();
            }
            catch (Exception ex)
            {
                LogError($"Error executing Python code: {ex.Message}");
                return null;
            }
        }

        private void SubmitPythonCodeButton_Click(object sender, RoutedEventArgs e)
        {
            string pythonCode = PythonCodeTextBox.Text;
            string result = ExecutePythonJob(pythonCode);
            ResultTextBox.Text = result;
        }

        private void UpdateJobStatus(bool isWorking)
        {
            if (isWorking)
            {
                JobStatusTextBox.Text = "Working on a job...";
            }
            else
            {
                completedJobsCount++;
                JobStatusTextBox.Text = $"Completed {completedJobsCount} jobs.";
            }
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            string ipAddress = ipAddressTextBox.Text;
            int port = int.Parse(portTextBox.Text);
            statusTextBlock.Text = $"Connected to IP: {ipAddress}, Port: {port}";
        }

        private void SendDataButton_Click(object sender, RoutedEventArgs e)
        {
            statusTextBlock.Text = "Data sent to the server.";
        }

        private void ReceiveDataButton_Click(object sender, RoutedEventArgs e)
        {
            statusTextBlock.Text = "Data received from the server.";
        }

        private void LogError(string errorMessage)
        {
            // This method can be expanded to log errors to a file or other logging mechanism.
            Console.WriteLine(errorMessage);
        }
    }

    public class RemoteService : MarshalByRefObject, IRemoteService
    {
        private Queue<string> jobQueue = new Queue<string>();

        public bool HasJob()
        {
            return jobQueue.Count > 0;
        }

        public string GetJob()
        {
            if (HasJob())
            {
                return jobQueue.Dequeue();
            }
            return null;
        }

        public void SubmitJob(string job)
        {
            jobQueue.Enqueue(job);
        }

        public void SubmitResult(string result)
        {
            // Process the result. For now, we'll just print it.
            Console.WriteLine($"Received result: {result}");
        }
    }
}

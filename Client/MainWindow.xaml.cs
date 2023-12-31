﻿using Newtonsoft.Json;
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
using System.ServiceModel;

namespace Client
{
    public partial class MainWindow : Window
    {
        private ClientClass _client;
        private Thread networkingThread;
        private Thread serverThread;
        NetworkingThreadStatus networkingStatus;
        private int completedJobsCount = 0;
        private object lockObject = new object();
        private RestClient client;
        private int? clientId = null;
        ChannelFactory<ServerInterface> factory;
        NetTcpBinding binding;
        string URL;
        ServerInterface channel;

        private volatile bool stop = false;

        public MainWindow()
        {
            InitializeComponent();
            JobProgressBar.Visibility = Visibility.Hidden;
        }

        private void StartThreads()
        {
            Task.Run(() => NetworkingThreadMethodAsync());

            serverThread = new Thread(new ThreadStart(ServerThreadMethod));
            serverThread.Start();
        }

        private async Task NetworkingThreadMethodAsync()
        {
            networkingStatus = new NetworkingThreadStatus();

            while (!stop)
            {
                try
                {
                    var client = new RestClient("http://localhost:5080");
                    var request = new RestRequest("api/Clients/getAll", Method.Get);
                    var response = await client.ExecuteAsync(request);
                    var clientsList = JsonConvert.DeserializeObject<List<ClientClass>>(response.Content);

                    foreach (ClientClass clientInfo in clientsList)
                    {
                        if (clientInfo.Id != clientId) // Ensure we're not checking our own client
                        {
                            binding = new NetTcpBinding();
                            URL = "net.tcp://" + ipAddressTextBox.Text + ":" + portTextBox.Text;
                            factory = new ChannelFactory<ServerInterface>(binding, URL);
                            channel = factory.CreateChannel();

                            if(channel.HasJob())
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    statusTextBlock.Text = "Working on job...";
                                    JobProgressBar.Visibility = Visibility.Visible;
                                });
                            }

                            string job = PythonCodeTextBox.Text;

                            if (job != null)
                            {
                                string result = ExecutePythonJob(job);
                                channel.SubmitResult(result.ToString());

                                UpdateJobStatus();

                                client = new RestClient("http://localhost:5080");
                                request = new RestRequest("api/Clients/updateJobs", Method.Put);
                                request.AddJsonBody(_client);
                                response = client.Execute(request);

                                Dispatcher.Invoke(() =>
                                {
                                    JobProgressBar.Visibility = Visibility.Hidden;
                                });
                            }
                        }
                    }
                    networkingStatus.CompletedJobsCount = completedJobsCount;

                    binding = new NetTcpBinding();
                    URL = "net.tcp://" + ipAddressTextBox.Text + ":" + portTextBox.Text;
                    factory = new ChannelFactory<ServerInterface>(binding, URL);
                    channel = factory.CreateChannel();

                    if (channel.GetResult() != null)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            ResultTextBox.Text = channel.GetResult();
                        });
                        channel.SubmitResult(null);
                    }
                }
                catch (Exception ex)
                {
                    LogError($"Error in NetworkingThread: {ex.Message}");
                }
                finally
                {
                    networkingStatus.IsWorking = false;
                }

                await Task.Delay(10000);
            }
        }

        private void ServerThreadMethod()
        {
            Console.WriteLine("Welcome to Jobs Server");
            ServiceHost host;
            NetTcpBinding tcp = new NetTcpBinding();
            host = new ServiceHost(typeof(Server));
            host.AddServiceEndpoint(typeof(ServerInterface), tcp,
            "net.tcp://" + ipAddressTextBox.Text + ":" + portTextBox.Text);
            host.Open();
            Console.WriteLine("System Online");
            Console.ReadLine();

            Thread.Sleep(Timeout.Infinite);
            host.Close();
        }



        private string ExecutePythonJob(string pythonCode)
        {
            try
            {
                ScriptEngine engine = Python.CreateEngine();
                ScriptScope scope = engine.CreateScope();

                try
                {
                    dynamic result = engine.Execute(pythonCode, scope);
                    return result.ToString();
                }
                catch (Exception ex)
                {
                    return $"Error: {ex.Message}";
                }
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

        private void QueryNetworkingThreadButton_Click(object sender, RoutedEventArgs e)
        {
            if (networkingStatus.IsWorking)
            {
                statusTextBlock.Text = $"Networking thread is currently working. Completed jobs: {networkingStatus.CompletedJobsCount}";
            }
            else
            {
                statusTextBlock.Text = $"Networking thread is idle. Completed jobs: {networkingStatus.CompletedJobsCount}";
            }
        }


        private void UpdateJobStatus()
        {
            completedJobsCount++;
            JobStatusTextBox.Text = $"Completed {completedJobsCount} jobs.";
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            _client = new ClientClass();
            _client.Port = int.Parse(portTextBox.Text);
            _client.IPAddress = ipAddressTextBox.Text;
            _client.CompletedJobsCount = 0;

            string ipAddress = _client.IPAddress;
            int port = _client.Port;

            // Check if the port is already registered
            var client = new RestClient("http://localhost:5080");
            var request = new RestRequest("api/Clients/getAll", Method.Get);
            var response = client.Execute(request);
            var clientsList = JsonConvert.DeserializeObject<List<ClientClass>>(response.Content);

            foreach (ClientClass clientInfo in clientsList)
            {
                if (clientInfo.Port == port)
                {
                    statusTextBlock.Text = "Error checking port status.";
                    return;
                }
            }
            StartThreads();
        }


        private void SendDataButton_Click(object sender, RoutedEventArgs e)
        {

            ChannelFactory<ServerInterface> factory;
            NetTcpBinding tcp = new NetTcpBinding();
            URL = "net.tcp://" + ipAddressTextBox.Text + ":" + portTextBox.Text;
            factory = new ChannelFactory<ServerInterface>(tcp, URL);
            ServerInterface channel = factory.CreateChannel();

            string job = PythonCodeTextBox.Text;
            channel.SubmitJob(job);      
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            stop = true;

            RestClient client = new RestClient("http://localhost:5080");
            RestRequest request = new RestRequest("api/Clients/delete", Method.Put);
            request.AddJsonBody(_client);
            RestResponse restResponse = client.Execute(request);
        }

        private void LogError(string errorMessage)
        {
            MessageBox.Show(errorMessage);
        }

    }
}

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using client.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace client.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool isConnected = false;

    // this doesnt need observable property, as oc alr notifies when items/added or removed, would need if we would be changing the whole collection
    public ObservableCollection<Message> messages {get;} = new();

    [ObservableProperty]
    private string inputText = string.Empty;

    [ObservableProperty]
    private string username = string.Empty;
    
    private const string ServerIP = "127.0.0.0";
    private const int Port = 9000;

    // TCP-related fields
    private TcpClient? client;
    private NetworkStream? stream;
    private StreamReader? reader;
    private StreamWriter? writer;

    [RelayCommand]
    private async Task Connect()
    {
        client = new TcpClient();
        try {
            await client.ConnectAsync(ServerIP, Port);
            stream = client.GetStream();
            reader = new StreamReader(stream, Encoding.UTF8);
            writer = new StreamWriter(stream, Encoding.UTF8) {AutoFlush = true};
            
            // get username
            await writer.WriteLineAsync(Username);
            IsConnected = true;

        } catch(Exception e)
        {
            Message errorMessage = new Message("Server", e.Message, DateTime.Now);
            messages.Add(errorMessage);
            IsConnected = false;
        }
    }

    [RelayCommand]
    private async Task Send()
    {
        if (writer == null)
        {
            Message errorMessage = new Message("Server","Something went wrong with the connection", DateTime.Now);
            messages.Add(errorMessage);
            return;
        }

        if (string.IsNullOrEmpty(InputText)) return;

        
        try {
            await writer.WriteLineAsync(InputText);
            if (InputText == "/exit")
            {
                await Disconnect();
            }
            InputText = string.Empty;
                
        } catch(Exception e)
        {
            Message errorMessage = new Message("Server",e.Message, DateTime.Now);
            messages.Add(errorMessage);
            return;
        }
    }
    

    [RelayCommand]
    private async Task Disconnect()
    {
        
    }
} 
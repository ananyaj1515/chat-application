using System.Collections.ObjectModel;
using client.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace client.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool isConnected;

    // this doesnt need observable property, as oc alr notifies when items/added or removed, would need if we would be changing the whole collection
    public ObservableCollection<Message> messages {get;} = new();

    [ObservableProperty]
    private string inputText = string.Empty;

    [ObservableProperty]
    private string username = string.Empty;
    
     private const string ServerIP = "127.0.0.0";
     private const int Port = 9000;
}

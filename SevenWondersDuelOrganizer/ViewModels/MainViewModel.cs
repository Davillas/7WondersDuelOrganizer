using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SevenWondersDuelOrganizer.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private int _Counter = 1;

    [RelayCommand]
    private void IncreaseCounter()
    {
        Counter++;
    }
}

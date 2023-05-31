using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SevenWondersDuelOrganizer.Models;
using SevenWondersDuelOrganizer.Services;
using SevenWondersDuelOrganizer.Services.Interfaces;

namespace SevenWondersDuelOrganizer.ViewModels;

public partial class MainViewModel : ObservableObject
{
    #region Private Members

    private IMenuService _menuService;

    #endregion

    #region Properties

    #region NavMenuItems : ObservableCollection - Collection of menu Items

    [ObservableProperty]
    /// <summary>Collection of menu Items</summary>
    private List<NavPanelElement> _NavMenuItems;

    #endregion

    #region SelectedNavMenuItem : NavPanelElement - Selected menu item

    [ObservableProperty]
    /// <summary>Selected menu item</summary>
    private NavPanelElement _SelectedNavMenuItem;

    #endregion

    #endregion

    #region Commands

    #region RelayCommand LoadMenuItemsAsyncCommand - Load Initial Menu Items

    /// <summary>Load Initial Menu Items</summary>
    [RelayCommand(CanExecute = nameof(CanLoadMenuItemsAsync))]
    private async Task LoadMenuItemsAsync()
    {
        var menuItems = await _menuService.LoadMenuTabsAsync();
        NavMenuItems = new List<NavPanelElement>(menuItems);
    }

    /// <summary>Load Initial Menu Items</summary>
    private bool CanLoadMenuItemsAsync() => true;

    #endregion



    #endregion

    public MainViewModel(IMenuService menuService)
    {
        _menuService = menuService;
    }

    public MainViewModel()
    {
        _menuService = new MenuService();
    }
}

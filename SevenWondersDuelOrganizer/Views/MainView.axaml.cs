using Avalonia.Controls;
using SevenWondersDuelOrganizer.ViewModels;
using System;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia;
using Avalonia.Threading;

namespace SevenWondersDuelOrganizer.Views;

public partial class MainView : UserControl
{
    private Control navDrawerPopup;
    private Control navDrawerButton;
    private Control mainGrid;

    protected override async void OnLoaded()
    {
        await ((MainViewModel)DataContext).LoadMenuItemsCommand.ExecuteAsync(null);

        base.OnLoaded();

    }

    public MainView()
    {
        InitializeComponent();


        navDrawerButton = this.FindControl<Control>("NavigationDrawerButton") ?? throw new Exception("Cannot Find Control");
        navDrawerPopup = this.FindControl<Control>("NavigationDrawer") ?? throw new Exception("Cannot Find Control");
        mainGrid = this.FindControl<Control>("MainGrid") ?? throw new Exception("Cannot Find Control");
    }

    public override void Render(DrawingContext context)
    {




        base.Render(context);

        Dispatcher.UIThread.InvokeAsync(() =>
        {
            // Get Relative position of button in relation to Main Grid
            var position = navDrawerButton.TranslatePoint(new Point(), mainGrid) ??
                           throw new Exception("Cannot translate point");
            //
            // channelConfigPopup.Margin = new Thickness(
            //     position.X,
            //     0,
            //     0,
            //     (mainGrid.Bounds.Height - position.Y - navDrawerButton.Bounds.Height));
            navDrawerPopup.Margin = new Thickness(
                position.X,
                0,
                0,
                (position.Y));
        });


    }
}
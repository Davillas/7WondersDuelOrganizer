using System;
using System.Threading;
using Avalonia.Animation.Easings;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;

namespace SevenWondersDuelOrganizer.Styles.CustomControls;

public partial class AnimatedNavigationDrawer : ContentControl
{
    #region Private Members

    // Control for closing this popup
    private Control underlyingControl;

    private bool isFirstAnimation = true;

    public bool isOpacityCaptured = false;


    /// <summary>
    /// Store control's original
    /// </summary>
    private double OrigOpacity;

    private TimeSpan _FrameRate = TimeSpan.FromSeconds(1 / 120.0);

    private int _TotalTicks => (int)(_AnimationTime.TotalSeconds / _FrameRate.TotalSeconds);


    /// <summary>
    /// Indicates if we found desired 100% auto size
    /// </summary>
    private bool _SizeFound;

    /// <summary>
    /// Stores the DesiredSize of the control
    /// </summary>
    private Size _DesiredSize;

    /// <summary>
    /// Flag for when control is animating
    /// </summary>
    private bool _Animating;

    /// <summary>
    /// Current position in animation
    /// </summary>
    private int _CurrentAnimationTick;

    /// <summary>
    /// Animation UI Timer 
    /// </summary>
    private DispatcherTimer _AnimationTimer = new DispatcherTimer();


    private Timer _SizingTimer;

    #endregion



    #region Public Properties



    /// <summary>
    /// Indicates if the control is currently opened
    /// </summary>
    public bool IsOpened => _CurrentAnimationTick >= _TotalTicks;



    #region IsOpen

    private bool _isOpen;

    public static readonly DirectProperty<AnimatedNavigationDrawer, bool> IsOpenProperty =
        AvaloniaProperty.RegisterDirect<AnimatedNavigationDrawer, bool>(nameof(IsOpen), o => o.IsOpen,
            (o, v) => o.IsOpen = v);

    public bool IsOpen
    {
        get => _isOpen;
        set
        {
            if (value == IsOpen)
            {
                return;
            }

            if (value)
            {

                //Inject the underlay
                if (Parent is Grid grid)
                {
                    underlyingControl.IsVisible = true;

                    // Set column/row span
                    if (grid.RowDefinitions?.Count > 0)
                        underlyingControl.SetValue(Grid.RowSpanProperty, grid.RowDefinitions.Count);

                    if (grid.ColumnDefinitions?.Count > 0)
                        underlyingControl.SetValue(Grid.ColumnSpanProperty, grid.ColumnDefinitions.Count);


                    if (!grid.Children.Contains(underlyingControl))
                        grid.Children.Insert(0, underlyingControl);
                }

            }

            // If closing...
            else
            {
                if (IsOpened)
                {
                    // Update Desired Size
                    UpdateDesiredSize();

                }

            }

            UpdateAnimation();
            //Raise property changed event
            SetAndRaise(IsOpenProperty, ref _isOpen, value);

        }
    }


    #endregion


    #region UnderLayOpacity

    private double _UnderlayOpacity = 1;

    public static readonly DirectProperty<AnimatedNavigationDrawer, double> UnderlayOpacityProperty =
        AvaloniaProperty.RegisterDirect<AnimatedNavigationDrawer, double>(nameof(UnderlayOpacity), o => o.UnderlayOpacity,
            (o, v) => o.UnderlayOpacity = v);

    public double UnderlayOpacity
    {
        get => _UnderlayOpacity;
        set => SetAndRaise(UnderlayOpacityProperty, ref _UnderlayOpacity, value);
    }

    #endregion

    #region AnimationTime

    private TimeSpan _AnimationTime = TimeSpan.FromSeconds(0.2);

    public static readonly DirectProperty<AnimatedNavigationDrawer, TimeSpan> AnimationTimeProperty =
        AvaloniaProperty.RegisterDirect<AnimatedNavigationDrawer, TimeSpan>(nameof(AnimationTime), o => o.AnimationTime,
            (o, v) => o.AnimationTime = v);

    public TimeSpan AnimationTime
    {
        get => _AnimationTime;
        set => SetAndRaise(AnimationTimeProperty, ref _AnimationTime, value);
    }

    #endregion

    #endregion


    #region Commands

    #region RelayCommand OpenCommand - IsOpen the control

    /// <summary>IsOpen the control</summary>
    [RelayCommand(CanExecute = nameof(CanOpen))]
    private void OpenCommand()
    {
        IsOpen = true;

    }

    /// <summary>IsOpen the control</summary>
    private bool CanOpen() => true;

    #endregion

    #region RelayCommand CloseCommand - Close the control

    /// <summary>Close the control</summary>
    [RelayCommand(CanExecute = nameof(CanClose))]
    private void CloseCommand()
    {
        IsOpen = false;


    }

    /// <summary>Close the control</summary>
    private bool CanClose() => true;

    #endregion



    #endregion


    #region Constructor



    public AnimatedNavigationDrawer()
    {

        underlyingControl = new Border
        {
            IsVisible = false,
            Background = Brushes.Black,
            ZIndex = 9,

        };

        underlyingControl.PointerPressed += (sender, e) =>
        {
            CloseCommand();
        };


        _AnimationTimer = new DispatcherTimer
        {
            Interval = _FrameRate
        };

        _CurrentAnimationTick = 0;

        _SizingTimer = new Timer((t) =>
        {
            // if already calculated the size
            if (_SizeFound)
            {
                return;
            }
            _SizeFound = true;


            Dispatcher.UIThread.InvokeAsync(() =>
            {
                // Set the desired size
                UpdateDesiredSize();


                //
                UpdateAnimation();

            });


        });



        _AnimationTimer.Tick += (sender, e) => AnimationTick();


    }
    #endregion


    #region Private Methods
    /// <summary>
    /// Calculate and start new animation
    /// </summary>
    private void UpdateAnimation()
    {
        if (!_SizeFound) return;


        _AnimationTimer.Start();


    }

    private void UpdateDesiredSize() => _DesiredSize = DesiredSize - Margin;


    /// <summary>
    /// Update control sizes based on the current tick
    /// </summary>
    private void AnimationTick()
    {
        {



            // if this is the first call
            if (isFirstAnimation)
            {

                _AnimationTimer.Stop();

                isFirstAnimation = false;



                //reset Opacity
                Opacity = OrigOpacity;

                //Set the final Size
                AnimationComplete();

                return;
            }




            if ((IsOpen && _CurrentAnimationTick > _TotalTicks)
                || (!IsOpen && _CurrentAnimationTick == 0))
            {
                _AnimationTimer.Stop();

                //Set the final Size
                AnimationComplete();

                _Animating = false;

                return;
            }

            // Set Animating flag
            _Animating = true;

            _CurrentAnimationTick += IsOpen ? 1 : -1;


            var percentageAnimated = (float)_CurrentAnimationTick / _TotalTicks;

            //Animation Easing
            var easing = new QuadraticEaseIn();


            // Calculate final width
            var finalWidth = _DesiredSize.Width * easing.Ease(percentageAnimated);
            var finalHeight = _DesiredSize.Height * easing.Ease(percentageAnimated);

            Width = finalWidth;
            Height = finalHeight;


            // Animate underlay
            underlyingControl.Opacity = UnderlayOpacity * easing.Ease(percentageAnimated);

        }
    }

    private void AnimationComplete()
    {
        if (IsOpen)
        {
            Width = double.NaN;
            Height = double.NaN;

        }
        else
        {
            Width = 0;
            Height = 0;


            //Remove the underlay
            if (Parent is Grid grid)
            {
                // if child is contained
                if (grid.Children.Contains(underlyingControl))
                    grid.Children.Remove(underlyingControl);
            }
        }

    }

    #endregion

    public override void Render(DrawingContext context)
    {
        if (!_SizeFound)
        {
            if (!isOpacityCaptured)
            {
                isOpacityCaptured = true;

                OrigOpacity = Opacity;


                // Opacity = 0;
            }

            _SizingTimer.Change(200, int.MaxValue);
        }



        base.Render(context);

    }
}
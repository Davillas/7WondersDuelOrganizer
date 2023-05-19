using Android.App;
using Android.Content.PM;
using Avalonia.Android;

namespace SevenWondersDuelOrganizer.Android;

[Activity(Label = "SevenWondersDuelOrganizer.Android", Theme = "@style/MyTheme.NoActionBar", Icon = "@drawable/icon", LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity
{
}

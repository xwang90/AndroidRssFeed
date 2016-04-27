using Android.App;
using Android.Widget;
using AndroidRssFeed.Interfaces;

namespace AndroidRssFeed.PlatformSpecific
{
  public class Message : IMessage
  {
    public void SendMessage(string message, string title = null)
    {
      Toast.MakeText(Application.Context, message, ToastLength.Long).Show();
    }
  }
}
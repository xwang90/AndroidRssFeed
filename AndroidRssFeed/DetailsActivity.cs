using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using AndroidRssFeed.Models;

namespace AndroidRssFeed
{
    [Activity(Label = "Details Activity")]
    public class DetailsActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            ActionBar.Title = "AndroidCentral RSS Viewer";

            ConnectivityManager connectivityManager = (ConnectivityManager)GetSystemService(Context.ConnectivityService);
            NetworkInfo activeConnection = connectivityManager.ActiveNetworkInfo;
            bool isOnline = (activeConnection != null) && activeConnection.IsConnected;

            if (isOnline)
            {
                string link = Intent.Extras.GetString("current_play_link");
                var details = DetailsFragment.NewInstanceFromLink(link); // Details
                var fragmentTransaction = FragmentManager.BeginTransaction();
                fragmentTransaction.Add(Android.Resource.Id.Content, details);
                fragmentTransaction.Commit();
            }
            else
            {
                string publishDate = Intent.Extras.GetString("current_play_publishdate");
                var details = DetailsFragment.NewInstanceFromPublishDate(publishDate); // Details
                var fragmentTransaction = FragmentManager.BeginTransaction();
                fragmentTransaction.Add(Android.Resource.Id.Content, details);
                fragmentTransaction.Commit();
            }
            
        }
    }
}

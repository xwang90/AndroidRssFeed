using System;

using Android.App;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidRssFeed.Models;
using AndroidRssFeed.Db;
using Android.Webkit;
using Android.Net;
using Android.Content;
using System.Net.Http;
using System.Net;

using System.Threading.Tasks;

namespace AndroidRssFeed
{
    internal class DetailsFragment : Fragment
    {
        private RSSFeedItem playItemFromDatabase;
        private const string DATABASE_NAME = "rssfeedposts";
        private WebView display_webview;
        private string htmlCode;

        public int ShownPlayId { get { return Arguments.GetInt("current_play_id", 0); } }

        public static DetailsFragment NewInstance(int playId)
        {
            var detailsFrag = new DetailsFragment { Arguments = new Bundle() };
            detailsFrag.Arguments.PutInt("current_play_id", playId);
            return detailsFrag;
        }

        public static DetailsFragment NewInstanceFromPublishDate(string publishDate)
        {
            var detailsFrag = new DetailsFragment { Arguments = new Bundle() };
            detailsFrag.Arguments.PutString("current_play_publishdate", publishDate);
            return detailsFrag;
        }

        public static DetailsFragment NewInstanceFromLink(string link)
        {
            var detailsFrag = new DetailsFragment { Arguments = new Bundle() };
            detailsFrag.Arguments.PutString("current_play_link", link);
            return detailsFrag;
        }



        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            if (container == null)
            {
                // Currently in a layout without a container, so no reason to create our view.
                return null;
            }

            ConnectivityManager connectivityManager = (ConnectivityManager)Activity.GetSystemService(Context.ConnectivityService);
            NetworkInfo activeConnection = connectivityManager.ActiveNetworkInfo;
            bool isOnline = (activeConnection != null) && activeConnection.IsConnected;

            View view = inflater.Inflate(Resource.Layout.DetailWebView, container, false);
            display_webview = view.FindViewById<WebView>(Resource.Id.displaywebview);

            if (isOnline)
            {
                string displayLink=this.Arguments.GetString("current_play_link");
                WebClient client = new WebClient();
                
                htmlCode = client.DownloadString(displayLink);
                display_webview.Settings.JavaScriptEnabled = true;
                display_webview.LoadData(htmlCode, "text/html", "charset=UTF-8");
                
            }
            else
            {
                display_webview.Settings.JavaScriptEnabled = true;
                display_webview.LoadData(playItemFromDatabase.Content, "text/html", "charset=UTF-8");
                
            }

            return view;
        }


        public override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            ConnectivityManager connectivityManager = (ConnectivityManager)Activity.GetSystemService(Context.ConnectivityService);
            NetworkInfo activeConnection = connectivityManager.ActiveNetworkInfo;
            bool isOnline = (activeConnection != null) && activeConnection.IsConnected;

            if (!isOnline)
            {
                string playPublishDate = this.Arguments.GetString("current_play_publishdate");
                Log.Debug("DetailsFragment", "OnCreate Display Fragment Date: " + playPublishDate);

                DbAdapter dba = new DbAdapter(Activity);
                dba.CreateDatabase(DATABASE_NAME);
                playItemFromDatabase = dba.getRssFeedItemListing(playPublishDate);

                if (playItemFromDatabase == null)
                {
                    Log.Debug("DetailsFragment", "Could not find item:" + playPublishDate);
                }

                dba.close();
            }
        }

    }
}

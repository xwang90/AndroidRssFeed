using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidRssFeed.ViewModels;
using AndroidRssFeed.Db;
using Android.Util;
using AndroidRssFeed.Models;
using AndroidRssFeed.Adapters;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Android.Support.V4.Widget;
using Android.Net;
using System.Net.Http;

namespace AndroidRssFeed
{
    public class RssFeedItemListFragment : ListFragment
    {
        private int _currentPlayId;
        private bool _isDualPane;
        private ObservableCollection<RSSFeedItem> DbAllFeedItems;
        private View previous;
        
        private const string DATABASE_NAME = "rssfeedposts";
        private static MasterViewModel viewModel;
        public static MasterViewModel ViewModel
        {
            get { return viewModel ?? (viewModel = new MasterViewModel()); }
        }

        public override async void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            var swipeRefreshLayout = Activity.FindViewById<View>(Resource.Id.refresher);

            previous = new View(Activity);
            
            ListView.ScrollStateChanged += (sender, e) => {
                // do nothing
            };

            ListView.Scroll += (sender, e) => {
                bool enable = false;
                if (ListView != null && ListView.ChildCount > 0)
                {
                    // check if the first item of the list is visible
                    bool firstItemVisible = ListView.FirstVisiblePosition == 0;

                    // check if the top of the first item is visible
                    bool topOfFirstItemVisible = ListView.GetChildAt(0).Top == 0;

                    // enabling or disabling the refresh layout
                    enable = firstItemVisible && topOfFirstItemVisible;
                }

                swipeRefreshLayout.Enabled = enable;
            };

            var detailsFrame = Activity.FindViewById<View>(Resource.Id.details);
            _isDualPane = detailsFrame != null && detailsFrame.Visibility == ViewStates.Visible;

            await RefreshList();

            if (savedInstanceState != null)
            {
                _currentPlayId = savedInstanceState.GetInt("current_play_id", 0);
            }

            if (_isDualPane)
            {
                ListView.ChoiceMode = ChoiceMode.Single;
                ShowDetails(_currentPlayId);
            }
        }

        public override void OnListItemClick(ListView l, View v, int position, long id)
        {
            previous.Selected = false;
            v.Selected = true;
            previous = v;     

            ShowDetails(position);
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutInt("current_play_id", _currentPlayId);
        }

        private void ShowDetails(int playId)
        {
            ConnectivityManager connectivityManager = (ConnectivityManager)Activity.GetSystemService(Context.ConnectivityService);
            NetworkInfo activeConnection = connectivityManager.ActiveNetworkInfo;
            bool isOnline = (activeConnection != null) && activeConnection.IsConnected;
            RSSFeedItem DisplayedRSSFeedItem;

            _currentPlayId = playId;

            if(isOnline)
              DisplayedRSSFeedItem = viewModel.FeedItems[_currentPlayId];
            else
              DisplayedRSSFeedItem = DbAllFeedItems[_currentPlayId];
            

            if (_isDualPane)
            {
                // We can display everything in-place with fragments.
                // Have the list highlight this item and show the data.
                ListView.SetItemChecked(playId, true);
              

                var details = FragmentManager.FindFragmentById(Resource.Id.details) as DetailsFragment;

                    // Make new fragment to show this selection.
                    if(isOnline)
                        details = DetailsFragment.NewInstanceFromLink(DisplayedRSSFeedItem.Link);
                    else
                        details = DetailsFragment.NewInstanceFromPublishDate(DisplayedRSSFeedItem.PublishDate);

                    // Execute a transaction, replacing any existing
                    // fragment with this one inside the frame.
                    var ft = FragmentManager.BeginTransaction();
                    ft.Replace(Resource.Id.details, details);
                    ft.SetTransition(FragmentTransit.FragmentFade);
                    ft.Commit();

            }
            else
            {
                // Otherwise we need to launch a new activity to display
                // the dialog fragment with selected text.
                var intent = new Intent();

                intent.SetClass(Activity, typeof(DetailsActivity));

                if (isOnline)
                {
                    intent.PutExtra("current_play_link", DisplayedRSSFeedItem.Link);
                }
                else
                {
                    intent.PutExtra("current_play_publishdate", DisplayedRSSFeedItem.PublishDate);
                }

                StartActivity(intent);
            }
        }

        public async Task RefreshList()
        {
            ConnectivityManager connectivityManager = (ConnectivityManager)Activity.GetSystemService(Context.ConnectivityService);
            NetworkInfo activeConnection = connectivityManager.ActiveNetworkInfo;
            bool isOnline = (activeConnection != null) && activeConnection.IsConnected;

            Log.Debug("RssFeedItemListFragment", "isOnline: " + isOnline);

            if (isOnline)
            {
                viewModel = new MasterViewModel();
                //progress.Visibility = ViewStates.Visible;
                await viewModel.ExecuteLoadItemsCommand();

                ListAdapter = new FeedItemAdapter(Activity, viewModel.FeedItems);

                UpdateDatebase(viewModel.FeedItems);

            }
            else
            {
                DbAdapter dba = new DbAdapter(Activity);
                dba.CreateDatabase(DATABASE_NAME);
                DbAllFeedItems = dba.getDbAllRssFeedItems();
                dba.close();

                if (DbAllFeedItems.Count==0)//Handle the case when there is no Internet connection and database is also empty.
                {
                    RSSFeedItem rssfeeditem = new RSSFeedItem();
                    rssfeeditem.Id = 1;
                    rssfeeditem.Description = "";
                    rssfeeditem.Link = "";
                    rssfeeditem.PublishDate = "";
                    rssfeeditem.Author = "";
                    rssfeeditem.AuthorEmail = "";
                    rssfeeditem.Title = "";
                    rssfeeditem.Content = "";
                    rssfeeditem.Image = "";
                    rssfeeditem.ShowImage = false;
                    DbAllFeedItems.Add(rssfeeditem);
                }


                ListAdapter = new FeedItemAdapter(Activity, DbAllFeedItems);
            }
           
            

        }

        private async Task<bool> UpdateDatebase(ObservableCollection<RSSFeedItem> ItemList)
        {
            DbAdapter dba = new DbAdapter(Activity);
            dba.CreateDatabase(DATABASE_NAME);
            //Database clear
            dba.deleteAllRows();
            var httpClient = new HttpClient();

            foreach (var item in ItemList)
            {
                item.Content = await httpClient.GetStringAsync(item.Link);
                dba.insertRssFeedItemListing(item);

            }

            dba.close();
            return true;
        }      

        
    }
}


using Android.App;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Views;
using AndroidRssFeed.ViewModels;

namespace AndroidRssFeed
{
  [Activity(Label = "@string/app_name", MainLauncher = true, Icon = "@drawable/ic_launcher", Theme = "@style/Theme")]
  public class MasterActivity : Activity
  {
        SwipeRefreshLayout refresher;
        RssFeedItemListFragment rssfeeditemlist;
        private static MasterViewModel viewModel;
        public static MasterViewModel ViewModel
        {
            get { return viewModel ?? (viewModel = new MasterViewModel()); }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Master);

            if (savedInstanceState==null)
            {
                rssfeeditemlist = new RssFeedItemListFragment();
                FragmentManager.BeginTransaction()
                    .Add(Resource.Id.RssFeedItemList_Fragment, rssfeeditemlist, "rssfeeditem-list")
                    .Commit();               
            }
            else
                rssfeeditemlist = FragmentManager.FindFragmentById(Resource.Id.RssFeedItemList_Fragment) as RssFeedItemListFragment;


            refresher = FindViewById<SwipeRefreshLayout>(Resource.Id.refresher);
            refresher.Refresh += async delegate {
                await rssfeeditemlist.RefreshList();
                refresher.Refreshing = false;
            };


        }

        
        }

    }



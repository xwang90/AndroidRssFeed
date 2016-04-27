using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Views;
using Android.Widget;
using AndroidRssFeed.PlatformSpecific;
using AndroidRssFeed.Models;

namespace AndroidRssFeed.Adapters
{
  public class FeedItemAdapter : BaseAdapter
  {
    private ImageLoader imageLoader;
    private Activity activity;
		IEnumerable<RSSFeedItem> items;
    public FeedItemAdapter(Activity activity, IEnumerable<RSSFeedItem> items)
		{
            this.imageLoader = new ImageLoader(activity);
			this.activity = activity;
			this.items = items;
		}

        //Wrapper class for adapter for cell re-use
        private class FeedItemAdapterHelper : Java.Lang.Object
		{
			public TextView Title { get; set; }
            public TextView PublishDate { get; set; }
            public TextView Author { get; set; }
            public TextView Caption { get; set; }
            public ImageView Image { get; set; }
		}




		#region implemented abstract members of BaseAdapter
		public override Java.Lang.Object GetItem (int position)
		{
			return position;
		}

		public override long GetItemId (int position)
        {
            if (items!=null&&items.Count()>0)
            {
                var item = items.ElementAt(position);
                return item.Id;
            }
            else
                return 0;
		}

    public override bool HasStableIds
    {
      get { return true; }
    }

    public override View GetView (int position, View convertView, ViewGroup parent)
		{
			FeedItemAdapterHelper helper = null;
			if (convertView == null) {
				convertView = activity.LayoutInflater.Inflate (Resource.Layout.RSSItem, null);
				helper = new FeedItemAdapterHelper ();
        helper.Title = convertView.FindViewById<TextView>(Resource.Id.text_title);
        helper.PublishDate = convertView.FindViewById<TextView>(Resource.Id.text_publishdate);
		
        helper.Title.SetMaxLines(2);
        helper.PublishDate.SetMaxLines(2);
        convertView.Tag = helper;
			} else {
				helper = convertView.Tag as FeedItemAdapterHelper;
			}

            if (items != null && items.Count() > 0)
            {
                var item = items.ElementAt(position);
                helper.Title.Text = item.Title;
                helper.PublishDate.Text = item.PublishDate;
            }
                
			return convertView;
		}

		public override int Count {
			get {
                if (items != null)
                    return items.Count();
                else
                    return 0;
			}
		}
		#endregion
  }
}
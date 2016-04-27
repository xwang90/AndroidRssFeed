﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using AndroidRssFeed.Helpers;
using AndroidRssFeed.Helpers.Web.Utilities;
using AndroidRssFeed.Models;
using Android.Util;

namespace AndroidRssFeed.ViewModels
{
  public class MasterViewModel : ViewModelBase
  {

    private ObservableCollection<RSSFeedItem> feedItems = new ObservableCollection<RSSFeedItem>();

    /// <summary>
    /// gets or sets the feed items
    /// </summary>
    public ObservableCollection<RSSFeedItem> FeedItems
    {
      get { return feedItems; }
      set { feedItems = value; OnPropertyChanged("FeedItems"); }
    }

    private RSSFeedItem selectedFeedItem;
    /// <summary>
    /// Gets or sets the selected feed item
    /// </summary>
    public RSSFeedItem SelectedFeedItem
    {
      get{ return selectedFeedItem; }
      set
      {
        selectedFeedItem = value;
        OnPropertyChanged("SelectedFeedItem");
      }
    }

    private RelayCommand loadItemsCommand;
    /// <summary>
    /// Command to load/refresh items
    /// </summary>
    public ICommand LoadItemsCommand
    {
      get { return loadItemsCommand ?? (loadItemsCommand = new RelayCommand(async () => await ExecuteLoadItemsCommand())); }
    }

    public async Task ExecuteLoadItemsCommand()
    {
      if (IsBusy)
        return;

      IsBusy = true;

      var httpClient = new HttpClient();
      var feed = "http://feeds.feedburner.com/androidcentral?format=xml";
      var responseString = await httpClient.GetStringAsync(feed);

      FeedItems.Clear();
      var items = await ParseFeed(responseString);
      foreach (var item in items)
      {
        //item.Image = Gravatar.GetUrl(item.Author);
        //item.Content = await httpClient.GetStringAsync(item.Link);
        

        FeedItems.Add(item);
      }


      IsBusy = false;
    }

    /// <summary>
    /// Parse the RSS Feed
    /// </summary>
    /// <param name="rss"></param>
    /// <returns></returns>
    private async Task<List<RSSFeedItem>> ParseFeed(string rss)
    {
      return await Task.Run(() =>
      {
        var xdoc = XDocument.Parse(rss);
        var id = 0;
        return (from item in xdoc.Descendants("item")
                select new RSSFeedItem
                {
                  Title = (string)item.Element("title"),
                  Description = (string)item.Element("description"),
                  Link = (string)item.Element("link"),
                  PublishDate = (string)item.Element("pubDate"),
                  AuthorEmail = (string)item.Element("author"),
                  Id = id++
                }).ToList();
      });
    }

    /// <summary>
    /// Gets a specific feed item for an Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public RSSFeedItem GetFeedItem(int id)
    {
      return FeedItems.FirstOrDefault(i => i.Id == id);
    }

  }
}

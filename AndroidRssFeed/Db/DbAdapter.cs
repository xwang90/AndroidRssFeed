using Android.Content;
using Android.Database;
using Android.Database.Sqlite;


using AndroidRssFeed.Models;
using System.Collections.ObjectModel;
using System.IO;

namespace AndroidRssFeed.Db
{

    public class DbAdapter
    {

        public const string KEY_ROWID = "_id";
        public const string KEY_DESCRIPTION = "description";
        public const string KEY_LINK = "link";
        public const string KEY_PUBLISHDATE = "publishdate";
        public const string KEY_AUTHOR = "author";
        public const string KEY_AUTHOREMAIL = "authoremail";
        public const string KEY_TITLE = "title";
        public const string KEY_CONTENT = "content";
        public const string KEY_CAPTION = "caption";
        public const string KEY_SHOWIMAGE = "showimage";
        public const string KEY_IMAGE = "image";

        private const string DATABASE_NAME = "rssfeedposts";
        private const string DATABASE_TABLE = "rssfeedpostlist";
        private const int DATABASE_VERSION = 1;

        private const string DATABASE_CREATE_LIST_TABLE = "create table " + DATABASE_TABLE + " (" +
                                                                    KEY_ROWID + " integer primary key autoincrement, " +
                                                                    KEY_DESCRIPTION + " text, " +
                                                                    KEY_LINK + " text, " +
                                                                    KEY_PUBLISHDATE + " text, " +
                                                                    KEY_AUTHOR + " text, " +
                                                                    KEY_AUTHOREMAIL + " text, " +
                                                                    KEY_TITLE + " text, " +
                                                                    KEY_CONTENT + " text, " +
                                                                    KEY_CAPTION + " text, " +
                                                                    KEY_IMAGE + " text, " +
                                                                    KEY_SHOWIMAGE + " boolean);";

        private string[] AllColumns ={
                            KEY_ROWID,
                            KEY_DESCRIPTION,
                            KEY_LINK,
                            KEY_PUBLISHDATE,
                            KEY_AUTHOR,
                            KEY_AUTHOREMAIL,
                            KEY_TITLE,
                            KEY_CONTENT,
                            KEY_CAPTION,
                            KEY_IMAGE,
                            KEY_SHOWIMAGE
    };


        private SQLiteDatabase sqLiteDatabase;
        
        //String for Message handling
        private string sqLiteDatabase_message;
        

        private Context context;

        public DbAdapter(Context c)
        {
            context = c;
        }

        //Creates a new database which name is given by the parameter
        public void CreateDatabase(string sqldb_name)
        {
            try
            {
                sqLiteDatabase_message = "";
                string sqldb_location = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                string sqldb_path = Path.Combine(sqldb_location, sqldb_name);
                bool sqldb_exists = File.Exists(sqldb_path);
                if (!sqldb_exists)
                {
                    sqLiteDatabase = SQLiteDatabase.OpenOrCreateDatabase(sqldb_path, null);
                    sqLiteDatabase.ExecSQL(DATABASE_CREATE_LIST_TABLE);
                    sqLiteDatabase_message = "Database: " + sqldb_name + " created";
                }
                else
                {
                    sqLiteDatabase = SQLiteDatabase.OpenDatabase(sqldb_path, null, DatabaseOpenFlags.OpenReadwrite);
                    sqLiteDatabase_message = "Database: " + sqldb_name + " opened";
                }
                
            }
            catch (SQLiteException ex)
            {
                sqLiteDatabase_message = ex.Message;
            }
        }

        public void deleteAllRows()
        {
            sqLiteDatabase.Delete(DATABASE_TABLE, null, null);
        }

        public void close()
        {
            sqLiteDatabase.Close();
        }

        public long insertRssFeedItemListing(RSSFeedItem item)
        {
            ContentValues initialValues = new ContentValues();
            initialValues.Put(KEY_DESCRIPTION, item.Description);
            initialValues.Put(KEY_LINK, item.Link);
            initialValues.Put(KEY_PUBLISHDATE, item.PublishDate);
            initialValues.Put(KEY_AUTHOR, item.Author);
            initialValues.Put(KEY_AUTHOREMAIL, item.AuthorEmail);
            initialValues.Put(KEY_TITLE, item.Title);
            initialValues.Put(KEY_CONTENT,item.Content);
            initialValues.Put(KEY_CAPTION, item.Caption);
            initialValues.Put(KEY_IMAGE, item.Image);
            initialValues.Put(KEY_SHOWIMAGE, item.ShowImage);
            return sqLiteDatabase.Insert(DATABASE_TABLE, null, initialValues);
        }

        public RSSFeedItem getRssFeedItemListing(string PublishDate)
        {
            ICursor mCursor =
                        sqLiteDatabase.Query(DATABASE_TABLE, AllColumns,
                                KEY_PUBLISHDATE + "= '" + PublishDate + "'",
                                null,
                                null,
                                null,
                                null);
            if (mCursor != null && mCursor.Count> 0)
            {
                mCursor.MoveToFirst();
                RSSFeedItem rssfeeditem = new RSSFeedItem();
                rssfeeditem.Id=mCursor.GetInt(mCursor.GetColumnIndex(KEY_ROWID));
                rssfeeditem.Description= mCursor.GetString(mCursor.GetColumnIndex(KEY_DESCRIPTION));
                rssfeeditem.Link= mCursor.GetString(mCursor.GetColumnIndex(KEY_LINK));
                rssfeeditem.PublishDate = mCursor.GetString(mCursor.GetColumnIndex(KEY_PUBLISHDATE));
                rssfeeditem.Author = mCursor.GetString(mCursor.GetColumnIndex(KEY_AUTHOR));
                rssfeeditem.AuthorEmail = mCursor.GetString(mCursor.GetColumnIndex(KEY_AUTHOREMAIL));
                rssfeeditem.Title = mCursor.GetString(mCursor.GetColumnIndex(KEY_TITLE));
                rssfeeditem.Content= mCursor.GetString(mCursor.GetColumnIndex(KEY_CONTENT));
                //rssfeeditme.Caption = mCursor.GetString(mCursor.GetColumnIndex(KEY_CAPTION));
                rssfeeditem.Image = mCursor.GetString(mCursor.GetColumnIndex(KEY_IMAGE));
                rssfeeditem.ShowImage = mCursor.GetInt(mCursor.GetColumnIndex(KEY_SHOWIMAGE))>0;
                mCursor.Close();

                return rssfeeditem;
            }

            mCursor.Close();
            return null;
        }

        public ObservableCollection<RSSFeedItem> getDbAllRssFeedItems()
        {

            ObservableCollection<RSSFeedItem> DbFeedItems = new ObservableCollection<RSSFeedItem>();

            ICursor mCursor =
                        sqLiteDatabase.Query(DATABASE_TABLE, AllColumns,
                                null,
                                null,
                                null,
                                null,
                                null);
            if (mCursor != null && mCursor.Count > 0)
            {
                mCursor.MoveToFirst();

                do
                {
                    RSSFeedItem rssfeeditem = new RSSFeedItem();
                    rssfeeditem.Id = mCursor.GetInt(mCursor.GetColumnIndex(KEY_ROWID));
                    rssfeeditem.Description = mCursor.GetString(mCursor.GetColumnIndex(KEY_DESCRIPTION));
                    rssfeeditem.Link = mCursor.GetString(mCursor.GetColumnIndex(KEY_LINK));
                    rssfeeditem.PublishDate = mCursor.GetString(mCursor.GetColumnIndex(KEY_PUBLISHDATE));
                    rssfeeditem.Author = mCursor.GetString(mCursor.GetColumnIndex(KEY_AUTHOR));
                    rssfeeditem.AuthorEmail = mCursor.GetString(mCursor.GetColumnIndex(KEY_AUTHOREMAIL));
                    rssfeeditem.Title = mCursor.GetString(mCursor.GetColumnIndex(KEY_TITLE));
                    rssfeeditem.Content = mCursor.GetString(mCursor.GetColumnIndex(KEY_CONTENT));
                    //rssfeeditem.Caption = mCursor.GetString(mCursor.GetColumnIndex(KEY_CAPTION));
                    rssfeeditem.Image = mCursor.GetString(mCursor.GetColumnIndex(KEY_IMAGE));
                    rssfeeditem.ShowImage = mCursor.GetInt(mCursor.GetColumnIndex(KEY_SHOWIMAGE)) > 0;
                    DbFeedItems.Add(rssfeeditem);
                }
                while (mCursor.MoveToNext());   
                                
            }

            mCursor.Close();

            return DbFeedItems;
        }

    }
}
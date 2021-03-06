﻿using CNBlogs.DataHelper.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;
using Windows.UI.Xaml.Controls;
using NotificationsExtensions.TileContent;
using Windows.Storage;
using System.IO.Compression;
using Windows.Storage.Streams;
using Windows.System.Profile;
using Windows.UI.Notifications;

namespace CNBlogs.DataHelper.Function
{
    public static class Functions
    {
        static Windows.ApplicationModel.Resources.ResourceLoader loader = new Windows.ApplicationModel.Resources.ResourceLoader();

        public static string GetUniqueDeviceId()
        {
            HardwareToken ht = Windows.System.Profile.HardwareIdentification.GetPackageSpecificToken(null);
            var id = ht.Id;
            var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(id);
            byte[] bytes = new byte[id.Length];
            dataReader.ReadBytes(bytes);
            string s = BitConverter.ToString(bytes);
            return s.Replace("-", "");
        }

        public static string LoadResourceString(string resourceName)
        {
            string str = loader.GetString(resourceName);
            return str;
        }

        public static void CreateTile(Post post)
        {
            TileUpdater updater = TileUpdateManager.CreateTileUpdaterForApplication();
            updater.EnableNotificationQueue(true);
            updater.Clear();

            var tileLarge = TileContentFactory.CreateTileWide310x150PeekImage01();
            tileLarge.Image.Src = "ms-appx:///Assets/TileUpdateBG_Wide_310x150.png";
            string title = post.Title;

            tileLarge.TextBodyWrap.Text = post.Summary;
            tileLarge.TextHeading.Text = title;

            var tileSmall = TileContentFactory.CreateTileSquare150x150PeekImageAndText02();
            tileSmall.Image.Src = "ms-appx:///Assets/TileUpdateBG_Square_150x150.png";
            tileSmall.TextHeading.Text = post.BlogApp;
            tileSmall.TextBodyWrap.Text = post.Title;

            tileLarge.Square150x150Content = tileSmall;
            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileLarge.CreateNotification());
        }

        //public static double GetWindowsWidth()
        //{
        //    Windows.UI.Xaml.Controls.Frame rootFrame = Windows.UI.Xaml.Window.Current.Content as Windows.UI.Xaml.Controls.Frame;
        //    double Width = rootFrame.ActualWidth;
        //    return Width;
        //}

        /// <summary>
        /// Deserlialize xml to specified type
        /// </summary>
        public static T Deserlialize<T>(string xml)
        {
            T result = default(T);

            if (!string.IsNullOrWhiteSpace(xml))
            {
                var ser = new XmlSerializer(typeof(T));

                using (var reader = new StringReader(xml))
                {
                    result = (T)ser.Deserialize(reader);
                }
            }

            return result;
        }

        public static async Task<string> Serialize<T>(T obj)
        {
            var result = string.Empty;

            if (obj != null)
            {
                var ser = new XmlSerializer(typeof(T));

                using (var writer = new StringWriter())
                {
                    ser.Serialize(writer, obj);

                    await writer.FlushAsync();

                    result = writer.GetStringBuilder().ToString();
                }
            }

            return result;
        }

        /// <summary>
        /// parse string to datetime,  if fail will return DateTime.MinValue
        /// </summary>
        public static DateTime ParseDateTime(string datetime)
        {
            var date = DateTime.MinValue;

            DateTime.TryParse(datetime, out date);

            return date;
        }


        public static int ParseInt(string number)
        {
            var result = 0;

            int.TryParse(number, out result);

            return result;
        }

        /// <summary>
        /// check if there is a file under specified folder
        /// </summary>
        /// <param name="folder">folder to check</param>
        /// <param name="filename">file name under folder</param>
        public async static Task<bool> IsFileExist(StorageFolder folder, string filename)
        {
            var fullpath = Path.Combine(folder.Path, filename);
            return await IsFileExist(fullpath);
        }

        /// <summary>
        /// is the file full path exist
        /// </summary>
        public async static Task<bool> IsFileExist(string path)
        {
            try
            {
                await StorageFile.GetFileFromPathAsync(path);
                return true;
            }
            catch (FileNotFoundException)
            {
                System.Diagnostics.Debug.WriteLine("File not exist: {0}", path);
            }

            return false;
        }

        public static string ParseBlogAppFromURL(string url)
        {
            if (!string.IsNullOrWhiteSpace(url) && Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
            {
                var uri = new Uri(url);

                var blogApp = uri.LocalPath.Trim(new[] { '/' });

                return blogApp;
            }

            return string.Empty;
        }

        public static string ParseBlogIDFromURL(string url)
        {
            return Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute) ? Path.GetFileNameWithoutExtension(url) : url;
        }

        public static async Task ShowMessage(string message)
        {
            var msgbox = new Windows.UI.Popups.MessageDialog(message);

            var result = await msgbox.ShowAsync();
        }

        public static void GridViewScrollToTop(GridView gv)
        {
            var item0 = gv.Items[0];
            gv.ScrollIntoView(item0, ScrollIntoViewAlignment.Leading);
        }
    }
}

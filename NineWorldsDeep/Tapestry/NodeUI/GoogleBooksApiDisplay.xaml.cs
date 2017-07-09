using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Books.v1;
using Google.Apis.Books.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;

namespace NineWorldsDeep.Tapestry.NodeUI
{
    /// <summary>
    /// Interaction logic for GoogleBooksApiDisplay.xaml
    /// </summary>
    public partial class GoogleBooksApiDisplay : UserControl
    {
        private UserCredential credential;
        private BooksService booksService;


        //async related 
        private readonly SynchronizationContext syncContext;
        private DateTime previousTime = DateTime.Now;
        
        public GoogleBooksApiDisplay()
        {
            InitializeComponent();
            syncContext = SynchronizationContext.Current;
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            ProcessExpanderState((Expander)sender);
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            ProcessExpanderState((Expander)sender);
        }

        /// <summary>
        /// manages grid rows to share space between multiple expanded expanders
        /// </summary>
        /// <param name="expander"></param>
        private void ProcessExpanderState(Expander expander)
        {
            Grid parent = FindAncestor<Grid>(expander);
            int rowIndex = Grid.GetRow(expander);

            if (parent.RowDefinitions.Count > rowIndex && rowIndex >= 0)
                parent.RowDefinitions[rowIndex].Height =
                    (expander.IsExpanded ? new GridLength(1, GridUnitType.Star) : GridLength.Auto);
        }

        public static T FindAncestor<T>(DependencyObject current)
            where T : DependencyObject
        {
            // Need this call to avoid returning current object if it is the 
            // same type as parent we are looking for
            current = VisualTreeHelper.GetParent(current);

            while (current != null)
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            };
            return null;
        }

        //adapted from: https://github.com/google/google-api-dotnet-client-samples/blob/master/Books.ListMyLibrary/Program.cs
        private async void btnRefreshBookShelves_Click(object sender, RoutedEventArgs e)
        {           
            using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    new[] { BooksService.Scope.Books },
                    "NWD Desktop", CancellationToken.None, new FileDataStore("NWD.Google.Books"));
            }

            // Create the service.
            var service = new BooksService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Nine Worlds Deep",
            });

            this.booksService = service;

            var shelves = await GetShelves();

            lvBookshelves.ItemsSource = shelves;
        }

        private async Task<List<BookshelfWrapper>> GetShelves()
        {
            List<BookshelfWrapper> lst = new List<BookshelfWrapper>();

            if (this.booksService == null)
            {
                return lst;
            }

            // Execute async.
            var shelves = await booksService.Mylibrary.Bookshelves.List().ExecuteAsync();

            foreach(Bookshelf b in shelves.Items)
            {
                lst.Add(new BookshelfWrapper(b));
            }

            return lst;
        }

        private async Task<List<VolumeWrapper>> GetVolumes(Bookshelf bookShelf)
        {
            List<VolumeWrapper> lst = new List<VolumeWrapper>();

            if(booksService == null  || bookShelf == null)
            {
                expDetails.Header = "Details";
                return lst;
            }

            expDetails.Header = "BookShelf: " + bookShelf.Title;
            
            // List all volumes in this bookshelf.
            if (bookShelf.VolumeCount > 0)
            {
                var request = booksService.Mylibrary.Bookshelves.Volumes.List(bookShelf.Id.ToString());
                Volumes inBookshelf = await request.ExecuteAsync();

                if (inBookshelf.Items == null)
                {
                    return lst; 
                }

                foreach (Volume volume in inBookshelf.Items)
                {
                    lst.Add(new VolumeWrapper(volume));
                }
            }

            return lst;
        }

        public void StatusDetailUpdateLibrary(string text)
        {
            var currentTime = DateTime.Now;

            if ((DateTime.Now - previousTime).Milliseconds <= 50) return;

            syncContext.Post(new SendOrPostCallback(s =>
            {
                tbStatusLibrary.Text = (string)s;
            }), text);

            previousTime = currentTime;
        }

        private async void lvBookshelves_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(SelectedBookshelfWrapper == null)
            {
                return;
            }

            var volumes = await GetVolumes(SelectedBookshelfWrapper.Bookshelf);

            lvVolumes.ItemsSource = volumes;            
        }

        private BookshelfWrapper SelectedBookshelfWrapper
        {
            get { return (BookshelfWrapper)lvBookshelves.SelectedItem; }
        }

        private async void btnRevokeCredentials_Click(object sender, RoutedEventArgs e)
        {
            await credential.RevokeTokenAsync(CancellationToken.None);
            Clear();
        }

        private void Clear()
        {
            expDetails.Header = "Details";
            lvBookshelves.ItemsSource = null;
            lvVolumes.ItemsSource = null;
            tbDetails.Text = "";
        }
    }

    internal class VolumeWrapper
    {
        public Volume Volume { get; private set; }

        public VolumeWrapper(Volume vol)
        {
            Volume = vol;
        }

        public override string ToString()
        {
            string output = VolumeTitle;
            string authors = GetVolumeAuthorsAsString();

            if (!string.IsNullOrWhiteSpace(authors))
            {
                output += " (" + authors +")";
            }

            return output;
        }

        public string VolumeDescription
        {
            get { return Volume.VolumeInfo.Description; }
        }

        public string VolumeTitle
        {
            get { return Volume.VolumeInfo.Title; }
        }

        public string GetVolumeAuthorsAsString()
        {
            if (Volume == null || 
                Volume.VolumeInfo == null ||
                Volume.VolumeInfo.Authors == null)
            {
                return "";
            }

            return string.Join(",", Volume.VolumeInfo.Authors);
        }
    }

    internal class BookshelfWrapper
    {
        public Bookshelf Bookshelf { get; private set; }

        public BookshelfWrapper(Bookshelf shelf)
        {
            Bookshelf = shelf;
        }

        public override string ToString()
        {
            return BookshelfTitle;
        }

        public string BookshelfTitle { get { return Bookshelf.Title; } }
    }
}

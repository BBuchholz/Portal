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

        private async void btnRefreshBookShelves_Click(object sender, RoutedEventArgs e)
        {
            //authenticate and load bookshelves here

            //store authentication? look into revoke/reauth in books example code
            //check if valid, and reauth if not

            //add button to "logout" (revoke credentials) to allow user switch


            using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    new[] { BooksService.Scope.Books },
                    "user", CancellationToken.None, new FileDataStore("Books.ListMyLibrary"));
            }

            // Create the service.
            var service = new BooksService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Books API Sample",
            });

            this.booksService = service;

            var shelves = await GetShelves();

            lvBookshelves.ItemsSource = shelves;
        }

        private async Task<List<Bookshelf>> GetShelves()
        {
            List<Bookshelf> lst = new List<Bookshelf>();

            if (this.booksService == null)
            {
                return lst;
            }

            // Execute async.
            var shelves = await booksService.Mylibrary.Bookshelves.List().ExecuteAsync();

            foreach(Bookshelf b in shelves.Items)
            {
                lst.Add(b);
            }

            return lst;
        }

        private async Task<List<Volume>> GetVolumes(Bookshelf bookShelf)
        {
            List<Volume> lst = new List<Volume>();

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
                    lst.Add(volume);
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
            var volumes = await GetVolumes(SelectedBookshelf);

            lvVolumes.ItemsSource = volumes;            
        }

        private Bookshelf SelectedBookshelf
        {
            get { return (Bookshelf)lvBookshelves.SelectedItem; }
        }

        private async void btnRevokeCredentials_Click(object sender, RoutedEventArgs e)
        {
            await credential.RevokeTokenAsync(CancellationToken.None);
        }
    }
}

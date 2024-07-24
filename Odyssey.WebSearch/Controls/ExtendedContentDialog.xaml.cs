using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;


namespace Odyssey.WebSearch.Controls
{
    public sealed partial class ExtendedContentDialog : ContentDialog
    {
        public ExtendedContentDialog()
        {
            this.InitializeComponent();
        }

        public event RoutedEventHandler BackButtonClick;
        public new event RoutedEventHandler CloseButtonClick;
        public new event RoutedEventHandler PrimaryButtonClick;
        public new event RoutedEventHandler SecondaryButtonClick;

        public bool IsTopBarVisible
        {
            get => topBar.Visibility == Visibility.Visible;
            set => topBar.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        public bool IsBottomBarVisible
        {
            get => bottomBar.Visibility == Visibility.Visible;
            set => bottomBar.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        } 

        public object PrimaryButtonContent { get => primaryButton.Content; set => primaryButton.Content = value; }
        public object SecondaryButtonContent { get => secondaryButton.Content; set => secondaryButton.Content = value; }
        public object CloseButtonContent { get => closeButton.Content; set => closeButton.Content = value; }
        public new UIElementCollection Content => mainContent.Children;
        public Frame Frame => mainframe;


        public new string Title { get; set; }
        public string Subtitle { get; set; }
        public bool HideOnButtonClick { get; set; }

    
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            BackButtonClick.Invoke(sender, e);
            mainframe.GoBack();
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            backButton.Visibility = mainframe.CanGoBack ? Visibility.Visible : Visibility.Collapsed;;
        }

        private void PrimaryButton_Click(object sender, RoutedEventArgs e) 
        { 
            PrimaryButtonClick.Invoke(sender, e);
            if (HideOnButtonClick) Hide();
        }
        private void SecondaryButton_Click(object sender, RoutedEventArgs e)  
        { 
            SecondaryButtonClick.Invoke(sender, e);
            if (HideOnButtonClick) Hide();
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e) 
        {
            CloseButtonClick.Invoke(sender, e);
            if (HideOnButtonClick) Hide();
        }
    }
}

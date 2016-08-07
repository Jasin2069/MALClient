﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MalClient.Shared.NavArgs;
using MalClient.Shared.Utils.Enums;
using MalClient.Shared.ViewModels;
using MalClient.Shared.ViewModels.Forums;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages.Forums
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ForumsMainPage : Page
    {
        public ForumsMainViewModel ViewModel => ViewModelLocator.ForumsMain;

        private ForumsNavigationArgs _args;

        public ForumsMainPage()
        {
            this.InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            ViewModel.NavigationRequested += ViewModelOnNavigationRequested;
            ViewModel.Init(_args);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _args = e.Parameter as ForumsNavigationArgs;
            base.OnNavigatedTo(e);
        }

        private void ViewModelOnNavigationRequested(int page, object args)
        {
            switch((ForumsPageIndex)page)
            {
                case ForumsPageIndex.PageIndex:
                    MainForumContentFrame.Navigate(typeof(ForumIndexPage), args);
                    break;
                case ForumsPageIndex.PageBoard:
                    MainForumContentFrame.Navigate(typeof(ForumBoardPage), args);
                    break;
                case ForumsPageIndex.PageTopic:
                    MainForumContentFrame.Navigate(typeof(ForumTopicPage), args);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(page), page, null);
            }
            
        }

        private void PinnedButtonOnRightClick(object sender, RoutedEventArgs e)
        {
            var btn = sender as FrameworkElement;
            (FlyoutBase.GetAttachedFlyout(btn)).ShowAt(btn);
        }

        private void PinnedButtonOnHoldingClick(object sender, HoldingRoutedEventArgs e)
        {
            var btn = sender as FrameworkElement;
            (FlyoutBase.GetAttachedFlyout(btn)).ShowAt(btn);
        }

        private async void BetaForumsFeedback(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/Mordonus/MALClient/issues/44"));
        }

        private void PinnedTopicSelectionchanged(object sender, SelectionChangedEventArgs e)
        {
            PinnedTopicListView.SelectedIndex = -1;
        }


    }
}

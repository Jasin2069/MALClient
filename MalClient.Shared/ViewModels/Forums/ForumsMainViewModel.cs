﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MalClient.Shared.Delegates;
using MalClient.Shared.Models.Forums;
using MalClient.Shared.NavArgs;
using MalClient.Shared.Utils;
using MalClient.Shared.Utils.Enums;

namespace MalClient.Shared.ViewModels.Forums
{
    public class ForumsMainViewModel : ViewModelBase
    {
        public event AmbiguousNavigationRequest NavigationRequested;

        public ObservableCollection<ForumBoards> PinnedBoards { get; } = new ObservableCollection<ForumBoards>();

        public ObservableCollection<ForumTopicLightEntry> PinnedTopics { get; } = new ObservableCollection<ForumTopicLightEntry>();

        public ForumTopicLightEntry SelectedForumTopicLightEntry
        {
            get { return null; }
            set
            {
                if(value != null)
                GotoPinnedTopic(value);
            }
        }

        private ICommand _removePinnedBoardCommand;

        public ICommand RemovePinnedBoardCommand
            =>
                _removePinnedBoardCommand ??
                (_removePinnedBoardCommand = new RelayCommand<ForumBoards>(RemoveFavouriteBoard));

        private ICommand _gotoPinnedBoardCommand;

        public ICommand GotoPinnedBoardCommand
            =>
                _gotoPinnedBoardCommand ??
                (_gotoPinnedBoardCommand = new RelayCommand<ForumBoards>(GotoFavouriteBoard));

        private ICommand _gotoPinnedTopicCommand;

        public ICommand GotoPinnedTopicCommand
            =>
                _gotoPinnedTopicCommand ??
                (_gotoPinnedTopicCommand = new RelayCommand<ForumTopicLightEntry>(GotoPinnedTopic));

        private ICommand _unpinTopicCommand;

        public ICommand UnpinTopicCommand
            =>
                _unpinTopicCommand ??
                (_unpinTopicCommand = new RelayCommand<ForumTopicLightEntry>(topic => PinnedTopics.Remove(topic)));

        public void Init(ForumsNavigationArgs args)
        {
            if (args == null)
            {
                ViewModelLocator.NavMgr.ResetMainBackNav();
                args = new ForumsNavigationArgs { Page = ForumsPageIndex.PageIndex };
            }
            else
            {
                ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageForumIndex,null);
            }         
            NavigationRequested?.Invoke((int)args.Page, args);
        }

        public async void LoadPinnedTopics()
        {
            foreach (var item in (await DataCache.RetrieveData<List<ForumTopicLightEntry>>("pinned_forum_topics.json", "", -1)) ?? new List<ForumTopicLightEntry>())
            {
                PinnedTopics.Add(item);
            }          
        }
         
        public async Task SavePinnedTopics()
        {
            await DataCache.SaveData(PinnedTopics.ToList(), "pinned_forum_topics.json", "");     
        }

        public ForumsMainViewModel()
        {
            if (!string.IsNullOrEmpty(Settings.ForumsPinnedBoards))
            {
                foreach (var item in Settings.ForumsPinnedBoards.Split(',').Select(int.Parse).Cast<ForumBoards>())
                {
                    PinnedBoards.Add(item);
                }
            }
        }


        public void AddFavouriteBoard(ForumBoards board)
        {
            if (!PinnedBoards.Contains(board))
            {
                PinnedBoards.Add(board);
                Settings.ForumsPinnedBoards = string.Join(",", PinnedBoards.Cast<int>());              
            }
        }

        private void RemoveFavouriteBoard(ForumBoards board)
        {
            PinnedBoards.Remove(board);
            Settings.ForumsPinnedBoards = string.Join(",", PinnedBoards.Cast<int>());              
        }

        private void GotoFavouriteBoard(ForumBoards board)
        {
            ViewModelLocator.NavMgr.ResetMainBackNav();
            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageForumIndex, new ForumsNavigationArgs());
            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageForumIndex,new ForumsBoardNavigationArgs(board));
        }

        private void GotoPinnedTopic(ForumTopicLightEntry topic)
        {
            ViewModelLocator.NavMgr.ResetMainBackNav();
            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageForumIndex, new ForumsNavigationArgs());
            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageForumIndex, new ForumsBoardNavigationArgs(topic.SourceBoard));
            ViewModelLocator.GeneralMain.Navigate(PageIndex.PageForumIndex, new ForumsTopicNavigationArgs(topic.Id,topic.SourceBoard,topic.Lastpost));
        }
    }
}

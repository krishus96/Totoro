﻿using System.Diagnostics;
using AnimDL.Core.Api;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Totoro.WinUI.Contracts;
using Totoro.WinUI.Dialogs.ViewModels;

namespace Totoro.WinUI.Services;

public class ViewService : IViewService
{
    private readonly IContentDialogService _contentDialogService;
    private readonly ITrackingService _trackingService;

    public ViewService(IContentDialogService contentDialogService,
                       ITrackingService trackingService)
    {
        _contentDialogService = contentDialogService;
        _trackingService = trackingService;
    }

    public async Task<Unit> UpdateTracking(IAnimeModel a)
    {
        var vm = App.GetService<UpdateAnimeStatusViewModel>();
        vm.Anime = a;

        var result = await _contentDialogService.ShowDialog(vm, d =>
        {
            d.Title = a.Title;
            d.PrimaryButtonText = "Update";
            d.IsSecondaryButtonEnabled = false;
            d.CloseButtonText = "Cancel";
        });

        if (result == ContentDialogResult.Primary)
        {
            var tracking = new Tracking();

            if (a.Tracking?.Status != vm.Status)
            {
                tracking.Status = vm.Status;
            }
            if (vm.Score is int score && score != (a.Tracking?.Score ?? 0))
            {
                tracking.Score = score;
            }
            if (vm.EpisodesWatched > 0)
            {
                tracking.WatchedEpisodes = (int)vm.EpisodesWatched;
            }
            if (vm.StartDate is { } sd)
            {
                tracking.StartDate = sd.Date;
            }
            if(vm.FinishDate is { } fd)
            {
                tracking.FinishDate = fd.Date;
            }

            _trackingService.Update(a.Id, tracking).Subscribe(x => a.Tracking = x);
        }

        return Unit.Default;
    }

    public async Task<SearchResult> ChoooseSearchResult(List<SearchResult> searchResults, ProviderType providerType)
    {
        var vm = App.GetService<ChooseSearchResultViewModel>();
        vm.SetValues(searchResults);
        vm.SelectedSearchResult = searchResults.FirstOrDefault();
        vm.SelectedProviderType = providerType;

        await _contentDialogService.ShowDialog(vm, d =>
        {
            d.Title = "Choose title";
            d.IsPrimaryButtonEnabled = false;
            d.IsSecondaryButtonEnabled = false;
            d.CloseButtonText = "Ok";
        });

        vm.Dispose();
        return vm.SelectedSearchResult;
    }

    public async Task AuthenticateMal()
    {
        await _contentDialogService.ShowDialog<AuthenticateMyAnimeListViewModel>(d =>
        {
            d.Title = "Authenticate";
            d.IsPrimaryButtonEnabled = false;
            d.IsSecondaryButtonEnabled = false;
            d.CloseButtonText = "Ok";
            d.Width = App.MainWindow.Bounds.Width;
        });
    }

    public async Task PlayVideo(string title, string url)
    {
        await _contentDialogService.ShowDialog<PlayVideoDialogViewModel>(d =>
        {
            d.Title = title;
            d.IsPrimaryButtonEnabled = false;
            d.IsSecondaryButtonEnabled = false;
            d.CloseButtonText = "Close";
        }, vm => vm.Url = url);
    }

    public async Task<T> SelectModel<T>(IEnumerable<object> models)
        where T: class
    {
        var vm = new SelectModelViewModel()
        {
            Models = models,
        };

        await _contentDialogService.ShowDialog(vm, d =>
        {
            d.Title = "Select";
            d.IsPrimaryButtonEnabled = false;
            d.IsSecondaryButtonEnabled = false;
            d.CloseButtonText = "Ok";
        });

        return vm.SelectedModel as T;
    }

    public async Task SubmitTimeStamp(long malId, int ep, VideoStream stream, double duration, double introStart)
    {
        var vm = new SubmitTimeStampsViewModel(App.GetService<ITimestampsService>()) // TODO fix later
        {
            Stream = stream,
            MalId = malId,
            Episode = ep,
            StartPosition = introStart,
            SuggestedStartPosition = introStart,
            EndPosition = introStart + 85,
            Duration = duration
        };

        await _contentDialogService.ShowDialog(vm, d =>
        {
            d.Title = "Submit Timestamp";
            d.IsPrimaryButtonEnabled = true;
            d.IsSecondaryButtonEnabled = false;
            d.PrimaryButtonText = "Close";
        });

        vm.MediaPlayer.Pause();
    }

    public async Task<bool> Question(string title, string message)
    {
        var dialog = new ContentDialog()
        {
            Title = title,
            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
            XamlRoot = App.MainWindow.Content.XamlRoot,
            DefaultButton = ContentDialogButton.Primary,
            Content = message,
            PrimaryButtonText = "Yes",
            SecondaryButtonText = "No"
        };

        var result = await dialog.ShowAsync();
        return result == ContentDialogResult.Primary;
    }
}

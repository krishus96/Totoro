﻿using System.Reflection;
using Microsoft.Extensions.Logging;
using Totoro.Core.ViewModels;

namespace Totoro.Core;

public static class Settings
{
    public static Key<bool> PreferSubs { get; } = new("PreferSubs", true);
    public static Key<string> DefaultProviderType { get; } = new("DefaultProviderType", "anime-pahe");
    public static Key<bool> UseDiscordRichPresense { get; } = new("UseDiscordRichPresense", false);
    public static Key<bool> ShowTimeRemainingOnDiscordRichPresense { get; } = new("ShowTimeRemainingOnDiscordRichPresense", true);
    public static Key<int> TimeRemainingWhenEpisodeCompletesInSeconds { get; } = new("TimeRemainingWhenEpisodeCompletesInSeconds", 120);
    public static Key<int> OpeningSkipDurationInSeconds { get; } = new("OpeningSkipDurationInSeconds", 85);
    public static Key<Guid> AniSkipId { get; } = new("AniSkipId", Guid.NewGuid);
    public static Key<bool> ContributeTimeStamps { get; } = new("ContributeTimeStamps", false);
    public static Key<LogLevel> MinimumLogLevel { get; } = new("MinimumLogLevel", LogLevel.Debug);
    public static Key<ListServiceType> DefaultListService { get; } = new("DefaultListService", ListServiceType.MyAnimeList);
    public static Key<string> HomePage { get; } = new("HomePage", "Discover");
    public static Key<bool> AllowSideLoadingPlugins { get; } = new("AllowSideLoadingPlugins", false);
    public static Key<StreamQualitySelection> DefaultStreamQualitySelection { get; } = new("DefaultStreamQualitySelection", StreamQualitySelection.Auto);
    public static Key<bool> IncludeNsfw { get; } = new("IncludeNsfw", false);
    public static Key<bool> EnterFullScreenWhenPlaying { get; } = new("EnterFullScreenWhenPlaying", false);
    public static Key<DebridServiceType> DebridServiceType { get; } = new("DebridServiceType", Models.DebridServiceType.Premiumize);
    public static Key<bool> AutoUpdate { get; } = new("AutoUpdate", true);
    public static Key<string> PremiumizeApiKey { get; } = new("PremiumizeApiKey", "");
    public static Key<AdvanceTorrentSearchOptions> TorrentSearchOptions { get; } = new("TorrentSearchOptions", () => AdvanceTorrentSearchOptions.Default);
    public static Key<MediaPlayerType> MediaPlayerType { get; } = new("MediaPlayerType", Models.MediaPlayerType.WindowsMediaPlayer);
    public static Key<bool> PreBufferTorrents { get; } = new("PreBufferTorrents", false);
    public static Key<bool> AutoRemoveWatchedTorrents { get; } = new("AutoRemoveWatchedTorrents", false);
    public static Key<bool> AutoDownloadTorrents { get; } = new("AutoDownloadTorrents", false);
    public static Key<string> AnimeCardClickAction { get; } = new("AnimeCardClickAction", "Watch");
    public static Key<string> DefaultTorrentTrackerType { get; } = new("DefaultTorrentTrackerType", "nya");
    public static Key<int> SmallSkipAmount { get; } = new("SmallSkipAmount", 5);
    public static Key<string> DefaultMediaPlayer { get; } = new("DefaultMediaPlayer", "vlc");
    public static Key<bool> MediaDetectionEnabled { get; } = new("MediaDetectionEnabled", false);
    public static Key<bool> OnlyDetectMediaInLibraryFolders { get; } = new("OnlyDetectMediaInLibraryFolders", false);
    public static Key<ObservableCollection<string>> LibraryFolders { get; } = new("LibraryFolders", new ObservableCollection<string>());
    public static Key<StartupOptions> StartupOptions { get; } = new("StartupOptions", () => new StartupOptions());
    public static Key<DisplayMode> ListDisplayMode { get; } = new("ListDisplayMode", DisplayMode.Grid);
    public static Key<DataGridSettings> UserListDataGridSettings { get; } = new("UserListDataGridSettings", GetDefaultUserListDataGridSettings);
    public static Key<GridViewSettings> UserListGridViewSettings { get; } = new("UserListGridViewSettings", new GridViewSettings());
    public static Key<string> DefaultMangaProviderType { get; } = new("DefaultMangaProviderType", "manga-dex");

    public static IEnumerable<string> GetObsoleteKeys()
    {
        return typeof(Settings)
            .GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Where(x => x.GetCustomAttribute<ObsoleteAttribute>() is not null)
            .Select(x => x.GetValue(null) as IKey)
            .Select(x => x.Name);
    }

    public static DataGridSettings GetDefaultUserListDataGridSettings()
    {
        return new DataGridSettings
        {
            Sort = new ("Title", true),
            Columns = new List<DataGridColumnModel>()
            {
                new()
                {
                    Name = "Title",
                    DisplayIndex = 0,
                    Width = 600
                },
                new()
                {
                    Name = "Season",
                    DisplayIndex = 1,
                },
                new()
                {
                    Name = "Mean Score",
                    DisplayIndex = 2,
                },
                new()
                {
                    Name = "User Score",
                    DisplayIndex = 3,
                },
                new()
                {
                    Name = "Tracking",
                    DisplayIndex = 4,
                },
                new()
                {
                    Name = "Date Started",
                    DisplayIndex = 5,
                    IsVisible = false
                },
                new()
                {
                    Name = "Date Completed",
                    DisplayIndex = 6,
                    IsVisible = false
                },
                new()
                {
                    Name = "Last Updated",
                    DisplayIndex = 7,
                    IsVisible = false
                },
                new()
                {
                    Name = "Type",
                    DisplayIndex = 8,
                    IsVisible = false
                },
                new()
                {
                    Name = "Next Episode",
                    DisplayIndex = 9,
                    IsVisible = false
                }
            }
        };
    }
}

﻿using System.Reactive.Subjects;
using Totoro.Plugins.Contracts;

namespace Totoro.Core.Services;

public class Initalizer : IInitializer
{
    private readonly IPluginManager _pluginManager;
    private readonly IKnownFolders _knownFolders;
    private readonly ILocalSettingsService _localSettingsService;
    private readonly IRssDownloader _rssDownloader;
    private readonly IConnectivityService _connectivityService;
    private readonly PluginOptionsStorage _pluginOptionsStorage;
    private readonly IUpdateService _updateService;
    private readonly IResumePlaybackService _playbackStateStorage;
    private readonly ITorrentEngine _torrentEngine;
    private readonly Subject<Unit> _onShutDown = new();

    public Initalizer(IPluginManager pluginManager,
                      IKnownFolders knownFolders,
                      IUpdateService updateService,
                      IResumePlaybackService playbackStateStorage,
                      ITorrentEngine torrentEngine,
                      ILocalSettingsService localSettingsService,
                      IRssDownloader rssDownloader,
                      IConnectivityService connectivityService,
                      PluginOptionsStorage pluginOptionsStorage)
    {
        _pluginManager = pluginManager;
        _knownFolders = knownFolders;
        _localSettingsService = localSettingsService;
        _rssDownloader = rssDownloader;
        _connectivityService = connectivityService;
        _pluginOptionsStorage = pluginOptionsStorage;
        _updateService = updateService;
        _playbackStateStorage = playbackStateStorage;
        _torrentEngine = torrentEngine;
    }

    public async Task Initialize()
    {
#if RELEASE
        await _pluginManager.Initialize(_knownFolders.Plugins); 
#endif
        if(_connectivityService.IsConnected)
        {
            await _torrentEngine.TryRestoreState();
            await _rssDownloader.Initialize();
        }
        _pluginOptionsStorage.Initialize();
        RemoveObsoleteSettings();
    }

    public async Task ShutDown()
    {
        _onShutDown.OnNext(Unit.Default);
        _updateService.ShutDown();
        _playbackStateStorage.SaveState();
        _rssDownloader.SaveState();
        await _torrentEngine.SaveState();
    }

    private void RemoveObsoleteSettings()
    {
        foreach (var key in Settings.GetObsoleteKeys())
        {
            _localSettingsService.RemoveSetting(key);
        }
    }

    public IObservable<Unit> OnShutDown => _onShutDown;
}

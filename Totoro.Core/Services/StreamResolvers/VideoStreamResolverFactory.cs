﻿using AnitomySharp;

namespace Totoro.Core.Services.StreamResolvers;

public class VideoStreamResolverFactory : IVideoStreamResolverFactory
{
    private readonly IProviderFactory _providerFactory;
    private readonly ISettings _settings;
    private readonly IDebridServiceContext _debridService;
    private readonly IKnownFolders _knownFolders;

    public VideoStreamResolverFactory(IProviderFactory providerFactory,
                                      ISettings settings,
                                      IDebridServiceContext debridService,
                                      IKnownFolders knownFolders)
    {
        _providerFactory = providerFactory;
        _settings = settings;
        _debridService = debridService;
        _knownFolders = knownFolders;
    }

    public IVideoStreamModelResolver CreateAnimDLResolver(string baseUrl)
    {
        return new AnimDLVideoStreamResolver(_providerFactory, _settings, baseUrl);
    }

    public IVideoStreamModelResolver CreateGogoAnimDLResolver(string baseUrlSub, string baseUrlDub)
    {
        return new AnimDLVideoStreamResolver(_providerFactory, _settings, baseUrlSub, baseUrlDub);
    }

    public async Task<IVideoStreamModelResolver> CreateDebridStreamResolver(string magnet)
    {
        var resolver = new DebridStreamModelResolver(_debridService, magnet);
        await resolver.Populate();
        return resolver;
    }

    public IVideoStreamModelResolver CreateWebTorrentStreamResolver(IEnumerable<Element> parsedResults, string magnet)
    {
        return new WebTorrentStreamModelResolver(_knownFolders, parsedResults, magnet);
    }

    public IVideoStreamModelResolver CreateMonoTorrentStreamResolver(IEnumerable<Element> parsedResults,string magnet)
    {
        return new MonoTorrentStreamModelResolver(_knownFolders, parsedResults, magnet);
    }
}


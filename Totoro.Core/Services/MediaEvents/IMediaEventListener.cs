﻿using Totoro.Plugins.Anime.Contracts;

namespace Totoro.Core.Services.MediaEvents;

public interface IMediaEventListener
{
    void SetMediaPlayer(IMediaPlayer mediaPlayer);
    void SetAnime(IAnimeModel anime);
    void SetSearchResult(ICatalogItem searchResult);
    void SetCurrentEpisode(int episode);
    void Stop();
    void SetVideoStreamModel(VideoStreamModel videoStreamModel);
    void SetTimeStamps(AniSkipResult timeStamps);
}



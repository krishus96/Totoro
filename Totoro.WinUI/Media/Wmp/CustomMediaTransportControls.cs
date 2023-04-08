﻿using System.Reactive.Subjects;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Totoro.WinUI.Contracts;

namespace Totoro.WinUI.Media.Wmp;

public class CustomMediaTransportControls : MediaTransportControls, IMediaTransportControls
{
    private readonly Subject<Unit> _onNextTrack = new();
    private readonly Subject<Unit> _onPrevTrack = new();
    private readonly Subject<Unit> _onSkipIntro = new();
    private readonly Subject<string> _onQualityChanged = new();
    private readonly Subject<Unit> _onDynamicSkipIntro = new();
    private readonly Subject<Unit> _onSubmitTimeStamp = new();
    private readonly Subject<Unit> _onAddCc = new();
    private readonly MenuFlyout _qualitiesFlyout = new() { Placement = Microsoft.UI.Xaml.Controls.Primitives.FlyoutPlacementMode.Top };
    private AppBarButton _qualitiesButton;
    private Button _dynamicSkipIntroButton;
    private SymbolIcon _fullWindowSymbol;

    public static readonly DependencyProperty ResolutionsProperty =
        DependencyProperty.Register(nameof(Resolutions), typeof(IEnumerable<string>), typeof(CustomMediaTransportControls), new PropertyMetadata(null, OnResolutionsChanged));

    public static readonly DependencyProperty IsSkipButtonVisibleProperty =
        DependencyProperty.Register("IsSkipButtonVisible", typeof(bool), typeof(CustomMediaTransportControls), new PropertyMetadata(false, OnSkipIntroVisibleChanged));

    public static readonly DependencyProperty SelectedResolutionProperty =
        DependencyProperty.Register(nameof(SelectedResolution), typeof(string), typeof(CustomMediaTransportControls), new PropertyMetadata("", OnSelectedResolutionChanged));
    private readonly IWindowService _windowService;

    private static void OnSelectedResolutionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is not string s)
        {
            return;
        }

        if (string.IsNullOrEmpty(s))
        {
            return;
        }

        var mtc = d as CustomMediaTransportControls;
        foreach (var item in mtc._qualitiesFlyout.Items.OfType<ToggleMenuFlyoutItem>())
        {
            item.IsChecked = item.Text == s;
        }
    }

    private static void OnSkipIntroVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var mtc = d as CustomMediaTransportControls;
        if (mtc._dynamicSkipIntroButton is Button btn && e.NewValue is bool b)
        {
            btn.Visibility = b ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    private static void OnResolutionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var mtc = d as CustomMediaTransportControls;
        var flyout = mtc._qualitiesFlyout;
        foreach (var item in flyout.Items.OfType<MenuFlyoutItem>())
        {
            item.Click -= mtc.FlyoutItem_Click;
        }
        flyout.Items.Clear();

        if (e.NewValue is IEnumerable<string> values)
        {
            var qualities = values.ToList();
            if (qualities.Count == 1)
            {
                mtc._qualitiesButton.Visibility = Visibility.Collapsed;
            }
            else if (qualities.Count > 1)
            {
                mtc._qualitiesButton.IsEnabled = true;
                foreach (var item in qualities)
                {
                    var flyoutItem = new ToggleMenuFlyoutItem { Text = item };
                    flyoutItem.Click += mtc.FlyoutItem_Click;
                    flyout.Items.Add(flyoutItem);
                }
            }
        }
    }

    public IEnumerable<string> Resolutions
    {
        get { return (IEnumerable<string>)GetValue(ResolutionsProperty); }
        set { SetValue(ResolutionsProperty, value); }
    }

    public bool IsSkipButtonVisible
    {
        get { return (bool)GetValue(IsSkipButtonVisibleProperty); }
        set { SetValue(IsSkipButtonVisibleProperty, value); }
    }

    public string SelectedResolution
    {
        get { return (string)GetValue(SelectedResolutionProperty); }
        set { SetValue(SelectedResolutionProperty, value); }
    }

    public IObservable<Unit> OnNextTrack => _onNextTrack;
    public IObservable<Unit> OnPrevTrack => _onPrevTrack;
    public IObservable<Unit> OnStaticSkip => _onSkipIntro;
    public IObservable<Unit> OnAddCc => _onAddCc;
    public IObservable<string> OnQualityChanged => _onQualityChanged;
    public IObservable<Unit> OnDynamicSkip => _onDynamicSkipIntro;
    public IObservable<Unit> OnSubmitTimeStamp => _onSubmitTimeStamp;

    public CustomMediaTransportControls(IWindowService windowService)
    {
        _windowService = windowService;

        DefaultStyleKey = typeof(CustomMediaTransportControls);
        IsCompact = false;
        IsNextTrackButtonVisible = false;
        IsPreviousTrackButtonVisible = false;
        IsSkipBackwardButtonVisible = true;
        IsSkipBackwardEnabled = true;
        IsSkipForwardButtonVisible = true;
        IsSkipForwardEnabled = true;
        IsTextScaleFactorEnabled = true;

        windowService?.IsFullWindowChanged?.Subscribe(isFullWindow =>
        {
            _fullWindowSymbol.Symbol = isFullWindow ? Symbol.BackToWindow : Symbol.FullScreen;
        });
    }

    protected override void OnApplyTemplate()
    {
        var nextTrackButton = GetTemplateChild("NextTrackButton") as AppBarButton;
        var prevTrackButton = GetTemplateChild("PreviousTrackButton") as AppBarButton;
        var skipIntroButton = GetTemplateChild("SkipIntroButton") as AppBarButton;
        var submitTimeStamp = GetTemplateChild("SubmitTimeStampsButton") as AppBarButton;
        var fullWindowButton = GetTemplateChild("FullWindowButton") as AppBarButton;
        var addccButon = GetTemplateChild("AddCCButton") as AppBarButton;
        _fullWindowSymbol = GetTemplateChild("FullWindowSymbol") as SymbolIcon;
        _qualitiesButton = GetTemplateChild("QualitiesButton") as AppBarButton;
        _qualitiesButton.Flyout = _qualitiesFlyout;
        _dynamicSkipIntroButton = GetTemplateChild("DynamicSkipIntroButton") as Button;

        prevTrackButton.Click += (_, _) => _onPrevTrack.OnNext(Unit.Default);
        nextTrackButton.Click += (_, _) => _onNextTrack.OnNext(Unit.Default);
        skipIntroButton.Click += (_, _) => _onSkipIntro.OnNext(Unit.Default);
        submitTimeStamp.Click += (_, _) => _onSubmitTimeStamp.OnNext(Unit.Default);
        addccButon.Click += (_, _) => _onAddCc.OnNext(Unit.Default);
        fullWindowButton.Click += (_, _) => _windowService?.ToggleIsFullWindow();
        _dynamicSkipIntroButton.Click += (_, __) => _onDynamicSkipIntro.OnNext(Unit.Default);

        base.OnApplyTemplate();
    }

    private void FlyoutItem_Click(object sender, RoutedEventArgs e)
    {
        _onQualityChanged.OnNext((sender as MenuFlyoutItem).Text);
    }
}

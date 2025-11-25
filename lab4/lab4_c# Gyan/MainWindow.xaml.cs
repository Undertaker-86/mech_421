using System;
using System.Windows;
using DistanceMonitor.ViewModels;

namespace DistanceMonitor;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel = new();

    public MainWindow()
    {
        InitializeComponent();
        DataContext = _viewModel;
        Loaded += (_, _) => _viewModel.RefreshPortsCommand.Execute(null);
    }

    protected override void OnClosed(EventArgs e)
    {
        _viewModel.CleanUp();
        base.OnClosed(e);
    }
}

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.Diagnostics;

namespace CL.CustomItem;

public partial class ItemManegerProfile : UserControl
{
    #region Данні для профілю
    public string NameAccount2 { get; set; }
    public string UUID { get; set; }
    public string AccessToken { get; set; }
    public string ImageUrl { get; set; }
    public int index { get; set; }
    public bool OfficalAccount { get; set; }
    #endregion
    #region Елементи UI
    public Grid _ClickSelectAccount => this.FindControl<Grid>("ClickSelectAccount")!;
    public TextBlock _NameAccount => this.FindControl<TextBlock>("NameAccount")!;
    public Image _DeleteAccount => this.FindControl<Image>("DeleteAccount")!;
    public Image _IconAccountType => this.FindControl<Image>("IconAccountType")!;
    #endregion
    public ItemManegerProfile()
    {
        InitializeComponent();
        Debug.WriteLine("ItemManegerProfile ініціалізовано");
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
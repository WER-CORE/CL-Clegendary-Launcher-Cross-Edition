using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

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
    public ItemManegerProfile()
    {
        InitializeComponent();
    }
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
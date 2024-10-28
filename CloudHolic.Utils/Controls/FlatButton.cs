using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace CloudHolic.Utils.Controls;

public class FlatButton : Button
{
    #region Radius Dependency Property

    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(nameof(CornerRadius),
        typeof(CornerRadius),
        typeof(FlatButton), new PropertyMetadata(new CornerRadius(0)));

    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    #endregion

    #region MouseOverBackground Dependency Property

    public static readonly DependencyProperty MouseOverBackgroundProperty = DependencyProperty.Register(
        nameof(MouseOverBackground), typeof(Color), typeof(FlatButton),
        new PropertyMetadata(ColorConverter.ConvertFromString("#FFBEE6FD"), OnMouseOverBackgroundPropertyChanged));

    private static readonly DependencyPropertyKey MouseOverBackgroundBrushPropertyKey =
        DependencyProperty.RegisterReadOnly(nameof(MouseOverBackgroundBrush), typeof(Brush), typeof(FlatButton),
            new FrameworkPropertyMetadata(new BrushConverter().ConvertFromString("#FFBEE6FD"),
                FrameworkPropertyMetadataOptions.None));

    public static readonly DependencyProperty MouseOverBackgroundBrushProperty = MouseOverBackgroundBrushPropertyKey.DependencyProperty;

    public Color MouseOverBackground
    {
        get => (Color)GetValue(MouseOverBackgroundProperty);
        set => SetValue(MouseOverBackgroundProperty, value);
    }

    public Brush MouseOverBackgroundBrush
    {
        get => (Brush)GetValue(MouseOverBackgroundBrushProperty);
        protected set => SetValue(MouseOverBackgroundBrushPropertyKey, value);
    }

    private static void OnMouseOverBackgroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((FlatButton)d).MouseOverBackgroundBrush = new SolidColorBrush((Color)e.NewValue);
    }

    #endregion

    #region MouseOverBorder Dependency Property

    public static readonly DependencyProperty MouseOverBorderProperty = DependencyProperty.Register(
        nameof(MouseOverBorder), typeof(Color), typeof(FlatButton),
        new PropertyMetadata(ColorConverter.ConvertFromString("#FF3C7FB1"), OnMouseOverBorderPropertyChanged));

    public static readonly DependencyPropertyKey MouseOverBorderBrushPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(MouseOverBorderBrush), typeof(Brush), typeof(FlatButton),
        new FrameworkPropertyMetadata(new BrushConverter().ConvertFromString("#FF3C7FB1"),
            FrameworkPropertyMetadataOptions.None));

    public static readonly DependencyProperty MouseOverBorderBrushProperty = MouseOverBorderBrushPropertyKey.DependencyProperty;

    public Color MouseOverBorder
    {
        get => (Color)GetValue(MouseOverBorderProperty);
        set => SetValue(MouseOverBorderProperty, value);
    }

    public Brush MouseOverBorderBrush
    {
        get => (Brush)GetValue(MouseOverBorderBrushProperty);
        protected set => SetValue(MouseOverBorderBrushPropertyKey, value);
    }

    private static void OnMouseOverBorderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((FlatButton)d).MouseOverBorderBrush = new SolidColorBrush((Color)e.NewValue);
    }

    #endregion

    #region PressedBackground Dependency Property

    public static readonly DependencyProperty PressedBackgroundProperty = DependencyProperty.Register(
        nameof(PressedBackground), typeof(Color), typeof(FlatButton),
        new PropertyMetadata(ColorConverter.ConvertFromString("#FFC4E5F6"), OnPressedBackgroundPropertyChanged));

    public static readonly DependencyPropertyKey PressedBackgroundBrushPropertyKey =
        DependencyProperty.RegisterReadOnly(nameof(PressedBackgroundBrush), typeof(Brush), typeof(FlatButton),
            new FrameworkPropertyMetadata(new BrushConverter().ConvertFromString("#FFC4E5F6"),
                FrameworkPropertyMetadataOptions.None));

    public static readonly DependencyProperty PressedBackgroundBrushProperty = PressedBackgroundBrushPropertyKey.DependencyProperty;

    public Color PressedBackground
    {
        get => (Color)GetValue(PressedBackgroundProperty);
        set => SetValue(PressedBackgroundProperty, value);
    }

    public Brush PressedBackgroundBrush
    {
        get => (Brush)GetValue(PressedBackgroundBrushProperty);
        protected set => SetValue(PressedBackgroundBrushPropertyKey, value);
    }

    private static void OnPressedBackgroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((FlatButton)d).PressedBackgroundBrush = new SolidColorBrush((Color)e.NewValue);
    }

    #endregion

    #region PressedBorder Dependency Property

    public static readonly DependencyProperty PressedBorderProperty = DependencyProperty.Register(nameof(PressedBorder),
        typeof(Color), typeof(FlatButton),
        new PropertyMetadata(ColorConverter.ConvertFromString("#FF2C628B"), OnPressedBorderPropertyChanged));

    public static readonly DependencyPropertyKey PressedBorderBrushPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(PressedBorderBrush), typeof(Brush), typeof(FlatButton),
        new FrameworkPropertyMetadata(new BrushConverter().ConvertFromString("#FF2C628B"),
            FrameworkPropertyMetadataOptions.None));

    public static readonly DependencyProperty PressedBorderBrushProperty = PressedBorderBrushPropertyKey.DependencyProperty;

    public Color PressedBorder
    {
        get => (Color)GetValue(PressedBorderProperty);
        set => SetValue(PressedBorderProperty, value);
    }

    public Brush PressedBorderBrush
    {
        get => (Brush)GetValue(PressedBorderBrushProperty);
        protected set => SetValue(PressedBorderBrushPropertyKey, value);
    }

    private static void OnPressedBorderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((FlatButton)d).PressedBorderBrush = new SolidColorBrush((Color)e.NewValue);
    }

    #endregion

    #region DisabledBackground Dependency Property

    public static readonly DependencyProperty DisabledBackgroundProperty = DependencyProperty.Register(
        nameof(DisabledBackground), typeof(Color), typeof(FlatButton),
        new PropertyMetadata(ColorConverter.ConvertFromString("#FFF4F4F4"), OnDisabledBackgroundPropertyChanged));

    public static readonly DependencyPropertyKey DisabledBackgroundBrushPropertyKey =
        DependencyProperty.RegisterReadOnly(nameof(DisabledBackgroundBrush), typeof(Brush), typeof(FlatButton),
            new FrameworkPropertyMetadata(new BrushConverter().ConvertFromString("#FFF4F4F4"),
                FrameworkPropertyMetadataOptions.None));

    public static readonly DependencyProperty DisabledBackgroundBrushProperty = DisabledBackgroundBrushPropertyKey.DependencyProperty;

    public Color DisabledBackground
    {
        get => (Color)GetValue(DisabledBackgroundProperty);
        set => SetValue(DisabledBackgroundProperty, value);
    }

    public Brush DisabledBackgroundBrush
    {
        get => (Brush)GetValue(DisabledBackgroundBrushProperty);
        protected set => SetValue(DisabledBackgroundBrushPropertyKey, value);
    }

    private static void OnDisabledBackgroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((FlatButton)d).DisabledBackgroundBrush = new SolidColorBrush((Color)e.NewValue);
    }

    #endregion

    #region DisabledBorder Dependency Property

    public static readonly DependencyProperty DisabledBorderProperty = DependencyProperty.Register(
        nameof(DisabledBorder), typeof(Color), typeof(FlatButton),
        new PropertyMetadata(ColorConverter.ConvertFromString("#FFADB2B5"), OnDisabledBorderPropertyChanged));

    public static readonly DependencyPropertyKey DisabledBorderBrushPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(DisabledBorderBrush), typeof(Brush), typeof(FlatButton),
        new FrameworkPropertyMetadata(new BrushConverter().ConvertFromString("#FFADB2B5"),
            FrameworkPropertyMetadataOptions.None));

    public static readonly DependencyProperty DisabledBorderBrushProperty = DisabledBorderBrushPropertyKey.DependencyProperty;

    public Color DisabledBorder
    {
        get => (Color)GetValue(DisabledBorderProperty);
        set => SetValue(DisabledBorderProperty, value);
    }

    public Brush DisabledBorderBrush
    {
        get => (Brush)GetValue(DisabledBorderBrushProperty);
        protected set => SetValue(DisabledBorderBrushPropertyKey, value);
    }

    private static void OnDisabledBorderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((FlatButton)d).DisabledBorderBrush = new SolidColorBrush((Color)e.NewValue);
    }

    #endregion

    #region DisabledForeground Dependency Property

    public static readonly DependencyProperty DisabledForegroundProperty = DependencyProperty.Register(
        nameof(DisabledForeground), typeof(Color), typeof(FlatButton),
        new PropertyMetadata(ColorConverter.ConvertFromString("#FF838383"), OnDisabledForegroundPropertyChanged));

    public static readonly DependencyPropertyKey DisabledForegroundBrushPropertyKey =
        DependencyProperty.RegisterReadOnly(nameof(DisabledForegroundBrush), typeof(Brush), typeof(FlatButton),
            new FrameworkPropertyMetadata(new BrushConverter().ConvertFromString("#FF838383"),
                FrameworkPropertyMetadataOptions.None));

    public static readonly DependencyProperty DisabledForegroundBrushProperty = DisabledForegroundBrushPropertyKey.DependencyProperty;

    public Color DisabledForeground
    {
        get => (Color)GetValue(DisabledForegroundProperty);
        set => SetValue(DisabledForegroundProperty, value);
    }

    public Brush DisabledForegroundBrush
    {
        get => (Brush)GetValue(DisabledForegroundBrushProperty);
        protected set => SetValue(DisabledForegroundBrushPropertyKey, value);
    }

    private static void OnDisabledForegroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((FlatButton)d).DisabledForegroundBrush = new SolidColorBrush((Color)e.NewValue);
    }

    #endregion
}
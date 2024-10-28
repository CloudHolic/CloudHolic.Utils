using System.Windows.Input;
using System.Windows;

namespace CloudHolic.Utils.Controls;

public class CustomWindow : Window
{
    public CustomWindow()
    {
        DefaultStyleKey = typeof(CustomWindow);

        CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, CloseWindow));
        CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, MaximizeWindow, CanResizeWindow));
        CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, MinimizeWindow, CanMinimizeWindow));
        CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, RestoreWindow, CanResizeWindow));
        CommandBindings.Add(new CommandBinding(SystemCommands.ShowSystemMenuCommand, ShowSystemMenu));
    }

    //protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    //{
    //    base.OnMouseLeftButtonDown(e);
    //    if(e.ButtonState == MouseButtonState.Pressed)
    //        DragMove();
    //}

    protected override void OnContentRendered(EventArgs e)
    {
        base.OnContentRendered(e);
        if (SizeToContent == SizeToContent.WidthAndHeight)
            InvalidateMeasure();
    }

    #region Window Commands
    private void CanResizeWindow(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = ResizeMode is ResizeMode.CanResize or ResizeMode.CanResizeWithGrip;
    }

    private void CanMinimizeWindow(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = ResizeMode != ResizeMode.NoResize;
    }

    private void CloseWindow(object sender, ExecutedRoutedEventArgs e)
    {
        Close();
        // SystemCommands.CloseWindow(this);
    }

    private void MaximizeWindow(object sender, ExecutedRoutedEventArgs e)
    {
        SystemCommands.MaximizeWindow(this);
    }

    private void MinimizeWindow(object sender, ExecutedRoutedEventArgs e)
    {
        SystemCommands.MinimizeWindow(this);
    }

    private void RestoreWindow(object sender, ExecutedRoutedEventArgs e)
    {
        SystemCommands.RestoreWindow(this);
    }

    private void ShowSystemMenu(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.OriginalSource is not FrameworkElement element)
            return;

        var point = WindowState == WindowState.Maximized
            ? new Point(0, element.ActualHeight)
            : new Point(Left + BorderThickness.Left, element.ActualHeight + Top + BorderThickness.Top);
        point = element.TransformToAncestor(this).Transform(point);
        SystemCommands.ShowSystemMenu(this, point);
    }
    #endregion
}
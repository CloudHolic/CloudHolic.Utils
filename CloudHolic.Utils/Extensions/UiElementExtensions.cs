using System.Windows;
using System.Windows.Media;

namespace CloudHolic.Utils.Extensions;

public static class UiElementExtensions
{
    public static T? GetParentOfType<T>(this UIElement element) where T : UIElement =>
        VisualTreeHelper.GetParent(element) is not FrameworkElement parent
            ? null
            : parent as T ?? parent.GetParentOfType<T>();
}

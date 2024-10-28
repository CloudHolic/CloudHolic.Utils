using CloudHolic.Utils.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;

namespace CloudHolic.Utils.Controls;

public enum StackPanelFill
{
    Auto,
    Fill,
    Ignored
}

public class AutoStackPanel : Panel
{
    #region Orientation Dependency Property

    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
        nameof(Orientation), typeof(Orientation), typeof(AutoStackPanel),
        new FrameworkPropertyMetadata(Orientation.Vertical,
            FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

    public Orientation Orientation
    {
        get => (Orientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    #endregion

    #region MarginBetweenChildren Dependency Property

    public static readonly DependencyProperty MarginBetweenChildrenProperty = DependencyProperty.Register(
        nameof(MarginBetweenChildren), typeof(double), typeof(AutoStackPanel),
        new FrameworkPropertyMetadata(0.0,
            FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

    public double MarginBetweenChildren
    {
        get => (double)GetValue(MarginBetweenChildrenProperty);
        set => SetValue(MarginBetweenChildrenProperty, value);
    }

    #endregion

    #region Fill Attached Dependency Property

    public static readonly DependencyProperty FillProperty = DependencyProperty.RegisterAttached("Fill",
        typeof(StackPanelFill), typeof(AutoStackPanel),
        new FrameworkPropertyMetadata(StackPanelFill.Auto,
            FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure |
            FrameworkPropertyMetadataOptions.AffectsParentArrange | FrameworkPropertyMetadataOptions.AffectsParentMeasure));

    public static StackPanelFill GetFill(DependencyObject d) => (StackPanelFill)d.GetValue(FillProperty);

    public static void SetFill(DependencyObject d, StackPanelFill value) => d.SetValue(FillProperty, value);

    #endregion

    #region Protected Methods
    
    protected override Size ArrangeOverride(Size finalSize)
    {
        var children = InternalChildren.OfType<UIElement>().ToList();
        var (accumulatedLeft, accumulatedTop) = (0.0, 0.0);

        var isHorizontal = Orientation == Orientation.Horizontal;
        var totalMarginToAdd = CalculateTotalMarginToAdd(children, MarginBetweenChildren);

        var allAutoSizedSum = 0.0;
        var countOfFillTypes = 0;

        children.ForEach(child =>
        {
            var fillType = GetFill(child);
            if (fillType != StackPanelFill.Auto)
            {
                if (child.Visibility != Visibility.Collapsed && fillType != StackPanelFill.Ignored)
                    countOfFillTypes = 1;
            }
            else
            {
                var desiredSize = isHorizontal ? child.DesiredSize.Width : child.DesiredSize.Height;
                allAutoSizedSum += desiredSize;
            }
        });

        var remainingForFillTypes = isHorizontal
            ? Positive(finalSize.Width - allAutoSizedSum - totalMarginToAdd)
            : Positive(finalSize.Height - allAutoSizedSum - totalMarginToAdd);
        var fillTypeSize = remainingForFillTypes / countOfFillTypes;

        children.ForEach(child =>
        {
            var childDesiredSize = child.DesiredSize;
            var fillType = GetFill(child);
            var isCollapsed = child.Visibility == Visibility.Collapsed || fillType == StackPanelFill.Ignored;
            var marginToAdd = child == children.Last() || isCollapsed ? 0 : MarginBetweenChildren;

            var childRect = new Rect(accumulatedLeft, accumulatedTop, Positive(finalSize.Width - accumulatedLeft),
                Positive(finalSize.Height - accumulatedTop));

            Action action = isHorizontal switch
            {
                true => () =>
                {
                    childRect.Width = fillType == StackPanelFill.Auto || isCollapsed
                        ? childDesiredSize.Width
                        : fillTypeSize;
                    childRect.Height = finalSize.Height;
                    accumulatedLeft += childRect.Width += marginToAdd;
                }
                ,
                _ => () =>
                {
                    childRect.Height = fillType == StackPanelFill.Auto || isCollapsed
                        ? childDesiredSize.Height
                        : fillTypeSize;
                    childRect.Width = finalSize.Width;
                    accumulatedTop += childRect.Height + marginToAdd;
                }
            };

            action.Invoke();

            child.Arrange(childRect);
        });

        return finalSize;
    }

    [SuppressMessage("ReSharper", "AccessToModifiedClosure")]
    protected override Size MeasureOverride(Size availableSize)
    {
        var children = InternalChildren.OfType<UIElement>().ToList();
        var (parentWidth, parentHeight, accumulatedWidth, accumulatedHeight) = (0.0, 0.0, 0.0, 0.0);

        var isHorizontal = Orientation == Orientation.Horizontal;
        var totalMarginToAdd = CalculateTotalMarginToAdd(children, MarginBetweenChildren);

        children.Where(x => GetFill(x) == StackPanelFill.Auto).ForEach(child =>
        {
            var childConstraint = new Size(Positive(availableSize.Width - accumulatedWidth),
                Positive(availableSize.Height - accumulatedHeight));

            child.Measure(childConstraint);
            var childDesiredSize = child.DesiredSize;

            Action action = isHorizontal switch
            {
                true => () =>
                {
                    accumulatedWidth += childDesiredSize.Width;
                    parentHeight = Math.Max(parentHeight, accumulatedHeight + childDesiredSize.Height);
                },
                _ => () =>
                {
                    accumulatedHeight += childDesiredSize.Height;
                    parentWidth = Math.Max(parentWidth, accumulatedWidth + childDesiredSize.Width);
                }
            };

            action.Invoke();
        });

        if (isHorizontal)
            accumulatedWidth += totalMarginToAdd;
        else
            accumulatedHeight += totalMarginToAdd;

        var totalCountOfFillTypes = children.Count(x => GetFill(x) == StackPanelFill.Fill && x.Visibility != Visibility.Collapsed);

        var availableSpaceRemaining = isHorizontal
            ? Positive(availableSize.Width - accumulatedWidth)
            : Positive(availableSize.Height - accumulatedHeight);

        var eachFillTypeSize = totalCountOfFillTypes > 0 ? availableSpaceRemaining / totalCountOfFillTypes : 0;

        children.Where(x => GetFill(x) == StackPanelFill.Fill).ForEach(child =>
        {
            var childConstraint = isHorizontal
                ? new Size(eachFillTypeSize, Positive(availableSize.Height - accumulatedHeight))
                : new Size(Positive(availableSize.Width - accumulatedWidth), eachFillTypeSize);

            child.Measure(childConstraint);
            var childDesiredSize = child.DesiredSize;

            Action action = isHorizontal switch
            {
                true => () =>
                {
                    accumulatedWidth += childDesiredSize.Width;
                    parentHeight = Math.Max(parentHeight, accumulatedHeight + childDesiredSize.Height);
                }
                ,
                _ => () =>
                {
                    accumulatedHeight += childDesiredSize.Height;
                    parentWidth = Math.Max(parentWidth, accumulatedWidth + childDesiredSize.Width);
                }
            };

            action.Invoke();
        });

        parentWidth = Math.Max(parentWidth, accumulatedWidth);
        parentHeight = Math.Max(parentHeight, accumulatedHeight);

        return new Size(parentWidth, parentHeight);
    }

    #endregion

    #region Private Methods

    private static double CalculateTotalMarginToAdd(IEnumerable<UIElement> children, double marginBetweenChildren)
    {
        var visibleChildrenCount = children.Count(x => x.Visibility != Visibility.Collapsed && GetFill(x) != StackPanelFill.Ignored);
        var marginMultiplier = Positive(visibleChildrenCount - 1);

        return marginBetweenChildren * marginMultiplier;
    }

    private static int Positive(int value) => Math.Max(0, value);

    private static double Positive(double value) => Math.Max(0, value);

    #endregion
}

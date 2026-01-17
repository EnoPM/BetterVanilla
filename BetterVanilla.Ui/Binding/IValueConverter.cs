using System;

namespace BetterVanilla.Ui.Binding;

/// <summary>
/// Provides a way to apply custom logic to a binding.
/// </summary>
public interface IValueConverter
{
    /// <summary>
    /// Converts a value from source to target.
    /// </summary>
    /// <param name="value">The value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <returns>A converted value.</returns>
    object? Convert(object? value, Type targetType, object? parameter);

    /// <summary>
    /// Converts a value from target back to source.
    /// </summary>
    /// <param name="value">The value that is produced by the binding target.</param>
    /// <param name="targetType">The type to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <returns>A converted value.</returns>
    object? ConvertBack(object? value, Type targetType, object? parameter);
}

/// <summary>
/// A value converter that accepts generic type parameters.
/// </summary>
public interface IValueConverter<TSource, TTarget> : IValueConverter
{
    /// <summary>
    /// Converts from source type to target type.
    /// </summary>
    TTarget? Convert(TSource? value, object? parameter);

    /// <summary>
    /// Converts from target type back to source type.
    /// </summary>
    TSource? ConvertBack(TTarget? value, object? parameter);
}

/// <summary>
/// Base class for strongly-typed value converters.
/// </summary>
public abstract class ValueConverterBase<TSource, TTarget> : IValueConverter<TSource, TTarget>
{
    public abstract TTarget? Convert(TSource? value, object? parameter);

    public virtual TSource? ConvertBack(TTarget? value, object? parameter)
    {
        throw new NotSupportedException($"ConvertBack is not supported for {GetType().Name}");
    }

    object? IValueConverter.Convert(object? value, Type targetType, object? parameter)
    {
        if (value is TSource sourceValue)
            return Convert(sourceValue, parameter);

        if (value == null && !typeof(TSource).IsValueType)
            return Convert(default, parameter);

        return Convert(default, parameter);
    }

    object? IValueConverter.ConvertBack(object? value, Type targetType, object? parameter)
    {
        if (value is TTarget targetValue)
            return ConvertBack(targetValue, parameter);

        if (value == null && !typeof(TTarget).IsValueType)
            return ConvertBack(default, parameter);

        return ConvertBack(default, parameter);
    }
}

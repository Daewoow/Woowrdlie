namespace Wordlie.Infrastructure.Attributes;

/// <summary>
/// По тем или иным причинам возвращает не json, а само слово
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class NoJsonReturn : Attribute;
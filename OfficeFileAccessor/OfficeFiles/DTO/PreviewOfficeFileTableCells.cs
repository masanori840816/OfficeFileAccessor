using System.ComponentModel.DataAnnotations;

namespace OfficeFileAccessor.OfficeFiles.DTO;

public record PreviewOfficeFileTableCells
{
    public required long SheetId { get; init; }
    public required long GroupId { get; init; }
    [Key]
    public required long CellId { get; init; }
    public required int GroupDisplayOrder { get; init; }
    public string? Title { get; init; }
    public required int Column { get; init; }
    public required int Row { get; init; }
    public required int VerticalLength { get; init; }
    public required int HorizontalLength { get; init; }
    public required string Value { get; init; }
    public string? Formula { get; init; }
    public required string ValueType { get; init; }
    public string? BackgroundColor { get; init; }
    public bool Editabled { get; init; }
    public bool VerticalWriting { get; init; }
    public int TextRotation { get; init; }
    public int? BorderLeft { get; init; }
    public int? BorderTop { get; init; }
    public int? BorderRight { get; init; }
    public int? BorderBottom { get; init; }
    public string? FontName { get; init; }
    public int? FontSize { get; init; }
    public string? FontColor { get; init; }
    public bool? Bold { get; init; }
    public int? MergedStartColumn { get; init; }
    public int? MergedStartRow { get; init; }
    public int? MergedEndColumn { get; init; }
    public int? MergedEndRow { get; init; }
}
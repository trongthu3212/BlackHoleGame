namespace BlackHole.Level
{
    public enum GridBlockType
    {
        None = 0,
        Normal = 1,
            
        EdgeTop = 4,
        EdgeBottom = 5,
        EdgeLeft = 6,
        EdgeRight = 7,
            
        CornerTopLeft = 8,
        CornerTopRight = 9,
        CornerBottomLeft = 10,
        CornerBottomRight = 11,
            
        EdgeBlockTypeStarts = EdgeTop,
        EdgeBlockTypeEnds = EdgeRight,
            
        CornerBlockTypeStarts = CornerTopLeft,
        CornerBlockTypeEnds = CornerBottomRight,
            
        DiagonalLeftToTop = 12,
        DiagonalLeftToBottom = 13,
        DiagonalRightToTop = 14,
        DiagonalRightToBottom = 15,
            
        DiagonalBlockTypeStarts = DiagonalLeftToTop,
        DiagonalBlockTypeEnds = DiagonalRightToBottom,
            
        DiagonalSupportLeftToTop = 16,
        DiagonalSupportLeftToBottom = 17,
        DiagonalSupportRightToTop = 18,
        DiagonalSupportRightToBottom = 19,
            
        DiagonalSupportBlockTypeStarts = DiagonalSupportLeftToTop,
        DiagonalSupportBlockTypeEnds = DiagonalSupportRightToBottom,
    }
}
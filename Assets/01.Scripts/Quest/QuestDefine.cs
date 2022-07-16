
public enum MatchType
{
    NONE = 0,
    THREE = 3,    // 3 Match
    FOUR = 4,    // 4 Match     -> CLEAR_HORZ �Ǵ� VERT ����Ʈ
    FIVE = 5,    // 5 Match     -> CLEAR_LAZER ����Ʈ
    THREE_THREE = 6,    // 3 + 3 Match -> CLEAR_CIRCLE ����Ʈ 
    THREE_FOUR = 7,    // 3 + 4 Match -> CLEAR_CIRCLE ����Ʈ
    THREE_FIVE = 8,    // 3 + 5 Match -> CLEAR_LAZER ����Ʈ
    FOUR_FIVE = 9,    // 4 + 5 Match -> CLEAR_LAZER ����Ʈ
    FOUR_FOUR = 10,   // 4 + 4 Match -> CLEAR_CIRCLE ����Ʈ
}

static class MatchTypeMethod
{
    public static short ToValue(this MatchType matchType)
    {
        return (short)matchType;
    }


    public static MatchType Add(this MatchType inMatchTypeSrc, MatchType inMatchTypeTarget)
    {
        if (inMatchTypeSrc == MatchType.FOUR && inMatchTypeTarget == MatchType.FOUR)
            return MatchType.FOUR_FOUR;

        return (MatchType)((int)inMatchTypeSrc + (int)inMatchTypeTarget);
    }
}
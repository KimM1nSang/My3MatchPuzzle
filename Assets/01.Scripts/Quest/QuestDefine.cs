
public enum MatchType
{
    NONE = 0,
    THREE = 3,    // 3 Match
    FOUR = 4,    // 4 Match     -> CLEAR_HORZ ¶Ç´Â VERT Äù½ºÆ®
    FIVE = 5,    // 5 Match     -> CLEAR_LAZER Äù½ºÆ®
    THREE_THREE = 6,    // 3 + 3 Match -> CLEAR_CIRCLE Äù½ºÆ® 
    THREE_FOUR = 7,    // 3 + 4 Match -> CLEAR_CIRCLE Äù½ºÆ®
    THREE_FIVE = 8,    // 3 + 5 Match -> CLEAR_LAZER Äù½ºÆ®
    FOUR_FIVE = 9,    // 4 + 5 Match -> CLEAR_LAZER Äù½ºÆ®
    FOUR_FOUR = 10,   // 4 + 4 Match -> CLEAR_CIRCLE Äù½ºÆ®
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
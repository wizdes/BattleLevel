public static class Util
{
	public static bool IsTouchEquivalent(float touch, float objectPosition)
	{
		return (touch >= objectPosition && touch < objectPosition + 1);
	}

}

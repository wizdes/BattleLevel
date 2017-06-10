public static class Util
{
	public static bool IsTouchEquivalent(float touch, float objectPosition)
	{
		return (touch >= objectPosition - 0.5f && touch < objectPosition + 0.5f);
	}

}

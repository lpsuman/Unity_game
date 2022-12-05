namespace Bluaniman.SpaceGame.Player
{
	public interface IInputAxisProvider
	{
		float GetInputAxis(int index);
		bool AreInputAxiiPresent(int startIndex, int count);
	}
}
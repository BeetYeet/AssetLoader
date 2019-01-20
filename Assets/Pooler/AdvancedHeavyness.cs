public class AdvancedHeavyness
{
	public readonly long memorySize; // total amount of bytes per GameObject
	public readonly float heavyness; // average amount of milliseconds per instantiation

	public AdvancedHeavyness( long memorySize, float heavyness )
	{
		this.memorySize = memorySize;
		this.heavyness = heavyness;
	}
}
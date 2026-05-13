using Sandbox;

public sealed class Logic : Component
{
	[Property] public double Money { get; set; } = 0;
	[Property] public double MoneyPerSecond { get; set; } = 0;

	protected override void OnUpdate()
	{
		Money += MoneyPerSecond * Time.Delta;
	}
}
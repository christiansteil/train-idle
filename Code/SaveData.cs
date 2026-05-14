public sealed class SaveData
{
	public double Money { get; set; }
	public int Trains { get; set; }
	public int TrainCars { get; set; }
    public double Coal { get; set; }
	public int CoalMiners { get; set; }
	public double Research { get; set; }

	public long LastSaveTimeMs { get; set; }

	public long GetLastSaveTimeMs()
	{
		if ( LastSaveTimeMs > 0 )
			return LastSaveTimeMs;

		return 0;
	}
}
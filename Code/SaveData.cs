public sealed class SaveData
{
	public double Money { get; set; }
	public int Trains { get; set; }
	public int TrainCars { get; set; }

	// New save time format.
	public long LastSaveTimeMs { get; set; }

	// Old save time format, kept temporarily so old save.json files still load.
	public long LastSaveTime { get; set; }

	public long GetLastSaveTimeMs()
	{
		if ( LastSaveTimeMs > 0 )
			return LastSaveTimeMs;

		if ( LastSaveTime > 0 )
			return LastSaveTime * 1000;

		return 0;
	}
}
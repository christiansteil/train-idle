using System;
using Sandbox;

public sealed class Logic : Component
{
	private const string SaveFile = "save.json";
	private const float AutoSaveInterval = 15f;
	private const double MaxOfflineSeconds = 60 * 60 * 24;

	private TimeUntil NextAutoSave;

	[Property] public double Money { get; set; } = 25;
	[Property] public int Trains { get; set; } = 0;
	[Property] public int TrainCars { get; set; } = 0;

	[Property] public double LastOfflineMoneyEarned { get; set; } = 0;
	[Property] public double LastOfflineSeconds { get; set; } = 0;
	[Property] public bool ShowOfflinePopup { get; set; } = false;

	public double MoneyPerSecond => TrainCars * 1.0;

	public double TrainCost => 25;
	public double TrainCarCost => 10 * Math.Pow( 1.15, TrainCars );

	protected override void OnStart()
	{
		LoadGame();
		NextAutoSave = AutoSaveInterval;
	}

	protected override void OnUpdate()
	{
		Money += MoneyPerSecond * Time.Delta;

		if ( NextAutoSave )
		{
			SaveGame();
			NextAutoSave = AutoSaveInterval;
		}
	}

	protected override void OnDestroy()
	{
		SaveGame();
	}

	public void ClickMoney()
	{
		Money++;
	}

	public bool CanBuyTrain()
	{
		return Trains < 1 && Money >= TrainCost;
	}

	public void BuyTrain()
	{
		if ( !CanBuyTrain() )
			return;

		Money -= TrainCost;
		Trains = 1;
	}

	public bool CanBuyTrainCar()
	{
		return Trains >= 1 && Money >= TrainCarCost;
	}

	public void BuyTrainCar()
	{
		if ( !CanBuyTrainCar() )
			return;

		Money -= TrainCarCost;
		TrainCars++;
	}

	public void CloseOfflinePopup()
	{
		ShowOfflinePopup = false;
	}

	private void SaveGame()
	{
		var data = new SaveData
		{
			Money = Money,
			Trains = Trains,
			TrainCars = TrainCars,
			LastSaveTimeMs = GetCurrentUnixTimeMs()
		};

		FileSystem.Data.WriteJson( SaveFile, data );
	}

	private void LoadGame()
	{
		if ( !FileSystem.Data.FileExists( SaveFile ) )
			return;

		var data = FileSystem.Data.ReadJson<SaveData>( SaveFile );

		if ( data is null )
			return;

		Money = data.Money;
		Trains = data.Trains;
		TrainCars = data.TrainCars;

		ApplyOfflineProgress( data );
	}

	private void ApplyOfflineProgress( SaveData data )
	{
		long lastSaveTimeMs = data.GetLastSaveTimeMs();

		if ( lastSaveTimeMs <= 0 )
			return;

		long nowMs = GetCurrentUnixTimeMs();
		double offlineSeconds = (nowMs - lastSaveTimeMs) / 1000.0;

		if ( offlineSeconds < 0 )
			offlineSeconds = 0;

		offlineSeconds = Math.Min( offlineSeconds, MaxOfflineSeconds );

		double offlineMoney = offlineSeconds * MoneyPerSecond;

		Money += offlineMoney;

		LastOfflineMoneyEarned = offlineMoney;
		LastOfflineSeconds = offlineSeconds;
		ShowOfflinePopup = offlineMoney > 0;
	}

	private long GetCurrentUnixTimeMs()
	{
		return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
	}
}
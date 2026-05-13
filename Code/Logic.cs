using System;
using Sandbox;

public sealed class Logic : Component
{
	[Property] public double Money { get; set; } = 25;
	[Property] public int Trains { get; set; } = 0;
	[Property] public int TrainCars { get; set; } = 0;

	public double MoneyPerSecond => TrainCars * 1.0;

	public double TrainCost => 25;
	public double TrainCarCost => 10 * Math.Pow( 1.15, TrainCars );

	private const string SaveFile = "save.json";
	private TimeUntil NextAutoSave = 15f;

	protected override void OnStart()
	{
		LoadGame();
	}

	protected override void OnUpdate()
	{
		Money += MoneyPerSecond * Time.Delta;

		if ( NextAutoSave )
		{
			SaveGame();
			NextAutoSave = 15f;
		}
	}

	protected override void OnDestroy()
	{
		SaveGame();
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

	public void SaveGame()
	{
		var data = new SaveData
		{
			Money = Money,
			Trains = Trains,
			TrainCars = TrainCars
		};

		FileSystem.Data.WriteJson( SaveFile, data );
	}

	public void LoadGame()
	{
		if ( !FileSystem.Data.FileExists( SaveFile ) )
			return;

		var data = FileSystem.Data.ReadJson<SaveData>( SaveFile );

		if ( data is null )
			return;

		Money = data.Money;
		Trains = data.Trains;
		TrainCars = data.TrainCars;
	}
}

public sealed class SaveData
{
	public double Money { get; set; }
	public int Trains { get; set; }
	public int TrainCars { get; set; }
}
using Godot;
using System;
using System.Collections.Generic;

public partial class LocationSelectController : TextureRect
{
	[Export] private PackedScene _LocationTextPrefab;
	private VBoxContainer _CountryContainer;
	private VBoxContainer _StateContainer;
	private ToggleGroup _CountriesGroup;
	private ToggleGroup _StatesGroup;

	private List<Toggle> _SpawnedToggles = new List<Toggle>();

    public override void _Ready()
    {
        base._Ready();

		_CountryContainer = GetNode<VBoxContainer>("Country/VBoxContainer");
		_StateContainer = GetNode<VBoxContainer>("States/VBoxContainer");

		_CountriesGroup = GetNode<ToggleGroup>("Country/VBoxContainer");
		if(_CountriesGroup != null)
		{
			_CountriesGroup.OnValueChanged += OnValueChanged;
		}

		_StatesGroup = GetNode<ToggleGroup>("States/VBoxContainer");

		LoadCountries();
    }

	private void LoadCountries()
	{
		CountryDatabase countries = GetNode<CountryDatabase>("/root/CountryDatabase");
		if(countries != null)
		{
			foreach(var country in countries.Countries)
			{
				if(_CountryContainer != null)
				{
					Label countryLabel = _LocationTextPrefab.Instantiate<Label>();
					if(countryLabel != null)
					{
						countryLabel.Text = country.Name;
						if(countryLabel is Toggle tgl)
							tgl.SetOwner(_CountriesGroup);
						_CountryContainer.AddChild(countryLabel);
					}
				}
			}
		}
	}

	public void OnValueChanged()
	{
		GetAllStates();
	}

	private void GetAllStates()
	{
		if(_SpawnedToggles.Count > 0)
		{
			foreach(var t in _SpawnedToggles)
			{
				t.QueueFree();
			}

			_SpawnedToggles.Clear();
		}
		CountryDatabase countryDb = GetNode<CountryDatabase>("/root/CountryDatabase");
		if(countryDb != null)
		{
			Country country = countryDb.GetCountryFromName(_CountriesGroup.GetToggleValue());
			foreach(var state in country.States)
			{
				 Label label = _LocationTextPrefab.Instantiate<Label>();
				 if(label != null && _StateContainer != null)
				 {
					label.Text = state;
					if(label is Toggle tgl)
					{
						tgl.SetOwner(_StatesGroup);
						_SpawnedToggles.Add(tgl);
					}

					_StateContainer.AddChild(label);
				 }
			}

		}
	}

	public void OnSaveBtnPressed()
	{
		GameController gameController = GetNode<GameController>("/root/GameController");
		CountryDatabase countryDb = GetNode<CountryDatabase>("/root/CountryDatabase");
		if(gameController != null && countryDb != null)
		{
			string countryName = _CountriesGroup.GetToggleValue();
			string stateName = _StatesGroup.GetToggleValue();
			
			Label locationText = GetNode<Label>("/root/CanvasGroup/Container/Location/Label");
			if(locationText != null)
			{
				locationText.Text = stateName + ", " + countryName;
			}

			this.Visible = false;

		}
	}

	public void OnCloseBtnPressed()
	{
		this.Visible = false;
	}
}

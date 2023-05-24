using Godot;
using static Godot.Collections.Array;
using System;
using System.Collections.Generic;

public partial class ToggleGroup : VBoxContainer
{
	private List<Toggle> _TogglesInGroup = new List<Toggle>();
	private Toggle _CurrentlySelected = null;

	public event Action OnValueChanged;

    public override void _Ready()
    {
        base._Ready();
		GetToggleGroup();
    }

	public void SetSelectedToggle(Toggle toggle)
	{
		if(_CurrentlySelected != null)
			_CurrentlySelected.UncheckButton();

		_CurrentlySelected = toggle;

		OnValueChanged?.Invoke();
	}

	private void GetToggleGroup()
	{
		Godot.Collections.Array<Node> _nodes = this.GetChildren();
		foreach(var node in _nodes)
		{
			if(node is Toggle toggleBtn)
			{
				toggleBtn.SetOwner(this);
				_TogglesInGroup.Add(toggleBtn);
			}
		}
	}

	public Toggle GetToggleNode() => _CurrentlySelected;
	public Toggle GetToggleNode(int index)
	{
		if(index < _TogglesInGroup.Count)
			return _TogglesInGroup[index];

		return null;
	}

	public string GetToggleValue() => _CurrentlySelected.GetToggleValue();
	public string GetToggleValue(int index) 
	{
		if(index < _TogglesInGroup.Count)
			return _TogglesInGroup[index].GetToggleValue();

		return "#ERROR#";
	}
}

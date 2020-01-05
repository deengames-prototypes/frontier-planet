extends Node2D

func _ready():
	for i in range(len(Globals.player_inventory)):
		var item = Globals.player_inventory[i]
		$Contents.text += item
		if i < len(Globals.player_inventory) - 1:
			$Contents.text += ", "
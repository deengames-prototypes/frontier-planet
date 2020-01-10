extends "res://Entities/World/TracksPlayer.gd"

func _ready():
	self.visible = Globals.community_level >= 3
	if self.visible:
		self.connect("player_interacted", self, "_on_use_stove")

func _process(delta):
	$Label.visible = _player_in_range
	if not $Label.visible:
		$Label.text = "Stove"
	
func _on_use_stove():
	$Label.text = "Not enough ingredients for an omelet."
	if len(Globals.player_inventory) >= 2 and Globals.player_inventory.find("egg") > -1:
		var egg = Globals.player_inventory[Globals.player_inventory.find("egg")]
		var ingredient = Globals.player_inventory[0]
		for item in Globals.player_inventory:
			if item != "egg":
				ingredient = item
				break
		
		Globals.player_inventory.erase(egg)
		Globals.player_inventory.erase(ingredient)
		
		var omelet = ingredient + " omelet"
		Globals.player_inventory.append(omelet)
		$Label.text = "Made " + omelet